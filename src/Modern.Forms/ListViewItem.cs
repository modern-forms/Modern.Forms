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
        public string Text { get; set; } = string.Empty;
        public SKBitmap? Image { get; set; }
        public bool Selected { get; set; }
        public object? Tag { get; set; }
        public ListView? Parent { get; internal set; }

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

                var text_bounds = new Rectangle (Bounds.Left, image_bounds.Bottom + LogicalToDeviceUnits (3), Bounds.Width, Bounds.Bottom - image_bounds.Bottom - LogicalToDeviceUnits (3));

                canvas.DrawText (Text, Theme.UIFont, font_size, text_bounds, Theme.DarkTextColor);

                canvas.Restore ();
            }
        }

        private int LogicalToDeviceUnits (int value) => Parent?.LogicalToDeviceUnits (value) ?? value;
    }
}
