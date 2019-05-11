using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Modern.Forms
{
    public static class TextMeasurer
    {
        public static SKSize MeasureText (string text, ControlStyle style)
        {
            var bounds = SKRect.Empty;

            using (var paint = new SKPaint { Typeface = style.GetFont (), TextSize = style.GetFontSize () }) {
                paint.MeasureText (text, ref bounds);
                return bounds.Size;
            }
        }

        public static float MeasureText (string text, SKTypeface font, int fontSize)
        {
            using (var paint = SkiaExtensions.CreateTextPaint (font, fontSize, SKColors.Black))
                return paint.MeasureText (text);
        }

        public static SKPoint[] MeasureCharacters (string text, SKTypeface font, int fontSize, float xOffset = 0, float yOffset = 0)
        {
            using (var paint = SkiaExtensions.CreateTextPaint (font, fontSize, SKColors.Black))
            using (var shaper = new SKShaper (font)) {
                var result = shaper.Shape (text, xOffset, yOffset, paint);
                return result.Points;
            }
        }

        public static bool IsWordSeparator (char c)
        {
            switch (c) {
                case ' ':
                case '\t':
                case '(':
                case ')':
                case '\r':
                case '\n':
                    return true;
            }

            return false;
        }

        public static int FindNextSeparator (string text, int start, bool forward)
        {
            var len = text.Length;

            if (forward) {
                for (var i = start + 1; i < len; i++) {
                    if (IsWordSeparator (text[i]))
                        return i + 1;
                }

                return len;
            } else {
                for (var i = start - 1; i > 0; i--) {
                    if (IsWordSeparator (text[i - 1]))
                        return i;
                }

                return 0;
            }
        }
    }
}
