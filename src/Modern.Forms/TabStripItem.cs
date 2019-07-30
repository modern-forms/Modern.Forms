using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class TabStripItem : ILayoutable
    {
        public string Text { get; set; } = string.Empty;
        public bool Selected { get; set; }
        public bool Hovered { get; set; }
        public object? Tag { get; set; }
        public TabStrip? Parent { get; internal set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin => new Padding (0);
        public Padding Padding { get; set; } = new Padding (14, 0, 14, 0);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public virtual void OnPaint (SKCanvas canvas)
        {
            var background_color = Selected ? Theme.NeutralGray : Hovered ? Theme.RibbonTabHighlightColor : Theme.RibbonColor;
            canvas.FillRectangle (Bounds, background_color);

            var font_color = Selected ? Theme.RibbonColor : Theme.LightTextColor;
            var font_size = Parent?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;

            canvas.DrawText (Text, Theme.UIFont, font_size, Bounds, font_color, ContentAlignment.MiddleCenter);
        }

        public Size GetPreferredSize (Size proposedSize)
        {
            var padding = Parent?.LogicalToDeviceUnits (Padding.Horizontal) ?? Padding.Horizontal;
            var font_size = Parent?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;
            var text_size = (int)Math.Round (TextMeasurer.MeasureText (Text, Theme.UIFont, font_size));

            return new Size (text_size + padding, Bounds.Height);
        }
    }
}
