using System;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class Control : ILayoutable, IDisposable
    {
        public static ControlStyle DefaultStyle = new ControlStyle (null,
            (style) => {
                style.ForegroundColor = Theme.DarkTextColor;
                style.BackgroundColor = Theme.NeutralGray;
                style.Font = Theme.UIFont;
                style.FontSize = Theme.FontSize;
                style.Border.Radius = 0;
                style.Border.Color = Theme.BorderGray;
                style.Border.Width = 0;
            });

        public static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle);

        private AnchorStyles anchor_style = AnchorStyles.Top | AnchorStyles.Left;
        private AutoSizeMode auto_size_mode;
        private SKBitmap back_buffer;
        private ControlBehaviors behaviors;
        private Rectangle bounds;
        private Control current_mouse_in;
        //private Cursor cursor;
        internal int dist_right;
        internal int dist_bottom;
        private DockStyle dock_style;
        //private Rectangle explicit_bounds;
        private bool is_captured;
        private bool is_enabled = true;
        private bool is_visible = true;
        private int layout_suspended;
        private bool layout_pending;
        private Padding margin;
        private Padding padding;
        private Control parent;
        private bool recalculate_distances = true;
        private Rectangle scaled_bounds;
        private int tab_index = -1;
        private bool tab_stop = true;
        private string text;

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        public virtual ControlStyle CurrentStyle => IsHovering ? StyleHover : Style;

        public Control ()
        {
            Controls = new ControlCollection (this);

            margin = DefaultMargin;
            padding = DefaultPadding;

            bounds = new Rectangle (Point.Empty, DefaultSize);

            behaviors = ControlBehaviors.Selectable;
        }

        public event EventHandler<MouseEventArgs> Click;
        public event EventHandler CursorChanged;
        public event EventHandler DockChanged;
        public event EventHandler EnabledChanged;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<LayoutEventArgs> Layout;
        public event EventHandler LocationChanged;
        public event EventHandler MarginChanged;
        public event EventHandler PaddingChanged;
        public event EventHandler SizeChanged;
        public event EventHandler TabIndexChanged;
        public event EventHandler TabStopChanged;
        public event EventHandler TextChanged;
        public event EventHandler VisibleChanged;

        public virtual AnchorStyles Anchor {
            get => anchor_style;
            set {
                UseAnchorLayoutInternal = true;

                if (anchor_style == value)
                    return;

                anchor_style = value;
                dock_style = DockStyle.None;

                RecalculateDistances ();

                parent?.PerformLayout (this, nameof (Anchor));
            }
        }

        public bool AutoSize => false;

        public int Bottom => bounds.Bottom;

        public Rectangle Bounds {
            get => bounds;
            set => SetBounds (value.Left, value.Top, value.Width, value.Height);
        }

        public bool CanSelect {
            get {
                if (!behaviors.HasFlag (ControlBehaviors.Selectable))
                    return false;

                var parent = this;

                while (parent != null) {
                    if (!parent.Visible || !parent.Enabled)
                        return false;

                    parent = parent.Parent;
                }

                return true;
            }
        }

        public bool Capture {
            get => is_captured || Controls.Any (c => c.Capture);
            set {
                is_captured = value;

                if (Parent != null)
                    Parent.Capture = value;
            }
        }

        /// <summary>
        /// The control canvas minus any borders
        /// </summary>
        public virtual Rectangle ClientRectangle {
            get {
                // TODO: We should be scaling the Border as well
                var x = CurrentStyle.Border.Left.GetWidth ();
                var y = CurrentStyle.Border.Top.GetWidth ();
                var w = CurrentStyle.Border.Right.GetWidth () + x;
                var h = CurrentStyle.Border.Bottom.GetWidth () + y;

                var bounds = GetScaledBounds (Bounds, ScaleFactor, BoundsSpecified.All);

                return new Rectangle (x, y, bounds.Width - w, bounds.Height - h);
            }
        }

        public Size ClientSize {
            get {
                var w = Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth ();
                var h = Height - CurrentStyle.Border.Bottom.GetWidth () - CurrentStyle.Border.Top.GetWidth ();
                return new Size (w, h);
            }
        }

        public bool Contains (Control control)
        {
            // Is control one of our children or grandchildren
            while (control != null) {
                control = control.Parent;

                if (control == this)
                    return true;
            }

            return false;
        }

        public ControlCollection Controls { get; }

        //public Cursor Cursor {
        //    get => cursor ?? Parent?.Cursor ?? Cursors.Default;
        //    set {
        //        if (cursor != value) {
        //            cursor = value;
        //            OnCursorChanged (EventArgs.Empty);
        //        }
        //    }
        //}

        public int DeviceDpi => (int)((FindWindow ()?.Scaling ?? 1) * 96);

        public virtual Rectangle DisplayRectangle => ClientRectangle;

        public DockStyle Dock {
            get => dock_style;
            set {
                if (value != DockStyle.None)
                    UseAnchorLayoutInternal = false;

                if (dock_style == value)
                    return;

                dock_style = value;
                anchor_style = AnchorStyles.Top | AnchorStyles.Left;

                if (dock_style == DockStyle.None) {
                    //bounds = explicit_bounds;
                    UseAnchorLayoutInternal = true;
                }

                if (Parent != null)
                    Parent.PerformLayout (this, nameof (Dock));
                else if (Controls.GetAllControls ().Count () > 0)
                    PerformLayout (this, nameof (Dock));

                OnDockChanged (EventArgs.Empty);
            }
        }

        public bool Enabled {
            get {
                if (!is_enabled)
                    return false;

                return Parent?.Enabled ?? true;
            }
            set {
                if (is_enabled == value)
                    return;

                is_enabled = value;

                OnEnabledChanged (EventArgs.Empty);
            }
        }

        public ControlAdapter FindAdapter ()
        {
            if (this is ControlAdapter adapter)
                return adapter;

            return Parent?.FindAdapter ();
        }

        public Form FindForm ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is Form f)
                return f;

            return Parent?.FindForm ();
        }

        public Control GetNextControl (Control start, bool forward = true)
        {
            if (Controls.Count == 0)
                return null;

            // Ignore start control if it isn't our child/grandchild
            if (!Contains (start))
                start = null;

            // If the start control is the only control, return null
            if (start != null && Controls.Count == 1)
                return null;

            // See if we need recurse into the start control
            var child_control = start?.GetNextControl (start, forward);

            if (child_control != null)
                return child_control;

            // If this is a grandchild, we need to give the parent a chance to move next
            while (start?.Parent != null && start?.Parent != this) {
                var old_start = start;
                start = start.Parent;

                var child_control2 = start?.GetNextControl (old_start, forward);

                if (child_control2 != null)
                    return child_control2;
            }

            // Build an array sorted by TabIndex then element order (OrderBy is stable)
            var array = Controls.OrderBy (c => c.TabIndex).ToList ();

            // If we don't have a start control, just return the first or last control
            if (start == null)
                return forward ? array[0] : array[array.Count - 1];

            var start_index = array.IndexOf (start);

            // Find the "next" control in the array
            if (forward && start_index + 1 < array.Count)
                return array[start_index + 1];

            if (!forward && start_index > 0)
                return array[start_index - 1];

            return null;
        }

        public virtual Size GetPreferredSize (Size proposedSize) => new Size (Width, Height);

        public int Height {
            get => bounds.Height;
            set => SetBounds (bounds.X, bounds.Y, bounds.Width, value, BoundsSpecified.Height);
        }

        public void Invalidate ()
        {
            FindWindow ()?.Invalidate ();
        }

        public void Invalidate (Rectangle rectangle)
        {
            FindWindow ()?.Invalidate (rectangle);
        }

        public bool IsHovering { get; private set; }

        public int Left {
            get => bounds.Left;
            set => SetBounds (value, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.X);
        }

        public Point Location {
            get => bounds.Location;
            set => SetBounds (value.X, value.Y, bounds.Width, bounds.Height, BoundsSpecified.Location);
        }

        public int LogicalToDeviceUnits (int value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        public Padding LogicalToDeviceUnits (Padding value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        public Size LogicalToDeviceUnits (Size value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        public virtual Padding Margin {
            get => margin;
            set {
                if (margin != value) {
                    margin = value;
                    Parent?.PerformLayout (this, nameof (Margin));
                    OnMarginChanged (EventArgs.Empty);
                }
            }
        }

        public string Name { get; set; }

        /// <summary>
        /// The control canvas minus any borders and Padding
        /// </summary>
        public Rectangle PaddedClientRectangle {
            get {
                var client_rect = ClientRectangle;

                var x = client_rect.Left + Padding.Left;
                var y = client_rect.Top + Padding.Top;
                var w = client_rect.Width - Padding.Horizontal;
                var h = client_rect.Height - Padding.Vertical;
                return new Rectangle (x, y, w, h);
            }
        }

        public virtual Padding Padding {
            get => padding;
            set {
                if (padding != value) {
                    padding = value;
                    PerformLayout (this, nameof (Padding));
                    OnPaddingChanged (EventArgs.Empty);
                }
            }
        }

        public Control Parent {
            get => parent;
            set {
                if (value == this)
                    throw new ArgumentException ("Control cannot be its own Parent.");

                if (parent == value)
                    return;

                if (value == null) {
                    parent.Controls.Remove (this);
                    parent = null;
                    return;
                }

                value.Controls.Add (this);
            }
        }

        public void PerformLayout () => PerformLayout (null, null);

        public void PerformLayout (Control affectedControl, string affectedProperty)
        {
            var levent = new LayoutEventArgs (affectedControl, affectedProperty);

            foreach (var c in Controls.GetAllControls ().Where (c => c.Visible))
                if (c.recalculate_distances)
                    c.RecalculateDistances ();

            if (layout_suspended > 0) {
                layout_pending = true;
                return;
            }

            layout_pending = false;

            // Prevent us from getting messed up
            layout_suspended++;

            // Perform all Dock and Anchor calculations
            try {
                OnLayout (levent);
            }

            // Need to make sure we decremend layout_suspended
            finally {
                layout_suspended--;
            }
        }

        public Size PreferredSize => GetPreferredSize (Size.Empty);

        public Point PointToScreen (Point point)
        {
            if (this is ControlAdapter)
                return FindWindow ().PointToScreen (point);

            var pt = Parent.PointToScreen (Location);

            pt.Offset (point);

            return pt;
        }

        public void ResumeLayout (bool performLayout = true)
        {
            if (layout_suspended > 0)
                layout_suspended--;

            if (layout_suspended == 0) {
                if (!performLayout)
                    foreach (var c in Controls.GetAllControls ().Where (c => c.Visible))
                        c.RecalculateDistances ();

                if (performLayout && layout_pending)
                    PerformLayout ();
            }
        }

        public int Right => bounds.Right;

        public void Scale (SizeF factor) => ScaleCore (factor.Width, factor.Height);

        public Rectangle ScaledBounds => GetScaledBounds (Bounds, ScaleFactor, BoundsSpecified.All);

        public SizeF ScaleFactor => new SizeF ((float)(DeviceDpi / DpiHelper.LogicalDpi), (float)(DeviceDpi / DpiHelper.LogicalDpi));

        public void Select ()
        {
            if (Selected || !CanSelect)
                return;

            Selected = true;

            var adapter = FindAdapter ();

            if (adapter != null)
                adapter.SelectedControl = this;

            Invalidate ();
        }

        public bool Selected { get; private set; }

        public bool SelectNextControl (Control start, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            Control c;

            if (!Contains (start) || (!nested && (start.Parent != this)))
                start = null;

            c = start;

            do {
                c = GetNextControl (c, forward);

                if (c is null) {
                    if (wrap) {
                        wrap = false;
                        continue;
                    }

                    break;
                }

                if (c.CanSelect && ((c.Parent == this) || nested) && (c.TabStop || !tabStopOnly)) {
                    c.Select ();
                    return true;
                }

            } while (c != start);

            return false;
        }

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            SetBoundsCore (x, y, width, height, specified);
        }

        protected virtual void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            var moved = bounds.X != x || bounds.Y != y;
            var resized = bounds.Width != width || bounds.Height != height;

            bounds.X = x;
            bounds.Y = y;
            bounds.Width = width;
            bounds.Height = height;

            if (moved)
                OnLocationChanged (EventArgs.Empty);

            if (resized) {
                OnSizeChanged (EventArgs.Empty);
                PerformLayout (this, nameof (Bounds));
            }

            // If the user explicitly moved or resized us, recalculate our anchor distances
            if (specified != BoundsSpecified.None)
                RecalculateDistances ();

            parent?.PerformLayout (this, nameof (Bounds));
        }

        public void SetScaledBounds (int x, int y, int width, int height, BoundsSpecified specified)
        {
            var rect = GetScaledBounds (new Rectangle (x, y, width, height), new SizeF (1 / ScaleFactor.Width, 1 / ScaleFactor.Height), BoundsSpecified.All);
            SetBoundsCore (rect.X, rect.Y, rect.Width, rect.Height, BoundsSpecified.None);
        }

        public Size ScaledSize => ScaledBounds.Size;

        public Size Size {
            get => bounds.Size;
            set => SetBounds (bounds.X, bounds.Y, value.Width, value.Height, BoundsSpecified.Size);
        }

        public void SuspendLayout () => layout_suspended++;

        public int TabIndex {
            get => tab_index != -1 ? tab_index : 0;
            set {
                if (tab_index != value) {
                    tab_index = value;
                    OnTabIndexChanged (EventArgs.Empty);
                }
            }
        }

        public bool TabStop {
            get => tab_stop;
            set {
                if (tab_stop != value) {
                    tab_stop = value;
                    OnTabStopChanged (EventArgs.Empty);
                }
            }
        }

        public object Tag { get; set; }

        public string Text {
            get => text;
            set {
                if (text == value)
                    return;

                text = value;

                if (behaviors.HasFlag (ControlBehaviors.InvalidateOnTextChanged))
                    Invalidate ();

                OnTextChanged (EventArgs.Empty);
            }
        }

        public int Top {
            get => bounds.Top;
            set => SetBounds (bounds.X, value, bounds.Width, bounds.Height, BoundsSpecified.Y);
        }

        public virtual bool Visible {
            get {
                if (!is_visible)
                    return false;

                return parent?.Visible ?? false;
            }
            set {
                if (is_visible != value) {
                    is_visible = value;
                    OnVisibleChanged (EventArgs.Empty);

                    parent?.PerformLayout (this, nameof (Visible));
                }
            }
        }

        public int Width {
            get => bounds.Width;
            set => SetBounds (bounds.X, bounds.Y, value, bounds.Height, BoundsSpecified.Width);
        }

        protected virtual Padding DefaultMargin => new Padding (3);
        protected virtual Padding DefaultPadding => Padding.Empty;
        protected virtual Size DefaultSize => Size.Empty;

        private MouseEventArgs MouseEventsForControl (MouseEventArgs e, Control control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.ScaledLeft, e.Location.Y - control.ScaledTop, e.Delta, e.Modifiers);
        }

        internal void Deselect ()
        {
            Selected = false;
            OnDeselected (EventArgs.Empty);

            Invalidate ();
        }

        internal Window FindWindow ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is Window w)
                return w;

            return Parent?.FindWindow ();
        }

        protected virtual Rectangle GetScaledBounds (Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            var dx = factor.Width;
            var dy = factor.Height;

            var sx = bounds.X;
            var sy = bounds.Y;
            var sw = bounds.Width;
            var sh = bounds.Height;

            // Scale the control location (unless this is the top level adapter)
            if (FindAdapter () != this) {
                if (specified.HasFlag (BoundsSpecified.X))
                    sx = (int)Math.Round (bounds.X * dx);
                if (specified.HasFlag (BoundsSpecified.Y))
                    sy = (int)Math.Round (bounds.Y * dy);
            }

            if (specified.HasFlag (BoundsSpecified.Width))
                sw = (int)Math.Round (bounds.Width * dx);
            if (specified.HasFlag (BoundsSpecified.Height))
                sh = (int)Math.Round (bounds.Height * dy);

            return new Rectangle (sx, sy, sw, sh);
        }

        internal void RaiseClick (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseClick (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseClick (MouseEventsForControl (e, child));
            else
                OnClick (e);
        }

        protected virtual void OnClick (MouseEventArgs e) => Click?.Invoke (this, e);

        protected virtual void OnCursorChanged (EventArgs e) => CursorChanged?.Invoke (this, e);

        protected virtual void OnDeselected (EventArgs e) { }

        protected virtual void OnDockChanged (EventArgs e) => DockChanged?.Invoke (this, e);

        internal void RaiseDoubleClick (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseDoubleClick (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseDoubleClick (MouseEventsForControl (e, child));
            else
                OnDoubleClick (e);
        }

        protected virtual void OnDoubleClick (MouseEventArgs e)
        {
        }

        protected virtual void OnEnabledChanged (EventArgs e) => EnabledChanged?.Invoke (this, e);

        protected virtual void OnLayout (LayoutEventArgs e)
        {
            Layout?.Invoke (this, e);

            DefaultLayout.Instance.Layout (this, e);
        }

        protected virtual void OnLocationChanged (EventArgs e) => LocationChanged?.Invoke (this, e);

        protected virtual void OnMarginChanged (EventArgs e) => MarginChanged?.Invoke (this, e);

        protected virtual void OnPaddingChanged (EventArgs e) => PaddingChanged?.Invoke (this, e);

        internal void RaiseKeyDown (KeyEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                adapter.SelectedControl?.RaiseKeyDown (e);
                return;
            }

            OnKeyDown (e);
        }

        protected virtual void OnKeyDown (KeyEventArgs e) => KeyDown?.Invoke (this, e);

        internal void RaiseKeyPress (KeyPressEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                // Tab
                if (e.KeyChar == 9) {
                    SelectNextControl (adapter.SelectedControl, !e.Shift, true, true, true);
                    e.Handled = true;
                    return;
                }

                adapter.SelectedControl?.RaiseKeyPress (e);
                return;
            }

            OnKeyPress (e);
        }

        protected virtual void OnKeyPress (KeyPressEventArgs e) => KeyPress?.Invoke (this, e);

        internal void RaiseMouseDown (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (MouseEventsForControl (e, child));
            else {
                Select ();
                Capture = true;
                OnMouseDown (e);
            }
        }

        internal void RaiseKeyUp (KeyEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                adapter.SelectedControl?.RaiseKeyUp (e);
                return;
            }

            OnKeyUp (e);
        }

        protected virtual void OnKeyUp (KeyEventArgs e) => KeyUp?.Invoke (this, e);

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
        }

        internal void RaiseMouseEnter (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseEnter (MouseEventsForControl (e, child));
            else
                OnMouseEnter (e);
        }

        protected virtual void OnMouseEnter (MouseEventArgs e)
        {
            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = true;
                Invalidate ();
            }
        }

        internal void RaiseMouseLeave (EventArgs e)
        {
            if (current_mouse_in != null)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = null;

            OnMouseLeave (e);
        }

        protected virtual void OnMouseLeave (EventArgs e)
        {
            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = false;
                Invalidate ();
            }
        }

        internal void RaiseMouseMove (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseMouseMove (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (current_mouse_in != null && current_mouse_in != child) {
                current_mouse_in.RaiseMouseLeave (e);
                current_mouse_in = null;
            }

            if (current_mouse_in == null && child != null)
                child.RaiseMouseEnter (MouseEventsForControl (e, child));

            current_mouse_in = child;

            if (child != null)
                child?.RaiseMouseMove (MouseEventsForControl (e, child));
            else
                OnMouseMove (e);
        }

        protected virtual void OnMouseMove (MouseEventArgs e)
        {
        }

        internal void RaiseMouseUp (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseMouseUp (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseUp (MouseEventsForControl (e, child));
            else {
                Capture = false;
                OnMouseUp (e);
            }
        }

        protected virtual void OnMouseUp (MouseEventArgs e)
        {
        }

        internal void RaiseMouseWheel (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseWheel (MouseEventsForControl (e, child));
            else
                OnMouseWheel (e);
        }

        protected virtual void OnMouseWheel (MouseEventArgs e)
        {
        }

        internal void RaisePaint (PaintEventArgs e) => OnPaint (e);

        protected virtual void OnPaint (PaintEventArgs e)
        {
            foreach (var control in Controls.GetAllControls ().Where (c => c.Visible)) {
                if (control.Width <= 0 || control.Height <= 0)
                    continue;

                var info = new SKImageInfo (control.ScaledSize.Width, control.ScaledSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                using (var canvas = new SKCanvas (buffer)) {
                    // start drawing
                    var args = new PaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, control.ScaledLeft, control.ScaledTop);
            }
        }

        protected virtual void OnSizeChanged (EventArgs e) => SizeChanged?.Invoke (this, e);

        protected virtual void OnTabIndexChanged (EventArgs e) => TabIndexChanged?.Invoke (this, e);

        protected virtual void OnTabStopChanged (EventArgs e) => TabStopChanged?.Invoke (this, e);

        protected virtual void OnTextChanged (EventArgs e) => TextChanged?.Invoke (this, e);

        protected virtual void OnVisibleChanged (EventArgs e) => VisibleChanged?.Invoke (this, e);

        protected void SetAutoSizeMode (AutoSizeMode mode)
        {
            if (auto_size_mode != mode) {
                auto_size_mode = mode;
                PerformLayout (this, "AutoSizeMode");
            }
        }

        protected void SetControlBehavior (ControlBehaviors behavior, bool value = true)
        {
            if (value)
                behaviors |= behavior;
            else
                behaviors &= behavior;
        }

        internal void RaisePaintBackground (PaintEventArgs e) => OnPaintBackground (e);

        protected virtual void OnPaintBackground (PaintEventArgs e)
        {
            e.Canvas.DrawBackground (ScaledBounds, CurrentStyle);
            e.Canvas.DrawBorder (ScaledBounds, CurrentStyle);
        }

        protected virtual void ScaleControl (SizeF factor, BoundsSpecified specified)
        {
            var raw_scaled = GetScaledBounds (Bounds, factor, specified);

            var dx = factor.Width;
            var dy = factor.Height;

            var padding = Padding;
            var margins = Margin;

            // Clear off specified bits for 1.0 scaling factors
            if (dx == 1.0F)
                specified &= ~(BoundsSpecified.X | BoundsSpecified.Width);

            if (dy == 1.0F)
                specified &= ~(BoundsSpecified.Y | BoundsSpecified.Height);

            if (dx != 1.0F) {
                padding.Left = (int)Math.Round (padding.Left * dx);
                padding.Right = (int)Math.Round (padding.Right * dx);
                margins.Left = (int)Math.Round (margins.Left * dx);
                margins.Right = (int)Math.Round (margins.Right * dx);
            }

            if (dy != 1.0F) {
                padding.Top = (int)Math.Round (padding.Top * dy);
                padding.Bottom = (int)Math.Round (padding.Bottom * dy);
                margins.Top = (int)Math.Round (margins.Top * dy);
                margins.Bottom = (int)Math.Round (margins.Bottom * dy);
            }

            // Apply padding and margins
            Padding = padding;
            Margin = margins;

            // Set in the scaled bounds as constrained by the newly scaled min/max size.
            SetBoundsCore (raw_scaled.X, raw_scaled.Y, raw_scaled.Width, raw_scaled.Height, BoundsSpecified.All);
        }

        protected virtual void ScaleCore (float dx, float dy)
        {
            SuspendLayout ();

            try {
                var sx = (int)Math.Round (Left * dx);
                var sy = (int)Math.Round (Top * dy);

                var sw = (int)(Math.Round ((Left + Width) * dx)) - sx;
                var sh = (int)(Math.Round ((Top + Height) * dy)) - sy;

                SetBounds (sx, sy, sw, sh, BoundsSpecified.All);

                foreach (var c in Controls)
                    c.ScaleCore (dx, dy);

            } finally {
                ResumeLayout ();
            }
        }

        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer == null || back_buffer.Width != ScaledSize.Width || back_buffer.Height != ScaledSize.Height) {
                FreeBitmap ();
                back_buffer = new SKBitmap (ScaledSize.Width, ScaledSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            }

            return back_buffer;
        }

        public int ScaledWidth => (int)(Width * ScaleFactor.Width);
        public int ScaledHeight => (int)(Height * ScaleFactor.Height);
        public int ScaledLeft => (int)(Left * ScaleFactor.Width);
        public int ScaledTop => (int)(Top * ScaleFactor.Height);
        // This is an internal control (like a scrollbar) that should
        // not show up in Controls for a user
        internal bool ImplicitControl { get; set; }

        private void FreeBitmap ()
        {
            if (back_buffer != null) {
                back_buffer.Dispose ();
                back_buffer = null;
            }
        }

        private void RecalculateDistances ()
        {
            if (parent != null) {
                if (bounds.Width >= 0)
                    dist_right = parent.ClientSize.Width - bounds.X - bounds.Width;
                if (bounds.Height >= 0)
                    dist_bottom = parent.ClientSize.Height - bounds.Y - bounds.Height;

                recalculate_distances = false;
            }
        }

        internal Size ScaleSize (Size startSize, float x, float y)
        {
            var size = startSize;

            size.Width = (int)Math.Round ((float)size.Width * x);
            size.Height = (int)Math.Round ((float)size.Height * y);

            return size;
        }

        // Used to break a StackOverflow circular reference
        internal void SetParentInternal (Control control)
        {
            parent = control;
        }

        internal bool UseAnchorLayoutInternal { get; private set; } = true;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                FreeBitmap ();

                foreach (var c in Controls)
                    c.Dispose (disposing);

                disposedValue = true;
            }
        }

        ~Control ()
        {
            Dispose (false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }
        #endregion
    }
}
