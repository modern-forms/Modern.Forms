using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class ListViewItem
    {
        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public object Tag { get; set; }
        public ListView Parent { get; internal set; }

        public Rectangle Bounds { get; private set; }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        internal void DrawItem (SKCanvas canvas)
        {
            if (Selected)
                canvas.FillRectangle (Bounds, Theme.RibbonItemHighlightColor);

            var image_size = LogicalToDeviceUnits (32);
            var image_area = new Rectangle (Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Width);
            var image_bounds = DrawingExtensions.CenterSquare (image_area, image_size);
            image_bounds.Y = Bounds.Top + LogicalToDeviceUnits (3);

            if (Image != null)
                canvas.DrawBitmap (Image, image_bounds);

            if (!string.IsNullOrWhiteSpace (Text)) {
                var font_size = LogicalToDeviceUnits (Theme.RibbonItemFontSize);

                canvas.Save ();
                canvas.Clip (Bounds);

                var lines = Text.Split (' ');

                canvas.DrawCenteredText (lines[0].Trim (), Theme.UIFont, font_size, Bounds.Left + Bounds.Width / 2, Bounds.Top + LogicalToDeviceUnits (50), Theme.DarkTextColor);

                if (lines.Length > 1)
                    canvas.DrawCenteredText (lines[1].Trim (), Theme.UIFont, font_size, Bounds.Left + Bounds.Width / 2, Bounds.Top + LogicalToDeviceUnits (66), Theme.DarkTextColor);

                canvas.Restore ();
            }
        }

        private int LogicalToDeviceUnits (int value) => Parent?.LogicalToDeviceUnits (value) ?? value;
    }
}
