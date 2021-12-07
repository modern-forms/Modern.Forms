using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class for all Controls.
    /// </summary>
    public class Control : Component, ILayoutable, IDisposable
    {
        private AnchorStyles anchor_style = AnchorStyles.Top | AnchorStyles.Left;
        //private AutoSizeMode auto_size_mode;
        private SKBitmap? back_buffer;
        private ControlBehaviors behaviors;
        private Rectangle bounds;
        private Control? current_mouse_in;
        private Cursor? cursor;
        internal int dist_right;
        internal int dist_bottom;
        private DockStyle dock_style;
        private bool is_captured;
        private bool is_dirty = true;
        private bool is_enabled = true;
        private bool is_visible = true;
        private int layout_suspended;
        private bool layout_pending;
        private Padding margin;
        private Padding padding;
        private Control? parent;
        private bool recalculate_distances = true;
        private int tab_index = -1;
        private bool tab_stop = true;
        private string text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Control class.
        /// </summary>
        public Control ()
        {
            Controls = new ControlCollection (this);

            margin = DefaultMargin;
            padding = DefaultPadding;

            bounds = new Rectangle (Point.Empty, DefaultSize);

            behaviors = ControlBehaviors.Selectable;

            Cursor = DefaultCursor;

            Theme.ThemeChanged += (o, e) => is_dirty = true;
        }

        /// <summary>
        /// Gets or sets a value indicating which sides of the control are anchored when its parent resizes.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating if this control's size can be changed automatically.
        /// </summary>
        public bool AutoSize => false;

        /// <summary>
        /// Gets the unscaled bottom location of the control.
        /// </summary>
        public int Bottom => bounds.Bottom;

        /// <summary>
        /// Gets or sets the unscaled bounds of the control.
        /// </summary>
        public Rectangle Bounds {
            get => bounds;
            set => SetBounds (value.Left, value.Top, value.Width, value.Height);
        }

        /// <summary>
        /// Moves this control to the front zorder.
        /// </summary>
        public void BringToFront ()
        {
            if (parent != null)
                parent.Controls.SetChildIndex (this, 0);
        }

        /// <summary>
        /// Gets a value indicating the control can receive focus.
        /// </summary>
        public bool CanSelect {
            get {
                if (!behaviors.HasFlag (ControlBehaviors.Selectable))
                    return false;

                var parent = (Control?)this;

                while (parent != null) {
                    if (!parent.Visible || !parent.Enabled)
                        return false;

                    parent = parent.Parent;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the control is currently getting system mouse events.
        /// </summary>
        public bool Capture {
            get => is_captured || Controls.Any (c => c.Capture);
            set {
                is_captured = value;

                if (Parent != null)
                    Parent.Capture = value;
            }
        }

        /// <summary>
        /// Raised when this control is clicked.
        /// </summary>
        public event EventHandler<MouseEventArgs>? Click;

        /// <summary>
        /// Gets the scaled bounds of the control's canvas minus any borders.
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

        /// <summary>
        /// Gets the scaled size of the control.
        /// </summary>
        public Size ClientSize => ClientRectangle.Size;

        /// <summary>
        /// Gets a value indicating if the specified control is parented to this control or any of its children.
        /// </summary>
        public bool Contains (Control control)
        {
            var start = (Control?)control;

            // Is control one of our children or grandchildren
            while (start != null) {
                start = start.Parent;

                if (start == this)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the context menu that will be shown for the control.
        /// </summary>
        public ContextMenu? ContextMenu { get; set; }

        /// <summary>
        /// Gets the collection of controls contained by the control.
        /// </summary>
        public ControlCollection Controls { get; }

        /// <summary>
        /// Gets the current style of this control instance.
        /// </summary>
        public virtual ControlStyle CurrentStyle => IsHovering && Enabled ? StyleHover : Style;

        /// <summary>
        /// Gets or sets the mouse cursor to be shown when the mouse is over the control.
        /// </summary>
        public Cursor Cursor {
            get => cursor ?? Parent?.Cursor ?? Cursor.Default;
            set {
                if (cursor != value) {
                    cursor = value;
                    OnCursorChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the Cursor property is changed.
        /// </summary>
        public event EventHandler? CursorChanged;

        /// <summary>
        /// Gets the default cursor.
        /// </summary>
        protected virtual Cursor DefaultCursor => Cursor.Default;

        /// <summary>
        /// Gets the default margin of the control.
        /// </summary>
        protected virtual Padding DefaultMargin => new Padding (3);

        /// <summary>
        /// Gets the default padding of the control.
        /// </summary>
        protected virtual Padding DefaultPadding => Padding.Empty;

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        protected virtual Size DefaultSize => Size.Empty;

        /// <summary>
        /// Gets the default style for all controls of this type.
        /// </summary>
        public static ControlStyle DefaultStyle = new ControlStyle (null,
            (style) => {
                style.ForegroundColor = Theme.PrimaryTextColor;
                style.BackgroundColor = Theme.NeutralGray;
                style.Font = Theme.UIFont;
                style.FontSize = Theme.FontSize;
                //style.Border.Radius = 0;
                style.Border.Color = Theme.BorderGray;
                style.Border.Width = 0;
            });

        /// <summary>
        /// Gets the default style for all controls of this type when the user is hovering over it.
        /// </summary>
        public static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Removes focus from the control.
        /// </summary>
        internal void Deselect ()
        {
            Selected = false;
            OnDeselected (EventArgs.Empty);

            Invalidate ();
        }

        /// <summary>
        /// Gets the DPI of the current monitor.
        /// </summary>
        public int DeviceDpi => (int)((FindWindow ()?.Scaling ?? 1) * 96);

        /// <summary>
        /// Gets the scaled bounds of the displayed control.
        /// </summary>
        public virtual Rectangle DisplayRectangle => ClientRectangle;

        /// <summary>
        /// Gets or sets which side the control is docked to.
        /// </summary>
        public virtual DockStyle Dock {
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

        /// <summary>
        /// Raised when the Dock property is changed.
        /// </summary>
        public event EventHandler? DockChanged;

        /// <summary>
        /// Raised when this control is double-clicked.
        /// </summary>
        public event EventHandler<MouseEventArgs>? DoubleClick;

        /// <summary>
        /// Gets or sets whether the control can be interacted with.
        /// </summary>
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
                Invalidate ();
            }
        }

        /// <summary>
        /// Raised when the Enabled property is changed.
        /// </summary>
        public event EventHandler? EnabledChanged;

        /// <summary>
        /// Gets the ControlAdapter the control is parented to.
        /// </summary>
        private ControlAdapter? FindAdapter ()
        {
            if (this is ControlAdapter adapter)
                return adapter;

            return Parent?.FindAdapter ();
        }

        /// <summary>
        /// Gets the Form that the control is parented to.
        /// </summary>
        public Form? FindForm ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is Form f)
                return f;

            return Parent?.FindForm ();
        }

        /// <summary>
        /// Gets the Window that the control is parented to. (Different from FindForm because it may return a PopupWindow.)
        /// </summary>
        internal Window? FindWindow ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is Window w)
                return w;

            return Parent?.FindWindow ();
        }

        /// <summary>
        /// Gets whether this control currently has keyboard focus.
        /// </summary>
        public bool Focused => Selected;

        /// <summary>
        /// Releases the back buffer.
        /// </summary>
        private void FreeBackBuffer ()
        {
            if (back_buffer != null) {
                back_buffer.Dispose ();
                back_buffer = null;
            }
        }

        /// <summary>
        /// Gets or creates a back buffer for rendering the control.
        /// </summary>
        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer is null || back_buffer.Width != ScaledSize.Width || back_buffer.Height != ScaledSize.Height) {
                FreeBackBuffer ();
                back_buffer = new SKBitmap (ScaledSize.Width, ScaledSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                is_dirty = true;
            }

            return back_buffer;
        }

        internal virtual Control? GetFirstChildControlInTabOrder (bool forward, bool includeImplicit)
        {
            Control? found = null;

            var controls = Controls.GetAllControls (includeImplicit).ToArray ();

            if (forward) {
                for (var c = 0; c < controls.Length; c++) {
                    if (found == null || found.TabIndex > controls[c].TabIndex)
                        found = controls[c];
                }
            } else {
                // Cycle through the controls in reverse z-order looking for the one with the highest
                // tab index.
                for (var c = controls.Length - 1; c >= 0; c--) {
                    if (found == null || found.TabIndex < controls[c].TabIndex)
                        found = controls[c];
                }
            }

            return found;
        }

        /// <summary>
        /// Gets the next control in tab order.
        /// </summary>
        /// <param name="start">The control to start from.</param>
        /// <param name="forward">True to get the next control, false to get the previous control.</param>
        public Control? GetNextControl (Control? start, bool forward = true)
            => GetNextControl (start, forward, false);

        // Ported from MS Winforms
        private Control? GetNextControl (Control? start, bool forward, bool includeImplicit)
        {
            if (start is null || !Contains (start))
                start = this;

            if (forward) {
                if (start.Controls.GetAllControls (includeImplicit).Count () > 0 && (start == this || !IsFocusManagingContainerControl (start))) {
                    var found = start.GetFirstChildControlInTabOrder (true, includeImplicit);

                    if (found != null)
                        return found;
                }

                while (start != this) {
                    var target_index = start.TabIndex;
                    var hit_control = false;
                    Control? found = null;

                    var p = start.Parent;

                    // Cycle through the controls in z-order looking for the one with the next highest
                    // tab index.  Because there can be dups, we have to start with the existing tab index and
                    // remember to exclude the current control.
                    var parent_controls = p?.Controls.GetAllControls (includeImplicit).ToArray ();
                    var parent_control_count = parent_controls?.Length ?? 0;

                    for (var c = 0; c < parent_control_count; c++) {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // clauses:

                        // We are not interested in ourself.
                        if (parent_controls![c] != start) {

                            // We are interested in controls with >= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            if (parent_controls![c].TabIndex >= target_index) {
                                // Check to see if this control replaces the "best match" we've already found.
                                if (found is null || found.TabIndex > parent_controls![c].TabIndex) {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    if (parent_controls![c].TabIndex != target_index || hit_control)
                                        found = parent_controls![c];
                                }
                            }
                        } else {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            hit_control = true;
                        }
                    }

                    if (found != null)
                        return found;

                    start = start.Parent!;
                }
            } else {

                if (start != this) {
                    var target_index = start.TabIndex;
                    var hit_control = false;
                    Control? found = null;

                    var p = start.Parent;

                    // Cycle through the controls in reverse z-order looking for the next lowest tab index.  We must
                    // start with the same tab index as ctl, because there can be dups.
                    var parent_controls = p?.Controls.GetAllControls (includeImplicit).ToArray ();
                    var parent_control_count = parent_controls?.Length ?? 0;

                    for (var c = parent_control_count - 1; c >= 0; c--) {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // clauses:

                        // We are not interested in ourself.
                        if (parent_controls![c] != start) {
                            // We are interested in controls with <= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            if (parent_controls![c].TabIndex <= target_index) {
                                // Check to see if this control replaces the "best match" we've already found.
                                if (found is null || found.TabIndex < parent_controls![c].TabIndex) {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    if (parent_controls![c].TabIndex != target_index || hit_control)
                                        found = parent_controls![c];
                                }
                            }
                        } else {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            hit_control = true;
                        }
                    }

                    // If we were unable to find a control we should return the control's parent.  
                    // However, if that parent is us, return NULL.
                    if (found != null)
                        start = found;
                    else
                        return p == this ? null : p;
                }

                // We found a control.  Walk into this control to find the proper sub control within it to select.
                var control_controls = start.Controls.GetAllControls (includeImplicit).ToArray ();

                while (control_controls.Length > 0 && (start == this || !IsFocusManagingContainerControl (start))) {
                    var found = start.GetFirstChildControlInTabOrder (false, includeImplicit);

                    if (found != null) {
                        start = found;
                        control_controls = start.Controls.GetAllControls (includeImplicit).ToArray ();
                    } else {
                        break;
                    }
                }

            }
            return start == this ? null : start;
        }

        private static bool IsFocusManagingContainerControl (Control ctl)
        {
            return false;// ((ctl._controlStyle & ControlStyles.ContainerControl) == ControlStyles.ContainerControl && ctl is IContainerControl);
        }

        /// <summary>
        /// Gets the size the control would prefer to be.
        /// </summary>
        /// <param name="proposedSize">A size the layout engine is proposing for the control.</param>
        public virtual Size GetPreferredSize (Size proposedSize) => new Size (Width, Height);

        /// <summary>
        /// Scales bounds by a specified factor.
        /// </summary>
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

        /// <summary>
        /// Raised when the control receives focus.
        /// </summary>
        public event EventHandler? GotFocus;

        /// <summary>
        /// Gets a value indicating if control contains any child controls.
        /// </summary>
        public bool HasChildren => Controls.Count > 0;

        /// <summary>
        /// Gets or sets the unscaled height of the control.
        /// </summary>
        public int Height {
            get => bounds.Height;
            set => SetBounds (bounds.X, bounds.Y, bounds.Width, value, BoundsSpecified.Height);
        }

        /// <summary>
        /// Hide this control from the user.
        /// </summary>
        public void Hide ()
        {
            Visible = false;
        }

        /// <summary>
        /// Marks the entire control as needing to be redrawn.
        /// </summary>
        public void Invalidate () => Invalidate (Bounds);

        /// <summary>
        /// Marks the specified portion of the control as needing to be redrawn.
        /// </summary>
        /// <param name="rectangle">The portion of the control to be redrawn.</param>
        public void Invalidate (Rectangle rectangle)
        {
            is_dirty = true;

            FindWindow ()?.Invalidate (rectangle);
        }

        /// <summary>
        /// Is the mouse currently over the control.
        /// </summary>
        private bool IsHovering { get; set; }

        /// <summary>
        /// Raised when the user presses down a key.
        /// </summary>
        public event EventHandler<KeyEventArgs>? KeyDown;

        /// <summary>
        /// Raised when the user presses a key.
        /// </summary>
        public event EventHandler<KeyPressEventArgs>? KeyPress;

        /// <summary>
        /// Raised when the user releases a key.
        /// </summary>
        public event EventHandler<KeyEventArgs>? KeyUp;

        /// <summary>
        /// Raised when the control performs a layout.
        /// </summary>
        public event EventHandler<LayoutEventArgs>? Layout;

        /// <summary>
        /// Gets or sets the unscaled left boundary of the control.
        /// </summary>
        public int Left {
            get => bounds.Left;
            set => SetBounds (value, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.X);
        }

        /// <summary>
        /// Gets or sets the unscaled location of the control.
        /// </summary>
        public Point Location {
            get => bounds.Location;
            set => SetBounds (value.X, value.Y, bounds.Width, bounds.Height, BoundsSpecified.Location);
        }

        /// <summary>
        /// Raised when the Location property is changed.
        /// </summary>
        public event EventHandler? LocationChanged;

        /// <summary>
        /// Converts an unscaled value to a scaled value.
        /// </summary>
        public int LogicalToDeviceUnits (int value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        /// <summary>
        /// Converts an unscaled Padding to a scaled Padding.
        /// </summary>
        public Padding LogicalToDeviceUnits (Padding value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        /// <summary>
        /// Converts an unscaled Size to a scaled Size.
        /// </summary>
        public Size LogicalToDeviceUnits (Size value)
        {
            return DpiHelper.LogicalToDeviceUnits (value, DeviceDpi);
        }

        /// <summary>
        /// Internal control (like a scrollbar) that should not show up in Controls for a user.
        /// </summary>
        internal bool ImplicitControl { get; set; }

        /// <summary>
        /// Gets or sets how much space there should be between the control and other controls.
        /// </summary>
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

        /// <summary>
        /// Raised when the Margin property is changed.
        /// </summary>
        public event EventHandler? MarginChanged;

        /// <summary>
        /// Raised when a mouse button is pressed.
        /// </summary>
        public event EventHandler<MouseEventArgs>? MouseDown;

        /// <summary>
        /// Raised when the mouse cursor enters the control.
        /// </summary>
        public event EventHandler<MouseEventArgs>? MouseEnter;

        /// <summary>
        /// Raised when the mouse cursor leaves the control.
        /// </summary>
        public event EventHandler? MouseLeave;

        /// <summary>
        /// Raised when the mouse cursor is moved within the control.
        /// </summary>
        public event EventHandler<MouseEventArgs>? MouseMove;

        /// <summary>
        /// Raised when a mouse button ir released.
        /// </summary>
        public event EventHandler<MouseEventArgs>? MouseUp;

        /// <summary>
        /// Raised when a mouse wheel is rotated.
        /// </summary>
        public event EventHandler<MouseEventArgs>? MouseWheel;

        /// <summary>
        /// Gets or sets a user specified name for the control.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Whether the control needs to be repainted.
        /// </summary>
        internal bool NeedsPaint => is_dirty || Controls.GetAllControls ().Any (c => c.NeedsPaint);

        /// <summary>
        /// The full control canvas.
        /// </summary>
        internal virtual Rectangle NonClientRectangle {
            get {
                var bounds = GetScaledBounds (Bounds, ScaleFactor, BoundsSpecified.All);
                return new Rectangle (0, 0, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// Raises the OnClick event.
        /// </summary>
        protected virtual void OnClick (MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && ContextMenu != null) {
                ContextMenu.Show (PointToScreen (e.Location));
                return;
            }

            Click?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the CursorChanged event.
        /// </summary>
        protected virtual void OnCursorChanged (EventArgs e) => CursorChanged?.Invoke (this, e);

        /// <summary>
        /// Called when the control is deselected.
        /// </summary>
        protected virtual void OnDeselected (EventArgs e) { }

        /// <summary>
        /// Raises the DockChanged event.
        /// </summary>
        protected virtual void OnDockChanged (EventArgs e) => DockChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the DoubleClick event.
        /// </summary>
        protected virtual void OnDoubleClick (MouseEventArgs e) => DoubleClick?.Invoke (this, e);

        /// <summary>
        /// Raises the EnabledChanged event.
        /// </summary>
        protected virtual void OnEnabledChanged (EventArgs e) => EnabledChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        protected virtual void OnGotFocus (EventArgs e) => GotFocus?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        protected virtual void OnKeyDown (KeyEventArgs e) => KeyDown?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyPress event.
        /// </summary>
        protected virtual void OnKeyPress (KeyPressEventArgs e) => KeyPress?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyUp event.
        /// </summary>
        protected virtual void OnKeyUp (KeyEventArgs e) => KeyUp?.Invoke (this, e);

        /// <summary>
        /// Raises the Layout event.
        /// </summary>
        protected virtual void OnLayout (LayoutEventArgs e)
        {
            Layout?.Invoke (this, e);

            DefaultLayout.Instance.Layout (this, e);
        }

        /// <summary>
        /// Raises the LocationChanged event.
        /// </summary>
        protected virtual void OnLocationChanged (EventArgs e) => LocationChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the MarginChanged event.
        /// </summary>
        protected virtual void OnMarginChanged (EventArgs e) => MarginChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        protected virtual void OnMouseDown (MouseEventArgs e) => MouseDown?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseEnter event.
        /// </summary>
        protected virtual void OnMouseEnter (MouseEventArgs e)
        {
            FindForm ()?.SetCursor (Cursor);

            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = true;
                Invalidate ();
            }

            MouseEnter?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        protected virtual void OnMouseLeave (EventArgs e)
        {
            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = false;
                Invalidate ();
            }

            MouseLeave?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the MouseMove event.
        /// </summary>
        protected virtual void OnMouseMove (MouseEventArgs e) => MouseMove?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseUp event.
        /// </summary>
        protected virtual void OnMouseUp (MouseEventArgs e) => MouseUp?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseWheel event.
        /// </summary>
        protected virtual void OnMouseWheel (MouseEventArgs e) => MouseWheel?.Invoke (this, e);

        /// <summary>
        /// Raises the PaddingChanged event.
        /// </summary>
        protected virtual void OnPaddingChanged (EventArgs e) => PaddingChanged?.Invoke (this, e);

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected virtual void OnPaint (PaintEventArgs e)
        {
            foreach (var control in Controls.GetAllControls ().Where (c => c.Visible)) {
                if (control.Width <= 0 || control.Height <= 0)
                    continue;

                var info = new SKImageInfo (control.ScaledSize.Width, control.ScaledSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                if (control.NeedsPaint) {
                    using (var canvas = new SKCanvas (buffer)) {
                        // start drawing
                        var args = new PaintEventArgs (info, canvas, Scaling);

                        control.RaisePaintBackground (args);
                        control.RaisePaint (args);

                        canvas.Flush ();
                    }
                }

                e.Canvas.DrawBitmap (buffer, control.ScaledLeft, control.ScaledTop);
            }
        }

        /// <summary>
        /// Paints the control's background.
        /// </summary>
        protected virtual void OnPaintBackground (PaintEventArgs e)
        {
            // The ControlAdapter itself should not have a background/border
            if (this is ControlAdapter)
                return;

            // Transparent controls should not draw a background or border
            if (behaviors.HasFlag (ControlBehaviors.Transparent)) {
                e.Canvas.Clear ();
                return;
            }

            e.Canvas.DrawBackground (CurrentStyle);
            e.Canvas.DrawBorder (ScaledBounds, CurrentStyle);
        }

        /// <summary>
        /// Called when the Parent's Visible property is changed.
        /// </summary>
        protected virtual void OnParentVisibleChanged (EventArgs e)
        {
            if (Visible)
                OnVisibleChanged (e);
        }

        /// <summary>
        /// Raises the SizeChanged event.
        /// </summary>
        protected virtual void OnSizeChanged (EventArgs e) => SizeChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the TabIndexChanged event.
        /// </summary>
        protected virtual void OnTabIndexChanged (EventArgs e) => TabIndexChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the TabStopChanged event.
        /// </summary>
        protected virtual void OnTabStopChanged (EventArgs e) => TabStopChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        protected virtual void OnTextChanged (EventArgs e) => TextChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the VisibleChanged event.
        /// </summary>
        protected virtual void OnVisibleChanged (EventArgs e)
        {
            VisibleChanged?.Invoke (this, e);

            foreach (var c in Controls.GetAllControls ())
                c.OnParentVisibleChanged (e);

            if (Visible)
                PerformLayout (this, nameof (Visible));
        }

        /// <summary>
        /// The scaled control canvas minus any borders and Padding.
        /// </summary>
        public virtual Rectangle PaddedClientRectangle {
            get {
                var client_rect = ClientRectangle;

                var x = client_rect.Left + Padding.Left;
                var y = client_rect.Top + Padding.Top;
                var w = client_rect.Width - Padding.Horizontal;
                var h = client_rect.Height - Padding.Vertical;
                return new Rectangle (x, y, w, h);
            }
        }

        /// <summary>
        /// Gets or sets the amount of space there should be between the control bounds and the control contents.
        /// </summary>
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

        /// <summary>
        /// Raised when the Padding property is changed.
        /// </summary>
        public event EventHandler? PaddingChanged;

        /// <summary>
        /// Gets or sets the control that contains this control.
        /// </summary>
        public Control? Parent {
            get => parent;
            set {
                if (value == this)
                    throw new ArgumentException ("Control cannot be its own Parent.");

                if (parent == value)
                    return;

                if (value == null) {
                    parent?.Controls.Remove (this);
                    parent = null;
                    return;
                }

                value.Controls.Add (this);
            }
        }

        /// <summary>
        /// Triggers the control to layout its children.
        /// </summary>
        public void PerformLayout () => PerformLayout (null, string.Empty);

        /// <summary>
        /// Triggers the control to layout its children.
        /// </summary>
        /// <param name="affectedControl">The control causing the layout.</param>
        /// <param name="affectedProperty">The property causing the layout.</param>
        public void PerformLayout (Control? affectedControl, string affectedProperty)
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

        /// <summary>
        /// Gets the size the control would prefer to be.
        /// </summary>
        public Size PreferredSize => GetPreferredSize (Size.Empty);

        /// <summary>
        /// Converts a point from control coordinates to monitor coordinates.
        /// </summary>
        public Point PointToScreen (Point point)
        {
            // If this is the top, add the point to our location
            if (this is ControlAdapter) {
                var window_location = FindWindow ()?.Location;

                if (window_location == null)
                    return point;

                var location = window_location.Value;
                location.Offset (point);

                return location;
            }

            // If this isn't the top, we need to add our location to the point
            // and ask our parent to translate that
            point.Offset (ScaledBounds.Location);

            // If we aren't parented to a Form, this method is pretty meaningless
            return Parent?.PointToScreen (point) ?? point;
        }

        /// <summary>
        /// Finds the correct control and calls its OnClick method.
        /// </summary>
        internal void RaiseClick (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseClick (TranslateMouseEvents (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseClick (TranslateMouseEvents (e, child));
            else if (Enabled)
                OnClick (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnDoubleClick method.
        /// </summary>
        internal void RaiseDoubleClick (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseDoubleClick (TranslateMouseEvents (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseDoubleClick (TranslateMouseEvents (e, child));
            else if (Enabled)
                OnDoubleClick (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnKeyDown method.
        /// </summary>
        internal void RaiseKeyDown (KeyEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                adapter.SelectedControl?.RaiseKeyDown (e);
                return;
            }

            if (Enabled)
                OnKeyDown (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnKeyPress method.
        /// </summary>
        internal void RaiseKeyPress (KeyPressEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                // Tab
                if (e.KeyChar == 9) {
                    if (adapter.FindForm () is Form f)
                        f.ShowFocusCues = true;

                    SelectNextControl (adapter.SelectedControl, !e.Shift, true, true, true);
                    e.Handled = true;
                    return;
                }

                adapter.SelectedControl?.RaiseKeyPress (e);
                return;
            }

            if (Enabled)
                OnKeyPress (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseDown method.
        /// </summary>
        internal void RaiseMouseDown (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (TranslateMouseEvents (e, child));
            else {
                // If we're clicking on the a Control that isn't the active menu, 
                // we need to close the active menu (if any)
                if ((this as MenuBase)?.GetTopLevelMenu () != Application.ActiveMenu)
                    Application.ActiveMenu?.Deactivate ();

                if (Enabled) {
                    Select ();
                    Capture = true;
                    OnMouseDown (e);
                }
            }
        }

        /// <summary>
        /// Finds the correct control and calls its OnKeyUp method.
        /// </summary>
        internal void RaiseKeyUp (KeyEventArgs e)
        {
            if (this is ControlAdapter adapter) {
                adapter.SelectedControl?.RaiseKeyUp (e);
                return;
            }

            OnKeyUp (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseEnter method.
        /// </summary>
        internal void RaiseMouseEnter (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseEnter (TranslateMouseEvents (e, child));
            else if (Enabled)
                OnMouseEnter (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseLeave method.
        /// </summary>
        internal void RaiseMouseLeave (EventArgs e)
        {
            if (current_mouse_in != null)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = null;

            if (Enabled)
                OnMouseLeave (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseMove method.
        /// </summary>
        internal void RaiseMouseMove (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseMouseMove (TranslateMouseEvents (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (current_mouse_in != null && current_mouse_in != child) {
                current_mouse_in.RaiseMouseLeave (e);
                current_mouse_in = null;

                // If we are leaving a child and not entering another child,
                // we need to raise MouseEnter on this control
                if (child == null)
                    OnMouseEnter (e);
            }

            if (current_mouse_in == null && child != null)
                child.RaiseMouseEnter (TranslateMouseEvents (e, child));

            current_mouse_in = child;

            if (child != null)
                child?.RaiseMouseMove (TranslateMouseEvents (e, child));
            else if (Enabled)
                OnMouseMove (e);
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseUp method.
        /// </summary>
        internal void RaiseMouseUp (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.GetAllControls ().LastOrDefault (c => c.Capture);

            if (captured != null) {
                captured.RaiseMouseUp (TranslateMouseEvents (e, captured));
                return;
            }

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseUp (TranslateMouseEvents (e, child));
            else {
                if (Enabled) {
                    Capture = false;
                    OnMouseUp (e);
                }
            }
        }

        /// <summary>
        /// Finds the correct control and calls its OnMouseWheel method.
        /// </summary>
        internal void RaiseMouseWheel (MouseEventArgs e)
        {
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseWheel (TranslateMouseEvents (e, child));
            else if (Enabled)
                OnMouseWheel (e);
        }

        /// <summary>
        /// Calls the OnPaint method.
        /// </summary>
        internal void RaisePaint (PaintEventArgs e)
        {
            OnPaint (e);

            is_dirty = false;
        }

        /// <summary>
        /// Calls the OnPaintBackground method.
        /// </summary>
        internal void RaisePaintBackground (PaintEventArgs e) => OnPaintBackground (e);

        /// <summary>
        /// Calculates the distances needed for anchored controls.
        /// </summary>
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

        /// <summary>
        /// Notifies the control to result performing layouts originally suspended with SuspendLayout.
        /// </summary>
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

        /// <summary>
        /// Gets the unscaled right boundary of the control.
        /// </summary>
        public int Right => bounds.Right;

        /// <summary>
        /// Scales the control by the specified factor.
        /// </summary>
        public void Scale (SizeF factor) => ScaleCore (factor.Width, factor.Height);

        /// <summary>
        /// Scales the control by the specified factor.
        /// </summary>
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

        /// <summary>
        /// Gets the scaled bounds of the control.
        /// </summary>
        public Rectangle ScaledBounds => GetScaledBounds (Bounds, ScaleFactor, BoundsSpecified.All);

        /// <summary>
        /// Gets the scaled height of the control.
        /// </summary>
        public int ScaledHeight => (int)(Height * ScaleFactor.Height);

        /// <summary>
        /// Gets the scaled left of the control.
        /// </summary>
        public int ScaledLeft => (int)(Left * ScaleFactor.Width);

        /// <summary>
        /// Gets the scaled size of the control.
        /// </summary>
        public Size ScaledSize => ScaledBounds.Size;

        /// <summary>
        /// Gets the scaled top of the control.
        /// </summary>
        public int ScaledTop => (int)(Top * ScaleFactor.Height);

        /// <summary>
        /// Gets the scaled width of the control.
        /// </summary>
        public int ScaledWidth => (int)(Width * ScaleFactor.Width);

        /// <summary>
        /// Gets the current scale factor of the control.
        /// </summary>
        public SizeF ScaleFactor => new SizeF ((float)(DeviceDpi / DpiHelper.LogicalDpi), (float)(DeviceDpi / DpiHelper.LogicalDpi));

        /// <summary>
        /// Gets the current scale factor of the form.
        /// </summary>
        public double Scaling => FindWindow ()?.Scaling ?? 1;

        /// <summary>
        /// Gives the control focus.
        /// </summary>
        public void Select ()
        {
            if (Selected || !CanSelect)
                return;

            Selected = true;

            OnGotFocus (EventArgs.Empty);

            var adapter = FindAdapter ();

            if (adapter != null)
                adapter.SelectedControl = this;

            Invalidate ();
        }

        /// <summary>
        /// Gets a value indicating the control has focus.
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// Moves focus to the next control.
        /// </summary>
        /// <param name="start">The control to start from.</param>
        /// <param name="forward">True to move focus to the next control, false for the previous control.</param>
        /// <param name="tabStopOnly">True to only move focus to controls with TabStop set to true, false for all selectable controls.</param>
        /// <param name="nested">True to recurse into the control's children, false for only this control's children.</param>
        /// <param name="wrap">True to wrap around if the end is found, false to not select a control if the end is hit.</param>
        /// <returns>A value indicating if a control was selected.</returns>
        public bool SelectNextControl (Control? start, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            Control? c;

            if (start == null || !Contains (start) || (!nested && (start.Parent != this)))
                start = null;

            c = start;

            do {
                c = GetNextControl (c, forward, true);

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

        /// <summary>
        /// Sends this control to the back of the zorder.
        /// </summary>
        public void SendToBack ()
        {
            if (parent != null)
                parent.Controls.SetChildIndex (this, parent.Controls.Count);
        }

        /// <summary>
        /// Sets the unscaled bounds of the control.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            SetBoundsCore (x, y, width, height, specified);
        }

        /// <summary>
        /// A version of SetBounds that can be overridden by subclasses.
        /// </summary>
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

        /// <summary>
        /// Sets behavior flags.
        /// </summary>
        protected void SetControlBehavior (ControlBehaviors behavior, bool value = true)
        {
            if (value)
                behaviors |= behavior;
            else
                behaviors &= ~behavior;
        }
        /// <summary>
        /// Used to break a StackOverflow circular reference
        /// </summary>
        internal void SetParentInternal (Control? control)
        {
            var was_visible = Visible;

            parent = control;

            if (Visible != was_visible)
                OnVisibleChanged (EventArgs.Empty);
        }

        /// <summary>
        /// Sets the bounds of the control from scaled dimensions.
        /// </summary>
        internal void SetScaledBounds (int x, int y, int width, int height, BoundsSpecified specified)
        {
            var rect = GetScaledBounds (new Rectangle (x, y, width, height), new SizeF (1 / ScaleFactor.Width, 1 / ScaleFactor.Height), BoundsSpecified.All);
            SetBoundsCore (rect.X, rect.Y, rect.Width, rect.Height, BoundsSpecified.None);
        }

        /// <summary>
        /// Shows this control to the user.
        /// </summary>
        public void Show ()
        {
            Visible = true;
        }

        /// <summary>
        /// Gets a value indicating a focus rectangle should be drawn on the selected control.
        /// </summary>
        public bool ShowFocusCues => FindForm ()?.ShowFocusCues == true;

        /// <summary>
        /// Gets or sets the unscaled size of the control.
        /// </summary>
        public Size Size {
            get => bounds.Size;
            set => SetBounds (bounds.X, bounds.Y, value.Width, value.Height, BoundsSpecified.Size);
        }

        /// <summary>
        /// Raised when the Size property is changed.
        /// </summary>
        public event EventHandler? SizeChanged;

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control.
        /// </summary>
        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control when the user is hovering over it.
        /// </summary>
        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        /// <summary>
        /// Pauses performing layouts until ResumeLayout is called.
        /// </summary>
        public void SuspendLayout () => layout_suspended++;

        /// <summary>
        /// Gets or sets a value indicating the order the control is selected when pressing tab.
        /// </summary>
        public int TabIndex {
            get => tab_index != -1 ? tab_index : 0;
            set {
                if (tab_index != value) {
                    tab_index = value;
                    OnTabIndexChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the TabIndex property is changed.
        /// </summary>
        public event EventHandler? TabIndexChanged;

        /// <summary>
        /// Gets or sets whether the control is selectable via pressing tab.
        /// </summary>
        public bool TabStop {
            get => tab_stop;
            set {
                if (tab_stop != value) {
                    tab_stop = value;
                    OnTabStopChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the TabStop property is changed.
        /// </summary>
        public event EventHandler? TabStopChanged;

        /// <summary>
        /// Gets or sets user defined data.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the text of the control.
        /// </summary>
        public virtual string Text {
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

        /// <summary>
        /// Raised when the Text property is changed.
        /// </summary>
        public event EventHandler? TextChanged;

        /// <summary>
        /// Gets or sets the unscaled top boundary of the control.
        /// </summary>
        public int Top {
            get => bounds.Top;
            set => SetBounds (bounds.X, value, bounds.Width, bounds.Height, BoundsSpecified.Y);
        }

        /// <summary>
        /// Changes mouse events to control coordinates.
        /// </summary>
        private MouseEventArgs TranslateMouseEvents (MouseEventArgs e, Control control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.ScaledLeft, e.Location.Y - control.ScaledTop, e.Delta, e.Location.X, e.Location.Y, e.Modifiers);
        }

        /// <summary>
        /// Indicates whether to use anchor or dock layout.
        /// </summary>
        internal bool UseAnchorLayoutInternal { get; private set; } = true;

        /// <summary>
        /// Gets or sets whether the control is displayed to the user.
        /// </summary>
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

        /// <summary>
        /// Raised when the Visisble property is changed.
        /// </summary>
        public event EventHandler? VisibleChanged;

        /// <summary>
        /// Gets or sets the unscaled width of the control.
        /// </summary>
        public int Width {
            get => bounds.Width;
            set => SetBounds (bounds.X, bounds.Y, value, bounds.Height, BoundsSpecified.Width);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes unmanaged resources used by the control.
        /// </summary>
        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                FreeBackBuffer ();

                foreach (var c in Controls)
                    c.Dispose (disposing);

                disposedValue = true;
            }
        }

        /// <summary>
        /// Destroys the control.
        /// </summary>
        ~Control ()
        {
            Dispose (false);
        }

        /// <summary>
        /// Disposes unmanaged resources used by the control. This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }
        #endregion
    }
}
