using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class StackLayoutEngine
    {
        public static StackLayoutEngine Horizontal = new StackLayoutEngine (Orientation.Horizontal);
        public static StackLayoutEngine HorizontalExpand = new StackLayoutEngine (Orientation.Horizontal, true);
        public static StackLayoutEngine Vertical = new StackLayoutEngine (Orientation.Vertical);
        public static StackLayoutEngine VerticalExpand = new StackLayoutEngine (Orientation.Vertical, true);

        private Orientation orientation;
        private bool expand;

        public StackLayoutEngine (Orientation orientation = Orientation.Vertical, bool expand = false)
        {
            this.orientation = orientation;
            this.expand = expand;
        }

        public void Layout (Rectangle bounds, IEnumerable<ILayoutable> items)
        {
            if (orientation == Orientation.Horizontal)
                LayoutHorizontal (bounds, items);
            else
                LayoutVertical (bounds, items);
        }

        private void LayoutHorizontal (Rectangle bounds, IEnumerable<ILayoutable> items)
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

        private void LayoutVertical (Rectangle bounds, IEnumerable<ILayoutable> items)
        {
            var x = bounds.Left;
            var y = bounds.Top;

            foreach (var item in items) {
                var local_x = x + item.Margin.Left;
                var local_y = y + item.Margin.Top;
                var width = bounds.Width - item.Margin.Horizontal;

                var size = item.GetPreferredSize (Size.Empty);

                item.SetBounds (local_x, local_y, width, size.Height);

                y += size.Height + item.Margin.Vertical;
            }
        }
    }
}
