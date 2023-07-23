using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ToolBar control.
    /// </summary>
    public class ToolBar : MenuBase
    {
        /// <summary>
        /// Initializes a new instance of the ToolBar class.
        /// </summary>
        public ToolBar ()
        {
            Dock = DockStyle.Top;
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 34);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
          (style) => {
              style.Border.Bottom.Width = 1;
          });

        /// <inheritdoc/>
        protected override bool IsTopLevelMenu => true;

        /// <inheritdoc/>
        protected override void LayoutItems ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ClientRectangle, Items.Cast<ILayoutable> ());
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
