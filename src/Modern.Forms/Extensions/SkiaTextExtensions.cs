using System;
using System.Drawing;
using SkiaSharp;
using Topten.RichTextKit;

namespace Modern.Forms
{
    public static class SkiaTextExtensions
    {
        public static void DrawText (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.CurrentStyle.GetForegroundColor (), alignment);

        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontSize, Rectangle bounds, SKColor color, ContentAlignment alignment)
        {
            var tb = TextMeasurer.CreateTextBlock (text, font, fontSize, bounds.Size, TextMeasurer.GetTextAlign (alignment), color);
            var location = bounds.Location;
            var vertical = TextMeasurer.GetVerticalAlign (alignment);

            if (vertical == SKTextAlign.Right)
                location.Y = bounds.Bottom - (int)tb.MeasuredHeight;
            else if (vertical == SKTextAlign.Center)
                location.Y += (bounds.Height - (int)tb.MeasuredHeight) / 2;

            var options = new TextPaintOptions { IsAntialias = true, LcdRenderText = true };

            canvas.Save ();
            canvas.Clip (bounds);

            tb.Paint (canvas, new SKPoint (location.X, location.Y), options);

            canvas.Restore ();
        }
    }
}
