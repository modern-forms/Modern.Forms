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

            SetBounds (0, 0, DefaultSize.Width, DefaultSize.Height, BoundsSpecified.Size);

            behaviors = ControlBehaviors.Selectable;
        }

        public event EventHandler<MouseEventArgs> Click;
        public event EventHandler CursorChanged;
        public event EventHandler DockChanged;
        public event EventHandler EnabledChanged;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyPressEventArgs> KeyPress;
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

                parent?.PerformLayout (this, "Anchor");
            }
        }

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
            get => is_captured;
            set {
                is_captured = value;

                if (Parent != null)
                    Parent.Capture = value;
            }
        }

        /// <summary>
        /// The control canvas minus any borders
        /// </summary>
        public Rectangle ClientRectangle {
            get {
                var x = CurrentStyle.Border.Left.GetWidth ();
                var y = CurrentStyle.Border.Top.GetWidth ();
                var w = Width - CurrentStyle.Border.Right.GetWidth () - x;
                var h = Height - CurrentStyle.Border.Bottom.GetWidth () - y;
                return new Rectangle (x, y, w, h);
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
                    Parent.PerformLayout (this, "Dock");
                else if (Controls.Count > 0)
                    PerformLayout (this, "Dock");

                OnDockChanged (EventArgs.Empty);
            }
        }

        public void DoLayout ()
        {
            DefaultLayout.Instance.Layout (this, null);

            foreach (var child in Controls)
                child.DoLayout ();
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
            if (this is ControlAdapter adapter)
                return adapter.ParentForm;

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
            FindForm ()?.Invalidate ();
        }

        public void Invalidate (Rectangle rectangle)
        {
            FindForm ()?.Invalidate (rectangle);
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

            foreach (var c in Controls)
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

        public void ResumeLayout (bool performLayout)
        {
            if (layout_suspended > 0)
                layout_suspended--;

            if (layout_suspended == 0) {
                if (!performLayout)
                    foreach (var c in Controls)
                        c.RecalculateDistances ();

                if (performLayout && layout_pending)
                    PerformLayout ();
            }
        }

        public int Right => bounds.Right;

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
                PerformLayout (this, "Bounds");
            }

            // If the user explicitly moved or resized us, recalculate our anchor distances
            if (specified != BoundsSpecified.None)
                RecalculateDistances ();

            parent?.PerformLayout (this, "Bounds");
        }

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

        public bool Visible {
            get {
                if (!is_visible)
                    return false;

                return parent?.Visible ?? true;
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

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.Left, e.Location.Y - control.Top, e.Delta, e.Modifiers);
        }

        internal void Deselect ()
        {
            Selected = false;
            Invalidate ();
        }

        internal void RaiseClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseClick (MouseEventsForControl (e, child));
            else
                OnClick (e);
        }

        protected virtual void OnClick (MouseEventArgs e) => Click?.Invoke (this, e);

        protected virtual void OnCursorChanged (EventArgs e) => CursorChanged?.Invoke (this, e);

        protected virtual void OnDockChanged (EventArgs e) => DockChanged?.Invoke (this, e);

        internal void RaiseDoubleClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

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
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (MouseEventsForControl (e, child));
            else {
                Select ();
                Capture = true;
                OnMouseDown (e);
            }
        }

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
        }

        internal void RaiseMouseEnter (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

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
            var captured = Controls.FirstOrDefault (c => c.is_captured);

            if (captured != null) {
                captured.RaiseMouseMove (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.FirstOrDefault (c => c.Visible && c.Bounds.Contains (e.Location));

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
            var captured = Controls.FirstOrDefault (c => c.is_captured);

            if (captured != null) {
                captured.RaiseMouseUp (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

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

        internal void RaisePaint (PaintEventArgs e) => OnPaint (e);

        protected virtual void OnPaint (PaintEventArgs e)
        {
            foreach (var control in Controls.Where (c => c.Visible)) {
                var info = new SKImageInfo (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                using (var canvas = new SKCanvas (buffer)) {
                    // start drawing
                    var args = new PaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, control.Left, control.Top);
            }
        }

        protected virtual void OnSizeChanged (EventArgs e) => SizeChanged?.Invoke (this, e);

        protected virtual void OnTabIndexChanged (EventArgs e) => TabIndexChanged?.Invoke (this, e);

        protected virtual void OnTabStopChanged (EventArgs e) => TabStopChanged?.Invoke (this, e);

        protected virtual void OnTextChanged (EventArgs e) => TextChanged?.Invoke (this, e);

        protected virtual void OnVisibleChanged (EventArgs e) => VisibleChanged?.Invoke (this, e);

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
            e.Canvas.DrawBackground (Bounds, CurrentStyle);
            e.Canvas.DrawBorder (Bounds, CurrentStyle);
        }

        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer == null || back_buffer.Width != Width || back_buffer.Height != Height) {
                FreeBitmap ();
                back_buffer = new SKBitmap (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            }

            return back_buffer;
        }

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
