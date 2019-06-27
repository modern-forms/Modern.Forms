using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItem : ILayoutable
    {
        private const int IMAGE_SIZE = 16;

        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public object Tag { get; set; }
        public TreeView Parent { get; internal set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin => new Padding (0);

        public Size GetPreferredSize (Size proposedSize)
        {
            var font_size = LogicalToDeviceUnits (Theme.FontSize);
            var padding = LogicalToDeviceUnits (16);

            return new Size (0, font_size + padding);
        }

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        internal void OnPaint (SKCanvas canvas)
        {
            var background_color = Selected ? Theme.RibbonItemHighlightColor : Theme.LightNeutralGray;

            canvas.FillRectangle (Bounds, background_color);

            var text_left = Bounds.Left + LogicalToDeviceUnits (7);

            if (Image != null) {
                var image_area = new Rectangle (Bounds.Left, Bounds.Top, Bounds.Height, Bounds.Height);
                var image_bounds = DrawingExtensions.CenterSquare (image_area, LogicalToDeviceUnits (IMAGE_SIZE));

                canvas.DrawBitmap (Image, image_bounds);

                text_left += image_bounds.Width + LogicalToDeviceUnits (7);
            }

            var text_bounds = new Rectangle (text_left, Bounds.Top, Bounds.Width - text_left, Bounds.Height);
            canvas.DrawText (Text.Trim (), Theme.UIFont, LogicalToDeviceUnits (Theme.FontSize), text_bounds, Theme.DarkTextColor, ContentAlignment.MiddleLeft);
        }

        private int LogicalToDeviceUnits (int value) => Parent?.LogicalToDeviceUnits (value) ?? value;
    }
}
