using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public abstract class Behavior
    {
        private readonly bool _callParentBehavior;
        private readonly BehaviorService? _behaviorService;

        protected Behavior ()
        {
        }

        /// <param name="callParentBehavior">
        ///  `true` if the parentBehavior should be called if it exists. The parentBehavior is the next behavior on
        ///  the behaviorService stack.If true, <paramref name="behaviorService"/> must be non-null.
        /// </param>
        protected Behavior (bool callParentBehavior, BehaviorService? behaviorService)
        {
            if ((callParentBehavior == true) && (behaviorService == null)) {
                throw new ArgumentException (null, nameof (behaviorService));
            }

            _callParentBehavior = callParentBehavior;
            _behaviorService = behaviorService;
        }

        /// <summary>
        ///  The cursor that should be displayed for this behavior.
        /// </summary>
        public virtual Cursor Cursor => Cursor.Default;

        /// <summary>
        ///  The heuristic we will follow when any of these methods are called
        ///  is that we will attempt to pass the message along to the glyph.
        ///  This is a helper method to ensure validity before forwarding the message.
        /// </summary>
        private Behavior? GetGlyphBehavior (Glyph? g)
        {
            return g?.Behavior != null && g.Behavior != this ? g.Behavior : null;
        }

        private Behavior? GetNextBehavior => _behaviorService?.GetNextBehavior (this);

        /// <summary>
        ///  A behavior can request mouse capture through the behavior service by pushing itself with
        ///  PushCaptureBehavior.  If it does so, it will be notified through OnLoseCapture when capture is lost.
        ///  Generally the behavior pops itself at this time. Capture is lost when one of the following occurs:
        ///
        ///  1. Someone else requests capture.
        ///  2. Another behavior is pushed.
        ///  3. This behavior is popped.
        ///
        ///  In each of these cases OnLoseCapture on the behavior will be called.
        /// </summary>
        public virtual void OnLoseCapture (Glyph? g, EventArgs e)
        {
            if (_callParentBehavior && GetNextBehavior != null) {
                GetNextBehavior.OnLoseCapture (g, e);
            } else if (GetGlyphBehavior (g) is Behavior behavior) {
                behavior.OnLoseCapture (g, e);
            }
        }

        /// <summary>
        ///  When any MouseDown message enters the BehaviorService's AdornerWindow (nclbuttondown, lbuttondown,
        ///  rbuttondown, nclrbuttondown) it is first passed here, to the top-most Behavior in the BehaviorStack.
        ///  Returning 'true' from this function signifies that the Message was 'handled' by the Behavior and
        ///  should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseDown (Glyph? g, MouseButtons button, Point mouseLoc)
        {
            if (_callParentBehavior && GetNextBehavior != null) {
                return GetNextBehavior.OnMouseDown (g, button, mouseLoc);
            } else if (GetGlyphBehavior (g) is Behavior behavior) {
                return behavior.OnMouseDown (g, button, mouseLoc);
            } else {
                return false;
            }
        }
    }
}
