using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Modern.Forms.Layout;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class for all Controls.
    /// </summary>
    public partial class Control : Component, ILayoutable, IArrangedElement, IDisposable
    {
        // Control instance members
        //
        // Note: Do not add anything to this list unless absolutely necessary.
        //       Every control on a form has the overhead of all of these
        //       variables!
        private Control? parent;
        private States _state = States.Visible | States.Enabled | States.TabStop | States.CausesValidation | States.IsDirty;
        private ExtendedStates _extendedState;
        private ControlBehaviors behaviors = ControlBehaviors.Selectable | ControlBehaviors.ReceivesMouseEvents;

        private int _x;
        private int _y;
        private int _width;
        private int _height;

        private int tab_index = -1;
        private string text = string.Empty;
        private byte layout_suspend_count;

        private SKBitmap? back_buffer;
        private Control? current_mouse_in;

        private bool is_captured;

        // Property store keys for properties.
        private static readonly int s_controlsCollectionProperty = PropertyStore.CreateKey ();
        private static readonly int s_contextMenuProperty = PropertyStore.CreateKey ();
        private static readonly int s_cursorProperty = PropertyStore.CreateKey ();
        private static readonly int s_namePropertyProperty = PropertyStore.CreateKey ();
        private static readonly int s_tagProperty = PropertyStore.CreateKey ();

        /// <summary>
        /// Initializes a new instance of the Control class.
        /// </summary>
        public Control ()
        {
            // We baked the "default default" margin and min size into CommonProperties
            // so that in the common case the PropertyStore would be empty.  If, however,
            // someone overrides these Default* methods, we need to write the default
            // value into the PropertyStore in the ctor.

            if (DefaultMargin != CommonProperties.DefaultMargin)
                Margin = DefaultMargin;

            if (DefaultMinimumSize != CommonProperties.DefaultMinimumSize)
                MinimumSize = DefaultMinimumSize;

            if (DefaultMaximumSize != CommonProperties.DefaultMaximumSize)
                MaximumSize = DefaultMaximumSize;

            var default_size = DefaultSize;

            _width = default_size.Width;
            _height = default_size.Height;

            Theme.ThemeChanged += OnThemeChanged;

        }

        private void OnThemeChanged (object? sender, EventArgs e) => SetState (States.IsDirty, true);

        /// <summary>
        ///  Assigns a new parent control. Sends out the appropriate property change
        ///  notifications for properties that are affected by the change of parent.
        /// </summary>
        internal virtual void AssignParent (Control? value)
        {
            // Adopt the parent's required scaling bits
            //if (value is not null) {
            //    RequiredScalingEnabled = value.RequiredScalingEnabled;
            //}

            // Store the old values for these properties
            var old_enabled = Enabled;
            var old_visible = Visible;

            // Update the parent
            parent = value;
            OnParentChanged (EventArgs.Empty);

            if (GetAnyDisposingInHierarchy ())
                return;

            // Compare property values with new parent to old values
            if (old_enabled != Enabled)
                OnEnabledChanged (EventArgs.Empty);

            // When a control seems to be going from invisible -> visible,
            // yet its parent is being set to null and it's not top level, do not raise OnVisibleChanged.
            var new_visible = Visible;

            if (old_visible != new_visible && !(!old_visible && new_visible && parent is null))
                OnVisibleChanged (EventArgs.Empty);

            //    if (Properties.GetObject (s_bindingManagerProperty) is null && Created) {
            //        // We do not want to call our parent's BindingContext property here.
            //        // We have no idea if us or any of our children are using data binding,
            //        // and invoking the property would just create the binding manager, which
            //        // we don't need.  We just blindly notify that the binding manager has
            //        // changed, and if anyone cares, they will do the comparison at that time.
            //        //
            //        OnBindingContextChanged (EventArgs.Empty);
            //    }

            if (Parent is not null)
                Parent.LayoutEngine.InitLayout (this, BoundsSpecified.All);
        }

        /// <summary>
        /// Gets the unscaled bottom location of the control.
        /// </summary>
        public int Bottom => _y + _height;

        /// <summary>
        /// Gets or sets the unscaled bounds of the control.
        /// </summary>
        public Rectangle Bounds {
            get => new Rectangle (_x, _y, _width, _height);
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
            get => is_captured || Controls.GetAllControls (true).Any (c => c.Capture);
            set {
                is_captured = value;

                if (Parent != null)
                    Parent.Capture = value;
            }
        }

        /// <summary>
        ///  Searches the parent/owner tree for bottom to find any instance
        ///  of toFind in the parent/owner tree.
        /// </summary>
        internal static void CheckParentingCycle (Control bottom, Control toFind)
        {
            ControlAdapter? lastOwner = null;
            Control? lastParent = null;

            for (var ctl = bottom; ctl is not null; ctl = ctl.Parent) {
                lastParent = ctl;

                if (ctl == toFind)
                    throw new ArgumentException (SR.CircularOwner);
            }

            if (lastParent is not null) {
                if (lastParent is ControlAdapter form) {
                    lastOwner = form;

                    if (form == toFind)
                        throw new ArgumentException (SR.CircularOwner);
                }
            }

            if (lastOwner?.Parent is not null)
                CheckParentingCycle (lastOwner.Parent, toFind);
        }

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
        public ContextMenu? ContextMenu {
            get => (ContextMenu?)Properties.GetObject (s_contextMenuProperty);
            set {
                if (value != ContextMenu) {
                    Properties.SetObject (s_contextMenuProperty, value);
                    OnContextMenuChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the collection of controls contained by the control.
        /// </summary>
        public ControlCollection Controls {
            get {
                var collection = (ControlCollection?)Properties.GetObject (s_controlsCollectionProperty);

                if (collection is null) {
                    collection = CreateControlsInstance ();
                    Properties.SetObject (s_controlsCollectionProperty, collection);
                }

                return collection;
            }
        }

        /// <summary>
        /// This doesn't do much because we don't have native window handles, but having
        /// the created state allows us to avoid some stuff like layouts if the controls
        /// aren't actually being used yet.
        /// </summary>
        internal void CreateControl ()
        {
            // Don't run this more than once
            if (Created)
                return;

            SetState (States.Created, true);

            // Create an array copy in case the collection changes
            foreach (var child in Controls.GetAllControls ().ToArray ())
                child.CreateControl ();

            OnCreateControl ();
        }

        /// <summary>
        ///  Constructs the new instance of the Controls collection objects. Subclasses
        ///  should not call base.CreateControlsInstance.
        /// </summary>
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        protected virtual ControlCollection CreateControlsInstance ()
        {
            return new ControlCollection (this);
        }

        /// <summary>
        ///  Indicates whether the control has been created. This property is read-only.
        /// </summary>
        [Browsable (false)]
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public bool Created => GetState (States.Created);

        /// <summary>
        /// Gets the current style of this control instance.
        /// </summary>
        public virtual ControlStyle CurrentStyle => IsHovering && Enabled ? StyleHover : Style;

        /// <summary>
        /// Gets or sets the mouse cursor to be shown when the mouse is over the control.
        /// </summary>
        public Cursor Cursor {
            get {
                if (GetState (States.UseWaitCursor))
                    return Cursors.Wait;

                if (Properties.GetObject (s_cursorProperty) is Cursor cursor)
                    return cursor;

                return Parent?.Cursor ?? DefaultCursor;
            }
            set {
                var old_cursor = Properties.GetObject (s_cursorProperty) as Cursor;

                if (old_cursor != value) {
                    Properties.SetObject (s_cursorProperty, value);
                    OnCursorChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the default cursor.
        /// </summary>
        protected virtual Cursor DefaultCursor => Cursor.Default;

        /// <summary>
        /// Gets the default margin of the control.
        /// </summary>
        protected virtual Padding DefaultMargin => CommonProperties.DefaultMargin;

        /// <summary>
        /// Gets the default maximum size of the control.
        /// </summary>
        protected virtual Size DefaultMaximumSize => CommonProperties.DefaultMaximumSize;

        /// <summary>
        /// Gets the default minimum size of the control.
        /// </summary>
        protected virtual Size DefaultMinimumSize => CommonProperties.DefaultMinimumSize;

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
                style.ForegroundColor = Theme.ForegroundColor;
                style.BackgroundColor = Theme.BackgroundColor;
                style.Font = Theme.UIFont;
                style.FontSize = Theme.FontSize;
                style.Border.Radius = 0;
                style.Border.Color = Theme.BorderLowColor;
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
        /// Gets the unscaled bounds of the displayed control.
        /// </summary>
        public virtual Rectangle DisplayRectangle {
            get {
                // TODO
                var x = CurrentStyle.Border.Left.GetWidth ();
                var y = CurrentStyle.Border.Top.GetWidth ();
                var w = CurrentStyle.Border.Right.GetWidth () + x;
                var h = CurrentStyle.Border.Bottom.GetWidth () + y;

                return new Rectangle (x, y, _width - w, _height - h);
            }
        }

        /// <summary>
        ///  Indicates whether the control is in the process of being disposed. This
        ///  property is read-only.
        /// </summary>
        [Browsable (false)]
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        public bool Disposing => GetState (States.Disposing);

        /// <summary>
        /// Gets or sets whether the control can be interacted with.
        /// </summary>
        public bool Enabled {
            get {
                // If we aren't enabled, that's easy
                if (!GetState (States.Enabled))
                    return false;

                // If we don't have a Parent, then we're enabled
                if (Parent is null)
                    return true;

                // If our Parent isn't enabled, neither are we
                return Parent.Enabled;
            }
            set {
                var old_value = Enabled;
                SetState (States.Enabled, value);

                // See if the computed Enabled actually changed
                if (old_value != value) {
                    if (!value)
                        SelectNextIfFocused ();

                    OnEnabledChanged (EventArgs.Empty);
                }
            }
        }

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
        public virtual Form? FindForm ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is Form f)
                return f;

            return Parent?.FindForm ();
        }

        /// <summary>
        /// Gets the Window that the control is parented to. (Different from FindForm because it may return a PopupWindow.)
        /// </summary>
        internal WindowBase? FindWindow ()
        {
            if (this is ControlAdapter adapter && adapter.ParentForm is WindowBase w)
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

        internal bool GetAnyDisposingInHierarchy ()
        {
            var up = this;

            while (up is not null) {
                if (up.Disposing)
                    return true;

                up = up.parent;
            }

            return false;
        }

        /// <summary>
        /// Gets or creates a back buffer for rendering the control.
        /// </summary>
        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer is null || back_buffer.Width != ScaledSize.Width || back_buffer.Height != ScaledSize.Height) {
                FreeBackBuffer ();
                back_buffer = new SKBitmap (ScaledSize.Width, ScaledSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                SetState (States.IsDirty, true);
            }

            return back_buffer;
        }

        /// <summary>
        ///  Returns the closest ContainerControl in the control's chain of parent controls
        ///  and forms.
        /// </summary>
        public IContainerControl? GetContainerControl ()
        {
            var c = this;

            // Refer to IsContainerControl property for more details.
            if (c is not null && IsContainerControl)
                c = c.Parent;

            while (c is not null && !IsFocusManagingContainerControl (c))
                c = c.Parent;

            return (IContainerControl?)c;
        }

        /// <summary>
        /// Gets behavior flag value.
        /// </summary>
        protected internal bool GetControlBehavior (ControlBehaviors behavior) => behaviors.HasFlag (behavior);

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's state2.
        /// </summary>
        private protected bool GetExtendedState (ExtendedStates flag) => (_extendedState & flag) != 0;

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
                if (start.Controls.GetAllControls (includeImplicit).Any () && (start == this || !IsFocusManagingContainerControl (start))) {
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

        /// <summary>
        /// Gets the position of the Control relative to the Form. (Differs from normal when
        /// the Control is parented to other controls.
        /// </summary>
        internal Point GetPositionInForm ()
        {
            var p = Location;
            var parent = Parent;

            while (parent is not null && parent is not ControlAdapter) {
                p.Offset (parent.Location.X, parent.Location.Y);
                parent = parent.Parent;
            }

            return p;
        }

        /// <summary>
        /// Scales bounds by a specified factor.
        /// </summary>
        protected virtual Rectangle GetScaledBounds (Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            var dx = factor.Width;
            var dy = factor.Height;

            var left = (int)Math.Round (bounds.X * dx, MidpointRounding.ToZero);
            var top = (int)Math.Round (bounds.Y * dy, MidpointRounding.ToZero);

            var sx = bounds.X;
            var sy = bounds.Y;
            var sw = bounds.Width;
            var sh = bounds.Height;

            // Scale the control location (unless this is the top level adapter)
            if (FindAdapter () != this) {
                if (specified.HasFlag (BoundsSpecified.X))
                    sx = left;
                if (specified.HasFlag (BoundsSpecified.Y))
                    sy = top;
            }

            // Don't just scale the Width/Height as it might round incorrectly
            if (specified.HasFlag (BoundsSpecified.Width)) {
                var right = (int)Math.Round ((bounds.Right) * dx, MidpointRounding.ToZero);
                sw = right - left;
            }

            if (specified.HasFlag (BoundsSpecified.Height)) {
                var bottom = (int)Math.Round ((bounds.Bottom) * dy, MidpointRounding.ToZero);
                sh = bottom - top;
            }

            return new Rectangle (sx, sy, sw, sh);
        }

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's state.
        /// </summary>
        private protected bool GetState (States flag) => (_state & flag) != 0;

        /// <summary>
        /// Gets a value indicating if control contains any child controls.
        /// </summary>
        public bool HasChildren => ((Properties.GetObject (s_controlsCollectionProperty) as ControlCollection)?.Count ?? 0) > 0;

        /// <summary>
        /// Gets or sets the unscaled height of the control.
        /// </summary>
        public int Height {
            get => _height;
            set => SetBounds (_x, _y, _width, value, BoundsSpecified.Height);
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
            if (!Created)
                return;

            SetState (States.IsDirty, true);

            FindWindow ()?.Invalidate (rectangle);

            OnInvalidated (new EventArgs<Rectangle> (rectangle));
        }

        /// <summary>
        /// Is the mouse currently over the control.
        /// </summary>
        public bool IsHovering {
            get => GetState (States.IsHovering);
            private set => SetState (States.IsHovering, value);
        }

        /// <summary>
        ///  Determines if <paramref name="charCode"/> is the mnemonic character in <paramref name="text"/>.
        ///  The mnemonic character is the character immediately following the first
        ///  instance of "&amp;" in text
        /// </summary>
        public static bool IsMnemonic (char charCode, string text)
        {
            // Special case handling:
            if (charCode == '&')
                return false;

            if (text is not null) {
                var pos = -1; // start with -1 to handle double &'s
                var c2 = char.ToUpper (charCode, CultureInfo.CurrentCulture);
                for (; ; )
                {
                    if (pos + 1 >= text.Length)
                        break;

                    pos = text.IndexOf ('&', pos + 1) + 1;

                    if (pos <= 0 || pos >= text.Length)
                        break;

                    var c1 = char.ToUpper (text[pos], CultureInfo.CurrentCulture);

                    if (c1 == c2 || char.ToLower (c1, CultureInfo.CurrentCulture) == char.ToLower (c2, CultureInfo.CurrentCulture))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the unscaled left boundary of the control.
        /// </summary>
        public int Left {
            get => _x;
            set => SetBounds (value, _y, _width, _height, BoundsSpecified.X);
        }

        /// <summary>
        /// Gets or sets the unscaled location of the control.
        /// </summary>
        public Point Location {
            get => new Point (_x, _y);
            set => SetBounds (value.X, value.Y, _width, _height, BoundsSpecified.Location);
        }

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
        internal bool ImplicitControl {
            get => GetState (States.IsImplicitControl);
            set => SetState (States.IsImplicitControl, value);
        }

        /// <summary>
        /// Gets or sets a user specified name for the control.
        /// The name can be used as a key into the ControlCollection.
        /// </summary>
        [Browsable (false)]
        public string Name {
            get {
                var name = (string?)Properties.GetObject (s_namePropertyProperty);

                if (string.IsNullOrEmpty (name))
                    name = Site?.Name;

                return name ?? string.Empty;
            }
            set {
                var s = string.IsNullOrEmpty (value) ? null : value;
                Properties.SetObject (s_namePropertyProperty, s);
            }
        }

        /// <summary>
        /// Whether the control needs to be repainted.
        /// </summary>
        internal bool NeedsPaint => GetState (States.IsDirty) || Controls.GetAllControls ().Any (c => c.NeedsPaint);

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
        ///  Called when a child is about to resume its layout.  The default implementation
        ///  calls OnChildLayoutResuming on the parent.
        /// </summary>
        internal virtual void OnChildLayoutResuming (Control child, bool performLayout)
        {
            Parent?.OnChildLayoutResuming (child, performLayout);
        }

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        protected virtual void OnClick (MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && ContextMenu != null) {
                ContextMenu.Show (this, PointToScreen (e.Location));
                return;
            }

            (Events[s_clickEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);
        }

        /// <summary>
        ///  Raises the <see cref='ContextMenuChanged'/> event.
        /// </summary>
        protected virtual void OnContextMenuChanged (EventArgs e) => (Events[s_contextMenuChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        ///  Raises the <see cref='ControlAdded'/> event.
        /// </summary>
        protected virtual void OnControlAdded (EventArgs<Control> e) => (Events[s_controlAddedEvent] as EventHandler<EventArgs<Control>>)?.Invoke (this, e);

        /// <summary>
        ///  Raises the <see cref='ControlRemoved'/> event.
        /// </summary>
        protected virtual void OnControlRemoved (EventArgs<Control> e) => (Events[s_controlRemovedEvent] as EventHandler<EventArgs<Control>>)?.Invoke (this, e);

        /// <summary>
        ///  Called when the control is first created.
        /// </summary>
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        protected virtual void OnCreateControl ()
        {
        }

        /// <summary>
        /// Raises the CursorChanged event.
        /// </summary>
        protected virtual void OnCursorChanged (EventArgs e) => (Events[s_cursorChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Called when the control is deselected.
        /// </summary>
        protected virtual void OnDeselected (EventArgs e) { }

        /// <summary>
        /// Raises the DoubleClick event.
        /// </summary>
        protected virtual void OnDoubleClick (MouseEventArgs e) => (Events[s_doubleClickEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the EnabledChanged event.
        /// </summary>
        protected virtual void OnEnabledChanged (EventArgs e)
        {
            Invalidate ();

            (Events[s_enabledChangedEvent] as EventHandler)?.Invoke (this, e);

            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we enumerate
            if (Properties.GetObject (s_controlsCollectionProperty) is ControlCollection collection)
                for (var i = 0; i < collection.Count; i++)
                    collection[i].OnParentEnabledChanged (e);
        }

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        protected virtual void OnGotFocus (EventArgs e) => (Events[s_gotFocusEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the Invalidated event.
        /// </summary>
        protected virtual void OnInvalidated (EventArgs<Rectangle> e) => (Events[s_invalidatedEvent] as EventHandler<EventArgs<Rectangle>>)?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        protected virtual void OnKeyDown (KeyEventArgs e) => (Events[s_keyDownEvent] as EventHandler<KeyEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyPress event.
        /// </summary>
        protected virtual void OnKeyPress (KeyPressEventArgs e) => (Events[s_keyPressEvent] as EventHandler<KeyPressEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the KeyUp event.
        /// </summary>
        protected virtual void OnKeyUp (KeyEventArgs e) => (Events[s_keyUpEvent] as EventHandler<KeyEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the LocationChanged event.
        /// </summary>
        protected virtual void OnLocationChanged (EventArgs e) => (Events[s_locationChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the MarginChanged event.
        /// </summary>
        protected virtual void OnMarginChanged (EventArgs e) => (Events[s_marginChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        protected virtual void OnMouseDown (MouseEventArgs e) => (Events[s_mouseDownEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);

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

            (Events[s_mouseEnterEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);
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

            (Events[s_mouseLeaveEvent] as EventHandler)?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the MouseMove event.
        /// </summary>
        protected virtual void OnMouseMove (MouseEventArgs e) => (Events[s_mouseMoveEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseUp event.
        /// </summary>
        protected virtual void OnMouseUp (MouseEventArgs e) => (Events[s_mouseUpEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the MouseWheel event.
        /// </summary>
        protected virtual void OnMouseWheel (MouseEventArgs e) => (Events[s_mouseWheelEvent] as EventHandler<MouseEventArgs>)?.Invoke (this, e);

        /// <summary>
        /// Raises the PaddingChanged event.
        /// </summary>
        protected virtual void OnPaddingChanged (EventArgs e) => (Events[s_paddingChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected virtual void OnPaint (PaintEventArgs e)
        {
            foreach (var control in Controls.GetAllControls ().Where (c => c.Visible).ToArray ()) {
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

            e.Canvas.DrawBackground (ScaledBounds, CurrentStyle);
            e.Canvas.DrawBorder (ScaledBounds, CurrentStyle);
        }

        /// <summary>
        /// Called when the Parent property is changed.
        /// </summary>
        protected virtual void OnParentChanged (EventArgs e) => (Events[s_parentEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Called when the Parent's Enabled property is changed.
        /// </summary>
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        protected virtual void OnParentEnabledChanged (EventArgs e)
        {
            if (GetState (States.Enabled))
                OnEnabledChanged (e);
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
        ///  Retrieves our internal property storage object. If you have a property
        ///  whose value is not always set, you should store it in here to save
        ///  space.
        /// </summary>
        internal PropertyStore Properties { get; } = new PropertyStore ();

        /// <summary>
        ///  Raises the <see cref='Resize'/> event.
        /// </summary>
        [EditorBrowsable (EditorBrowsableState.Advanced)]
        protected virtual void OnResize (EventArgs e)
        {
            // TODO?
            //if ((_controlStyle & ControlStyles.ResizeRedraw) == ControlStyles.ResizeRedraw
            //    || GetState (States.ExceptionWhilePainting)) {
            //    Invalidate ();
            //}

            LayoutTransaction.DoLayout (this, this, PropertyNames.Bounds);
            (Events[s_resizeEvent] as EventHandler)?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the SizeChanged event.
        /// </summary>
        protected virtual void OnSizeChanged (EventArgs e)
        {
            OnResize (EventArgs.Empty);

            (Events[s_sizeChangedEvent] as EventHandler)?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the TabIndexChanged event.
        /// </summary>
        protected virtual void OnTabIndexChanged (EventArgs e) => (Events[s_tabIndexChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the TabStopChanged event.
        /// </summary>
        protected virtual void OnTabStopChanged (EventArgs e) => (Events[s_tabStopChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        protected virtual void OnTextChanged (EventArgs e) => (Events[s_textChangedEvent] as EventHandler)?.Invoke (this, e);

        /// <summary>
        /// Raises the VisibleChanged event.
        /// </summary>
        protected virtual void OnVisibleChanged (EventArgs e)
        {
            CreateControl ();

            (Events[s_visibleChangedEvent] as EventHandler)?.Invoke (this, e);

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
        /// Gets or sets the control that contains this control.
        /// </summary>
        public Control? Parent {
            get => parent;
            set {
                if (value == parent)
                    return;

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

                OnParentChanged (EventArgs.Empty);
            }
        }

        /// <summary>
        /// Converts a point from control coordinates to monitor coordinates.
        /// </summary>
        public Point PointToScreen (Point point)
        {
            // If this is the top, add the point to our location
            if (this is ControlAdapter) {
                var window = FindWindow ();

                if (window is null)
                    return point;

                var window_location = window.Location;
                
                // For Mac, the desktop coordinates are measured at a different scale than
                // our form coordinates, so we need to fix that. For other platforms, ratio is 1.
                var desktop_ratio = window.DesktopScaling / window.Scaling;
                point = new Point ((int)(point.X * desktop_ratio), (int)(point.Y * desktop_ratio));

                window_location.Offset (point);

                return window_location;
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

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (TranslateMouseEvents (e, child));
            else {
                // If we're clicking on a Control that isn't the active menu, 
                // we need to close the active menu (if any)
                if ((this as MenuBase)?.GetTopLevelMenu () != Application.ActiveMenu || Application.ActiveMenu is null)
                    Application.ClosePopups (true, false);

                // If we're clicking on a Control that isn't a child of the active PopupWindow, 
                // we need to close the active popup (if any)
                if (FindWindow () != Application.ActivePopupWindow)
                    Application.ClosePopups (false, true);

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
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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

            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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
            var child = Controls.GetAllControls ().LastOrDefault (c => c.Visible && c.GetControlBehavior (ControlBehaviors.ReceivesMouseEvents) && c.ScaledBounds.Contains (e.Location));

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

            SetState (States.IsDirty, false);
        }

        /// <summary>
        /// Calls the OnPaintBackground method.
        /// </summary>
        internal void RaisePaintBackground (PaintEventArgs e) => OnPaintBackground (e);

        /// <summary>
        /// Gets the unscaled right boundary of the control.
        /// </summary>
        public int Right => _x + _width;

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

                foreach (var c in Controls.GetAllControls ())
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
        public bool Selected {
            get => GetState (States.IsSelected);
            private set => SetState (States.IsSelected, value);
        }

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
        ///  This is called recursively when visibility is changed for a control, this
        ///  forces focus to be moved to a visible control.
        /// </summary>
        private void SelectNextIfFocused ()
        {
            if (Focused && Parent is not null)
                if (Parent.GetContainerControl () is Control c)
                    c.SelectNextControl (this, true, true, true, true);
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
        /// Sets behavior flags.
        /// </summary>
        protected internal void SetControlBehavior (ControlBehaviors behavior, bool value = true)
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

            OnParentChanged (EventArgs.Empty);
        }

        /// <summary>
        /// Sets the bounds of the control from scaled dimensions.
        /// </summary>
        internal void SetScaledBounds (int x, int y, int width, int height, BoundsSpecified specified)
        {
            var rect = GetScaledBounds (new Rectangle (x, y, width, height), new SizeF (1 / ScaleFactor.Width, 1 / ScaleFactor.Height), BoundsSpecified.All);
            SetBoundsCore (rect.X, rect.Y, rect.Width, rect.Height, BoundsSpecified.None);
        }

        private protected void SetState (States flag, bool value)
        {
            _state = value ? _state | flag : _state & ~flag;
        }

        private protected void SetExtendedState (ExtendedStates flag, bool value)
        {
            _extendedState = value ? _extendedState | flag : _extendedState & ~flag;
        }

        /// <summary>
        /// Performs the logic needed to change a control's visibility.
        /// </summary>
        protected virtual void SetVisibleCore (bool value)
        {
            if (value != GetState (States.Visible)) {
                if (!value)
                    SelectNextIfFocused ();

                SetState (States.Visible, value);

                if (Parent is not null)
                    using (new LayoutTransaction (Parent, this, PropertyNames.Visible))
                        OnVisibleChanged (EventArgs.Empty);
                else
                    OnVisibleChanged (EventArgs.Empty);
            }
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
            get => new Size (_width, _height);
            set => SetBounds (_x, _y, value.Width, value.Height, BoundsSpecified.Size);
        }

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control.
        /// </summary>
        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control when the user is hovering over it.
        /// </summary>
        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        /// <summary>
        ///  Suspends the layout logic for the control.
        /// </summary>
        public void SuspendLayout () => layout_suspend_count++;

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
        /// Gets or sets whether the control is selectable via pressing tab.
        /// </summary>
        public bool TabStop {
            get => GetState (States.TabStop);
            set {
                if (TabStop != value) {
                    SetState (States.TabStop, value);
                    OnTabStopChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets user defined data.
        /// </summary>
        public object? Tag {
            get => Properties.GetObject (s_tagProperty);
            set => Properties.SetObject (s_tagProperty, value);
        }

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
        /// Gets or sets the unscaled top boundary of the control.
        /// </summary>
        public int Top {
            get => _y;
            set => SetBounds (_x, value, _width, _height, BoundsSpecified.Y);
        }

        /// <summary>
        /// Changes mouse events to control coordinates.
        /// </summary>
        private static MouseEventArgs TranslateMouseEvents (MouseEventArgs e, Control control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.ScaledLeft, e.Location.Y - control.ScaledTop, e.Delta, e.Location.X, e.Location.Y, e.Modifiers);
        }

        /// <summary>
        /// Gets or sets whether the control is displayed to the user.
        /// </summary>
        public virtual bool Visible {
            get {
                if (!GetState (States.Visible))
                    return false;

                return parent?.Visible ?? false;
            }
            set => SetVisibleCore (value);
        }

        /// <summary>
        /// Gets or sets the unscaled width of the control.
        /// </summary>
        public int Width {
            get => _width;
            set => SetBounds (_x, _y, value, _height, BoundsSpecified.Width);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes unmanaged resources used by the control.
        /// </summary>
        protected override void Dispose (bool disposing)
        {
            if (!disposedValue) {
                FreeBackBuffer ();

                Theme.ThemeChanged -= OnThemeChanged;

                foreach (var c in Controls.GetAllControls (true))
                    c.Dispose (disposing);

                disposedValue = true;
            }

            base.Dispose (disposing);
        }

        /// <summary>
        /// Destroys the control.
        /// </summary>
        ~Control ()
        {
            Dispose (false);
        }
        #endregion
    }
}
