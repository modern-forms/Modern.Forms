﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    /// <summary>
    ///  This Glyph is placed on every control sized to the exact bounds of the control.
    /// </summary>
    public class ControlBodyGlyph : ComponentGlyph
    {
        private Rectangle _bounds;                  //bounds of the related control
        private readonly Cursor? _hitTestCursor;    //cursor used to hit test
        private readonly IComponent? _component;

        /// <summary>
        ///  Standard Constructor.
        /// </summary>
        public ControlBodyGlyph (Rectangle bounds, Cursor? cursor, IComponent? relatedComponent, ControlDesigner? designer)
            : base (relatedComponent, new TransparentBehavior (designer))
        {
            _bounds = bounds;
            _hitTestCursor = cursor;
            _component = relatedComponent;
        }

        public ControlBodyGlyph (Rectangle bounds, Cursor? cursor, IComponent? relatedComponent, Behavior? behavior)
            : base (relatedComponent, behavior)
        {
            _bounds = bounds;
            _hitTestCursor = cursor;
            _component = relatedComponent;
        }

        /// <summary>
        ///  The bounds of this glyph.
        /// </summary>
        public override Rectangle Bounds => _bounds;

        /// <summary>
        ///  Simple hit test rule: if the point is contained within the bounds
        ///  AND the component is Visible (controls on some tab pages may
        ///  not be, for ex) then it is a positive hit test.
        /// </summary>
        public override Cursor? GetHitTest (Point p)
        {
            bool isVisible = (_component is Control control) ? control.Visible : true; /*non-controls are always visible here*/

            if (isVisible && _bounds.Contains (p)) {
                return _hitTestCursor;
            }

            return null;
        }
    }
}
