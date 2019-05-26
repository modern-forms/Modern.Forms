using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    // This is used by ScrollableControl when both scrollbars are shown to draw
    // the corner where they meet, so any control behind it doesn't bleed through
    internal class SizeGrip : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public SizeGrip ()
        {
            TabStop = false;

            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        protected override Size DefaultSize => new Size (15, 15);
    }
}
