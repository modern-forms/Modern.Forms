using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItemGroup
    {
        public string Text { get; set; }

        public List<RibbonItem> Items { get; } = new List<RibbonItem> ();

        public Rectangle Bounds { get; private set; }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);

            var group_padding = 8;
            var item_x = x + group_padding;
            var item_width = 45;
            var item_padding = 0;

            // Lay out each item
            foreach (var item in Items) {
                item.SetBounds (item_x, y, item_width, height);
                item_x += item_width + item_padding;
            }
        }

        public void DrawGroup (SKCanvas canvas)
        {
            // Draw each ribbon item
            foreach (var item in Items)
                item.DrawItem (canvas);

            // Right border (group separator)
            canvas.DrawLine (Bounds.Right - 1, Bounds.Top, Bounds.Right - 1, Bounds.Bottom, ModernTheme.BorderGray);
        }
    }
}
