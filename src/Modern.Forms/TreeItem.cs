using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeItem
    {
        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public object Tag { get; set; }

        public Rectangle Bounds { get; private set; }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        internal void DrawItem (SKCanvas canvas)
        {
            var background_color = Selected ? Theme.RibbonItemHighlightColor : Theme.LightNeutralGray;

            canvas.FillRectangle (Bounds, background_color);

            if (Image != null)
                canvas.DrawBitmap (Image, Bounds.Left + 7, Bounds.Top + 7);

            canvas.DrawText (Text.Trim (), Theme.UIFont, 14, Bounds.Left + 31, Bounds.Top + 20, Theme.DarkTextColor);
        }
    }
}
