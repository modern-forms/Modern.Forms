using System;
using System.Drawing;
using SkiaSharp;
using Topten.RichTextKit;

namespace Modern.Forms
{
    public static class SkiaTextExtensions
    {
        private static TextPaintOptions CreateOptions () => new TextPaintOptions { IsAntialias = true, LcdRenderText = true };

        public static void DrawTextLine (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.CurrentStyle.GetForegroundColor (), alignment, maxLines: 1);

        public static void DrawText (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment, int selectionStart = -1, int selectionEnd = -1, SKColor? selectionColor = null, int? maxLines = null)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.Enabled ? control.CurrentStyle.GetForegroundColor () : Theme.DisabledTextColor, alignment, selectionStart, selectionEnd, selectionColor, maxLines);

        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontSize, Rectangle bounds, SKColor color, ContentAlignment alignment, int selectionStart = -1, int selectionEnd = -1, SKColor? selectionColor = null, int? maxLines = null)
        {
            var tb = TextMeasurer.CreateTextBlock (text, font, fontSize, bounds.Size, TextMeasurer.GetTextAlign (alignment), color, maxLines);
            var location = bounds.Location;
            var vertical = TextMeasurer.GetVerticalAlign (alignment);

            if (vertical == SKTextAlign.Right)
                location.Y = bounds.Bottom - (int)tb.MeasuredHeight;
            else if (vertical == SKTextAlign.Center)
                location.Y += (bounds.Height - (int)tb.MeasuredHeight) / 2;

            var options = CreateOptions ();

            if (selectionStart >= 0 && selectionEnd >= 0 && selectionStart != selectionEnd) {
                options.SelectionStart = selectionStart;
                options.SelectionEnd = selectionEnd;
                options.SelectionColor = selectionColor ?? SKColors.Blue;
            }

            canvas.Save ();
            canvas.Clip (bounds);

            tb.Paint (canvas, new SKPoint (location.X, location.Y), options);

            canvas.Restore ();
        }

        public static void DrawTextBlock (this SKCanvas canvas, TextBlock block, Point location, TextSelection selection)
        {
            var options = CreateOptions ();

            if (!selection.IsEmpty ()) {
                options.SelectionStart = selection.Start;
                options.SelectionEnd = selection.End;
                options.SelectionColor = selection.Color;
            }

            block.Paint (canvas, new SKPoint (location.X, location.Y), options);
        }
    }
}
