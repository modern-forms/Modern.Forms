using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class ToolBar : MenuBase
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
          (style) => {
              style.BackgroundColor = Theme.NeutralGray;
              style.Border.Bottom.Width = 1;
          });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public ToolBar ()
        {
            Dock = DockStyle.Top;
        }

        protected override Size DefaultSize => new Size (600, 34);

        protected override bool IsTopLevelMenu => true;

        protected override void LayoutItems ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ClientRectangle, Items.Cast<ILayoutable> ());
        }
    }
}
