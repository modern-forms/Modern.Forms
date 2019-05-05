using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public static class TextMeasurer
    {
        public static SKSize MeasureText (string text, ControlStyle style)
        {
            SKRect bounds = SKRect.Empty;

            using (var paint = new SKPaint { Typeface = style.GetFont (), TextSize = style.GetFontSize () }) {
                paint.MeasureText (text, ref bounds);
                return bounds.Size;
            }
        }

        public static float MeasureText (string text, SKTypeface font, int fontSize)
        {
            using (var paint = new SKPaint { Typeface = font, TextSize = fontSize })
                return paint.MeasureText (text);
        }
    }
}
