using System.Drawing;
using SkiaSharp;
using Topten.RichTextKit;

namespace Modern.Forms
{
    /// <summary>
    /// A collection of extension methods to facilitate text drawing operations.
    /// </summary>
    public static class SkiaTextExtensions
    {
        private static TextPaintOptions CreateOptions () => new TextPaintOptions { IsAntialias = true, LcdRenderText = true };

        /// <summary>
        /// Draws a string of text.
        /// </summary>
        public static void DrawText (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment, int selectionStart = -1, int selectionEnd = -1, SKColor? selectionColor = null, int? maxLines = null, bool ellipsis = false)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.Enabled ? control.CurrentStyle.GetForegroundColor () : Theme.DisabledTextColor, alignment, selectionStart, selectionEnd, selectionColor, maxLines, ellipsis);

        /// <summary>
        /// Draws a string of text.
        /// </summary>
        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontSize, Rectangle bounds, SKColor color, ContentAlignment alignment, int selectionStart = -1, int selectionEnd = -1, SKColor? selectionColor = null, int? maxLines = null, bool ellipsis = false)
        {
            if (string.IsNullOrWhiteSpace (text))
                return;

            var tb = TextMeasurer.CreateTextBlock (text, font, fontSize, bounds.Size, TextMeasurer.GetTextAlign (alignment), color, maxLines, ellipsis);
            var location = bounds.Location;
            var vertical = TextMeasurer.GetVerticalAlign (alignment);

            if (vertical == SKTextAlign.Right)
                location.Y = bounds.Bottom - (int)tb.MeasuredHeight;
            else if (vertical == SKTextAlign.Center)
                location.Y += (bounds.Height - (int)tb.MeasuredHeight) / 2;

            var options = CreateOptions ();

            if (selectionStart >= 0 && selectionEnd >= 0 && selectionStart != selectionEnd) {
                options.Selection = new TextRange (selectionStart, selectionEnd);
                options.SelectionColor = selectionColor ?? SKColors.Blue;
            }

            canvas.Save ();
            canvas.Clip (bounds);

            tb.Paint (canvas, new SKPoint (location.X, location.Y), options);

            canvas.Restore ();
        }

        /// <summary>
        /// Draws a block of text.
        /// </summary>
        public static void DrawTextBlock (this SKCanvas canvas, TextBlock block, Point location, TextSelection selection)
        {
            var options = CreateOptions ();

            if (!selection.IsEmpty ()) {
                options.Selection = new TextRange (selection.Start, selection.End);
                options.SelectionColor = selection.Color;
            }

            block.Paint (canvas, new SKPoint (location.X, location.Y), options);
        }

        /// <summary>
        /// Draws a single line of text.
        /// </summary>
        public static void DrawTextLine (this SKCanvas canvas, string text, Rectangle bounds, Control control, ContentAlignment alignment, bool ellipsis = false)
            => canvas.DrawText (text, control.CurrentStyle.GetFont (), control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()), bounds, control.Enabled ? control.CurrentStyle.GetForegroundColor () : Theme.DisabledTextColor, alignment, maxLines: 1, ellipsis: ellipsis);
    }
}
