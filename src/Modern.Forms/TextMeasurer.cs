using System;
using System.Drawing;
using SkiaSharp;
using Topten.RichTextKit;

namespace Modern.Forms
{
    public static class TextMeasurer
    {
        public static SKSize MeasureText (string text, Control control)
            => MeasureText (text, control, new Size (1000, 1000));

        public static SKSize MeasureText (string text, Control control, Size maxSize)
            => MeasureText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), maxSize);

        public static SKSize MeasureText (string text, SKTypeface font, int fontSize)
            => MeasureText (text, font, fontSize, new Size (1000, 1000));

        public static SKSize MeasureText (string text, SKTypeface font, int fontSize, Size maxSize)
        {
            var tb = CreateTextBlock (text, font, fontSize, maxSize);

            return new SKSize (tb.MeasuredWidth, tb.MeasuredHeight);
        }

        public static Rectangle GetCursorLocation (string text, Rectangle bounds, SKTypeface font, int fontSize, Size maxSize, ContentAlignment alignment, int codePoint)
        {
            // If there isn't any text the cursor height will be 0, so put a dummy character here
            if (string.IsNullOrWhiteSpace (text))
                text = "l";

            var tb = CreateTextBlock (text, font, fontSize, maxSize, GetTextAlign (alignment));
            var caret_rect = tb.GetCaretInfo (codePoint).CaretRectangle;

            // We need to offset the caret to client bounds
            var vertical = GetVerticalAlign (alignment);

            if (vertical == SKTextAlign.Left)
                return new Rectangle (bounds.X + (int)caret_rect.Left, bounds.Y, (int)caret_rect.Width, (int)caret_rect.Height);
            else if (vertical == SKTextAlign.Right)
                return new Rectangle (bounds.X + (int)caret_rect.Left, bounds.Bottom - (int)caret_rect.Width, (int)caret_rect.Width, (int)caret_rect.Height);

            // Centered
            return new Rectangle (bounds.X + (int)caret_rect.Left, bounds.Y + ((bounds.Height - (int)caret_rect.Height) / 2), (int)caret_rect.Width, (int)caret_rect.Height);
        }

        public static int GetMaxCaretIndex (string text)
        {
            var tb = new TextBlock ();

            tb.AddText (text, new Style ());

            return tb.CaretIndicies[tb.CaretIndicies.Count - 1];
        }

        public static HitTestResult HitTest (string text, Rectangle bounds, SKTypeface font, int fontSize, Size maxSize, ContentAlignment alignment, Point location)
        {
            var tb = CreateTextBlock (text, font, fontSize, maxSize, GetTextAlign (alignment));

            // We need to offset the requested location to fit the desired bounds
            var x = location.X + bounds.X;
            var y = location.Y + bounds.Y;

            return tb.HitTest (x, y);
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

        internal static TextBlock CreateTextBlock (string text, SKTypeface font, int fontSize, Size maxSize, TextAlignment alignment = TextAlignment.Auto, SKColor color = new SKColor ())
        {
            var tb = new TextBlock {
                MaxWidth = maxSize.Width,
                MaxHeight = maxSize.Height,
                Alignment = alignment
            };

            var styleNormal = new Style {
                FontFamily = font.FamilyName,
                FontSize = fontSize,
                TextColor = color
            };

            tb.AddText (text, styleNormal);
            
            return tb;
        }

        internal static TextAlignment GetTextAlign (ContentAlignment alignment)
        {
            switch (alignment) {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return TextAlignment.Left;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return TextAlignment.Center;
                default:
                    return TextAlignment.Right;
            }
        }

        internal static SKTextAlign GetVerticalAlign (ContentAlignment alignment)
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
