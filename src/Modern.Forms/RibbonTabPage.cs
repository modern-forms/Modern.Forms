using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonTabPage
    {
        public string Text { get; set; }
        public bool Selected { get; set; }
        public bool Highlighted { get; set; }

        public List<RibbonItemGroup> Groups { get; } = new List<RibbonItemGroup> ();

        // This is the bounds for the tab page, where the buttons are
        public Rectangle Bounds { get; private set; }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public void DrawTabPage (SKCanvas canvas)
        {
            canvas.FillRectangle (Bounds, ModernTheme.NeutralGray);

            LayoutItems ();

            foreach (var group in Groups)
                group.DrawGroup (canvas);
        }

        private void LayoutItems ()
        {
            var group_padding = 8;
            var x = 0;
            var y = Bounds.Y + 5;
            var item_width = 45;
            var item_height = 73;
            var item_padding = 0;

            // Lay out each group
            foreach (var group in Groups) {
                var item_count = group.Items.Count;
                var width = group_padding + item_count * item_width + (item_count - 1) * item_padding + group_padding + 1;

                group.SetBounds (x, y, width, item_height);
                x += width;
            }
        }
    }
}
