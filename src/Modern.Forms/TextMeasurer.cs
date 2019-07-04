using System;
using System.Collections.Generic;
using System.Linq;
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

            using var paint = new SKPaint { Typeface = style.GetFont (), TextSize = style.GetFontSize () };

            paint.MeasureText (text, ref bounds);
            return bounds.Size;
        }

        public static float MeasureText (string text, SKTypeface font, int fontSize)
        {
            using var paint = SkiaTextExtensions.CreateTextPaint (font, fontSize, SKColors.Black);

            return paint.MeasureText (text);
        }

        public static SKSize MeasureText (string text, SKTypeface font, int fontSize, SKSize proposedSize)
        {
            var text_bounds = SKRect.Empty;

            using var paint = SkiaTextExtensions.CreateTextPaint (font, fontSize, SKColors.Black);

            paint.MeasureText (text, ref text_bounds);

            // If we fit in the proposed size then just use that
            if (text_bounds.Width <= proposedSize.Width && text_bounds.Height <= proposedSize.Height)
                return new SKSize (text_bounds.Width, proposedSize.Height);

            // Figure out how many lines we have room for
            var line_count = proposedSize.Height / text_bounds.Height;

            // If we only have room for one line there's not a lot we can do
            if (line_count <= 1)
                return new SKSize (text_bounds.Width, proposedSize.Height);

            var words = BreakDownIntoWords (text);
            var max_word_width = Math.Max (words.Select (w => MeasureText (w, font, fontSize)).Max (), proposedSize.Width);

            return new SKSize (max_word_width, proposedSize.Height);
        }

        public static SKPoint[] MeasureCharacters (string text, SKTypeface font, int fontSize, float xOffset = 0, float yOffset = 0)
        {
            using var paint = SkiaTextExtensions.CreateTextPaint (font, fontSize, SKColors.Black);
            using var shaper = new SKShaper (font);

            var result = shaper.Shape (text, xOffset, yOffset, paint);
            return result.Points;
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

        public static string[] BreakDownIntoWords (string text)
        {
            var words = text.Split (new[] { ' ', '\t', '(', ')', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            words = words.Select (w => w.Trim ()).Where (w => !string.IsNullOrWhiteSpace (w)).ToArray ();

            return words;
        }
    }
}
