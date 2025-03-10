// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using Modern.Forms.Layout;

namespace Modern.Forms;

// Put the Layout things here, since most of them are copied from SWF.
public partial class Control
{
    private LayoutEventArgs? _cachedLayoutEventArgs;

    /// <summary>
    ///  The current value of the anchor property. The anchor property
    ///  determines which edges of the control are anchored to the container's
    ///  edges.
    /// </summary>
    public virtual AnchorStyles Anchor {
        get => DefaultLayout.GetAnchor (this);
        set => DefaultLayout.SetAnchor (Parent, this, value);
    }

    internal virtual Rectangle ApplyBoundsConstraints (int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
    {
        // COMPAT: in Everett we would allow you to set negative values in pre-handle mode
        // in Whidbey, if you've set Min/Max size we will constrain you to 0,0.  Everett apps didnt
        // have min/max size on control, which is why this works.
        if (MaximumSize != Size.Empty || MinimumSize != Size.Empty) {
            var maximumSize = LayoutUtils.ConvertZeroToUnbounded (MaximumSize);

            var newBounds = new Rectangle (suggestedX, suggestedY, 0, 0) {
                // Clip the size to maximum and inflate it to minimum as necessary.
                Size = LayoutUtils.IntersectSizes (new Size (proposedWidth, proposedHeight), maximumSize)
            };

            newBounds.Size = LayoutUtils.UnionSizes (newBounds.Size, MinimumSize);

            return newBounds;
        }

        return new Rectangle (suggestedX, suggestedY, proposedWidth, proposedHeight);
    }

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal Size ApplySizeConstraints (int width, int height) => ApplyBoundsConstraints (0, 0, width, height).Size;

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal Size ApplySizeConstraints (Size proposedSize) => ApplyBoundsConstraints (0, 0, proposedSize.Width, proposedSize.Height).Size;

    /// <summary>
    /// Gets or sets a value indicating if this control's size can be changed automatically.
    /// </summary>
    public virtual bool AutoSize {
        get => CommonProperties.GetAutoSize (this);
        set {
            if (value != AutoSize) {
                CommonProperties.SetAutoSize (this, value);

                if (Parent is not null) {
                    // DefaultLayout does not keep anchor information until it needs to.  When
                    // AutoSize became a common property, we could no longer blindly call into
                    // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                    if (value && Parent.LayoutEngine == DefaultLayout.Instance)
                        Parent.LayoutEngine.InitLayout (this, BoundsSpecified.Size);

                    LayoutTransaction.DoLayout (Parent, this, PropertyNames.AutoSize);
                }

                OnAutoSizeChanged (EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  The dock property. The dock property controls to which edge
    ///  of the container this control is docked to. For example, when docked to
    ///  the top of the container, the control will be displayed flush at the
    ///  top of the container, extending the length of the container.
    /// </summary>
    public virtual DockStyle Dock {
        get => DefaultLayout.GetDock (this);
        set {
            if (value != Dock) {

                SuspendLayout ();

                try {
                    DefaultLayout.SetDock (this, value);
                    OnDockChanged (EventArgs.Empty);
                } finally {
                    ResumeLayout ();
                }
            }
        }
    }

    /// <summary>
    /// Derived controls can use to provide a public AutoSizeMode property.
    /// </summary>
    protected AutoSizeMode GetAutoSizeMode () => CommonProperties.GetAutoSizeMode (this);

    /// <summary>
    /// Gets the size the control would prefer to be.
    /// </summary>
    /// <param name="proposedSize">A size the layout engine is proposing for the control.</param>
    public virtual Size GetPreferredSize (Size proposedSize)
    {
        Size prefSize;

        if (GetState (States.Disposing | States.Disposed)) {
            // if someone's asking when we're disposing just return what we last had.
            prefSize = CommonProperties.xGetPreferredSizeCache (this);
        } else {
            // Switch Size.Empty to maximum possible values
            proposedSize = LayoutUtils.ConvertZeroToUnbounded (proposedSize);

            // Force proposedSize to be within the elements constraints.  (This applies
            // minimumSize, maximumSize, etc.)
            proposedSize = ApplySizeConstraints (proposedSize);
            if (GetExtendedState (ExtendedStates.UserPreferredSizeCache)) {
                var cachedSize = CommonProperties.xGetPreferredSizeCache (this);

                // If the "default" preferred size is being requested, and we have a cached value for it, return it.
                if (!cachedSize.IsEmpty && (proposedSize == LayoutUtils.s_maxSize))
                    return cachedSize;
            }

            prefSize = GetPreferredSizeCore (proposedSize);

            // There is no guarantee that GetPreferredSizeCore() return something within
            // proposedSize, so we apply the element's constraints again.
            prefSize = ApplySizeConstraints (prefSize);

            // If the "default" preferred size was requested, cache the computed value.
            if (GetExtendedState (ExtendedStates.UserPreferredSizeCache) && proposedSize == LayoutUtils.s_maxSize)
                CommonProperties.xSetPreferredSizeCache (this, prefSize);
        }

        return prefSize;
    }

    // Overriding this method allows us to get the caching and clamping the proposedSize/output to
    // MinimumSize / MaximumSize from GetPreferredSize for free.
    internal virtual Size GetPreferredSizeCore (Size proposedSize)
    {
        return CommonProperties.GetSpecifiedBounds (this).Size;
    }

    IEnumerable<Control> IArrangedElement.Children => Controls.GetAllControls (true);

    IArrangedElement? IArrangedElement.Container {
        get {
            // This is safe because the IArrangedElement interface is internal
            return Parent;
        }
    }

    bool IArrangedElement.ParticipatesInLayout => GetState (States.Visible);

    void IArrangedElement.PerformLayout (IArrangedElement affectedElement, string? affectedProperty)
    {
        PerformLayout (new LayoutEventArgs ((Control)affectedElement, affectedProperty));
    }

    /// <summary>
    /// Derived controls can use to provide a public AutoSizeMode property.
    /// </summary>
    protected void SetAutoSizeMode (AutoSizeMode mode) => CommonProperties.SetAutoSizeMode (this, mode);

    PropertyStore IArrangedElement.Properties => Properties;

    // CAREFUL: This really calls SetBoundsCore, not SetBounds.
    void IArrangedElement.SetBounds (Rectangle bounds, BoundsSpecified specified)
    {
        var site = Site;
        var sizeChanged = false;
        var locationChanged = false;

        IComponentChangeService? changeService = null;
        PropertyDescriptor? sizeProperty = null;
        PropertyDescriptor? locationProperty = null;

#if DESIGN_TIME
        if (site is not null && site.DesignMode && site.TryGetService (out changeService)) {
            sizeProperty = TypeDescriptor.GetProperties (this)[PropertyNames.Size];
            locationProperty = TypeDescriptor.GetProperties (this)[PropertyNames.Location];
            Debug.Assert (sizeProperty is not null && locationProperty is not null, "Error retrieving Size/Location properties on Control.");

            try {
                if (sizeProperty is not null && !sizeProperty.IsReadOnly && (bounds.Width != Width || bounds.Height != Height)) {
                    if (site is not INestedSite)
                        changeService.OnComponentChanging (this, sizeProperty);

                    sizeChanged = true;
                }

                if (locationProperty is not null && !locationProperty.IsReadOnly && (bounds.X != _x || bounds.Y != _y)) {
                    if (site is not INestedSite)
                        changeService.OnComponentChanging (this, locationProperty);

                    locationChanged = true;
                }
            } catch (InvalidOperationException) {
                // The component change events can throw InvalidOperationException if a change is
                // currently not allowed (typically because the doc data in VS is locked).
                // When this happens, we just eat the exception and proceed with the change.
            }
        }
#endif

        SetBoundsCore (bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);

        if (changeService is not null) {
            try {
                if (sizeChanged)
                    changeService.OnComponentChanged (this, sizeProperty, null, null);

                if (locationChanged)
                    changeService.OnComponentChanged (this, locationProperty, null, null);
            } catch (InvalidOperationException) {
                // The component change events can throw InvalidOperationException if a change is
                // currently not allowed (typically because the doc data in VS is locked).
                // When this happens, we just eat the exception and proceed with the change.
            }
        }
    }

    /// <summary>
    ///  Called after the control has been added to another container.
    /// </summary>
    protected virtual void InitLayout ()
    {
        LayoutEngine.InitLayout (this, BoundsSpecified.All);
    }

    // If the control on which GetContainerControl( ) is called is a ContainerControl, then we don't return the parent
    // but return the same control. This is Everett behavior so we cannot change this since this would be a breaking change.
    // Hence we have a new internal property IsContainerControl which returns false for all Everett control, but
    // this property is overidden in SplitContainer to return true so that we skip the SplitContainer
    // and the correct Parent ContainerControl is returned by GetContainerControl().
    internal virtual bool IsContainerControl => false;

    private static bool IsFocusManagingContainerControl (Control ctl)
    {
        // TODO probably
        return false;// ((ctl._controlStyle & ControlStyles.ContainerControl) == ControlStyles.ContainerControl && ctl is IContainerControl);
    }

    // Public because this is interesting for ControlDesigners.
    /// <summary>
    /// Gets the LayoutEngine for the current control.
    /// </summary>
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public virtual LayoutEngine LayoutEngine => DefaultLayout.Instance;

    /// <summary>
    /// Gets or sets how much space there should be between the control and other controls.
    /// </summary>
    public Padding Margin {
        get => CommonProperties.GetMargin (this);
        set {
            // This should be done here rather than in the property store as
            // some IArrangedElements actually support negative padding.
            value = LayoutUtils.ClampNegativePaddingToZero (value);

            // SetMargin causes a layout as a side effect.
            if (value != Margin) {
                CommonProperties.SetMargin (this, value);
                OnMarginChanged (EventArgs.Empty);
            }

            Debug.Assert (Margin == value, "Error detected while setting Margin.");
        }
    }

    /// <summary>
    /// Gets or sets the maximum size for the control.
    /// </summary>
    public virtual Size MaximumSize {
        get => CommonProperties.GetMaximumSize (this, DefaultMaximumSize);
        set {
            if (value == Size.Empty) {
                CommonProperties.ClearMaximumSize (this);
                Debug.Assert (MaximumSize == DefaultMaximumSize, "Error detected while resetting MaximumSize.");
            } else if (value != MaximumSize) {
                // SetMaximumSize causes a layout as a side effect.
                CommonProperties.SetMaximumSize (this, value);
                Debug.Assert (MaximumSize == value, "Error detected while setting MaximumSize.");
            }
        }
    }

    /// <summary>
    /// Gets or sets the minimum size for the control.
    /// </summary>
    public virtual Size MinimumSize {
        get => CommonProperties.GetMinimumSize (this, DefaultMinimumSize);
        set {
            if (value != MinimumSize) {
                // SetMinimumSize causes a layout as a side effect.
                CommonProperties.SetMinimumSize (this, value);
            }

            Debug.Assert (MinimumSize == value, "Error detected while setting MinimumSize.");
        }
    }

    /// <summary>
    /// Raises the AutoSizeChanged event.
    /// </summary>
    protected virtual void OnAutoSizeChanged (EventArgs e) => (Events[s_autoSizeChangedEvent] as EventHandler)?.Invoke (this, e);

    // Give a chance for derived controls to do what they want, just before we resize.
    internal virtual void OnBoundsUpdate (int x, int y, int width, int height)
    {
    }

    /// <summary>
    /// Raises the DockChanged event.
    /// </summary>
    protected virtual void OnDockChanged (EventArgs e) => (Events[s_dockChangedEvent] as EventHandler)?.Invoke (this, e);

    /// <summary>
    /// Raises the Layout event.
    /// </summary>
    protected virtual void OnLayout (LayoutEventArgs e)
    {
        (Events[s_layoutEvent] as EventHandler<LayoutEventArgs>)?.Invoke (this, e);

        var parentRequiresLayout = LayoutEngine.Layout (this, e);

        if (parentRequiresLayout && Parent is not null) {
            // LayoutEngine.Layout can return true to request that our parent resize us because
            // we did not have enough room for our contents. We can not just call PerformLayout
            // because this container is currently suspended. PerformLayout will check this state
            // flag and PerformLayout on our parent.
            Parent.SetState (States.LayoutIsDirty, true);
        }
    }

    /// <summary>
    ///  Called when the last resume layout call is made. If performLayout is true a layout will
    ///  occur as soon as this call returns. Layout is still suspended when this call is made.
    ///  The default implementation calls OnChildLayoutResuming on the parent, if it exists.
    /// </summary>
    internal virtual void OnLayoutResuming (bool performLayout)
    {
        Parent?.OnChildLayoutResuming (this, performLayout);
    }

    internal virtual void OnLayoutSuspended ()
    {
    }

    /// <summary>
    /// Gets or sets the amount of space there should be between the control bounds and the control contents.
    /// </summary>
    public Padding Padding {
        get => CommonProperties.GetPadding (this, DefaultPadding);
        set {
            if (value != Padding) {
                CommonProperties.SetPadding (this, value);
                // Ideally we are being laid out by a LayoutEngine that cares about our preferred size.
                // We set our LAYOUTISDIRTY bit and ask our parent to refresh us.
                SetState (States.LayoutIsDirty, true);

                using (new LayoutTransaction (Parent, this, PropertyNames.Padding))
                    OnPaddingChanged (EventArgs.Empty);

                if (GetState (States.LayoutIsDirty)) {
                    // The above did not cause our layout to be refreshed.  We explicitly refresh our
                    // layout to ensure that any children are repositioned to account for the change
                    // in padding.
                    LayoutTransaction.DoLayout (this, this, PropertyNames.Padding);
                }
            }
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
    public void PerformLayout (Control? affectedControl, string? affectedProperty)
    {
        PerformLayout (new LayoutEventArgs (affectedControl, affectedProperty));
    }

    internal void PerformLayout (LayoutEventArgs args)
    {
        Debug.Assert (args is not null, "This method should never be called with null args.");

        if (layout_suspend_count > 0) {
            SetState (States.LayoutDeferred, true);

            if (_cachedLayoutEventArgs is null || GetExtendedState (ExtendedStates.ClearLayoutArgs)) {
                _cachedLayoutEventArgs = args;

                if (GetExtendedState (ExtendedStates.ClearLayoutArgs))
                    SetExtendedState (ExtendedStates.ClearLayoutArgs, false);
            }

            LayoutEngine.ProcessSuspendedLayoutEventArgs (this, args);

            return;
        }

        // (Essentially the same as suspending layout while we layout, but we clear differently below.)
        layout_suspend_count = 1;

        try {
            //CacheTextInternal = true;
            OnLayout (args);
        } finally {
            //CacheTextInternal = false;
            // Rather than resume layout (which will could allow a deferred layout to layout the
            // the container we just finished laying out) we set layoutSuspendCount back to zero
            // and clear the deferred and dirty flags.
            SetState (States.LayoutDeferred | States.LayoutIsDirty, false);
            layout_suspend_count = 0;

            // LayoutEngine.Layout can return true to request that our parent resize us because
            // we did not have enough room for our contents.  Now that we are unsuspended,
            // see if this happened and layout parent if necessary.  (See also OnLayout)
            if (Parent is not null && Parent.GetState (States.LayoutIsDirty))
                LayoutTransaction.DoLayout (Parent, this, PropertyNames.PreferredSize);
        }
    }

    /// <summary>
    /// Gets the size the control would prefer to be.
    /// </summary>
    public Size PreferredSize => GetPreferredSize (Size.Empty);

    /// <summary>
    /// Notifies the control to result performing layouts originally suspended with SuspendLayout.
    /// </summary>
    public void ResumeLayout (bool performLayout = true)
    {
        var performedLayout = false;

        if (layout_suspend_count > 0) {
            if (layout_suspend_count == 1) {
                layout_suspend_count++;
                try {
                    OnLayoutResuming (performLayout);
                } finally {
                    layout_suspend_count--;
                }
            }

            layout_suspend_count--;
            if (layout_suspend_count == 0
                && GetState (States.LayoutDeferred)
                && performLayout) {
                PerformLayout ();
                performedLayout = true;
            }
        }

        if (!performedLayout)
            SetExtendedState (ExtendedStates.ClearLayoutArgs, true);

        /*
        We've had this since Everett,but it seems wrong, redundant and a performance hit.  The
        correct layout calls are already made when bounds or parenting changes, which is all
        we care about. We may want to call this at layout suspend count == 0, but certainly
        not for all resumes.  I  tried removing it, and doing it only when suspendCount == 0,
        but we break things at every step.
        */
        if (!performLayout) {
            CommonProperties.xClearPreferredSizeCache (this);
            var controlsCollection = (ControlCollection?)Properties.GetObject (s_controlsCollectionProperty);

            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            if (controlsCollection is not null) {
                for (var i = 0; i < controlsCollection.Count; i++) {
                    LayoutEngine.InitLayout (controlsCollection[i], BoundsSpecified.All);
                    CommonProperties.xClearPreferredSizeCache (controlsCollection[i]);
                }
            }
        }
    }

    /// <summary>
    /// Sets the unscaled bounds of the control.
    /// </summary>
    public void SetBounds (int x, int y, int width, int height)
    {
        if (_x != x || _y != y || _width != width || _height != height) {
            SetBoundsCore (x, y, width, height, BoundsSpecified.All);

            // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
            // have refreshed the interior layout.  We only need to layout the parent.
            LayoutTransaction.DoLayout (Parent, this, PropertyNames.Bounds);
        } else {
            // Still need to init scaling.
            //InitScaling (BoundsSpecified.All);
        }
    }

    /// <summary>
    /// Sets the unscaled bounds of the control.
    /// </summary>
    public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified)
    {
        if ((specified & BoundsSpecified.X) == BoundsSpecified.None)
            x = _x;

        if ((specified & BoundsSpecified.Y) == BoundsSpecified.None)
            y = _y;

        if ((specified & BoundsSpecified.Width) == BoundsSpecified.None)
            width = _width;

        if ((specified & BoundsSpecified.Height) == BoundsSpecified.None)
            height = _height;

        if (_x != x || _y != y || _width != width || _height != height) {
            SetBoundsCore (x, y, width, height, specified);

            // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
            // have refreshed the interior layout or the resized control.  We only need to layout
            // the parent.  This happens after InitLayout has been invoked.
            LayoutTransaction.DoLayout (Parent, this, PropertyNames.Bounds);
        } else {
            // Still need to init scaling.
            //InitScaling (specified);
        }
    }

    /// <summary>
    ///  Performs the work of setting the bounds of this control. Inheriting
    ///  classes can override this function to add size restrictions. Inheriting
    ///  classes must call base.setBoundsCore to actually cause the bounds
    ///  of the control to change.
    /// </summary>
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    protected virtual void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
    {
        // SetWindowPos below sends a WmWindowPositionChanged (not posts) so we immediately
        // end up in WmWindowPositionChanged which may cause the parent to layout.  We need to
        // suspend/resume to defer the parent from laying out until after InitLayout has been called
        // to update the layout engine's state with the new control bounds.
        if (Parent is not null)
            Parent.SuspendLayout ();

        try {
            if (_x != x || _y != y || _width != width || _height != height) {
                CommonProperties.UpdateSpecifiedBounds (this, x, y, width, height, specified);

                // Provide control with an opportunity to apply self imposed constraints on its size.
                var adjustedBounds = ApplyBoundsConstraints (x, y, width, height);
                width = adjustedBounds.Width;
                height = adjustedBounds.Height;
                x = adjustedBounds.X;
                y = adjustedBounds.Y;

                // Give a chance for derived controls to do what they want, just before we resize.
                if (Created)
                    OnBoundsUpdate (x, y, width, height);

                UpdateBounds (x, y, width, height);
            }
        } finally {
            // Initialize the scaling engine.
            //InitScaling (specified);

            if (Parent is not null) {
                // Some layout engines (DefaultLayout) base their PreferredSize on
                // the bounds of their children.  If we change change the child bounds, we
                // need to clear their PreferredSize cache.  The semantics of SetBoundsCore
                // is that it does not cause a layout, so we just clear.
                CommonProperties.xClearPreferredSizeCache (Parent);

                // Cause the current control to initialize its layout (e.g., Anchored controls
                // memorize their distance from their parent's edges).  It is your parent's
                // LayoutEngine which manages your layout, so we call into the parent's
                // LayoutEngine.
                Parent.LayoutEngine.InitLayout (this, specified);
                Parent.ResumeLayout ( /* performLayout = */ true);
            }
        }
    }

    /// <summary>
    ///  Updates the bounds of the control based on the bounds passed in.
    /// </summary>
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    protected void UpdateBounds (int x, int y, int width, int height)//, int clientWidth, int clientHeight)
    {

        var newLocation = _x != x || _y != y;
        var newSize = Width != width || Height != height;// ||
                                                          //_clientWidth != clientWidth || _clientHeight != clientHeight;

        _x = x;
        _y = y;
        _width = width;
        _height = height;
        //_clientWidth = clientWidth;
        //_clientHeight = clientHeight;

        if (newLocation)
            OnLocationChanged (EventArgs.Empty);

        if (newSize) {
            OnSizeChanged (EventArgs.Empty);
            // OnClientSizeChanged (EventArgs.Empty);
            //PerformLayout (this, nameof (Bounds)); // TESTING
            // Clear PreferredSize cache for this control
            CommonProperties.xClearPreferredSizeCache (this);
            LayoutTransaction.DoLayout (Parent, this, PropertyNames.Bounds);
        }
    }
}
