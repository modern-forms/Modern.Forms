using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    // TODO: Base class should be ScrollableControl
    public class Panel : LiteControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public Panel ()
        {
            TabStop = false;

            //SetStyle (ControlStyles.Selectable, false);
        }

        protected override Size DefaultSize => new Size (200, 100);

        public override Size GetPreferredSize (Size proposedSize)
        {
            var size = Size.Empty;

            foreach (var child in Controls) {
                if (child.Dock == DockStyle.Fill) {
                    if (child.Bounds.Right > size.Width)
                        size.Width = child.Bounds.Right;
                } else if (child.Dock != DockStyle.Top && child.Dock != DockStyle.Bottom && (child.Anchor & AnchorStyles.Right) == 0 && (child.Bounds.Right + child.Margin.Right) > size.Width)
                    size.Width = child.Bounds.Right + child.Margin.Right;

                if (child.Dock == DockStyle.Fill) {
                    if (child.Bounds.Bottom > size.Height)
                        size.Height = child.Bounds.Bottom;
                } else if (child.Dock != DockStyle.Left && child.Dock != DockStyle.Right && (child.Anchor & AnchorStyles.Bottom) == 0 && (child.Bounds.Bottom + child.Margin.Bottom) > size.Height)
                    size.Height = child.Bounds.Bottom + child.Margin.Bottom;
            }

            return size;
        }
    }
}
