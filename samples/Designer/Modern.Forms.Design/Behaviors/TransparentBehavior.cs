using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    /// <summary>
    ///  This TransparentBehavior is associated with the BodyGlyph for this ControlDesigner.  When the
    ///  BehaviorService hittests a glyph w/a TransparentBehavior, all messages will be passed through the
    ///  BehaviorService directly to the ControlDesigner. During a Drag operation, when the BehaviorService hittests
    /// </summary>
    internal class TransparentBehavior : Behavior
    {
        private readonly ControlDesigner _designer;
        private Rectangle _controlRect = Rectangle.Empty;

        /// <summary>
        ///  Constructor that accepts the related ControlDesigner.
        /// </summary>
        internal TransparentBehavior (ControlDesigner designer) => _designer = designer;

        /// <summary>
        ///  This property performs a hit test on the ControlDesigner to determine if the BodyGlyph should return
        ///  '-1' for hit testing (letting all messages pass directly to the control).
        /// </summary>
        //internal bool IsTransparent (Point p) => _designer.GetHitTest (p);
    }
}
