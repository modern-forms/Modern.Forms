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
    internal class ResizeBehavior : Behavior
    {
        private readonly IServiceProvider _serviceProvider;
        private BehaviorService _behaviorService;

        private ResizeComponent[] _resizeComponents;
        private SelectionRules _targetResizeRules; //rules dictating which sizes we can change
        private bool _pushedBehavior;
        private Cursor _cursor = Cursor.Default; //used to set the correct cursor during resizing
        private Point _initialPoint; //the initial point of the mouse down
        private Control _primaryControl; //the primary control the status bar will queue off of
        private Point _lastMouseLoc; //helps us avoid re-entering code if the mouse hasn't moved

        /// <summary>
        ///  Constructor that caches all values for perf. reasons.
        /// </summary>
        internal ResizeBehavior (IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //_dragging = false;
            _pushedBehavior = false;
            //_lastSnapOffset = Point.Empty;
            //_didSnap = false;
            //_statusCommandUI = new StatusCommandUI (serviceProvider);
        }

        /// <summary>
        ///  Demand creates the BehaviorService.
        /// </summary>
        private BehaviorService BehaviorService {
            get {
                if (_behaviorService is null) {
                    _behaviorService = (BehaviorService)_serviceProvider.GetService (typeof (BehaviorService));
                }

                return _behaviorService;
            }
        }

        /// <summary>
        ///  In response to a MouseDown, the SelectionBehavior will push (initiate) a dragBehavior by alerting the SelectionMananger that a new control has been selected and the mouse is down. Note that this is only if we find the related control's Dock property == none.
        /// </summary>
        public override bool OnMouseDown (Glyph g, MouseButtons button, Point mouseLoc)
        {
            //we only care about the right mouse button for resizing
            if (button != MouseButtons.Left) {
                //pass any other mouse click along - unless we've already started our resize in which case we'll ignore it
                return _pushedBehavior;
            }

            //start with no selection rules and try to obtain this info from the glyph
            _targetResizeRules = SelectionRules.None;
            if (g is SelectionGlyphBase sgb) {
                _targetResizeRules = sgb.SelectionRules;
                _cursor = sgb.HitTestCursor;
            }

            if (_targetResizeRules == SelectionRules.None) {
                return false;
            }

            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService (typeof (ISelectionService));
            if (selSvc is null) {
                return false;
            }

            _initialPoint = mouseLoc;
            _lastMouseLoc = mouseLoc;
            //build up a list of our selected controls
            _primaryControl = selSvc.PrimarySelection as Control;

            // Since we don't know exactly how many valid objects we are going to have we use this temp
            ArrayList components = new ArrayList ();
            foreach (object o in selSvc.GetSelectedComponents ()) {
                if (o is Control) {
                    //don't drag locked controls
                    PropertyDescriptor prop = TypeDescriptor.GetProperties (o)["Locked"];
                    if (prop != null) {
                        if ((bool)prop.GetValue (o)) {
                            continue;
                        }
                    }

                    components.Add (o);
                }
            }

            if (components.Count == 0) {
                return false;
            }

            _resizeComponents = new ResizeComponent[components.Count];
            for (int i = 0; i < components.Count; i++) {
                _resizeComponents[i].resizeControl = components[i];
            }

            //push this resizebehavior
            _pushedBehavior = true;
            BehaviorService.PushCaptureBehavior (this);
            return false;
        }

        private struct ResizeComponent
        {
            public object resizeControl;
            public Rectangle resizeBounds;
            public SelectionRules resizeRules;
        }
    }
}
