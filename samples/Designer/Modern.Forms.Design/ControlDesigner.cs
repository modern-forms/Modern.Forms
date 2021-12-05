using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public class ControlDesigner : ComponentDesigner
    {
        private IDesignerHost _host;                        // the host for our designer
        private ResizeBehavior _resizeBehavior;             // the standard behavior for our selection glyphs - demand created
        private BehaviorService _behaviorService;           // we cache this 'cause we use it so often

        protected static readonly Point InvalidPoint = new Point (int.MinValue, int.MinValue);

        // Transient values that are used during mouse drags
        private Point _mouseDragLast = InvalidPoint;        // the last position of the mouse during a drag.
        private bool _mouseDragMoved;                       // has the mouse been moved during this drag?
                                                            //private int _lastMoveScreenX;
                                                            //private int _lastMoveScreenY;
        private bool _ctrlSelect;                           // if the CTRL key was down at the mouse down

        protected BehaviorService BehaviorService => _behaviorService ??= GetService<BehaviorService> ();

        /// <summary>
        ///  Retrieves the control we're designing.
        /// </summary>
        public virtual IVisual? Control => Component as IVisual;

        /// <summary>
        ///  Retrieves the form we're designing.
        /// </summary>
        public virtual Form? Form => Component as Form;

        /// <summary>
        ///  Accessor method for the Enabled property on control. We shadow this property at design time.
        /// </summary>
        private bool Enabled {
            get => (bool)ShadowProperties[nameof (Enabled)];
            set => ShadowProperties[nameof (Enabled)] = value;
        }

        /// <summary>
        ///  Returns a 'BodyGlyph' representing the bounds of this control. The BodyGlyph is responsible for hit
        ///  testing the related CtrlDes and forwarding messages directly to the designer.
        /// </summary>
        protected virtual ControlBodyGlyph GetControlGlyph (GlyphSelectionType selectionType)
        {
            // get the right cursor for this component
            Cursor cursor = OnSetCursor ();
            //Cursor cursor = Cursor.Current;
            //Cursor cursor = Cursor.Default;

            // get the correctly translated bounds  // TODO
            //Rectangle translatedBounds = BehaviorService.ControlRectInAdornerWindow (Control);
            Rectangle translatedBounds = Control.Bounds;

            // create our glyph, and set its cursor appropriately
            ControlBodyGlyph g = null;
            //Control parent = Control.Parent;

            //if (parent != null && _host != null && _host.RootComponent != Component) {
            //    Rectangle parentRect = parent.RectangleToScreen (parent.ClientRectangle);
            //    Rectangle controlRect = Control.RectangleToScreen (Control.ClientRectangle);
            //    if (!parentRect.Contains (controlRect) && !parentRect.IntersectsWith (controlRect)) {
            //        // Since the parent is completely clipping the control, the control cannot be a drop target, and
            //        // it will not get mouse messages. So we don't have to give the glyph a transparentbehavior
            //        // (default for ControlBodyGlyph). But we still would like to be able to move the control, so push
            //        // a MoveBehavior. If we didn't we wouldn't be able to move the control, since it won't get any
            //        // mouse messages.

            //        if (TryGetService (out ISelectionService sel) && sel.GetComponentSelected (Control)) {
            //            g = new ControlBodyGlyph (translatedBounds, cursor, Control, MoveBehavior);
            //        } else if (cursor == Cursors.SizeAll) {
            //            // If we get here, OnSetCursor could have set the cursor to SizeAll. But if we fall into this
            //            // category, we don't have a MoveBehavior, so we don't want to show the SizeAll cursor. Let's
            //            // make sure the cursor is set to the default cursor.
            //            cursor = Cursor.Default;
            //        }
            //    }
            //}

            // If null, we are not totally clipped by the parent
            g ??= new ControlBodyGlyph (translatedBounds, cursor, Component, this);

            return g;
        }

        internal ControlBodyGlyph GetControlGlyphInternal (GlyphSelectionType selectionType) => GetControlGlyph (selectionType);

        /// <summary>
        ///  Returns a collection of Glyph objects representing the selection borders and grab handles for a standard
        ///  control.  Note that based on 'selectionType' the Glyphs returned will either: represent a fully resizeable
        ///  selection border with grab handles, a locked selection border, or a single 'hidden' selection Glyph.
        /// </summary>
        public virtual GlyphCollection GetGlyphs (GlyphSelectionType selectionType)
        {
            var glyphs = new GlyphCollection ();

            if (selectionType == GlyphSelectionType.NotSelected) {
                return glyphs;
            }

            Rectangle translatedBounds = Control.Bounds;
            //Rectangle translatedBounds = BehaviorService.ControlRectInAdornerWindow (Control);

            var primarySelection = (selectionType == GlyphSelectionType.SelectedPrimary);
            SelectionRules rules = SelectionRules;

            //if (Locked || (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)) {
            //    // the lock glyph
            //    glyphs.Add (new LockedHandleGlyph (translatedBounds, primarySelection));

            //    // the four locked border glyphs
            //    glyphs.Add (new LockedBorderGlyph (translatedBounds, SelectionBorderGlyphType.Top));
            //    glyphs.Add (new LockedBorderGlyph (translatedBounds, SelectionBorderGlyphType.Bottom));
            //    glyphs.Add (new LockedBorderGlyph (translatedBounds, SelectionBorderGlyphType.Left));
            //    glyphs.Add (new LockedBorderGlyph (translatedBounds, SelectionBorderGlyphType.Right));
            //} else if ((rules & SelectionRules.AllSizeable) == SelectionRules.None) {
            //    // the non-resizeable grab handle
            //    glyphs.Add (new NoResizeHandleGlyph (translatedBounds, rules, primarySelection, MoveBehavior));

            //    // the four resizeable border glyphs
            //    glyphs.Add (new NoResizeSelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Top, MoveBehavior));
            //    glyphs.Add (new NoResizeSelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Bottom, MoveBehavior));
            //    glyphs.Add (new NoResizeSelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Left, MoveBehavior));
            //    glyphs.Add (new NoResizeSelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Right, MoveBehavior));

            //    // enable the designeractionpanel for this control if it needs one
            //    if (TypeDescriptor.GetAttributes (Component).Contains (DesignTimeVisibleAttribute.Yes)
            //        && _behaviorService.DesignerActionUI != null) {
            //        Glyph dapGlyph = _behaviorService.DesignerActionUI.GetDesignerActionGlyph (Component);
            //        if (dapGlyph != null) {
            //            glyphs.Insert (0, dapGlyph); // we WANT to be in front of the other UI
            //        }
            //    }
            //} else {
                // Grab handles
                if ((rules & SelectionRules.TopSizeable) != 0) {
                    glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.MiddleTop, StandardBehavior, primarySelection));
                    if ((rules & SelectionRules.LeftSizeable) != 0) {
                        glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.UpperLeft, StandardBehavior, primarySelection));
                    }

                    if ((rules & SelectionRules.RightSizeable) != 0) {
                        glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.UpperRight, StandardBehavior, primarySelection));
                    }
                }

                if ((rules & SelectionRules.BottomSizeable) != 0) {
                    glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.MiddleBottom, StandardBehavior, primarySelection));
                    if ((rules & SelectionRules.LeftSizeable) != 0) {
                        glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.LowerLeft, StandardBehavior, primarySelection));
                    }

                    if ((rules & SelectionRules.RightSizeable) != 0) {
                        glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.LowerRight, StandardBehavior, primarySelection));
                    }
                }

                if ((rules & SelectionRules.LeftSizeable) != 0) {
                    glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.MiddleLeft, StandardBehavior, primarySelection));
                }

                if ((rules & SelectionRules.RightSizeable) != 0) {
                    glyphs.Add (new GrabHandleGlyph (translatedBounds, GrabHandleGlyphType.MiddleRight, StandardBehavior, primarySelection));
                }

            // the four resizeable border glyphs
            glyphs.Add (new SelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Top, StandardBehavior));
            glyphs.Add (new SelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Bottom, StandardBehavior));
            glyphs.Add (new SelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Left, StandardBehavior));
            glyphs.Add (new SelectionBorderGlyph (translatedBounds, rules, SelectionBorderGlyphType.Right, StandardBehavior));

            // enable the designeractionpanel for this control if it needs one
            //if (TypeDescriptor.GetAttributes (Component).Contains (DesignTimeVisibleAttribute.Yes)
            //    && _behaviorService.DesignerActionUI != null) {
            //    Glyph dapGlyph = _behaviorService.DesignerActionUI.GetDesignerActionGlyph (Component);
            //    if (dapGlyph != null) {
            //        glyphs.Insert (0, dapGlyph); // we WANT to be in front of the other UI
            //    }
            //}
            //}

            return glyphs;
        }

        internal T GetService<T> () where T : class => GetService (typeof (T)) as T;

        /// <summary>
        ///  Called by the host when we're first initialized.
        /// </summary>
        public override void Initialize (IComponent component)
        {
            // Visibility works as follows:  If the control's property is not actually set, then set our shadow to true.
            // Otherwise, grab the shadow value from the control directly and then set the control to be visible if it
            // is not the root component.  Root components will be set to visible = true in their own time by the view.
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties (component.GetType ());
            PropertyDescriptor visibleProp = props["Visible"];
            Visible = visibleProp is null
                || visibleProp.PropertyType != typeof (bool)
                || !visibleProp.ShouldSerializeValue (component)
                || (bool)visibleProp.GetValue (component);

            PropertyDescriptor enabledProp = props["Enabled"];
            Enabled = enabledProp is null
                || enabledProp.PropertyType != typeof (bool)
                || !enabledProp.ShouldSerializeValue (component)
                || (bool)enabledProp.GetValue (component);

            base.Initialize (component);

            // And get other commonly used services.
            _host = GetService<IDesignerHost> ();

            // This is to create the action in the DAP for this component if it requires docking/undocking logic
            //AttributeCollection attributes = TypeDescriptor.GetAttributes (Component);
            //DockingAttribute dockingAttribute = (DockingAttribute)attributes[typeof (DockingAttribute)];
            //if (dockingAttribute != null && dockingAttribute.DockingBehavior != DockingBehavior.Never) {
            //    // Create the action for this control
            //    _dockingAction = new DockingActionList (this);

            //    // Add our 'dock in parent' or 'undock in parent' action
            //    if (TryGetService (out DesignerActionService das)) {
            //        das.Add (Component, _dockingAction);
            //    }
            //}

            // Hook up the property change notifications we need to track. One for data binding.
            // More for control add / remove notifications
            //_dataBindingsCollectionChanged = new CollectionChangeEventHandler (DataBindingsCollectionChanged);
            //Control.DataBindings.CollectionChanged += _dataBindingsCollectionChanged;

            //Control.ControlAdded += new ControlEventHandler (OnControlAdded);
            //Control.ControlRemoved += new ControlEventHandler (OnControlRemoved);
            //Control.ParentChanged += new EventHandler (OnParentChanged);

            //Control.SizeChanged += new EventHandler (OnSizeChanged);
            //Control.LocationChanged += new EventHandler (OnLocationChanged);

            //// Replace the control's window target with our own. This allows us to hook messages.
            //DesignerTarget = new DesignerWindowTarget (this);

            //// If the handle has already been created for this control, invoke OnCreateHandle so we can hookup our
            //// child control subclass.
            //if (Control.IsHandleCreated) {
            //    OnCreateHandle ();
            //}

            // If we are an inherited control, notify our inheritance UI
            //if (Inherited && _host != null && _host.RootComponent != component) {
            //    _inheritanceUI = GetService<InheritanceUI> ();
            //    _inheritanceUI?.AddInheritedControl (Control, InheritanceAttribute.InheritanceLevel);
            //}

            // When we drag one control from one form to another, we will end up here. In this case we do not want to
            // set the control to visible, so check ForceVisible.
            //if ((_host is null || _host.RootComponent != component) && ForceVisible) {
            //    Control.Visible = true;
            //}

            // Always make controls enabled, event inherited ones.  Otherwise we won't be able to select them.
            Control.Enabled = true;

            // we move enabledchanged below the set to avoid any possible stack overflows. this can occur if the parent
            // is not enabled when we set enabled to true.
           // Control.EnabledChanged += new EventHandler (OnEnabledChanged);

            // And force some shadow properties that we change in the course of initializing the form.
            //AllowDrop = Control.AllowDrop;

            // update the Status Command
            //_statusCommandUI = new StatusCommandUI (component.Site);
        }

        /// <summary>
        ///  Called in response to the left mouse button being pressed on a component. It ensures that the component is selected.
        /// </summary>
        public virtual void OnMouseDragBegin (int x, int y)
        {
            // Ignore another mouse down if we are already in a drag.
            //if (BehaviorService is null && _mouseDragLast != InvalidPoint) {
            //    return;
            //}

            _mouseDragLast = new Point (x, y);
            //_ctrlSelect = (Control.ModifierKeys & Keys.Control) != 0;
            ISelectionService selectionService = GetService<ISelectionService> ();

            // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up. Make sure the component is selected
            //if (!_ctrlSelect && selectionService != null) {
                selectionService.SetSelectedComponents (new object[] { Component }, SelectionTypes.Primary);
            //}

            Control.Capture = true;
        }

        /// <summary>
        ///  Called at the end of a drag operation.  This either commits or rolls back the drag.
        /// </summary>
        internal virtual void OnMouseDragEnd (bool cancel)
        {
            _mouseDragLast = InvalidPoint;
            Control.Capture = false;

            if (!_mouseDragMoved) {
                // ParentControlDesigner.Dispose depends on cancel having this behavior.
                if (!cancel) {
                    ISelectionService selectionService = GetService<ISelectionService> ();
                    bool shiftSelect = false;// (Control.ModifierKeys & Keys.Shift) != 0;
                    if (!shiftSelect &&
                        (_ctrlSelect
                            || (selectionService != null && !selectionService.GetComponentSelected (Component)))) {
                        selectionService?.SetSelectedComponents (new object[] { Component }, SelectionTypes.Primary);
                        _ctrlSelect = false;
                    }
                }

                return;
            }

            _mouseDragMoved = false;
            _ctrlSelect = false;

            // And now finish the drag.
            if (BehaviorService != null && BehaviorService.Dragging && cancel) {
                BehaviorService.CancelDrag = true;
            }

            // Leave this here in case we are doing a ComponentTray drag
            //_selectionUIService ??= GetService<ISelectionUIService> ();

            //if (_selectionUIService is null) {
            //    return;
            //}

            // We must check to ensure that UI service is still in drag mode.  It is possible that the user hit escape,
            // which will cancel drag mode.
            //if (_selectionUIService.Dragging) {
            //    _selectionUIService.EndDrag (cancel);
            //}
        }

        /// <summary>
        ///  Called for each movement of the mouse. This will check to see if a drag operation is in progress. If so,
        ///  it will pass the updated drag dimensions on to the selection UI service.
        /// </summary>
        public virtual void OnMouseDragMove (int x, int y)
        {
            if (!_mouseDragMoved) {
                Size minDrag = System.Windows.Forms.SystemInformation.DragSize;
                Size minDblClick = System.Windows.Forms.SystemInformation.DoubleClickSize;
                minDrag.Width = Math.Max (minDrag.Width, minDblClick.Width);
                minDrag.Height = Math.Max (minDrag.Height, minDblClick.Height);

                // we have to make sure the mouse moved farther than the minimum drag distance before we actually start the drag
                if (_mouseDragLast == InvalidPoint ||
                    (Math.Abs (_mouseDragLast.X - x) < minDrag.Width &&
                     Math.Abs (_mouseDragLast.Y - y) < minDrag.Height)) {
                    return;
                } else {
                    _mouseDragMoved = true;

                    // we're on the move, so we're not in a ctrlSelect
                    _ctrlSelect = false;
                }
            }

            // Make sure the component is selected
            // But only select it if it is not already the primary selection, and we want to toggle the current primary selection.
            ISelectionService selectionService = GetService<ISelectionService> ();
            if (selectionService != null && !Component.Equals (selectionService.PrimarySelection)) {
                selectionService.SetSelectedComponents (new object[] { Component }, SelectionTypes.Primary | SelectionTypes.Toggle);
            }

            if (BehaviorService != null && selectionService != null) {
                // create our list of controls-to-drag
                ArrayList dragControls = new ArrayList ();
                ICollection selComps = selectionService.GetSelectedComponents ();

                // must identify a required parent to avoid dragging mixes of children
                Control requiredParent = null;
                foreach (IComponent comp in selComps) {
                    if (comp is Control control) {
                        if (requiredParent is null) {
                            requiredParent = control.Parent;
                        } else if (!requiredParent.Equals (control.Parent)) {
                            continue;//mixed selection of different parents - don't add this
                        }

                        if (_host.GetDesigner (comp) is ControlDesigner des && (des.SelectionRules & SelectionRules.Moveable) != 0) {
                            dragControls.Add (comp);
                        }
                    }
                }

                // if we have controls-to-drag, create our new behavior and start the drag/drop operation
                //if (dragControls.Count > 0) {
                //    using Graphics adornerGraphics = BehaviorService.AdornerWindowGraphics;
                //    DropSourceBehavior dsb = new DropSourceBehavior (dragControls, Control.Parent, _mouseDragLast);
                //    BehaviorService.DoDragDrop (dsb);
                //}
            }

            _mouseDragLast = InvalidPoint;
            _mouseDragMoved = false;
        }

        /// <summary>
        ///  Called each time the cursor needs to be set.
        /// </summary>
        /// <remarks>
        /// The ControlDesigner behavior here will set the cursor to one of three things:
        ///
        ///  1.  If the toolbox service has a tool selected, it will allow the toolbox service to set the cursor.
        ///  2.  If the selection UI service shows a locked selection, or if there is no location property on the
        ///  control, then the default arrow will be set.
        ///  3.  Otherwise, the four headed arrow will be set to indicate that the component can be clicked and moved.
        ///  4.  If the user is currently dragging a component, the crosshair cursor will be used instead of the four
        ///  headed arrow.
        /// </remarks>
        protected virtual Cursor OnSetCursor ()
        {
            //if (Control.Dock != DockStyle.None) {
            //    Cursor.Current = Cursors.Default;
            //    return;
            //}

            //_toolboxService ??= GetService<IToolboxService> ();

            //if (_toolboxService != null && _toolboxService.SetCursor ()) {
            //    return;
            //}

            //if (!_locationChecked) {
            //    _locationChecked = true;
            //    try {
            //        _hasLocation = TypeDescriptor.GetProperties (Component)["Location"] != null;
            //    } catch {
            //    }
            //}

            //if (!_hasLocation) {
            //    Cursor.Current = Cursor.Default;
            //    return;
            //}

            //if (Locked) {
            //    Cursor.Current = Cursor.Default;
            //    return;
            //}

            //Cursor.Current = Cursors.SizeAll;
            return Cursors.SizeAll;
        }

        /// <summary>
        ///  Retrieves a set of rules concerning the movement capabilities of a component. This should be one or more
        ///  flags from the SelectionRules class.  If no designer provides rules for a component, the component will
        ///  not get any UI services.
        /// </summary>
        public virtual SelectionRules SelectionRules {
            get {
                object component = Component;
                SelectionRules rules = SelectionRules.Visible;
                PropertyDescriptor prop;
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties (component);
                PropertyDescriptor autoSizeProp = props["AutoSize"];
                PropertyDescriptor autoSizeModeProp = props["AutoSizeMode"];

                if ((prop = props["Location"]) != null && !prop.IsReadOnly) {
                    rules |= SelectionRules.Moveable;
                }

                if ((prop = props["Size"]) != null && !prop.IsReadOnly) {
                    //if (AutoResizeHandles && Component != _host.RootComponent) {
                    //    rules = IsResizableConsiderAutoSize (autoSizeProp, autoSizeModeProp)
                    //        ? rules | SelectionRules.AllSizeable
                    //        : rules;
                    //} else {
                        rules |= SelectionRules.AllSizeable;
                    //}
                }

                PropertyDescriptor propDock = props["Dock"];
                if (propDock != null) {
                    DockStyle dock = (DockStyle)(int)propDock.GetValue (component);

                    // gotta adjust if the control's parent is mirrored... this is just such that we add the right
                    // resize handles. We need to do it this way, since resize glyphs are added in  AdornerWindow
                    // coords, and the AdornerWindow is never mirrored.
                    //if (Control.Parent != null Control.Parent.IsMirrored) {
                    //    if (dock == DockStyle.Left) {
                    //        dock = DockStyle.Right;
                    //    } else if (dock == DockStyle.Right) {
                    //        dock = DockStyle.Left;
                    //    }
                    //}

                    switch (dock) {
                        case DockStyle.Top:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Left:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable);
                            break;
                        case DockStyle.Right:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Bottom:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Fill:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.BottomSizeable);
                            break;
                    }
                }

                PropertyDescriptor pd = props["Locked"];
                if (pd != null) {
                    object value = pd.GetValue (component);

                    // Make sure that value is a boolean, in case someone else added this property
                    if (value is bool boolean && boolean == true) {
                        rules = SelectionRules.Locked | SelectionRules.Visible;
                    }
                }

                return rules;
            }
        }

        /// <summary>
        ///  Demand creates the StandardBehavior related to this
        ///  ControlDesigner.  This is used to associate the designer's
        ///  selection glyphs to a common Behavior (resize in this case).
        /// </summary>
        internal virtual Behavior StandardBehavior => _resizeBehavior ??= new ResizeBehavior (Component.Site);

        internal bool TryGetService<T> (out T service) where T : class
        {
            service = GetService<T> ();
            return service is not null;
        }

        /// <summary>
        ///  Accessor method for the Visible property on control. We shadow this property at design time.
        /// </summary>
        private bool Visible {
            get => (bool)ShadowProperties[nameof (Visible)];
            set => ShadowProperties[nameof (Visible)] = value;
        }
    }
}
