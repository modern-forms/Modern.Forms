using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public static class SkiaTextExtensions
    {

        //public static void DrawCharacters (this SKCanvas canvas, string text, SKTypeface font, int fontsize, int x, int y, SKColor color)
        //{
        //    var emoji = StringUtilities.GetUnicodeCharacterCode ("🚀", SKTextEncoding.Utf32);
        //    font = SKFontManager.Default.MatchCharacter ('c');
        //    font.
        //    using (var paint = CreateTextPaint (font, fontsize, color)) {
        //        var ranges = TextMeasurer.MeasureCharacters (text, font, fontsize, x, y);
        //        var x_float = (float)x;
        //       // if (SKTypeface.Default.)
        //            canvas.DrawText (text, x_float, y, paint);
        //        //for (var i = 0; i < text.Length - 1; i++) {
        //        //    x_float = ranges[i].X;
        //        //}
        //    }
        //}

        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, Rectangle bounds, SKColor color, ContentAlignment alignment)
        {
            if (string.IsNullOrWhiteSpace (text))
                return;

            using var paint = CreateTextPaint (font, fontsize, color);

            var font_height = new SKRect ();
            paint.MeasureText ("O", ref font_height);

            var vertical_align = GetVerticalAlign (alignment);
            var y = bounds.Top + (int)font_height.Height;

            if (vertical_align == SKTextAlign.Center) {
                var center_bounds = bounds.Top + (bounds.Height / 2);
                var text_center = (font_height.Top + font_height.Bottom) / 2;

                y = (int)(center_bounds - text_center);
            } else if (vertical_align == SKTextAlign.Right)
                y = bounds.Bottom - (int)font_height.Bottom - 1;

            canvas.Save ();
            canvas.Clip (bounds);

            bounds.Y = y;
            canvas.DrawTextLine (text, bounds, paint, alignment);

            canvas.Restore ();
        }

        private static void DrawTextLine (this SKCanvas canvas, string text, Rectangle bounds, SKPaint paint, ContentAlignment alignment)
        {
            // bounds.Top is the y coordinate to draw at, not actually the bounds
            var text_align = GetTextAlign (alignment);
            var x = bounds.Left + 1;

            if (text_align == SKTextAlign.Center)
                x += bounds.Width / 2;
            else if (text_align == SKTextAlign.Right)
                x = bounds.Right - 1;

            paint.TextAlign = text_align;

            canvas.DrawText (text, x, bounds.Top, paint);
        }

        public static void DrawText (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.CurrentStyle.GetForegroundColor (), alignment);

        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, Rectangle bounds, SKColor color)
        {
            if (string.IsNullOrWhiteSpace (text))
                return;

            using var paint = CreateTextPaint (font, fontsize, color, SKTextAlign.Center);

            SkiaTextBox.Draw (text, bounds.X, bounds.Y, bounds.Width, bounds.Height, canvas, paint, true);
        }

        public static SKPaint CreateTextPaint (SKTypeface font, int fontsize, SKColor color, SKTextAlign align = SKTextAlign.Left)
        {
            return new SKPaint {
                Color = color,
                Typeface = font,
                IsAntialias = true,
                LcdRenderText = true,
                TextSize = fontsize,
                SubpixelText = true,
                DeviceKerningEnabled = true,
                FilterQuality = SKFilterQuality.High,
                HintingLevel = SKPaintHinting.Full,
                IsAutohinted = true,
                TextAlign = align,
                TextEncoding = SKTextEncoding.Utf32,
                IsLinearText = true
            };
        }

        private static SKTextAlign GetTextAlign (ContentAlignment alignment)
        {
            switch (alignment) {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return SKTextAlign.Left;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return SKTextAlign.Center;
                default:
                    return SKTextAlign.Right;
            }
        }

        private static SKTextAlign GetVerticalAlign (ContentAlignment alignment)
        {
            // We are reusing Left, Center, Right as Top, Center, Bottom
            switch (alignment) {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    return SKTextAlign.Left;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    return SKTextAlign.Center;
                default:
                    return SKTextAlign.Right;
            }
        }
    }
}
