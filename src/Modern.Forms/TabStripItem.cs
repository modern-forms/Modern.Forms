using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class TabStripItem : ILayoutable
    {
        public string Text { get; set; }
        public bool Selected { get; set; }
        public bool Hovered { get; set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin => new Padding (0);
        public Padding Padding { get; set; } = new Padding (14, 0, 14, 0);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public virtual void OnPaint (SKCanvas canvas)
        {
            var background_color = Selected ? ModernTheme.NeutralGray : Hovered ? ModernTheme.RibbonTabHighlightColor : ModernTheme.RibbonColor;
            canvas.FillRectangle (Bounds, background_color);

            var font_color = Selected ? ModernTheme.RibbonColor : ModernTheme.LightTextColor;
            canvas.DrawCenteredText (Text, ModernTheme.UIFont, 14, Bounds, font_color);
        }

        public Size GetPreferredSize (Size proposedSize)
            => new Size ((int)Math.Round (TextMeasurer.MeasureText (Text, ModernTheme.UIFont, 14) + Padding.Horizontal, 0), 28);
    }
}
