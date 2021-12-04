using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public sealed class BehaviorService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;             // standard service provider
        private readonly AdornerWindow _adornerWindow;                  // the transparent window all glyphs are drawn to
        private readonly ArrayList _behaviorStack;                      // the stack behavior objects can be pushed to and popped from

        private Behavior? _captureBehavior;                             // the behavior that currently has capture; may be null
        private Glyph? _hitTestedGlyph;                                  // the last valid glyph that was hit tested
        private System.Windows.Forms.DragEventArgs _validDragArgs;                           // if valid - this is used to fabricate drag enter/leave events

        internal BehaviorService (IServiceProvider serviceProvider, DesignerFrame windowFrame)
        {
            _serviceProvider = serviceProvider;
            _adornerWindow = new AdornerWindow (this, windowFrame);

            // Start with an empty adorner collection & no behavior on the stack
            Adorners = new BehaviorServiceAdornerCollection (this);
            _behaviorStack = new ArrayList ();
        }

        /// <summary>
        ///  Read-only property that returns the AdornerCollection that the BehaviorService manages.
        /// </summary>
        public BehaviorServiceAdornerCollection Adorners { get; }

        public void Dispose ()
        {
        }

        private Behavior GetAppropriateBehavior (Glyph g)
        {
            if (_behaviorStack != null && _behaviorStack.Count > 0) {
                return _behaviorStack[0] as Behavior;
            }

            if (g != null && g.Behavior != null) {
                return g.Behavior;
            }

            return null;
        }

        /// <summary>
        ///  Given a behavior returns the behavior immediately after the behavior in the behaviorstack.
        ///  Can return null.
        /// </summary>
        public Behavior GetNextBehavior (Behavior behavior)
        {
            if (_behaviorStack != null && _behaviorStack.Count > 0) {
                int index = _behaviorStack.IndexOf (behavior);
                if ((index != -1) && (index < _behaviorStack.Count - 1)) {
                    return _behaviorStack[index + 1] as Behavior;
                }
            }

            return null;
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refresh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate ()
        {
            _adornerWindow.Invalidate ();
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refresh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate (Rectangle rect)
        {
            _adornerWindow.Invalidate (rect);
        }

        private void InvokeMouseEnterLeave (Glyph leaveGlyph, Glyph enterGlyph)
        {
            if (leaveGlyph != null) {
                if (enterGlyph != null && leaveGlyph.Equals (enterGlyph)) {
                    // Same glyph - no change
                    return;
                }
                // TODO
                if (_validDragArgs != null) {
                    //OnDragLeave (leaveGlyph, EventArgs.Empty);
                } else {
                    //OnMouseLeave (leaveGlyph);
                }
            }

            if (enterGlyph != null) {
                if (_validDragArgs != null) {
                    //OnDragEnter (enterGlyph, _validDragArgs);
                } else {
                    //OnMouseEnter (enterGlyph);
                }
            }
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refresh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        //public void Invalidate (Region r) => _adornerWindow.InvalidateAdornerWindow (r);

        internal void OnLoseCapture ()
        {
            if (_captureBehavior != null) {
                Behavior b = _captureBehavior;
                _captureBehavior = null;
                try {
                    b.OnLoseCapture (_hitTestedGlyph, EventArgs.Empty);
                } catch {
                }
            }
        }

        internal bool OnMouseDown (MouseButtons button, Point mouseLoc)
            => GetAppropriateBehavior (_hitTestedGlyph)?.OnMouseDown (_hitTestedGlyph, button, mouseLoc) ?? false;

        internal bool PropagateHitTest (Point pt)
        {
            for (int i = Adorners.Count - 1; i >= 0; i--) {
                if (!Adorners[i].Enabled) {
                    continue;
                }

                for (int j = 0; j < Adorners[i].Glyphs.Count; j++) {
                    Cursor hitTestCursor = Adorners[i].Glyphs[j].GetHitTest (pt);
                    if (hitTestCursor != null) {
                        // InvokeMouseEnterGlyph will cause the selection to change, which might change the number of glyphs, so we need to remember the new glyph before calling InvokeMouseEnterLeave. VSWhidbey #396611
                        Glyph newGlyph = Adorners[i].Glyphs[j];

                        //with a valid hit test, fire enter/leave events
                        InvokeMouseEnterLeave (_hitTestedGlyph, newGlyph);
                        if (_validDragArgs is null) {
                            //if we're not dragging, set the appropriate cursor
                            SetAppropriateCursor (hitTestCursor);
                        }

                        _hitTestedGlyph = newGlyph;
                        //return true if we hit on a transparentBehavior, otherwise false
                        return (_hitTestedGlyph.Behavior is TransparentBehavior);
                    }
                }
            }

            InvokeMouseEnterLeave (_hitTestedGlyph, null);
            if (_validDragArgs is null) {
                Cursor cursor = Cursor.Default;
                if ((_behaviorStack != null) && (_behaviorStack.Count > 0)) {
                    if (_behaviorStack[0] is Behavior behavior) {
                        cursor = behavior.Cursor;
                    }
                }

                SetAppropriateCursor (cursor);
            }

            _hitTestedGlyph = null;

            // Returning false will cause the transparent window to return HTCLIENT when handling WM_NCHITTEST,
            // thus blocking underline window to receive mouse events.
            return true;
        }

        internal void PropagatePaint (PaintEventArgs pe)
        {
            for (int i = 0; i < Adorners.Count; i++) {
                if (!Adorners[i].Enabled) {
                    continue;
                }

                for (int j = Adorners[i].Glyphs.Count - 1; j >= 0; j--) {
                    Adorners[i].Glyphs[j].Paint (pe);
                }
            }
        }

        /// <summary>
        ///  Pushes a Behavior object onto the BehaviorStack.  This is often done through hit-tested Glyph.
        /// </summary>
        public void PushBehavior (Behavior behavior)
        {
            ArgumentNullException.ThrowIfNull (behavior);

            // Should we catch this
            _behaviorStack.Insert (0, behavior);

            // If there is a capture behavior, and it isn't this behavior, notify it that it no longer has capture.
            if (_captureBehavior != null && _captureBehavior != behavior) {
                OnLoseCapture ();
            }
        }

        private void SetAppropriateCursor (Cursor cursor)
        {
            //default cursors will let the toolbox svc set a cursor if needed
            //if (cursor == Cursor.Default) {
            //    if (_toolboxSvc is null) {
            //        _toolboxSvc = (IToolboxService)_serviceProvider.GetService (typeof (IToolboxService));
            //    }

            //    if (_toolboxSvc != null && _toolboxSvc.SetCursor ()) {
            //        cursor = new Cursor (User32.GetCursor ());
            //    }
            //}

            _adornerWindow.Cursor = cursor;
        }
    }
}
