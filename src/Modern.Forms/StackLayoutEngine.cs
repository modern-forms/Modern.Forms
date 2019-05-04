using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class StackLayoutEngine
    {
        private Orientation orientation;
        private bool expand;

        public StackLayoutEngine (Orientation orientation = Orientation.Vertical, bool expand = false)
        {
            this.orientation = orientation;
            this.expand = expand;
        }

        public void Layout (Rectangle bounds, IEnumerable<ILayoutable> items)
        {
            var x = bounds.Left;
            var y = bounds.Top;

            foreach (var item in items) {
                var local_x = x + item.Margin.Left;
                var local_y = y + item.Margin.Top;
                var height = bounds.Height - item.Margin.Vertical;

                var size = item.GetPreferredSize (Size.Empty);

                item.SetBounds (local_x, local_y, size.Width, height);

                x += size.Width + item.Margin.Horizontal;
            }
        }
    }
}
