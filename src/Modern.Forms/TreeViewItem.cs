using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItem : ILayoutable
    {
        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public object Tag { get; set; }
        public TreeView Parent { get; internal set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin => new Padding (0);

        public Size GetPreferredSize (Size proposedSize) => new Size (0, 30);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        internal void OnPaint (SKCanvas canvas)
        {
            var background_color = Selected ? Theme.RibbonItemHighlightColor : Theme.LightNeutralGray;

            canvas.FillRectangle (Bounds, background_color);

            if (Image != null)
                canvas.DrawBitmap (Image, Bounds.Left + 7, Bounds.Top + 7);

            canvas.DrawText (Text.Trim (), Theme.UIFont, 14, Bounds.Left + 31, Bounds.Top + 20, Theme.DarkTextColor);
        }
    }
}
