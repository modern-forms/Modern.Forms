using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    /// <summary>
    ///  The SelectionBehavior is pushed onto the BehaviorStack in response to a positively hit tested SelectionGlyph.  The SelectionBehavior performs  two main tasks: 1) forward messages to the related ControlDesigner, and 2) calls upon the SelectionManager to push a potential DragBehavior.
    /// </summary>
    internal sealed class SelectionManager : IDisposable
    {
        private Adorner _selectionAdorner;                  //used to provide all selection glyphs
        private Adorner _bodyAdorner;                       //used to track all body glyphs for each control
        private BehaviorService _behaviorService;           //ptr back to our BehaviorService
        private IServiceProvider _serviceProvider;          //standard service provider
        private readonly Hashtable _componentToDesigner;    //used for quick look up of designers related to comps
        private readonly Form _rootComponent;            //root component being designed
        private ISelectionService _selSvc;                  //we cache the selection service for perf.
        private IDesignerHost _designerHost;                //we cache the designerhost for perf.
        private bool _needRefresh;                          // do we need to refresh?
        private Rectangle[] _prevSelectionBounds;           //used to only repaint the changing part of the selection
        private object _prevPrimarySelection;               //used to check if the primary selection changed
        private Rectangle[] _curSelectionBounds;
        private int _curCompIndex;
        //private DesignerActionUI _designerActionUI;         // the "container" for all things related to the designer action (smarttags) UI
        private bool _selectionChanging;                    //we don't want the OnSelectionChanged to be recursively called.

        /// <summary>
        ///  Constructor.  Here we query for necessary services and cache them for perf. reasons. We also hook to Component Added/Removed/Changed notifications so we can keep in sync when the designers' components change.  Also, we create our custom Adorner and add it to the BehaviorService.
        /// </summary>
        public SelectionManager (IServiceProvider serviceProvider, BehaviorService behaviorService)
        {
            _prevSelectionBounds = null;
            _prevPrimarySelection = null;
            _behaviorService = behaviorService;
            _serviceProvider = serviceProvider;

            _selSvc = (ISelectionService)serviceProvider.GetService (typeof (ISelectionService));
            _designerHost = (IDesignerHost)serviceProvider.GetService (typeof (IDesignerHost));

            if (_designerHost is null || _selSvc is null) {
                Debug.Fail ("SelectionManager - Host or SelSvc is null, can't continue");
            }

            //sync the BehaviorService's begindrag event
            //behaviorService.BeginDrag += new BehaviorDragDropEventHandler (OnBeginDrag);

            //sync the BehaviorService's Synchronize event
            //behaviorService.Synchronize += new EventHandler (OnSynchronize);

            _selSvc.SelectionChanged += new EventHandler (OnSelectionChanged);
            _rootComponent = (Form)_designerHost.RootComponent;

            //create and add both of our adorners,
            //one for selection, one for bodies
            _selectionAdorner = new Adorner ();
            _bodyAdorner = new Adorner ();
            behaviorService.Adorners.Add (_bodyAdorner);
            behaviorService.Adorners.Add (_selectionAdorner); //adding this will cause the adorner to get setup with a ptr
                                                              //to the beh.svc.

            _componentToDesigner = new Hashtable ();

            IComponentChangeService cs = (IComponentChangeService)serviceProvider.GetService (typeof (IComponentChangeService));
            if (cs != null) {
                cs.ComponentAdded += new ComponentEventHandler (OnComponentAdded);
                cs.ComponentRemoved += new ComponentEventHandler (OnComponentRemoved);
                //cs.ComponentChanged += new ComponentChangedEventHandler (OnComponentChanged);
            }

            //_designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler (OnTransactionClosed);

            // designeraction UI
            //if (_designerHost.GetService (typeof (DesignerOptionService)) is DesignerOptionService options) {
            //    PropertyDescriptor p = options.Options.Properties["UseSmartTags"];
            //    if (p != null && p.PropertyType == typeof (bool) && (bool)p.GetValue (null)) {
            //        _designerActionUI = new DesignerActionUI (serviceProvider, _selectionAdorner);
            //        behaviorService.DesignerActionUI = _designerActionUI;
            //    }
            //}
        }

        /// <summary>
        ///  This method fist calls the recursive AddControlGlyphs() method. When finished, we add the final glyph(s)
        ///  to the root comp.
        /// </summary>
        private void AddAllControlGlyphs (Control parent, ArrayList selComps, object primarySelection)
        {
            foreach (Control control in parent.Controls) {
                AddAllControlGlyphs (control, selComps, primarySelection);
            }

            GlyphSelectionType selType = GlyphSelectionType.NotSelected;
            if (selComps.Contains (parent)) {
                if (parent.Equals (primarySelection)) {
                    selType = GlyphSelectionType.SelectedPrimary;
                } else {
                    selType = GlyphSelectionType.Selected;
                }
            }

            AddControlGlyphs (parent, selType);
        }

        /// <summary>
        ///  Recursive method that goes through and adds all the glyphs of every child to our global Adorner.
        /// </summary>
        private void AddControlGlyphs (Control c, GlyphSelectionType selType)
        {
            ControlDesigner cd = (ControlDesigner)_componentToDesigner[c];
            if (cd != null) {
                ControlBodyGlyph bodyGlyph = cd.GetControlGlyphInternal (selType);
                if (bodyGlyph != null) {
                    _bodyAdorner.Glyphs.Add (bodyGlyph);
                    if (selType == GlyphSelectionType.SelectedPrimary ||
                        selType == GlyphSelectionType.Selected) {
                        if (_curSelectionBounds[_curCompIndex] == Rectangle.Empty) {
                            _curSelectionBounds[_curCompIndex] = bodyGlyph.Bounds;
                        } else {
                            _curSelectionBounds[_curCompIndex] = Rectangle.Union (_curSelectionBounds[_curCompIndex], bodyGlyph.Bounds);
                        }
                    }
                }

                GlyphCollection glyphs = cd.GetGlyphs (selType);
                if (glyphs != null) {
                    _selectionAdorner.Glyphs.AddRange (glyphs);
                    if (selType == GlyphSelectionType.SelectedPrimary ||
                        selType == GlyphSelectionType.Selected) {
                        foreach (Glyph glyph in glyphs) {
                            _curSelectionBounds[_curCompIndex] = Rectangle.Union (_curSelectionBounds[_curCompIndex], glyph.Bounds);
                        }
                    }
                }
            }

            if (selType == GlyphSelectionType.SelectedPrimary || selType == GlyphSelectionType.Selected) {
                _curCompIndex++;
            }
        }

        /// <summary>
        ///  Returns the Adorner that contains all the BodyGlyphs for the current selection state.
        /// </summary>
        internal Adorner BodyGlyphAdorner {
            get => _bodyAdorner;
        }

        /// <summary>
        ///  Computes the region representing the difference between the old selection and the new selection.
        /// </summary>
        private Region DetermineRegionToRefresh (object primarySelection)
        {
            Region toRefresh = new Region (Rectangle.Empty);
            Rectangle[] larger;
            Rectangle[] smaller;
            if (_curSelectionBounds.Length >= _prevSelectionBounds.Length) {
                larger = _curSelectionBounds;
                smaller = _prevSelectionBounds;
            } else {
                larger = _prevSelectionBounds;
                smaller = _curSelectionBounds;
            }

            // we need to make sure all of the rects in the smaller array are
            // accounted for.  Any that don't intersect a rect in the larger
            // array need to be included in the region to repaint.
            bool[] intersected = new bool[smaller.Length];
            for (int i = 0; i < smaller.Length; i++) {
                intersected[i] = false;
            }

            // determine which rects in the larger array need to be
            // included in the region to invalidate by intersecting
            // with rects in the smaller array.
            for (int l = 0; l < larger.Length; l++) {
                bool largeIntersected = false;
                Rectangle large = larger[l];
                for (int s = 0; s < smaller.Length; s++) {
                    if (large.IntersectsWith (smaller[s])) {
                        Rectangle small = smaller[s];
                        largeIntersected = true;
                        if (large != small) {
                            toRefresh.Union (large);
                            toRefresh.Union (small);
                        }

                        intersected[s] = true;
                        break;
                    }
                }

                if (!largeIntersected) {
                    toRefresh.Union (large);
                }
            }

            // now add any rects from the smaller array that weren't accounted for
            for (int k = 0; k < intersected.Length; k++) {
                if (!intersected[k]) {
                    toRefresh.Union (smaller[k]);
                }
            }

            //using (Graphics g = _behaviorService.AdornerWindowGraphics) {
            //    // If all that changed was the primary selection, then the refresh region was empty, but we do need to update the 2 controls.
            //    if (toRefresh.IsEmpty (g) && primarySelection != null && !primarySelection.Equals (_prevPrimarySelection)) {
            //        for (int i = 0; i < _curSelectionBounds.Length; i++) {
            //            toRefresh.Union (_curSelectionBounds[i]);
            //        }
            //    }
            //}

            return toRefresh;
        }

        /// <summary>
        ///  There are certain cases like Adding Item to ToolStrips through InSitu Editor, where there is
        ///  ParentTransaction that has to be cancelled depending upon the user action When this parent transaction is
        ///  cancelled, there may be no reason to REFRESH the selectionManager which actually clears all the glyphs and
        ///  readds them This REFRESH causes a lot of flicker and can be avoided by setting this property to false.
        ///  Since this property is checked in the TransactionClosed, the SelectionManager won't REFRESH and hence
        ///  just eat up the refresh thus avoiding unnecessary flicker.
        /// </summary>
        internal bool NeedRefresh {
            get => _needRefresh;
            set => _needRefresh = value;
        }

        /// <summary>
        ///  When a component is added, we get the designer and add it to our hashtable for quick lookup.
        /// </summary>
        private void OnComponentAdded (object source, ComponentEventArgs ce)
        {
            IComponent component = ce.Component;
            IDesigner designer = _designerHost.GetDesigner (component);
            if (designer is ControlDesigner) {
                _componentToDesigner.Add (component, designer);
            }
        }

        /// <summary>
        ///  When a component is removed - we remove the key and value from our hashtable.
        /// </summary>
        private void OnComponentRemoved (object source, ComponentEventArgs ce)
        {
            if (_componentToDesigner.Contains (ce.Component)) {
                _componentToDesigner.Remove (ce.Component);
            }

            //remove the associated designeractionpanel
            //if (_designerActionUI != null) {
            //    _designerActionUI.RemoveActionGlyph (ce.Component);
            //}
        }

        /// <summary>
        ///  On every selectionchange, we remove all glyphs, get the newly selected components, and re-add all glyphs back to the Adorner.
        /// </summary>
        private void OnSelectionChanged (object sender, EventArgs e)
        {
            // Note: selectionChanging would guard against a re-entrant code...
            // Since we don't want to be in messed up state when adding new Glyphs.
            if (!_selectionChanging) {
                _selectionChanging = true;

                _selectionAdorner.Glyphs.Clear ();
                _bodyAdorner.Glyphs.Clear ();

                ArrayList selComps = new ArrayList (_selSvc.GetSelectedComponents ());
                object primarySelection = _selSvc.PrimarySelection;

                //add all control glyphs to all controls on rootComp
                _curCompIndex = 0;
                _curSelectionBounds = new Rectangle[selComps.Count];
                AddAllControlGlyphs (_rootComponent.adapter, selComps, primarySelection);

                if (_prevSelectionBounds != null) {
                    //Region toUpdate = DetermineRegionToRefresh (primarySelection);
                    //using (Graphics g = _behaviorService.AdornerWindowGraphics) {
                    //    if (!toUpdate.IsEmpty (g)) {
                            _selectionAdorner.Invalidate (/*toUpdate*/);
                    //    }
                    //}
                } else {
                    // There was no previous selection, so just invalidate
                    // the current selection
                    if (_curSelectionBounds.Length > 0) {
                        Rectangle toUpdate = _curSelectionBounds[0];
                        for (int i = 1; i < _curSelectionBounds.Length; i++) {
                            toUpdate = Rectangle.Union (toUpdate, _curSelectionBounds[i]);
                        }

                        if (toUpdate != Rectangle.Empty) {
                            _selectionAdorner.Invalidate (toUpdate);
                        }
                    } else {
                        _selectionAdorner.Invalidate ();
                    }
                }

                _prevPrimarySelection = primarySelection;
                if (_curSelectionBounds.Length > 0) {
                    _prevSelectionBounds = new Rectangle[_curSelectionBounds.Length];
                    Array.Copy (_curSelectionBounds, _prevSelectionBounds, _curSelectionBounds.Length);
                } else {
                    _prevSelectionBounds = null;
                }

                _selectionChanging = false;
            }
        }

        /// <summary>
        ///  Refreshes all selection Glyphs.
        /// </summary>
        public void Refresh ()
        {
            NeedRefresh = false;
            OnSelectionChanged (this, null);
        }

        /// <summary>
        ///  Returns the Adorner that contains all the BodyGlyphs for the current selection state.
        /// </summary>
        internal Adorner SelectionGlyphAdorner {
            get => _selectionAdorner;
        }

        public void Dispose ()
        {
            //throw new NotImplementedException ();
        }
    }
}
