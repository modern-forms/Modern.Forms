using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Designer;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Modern.Forms.Design
{
    static class DrawingExtensions
    {
        public static void DrawResizeAdornment (this SKCanvas canvas, Rectangle rectangle, bool resizeOnly = false)
        {
            DrawSelectionAdornment (canvas, rectangle);

            foreach (var adornment in GetResizeAdornments (rectangle, resizeOnly))
                adornment.Draw (canvas);
        }

        public static void DrawGrabHandleGlyphs (PaintEventArgs pe, Rectangle rectangle, bool resizeOnly = false)
        {
            DrawSelectionAdornment (pe.Canvas, rectangle);

            foreach (var adornment in GetGrabHandleGlyphs (rectangle, resizeOnly))
                adornment.Paint (pe);
        }

        public static void DrawSelectionAdornment (this SKCanvas canvas, Rectangle rectangle)
        {
            canvas.DrawFocusRectangle (rectangle, -3);
            //using var paint = new SKPaint { Color = new SKColor (105, 105, 105), IsStroke = true, StrokeWidth = 1 };
            //canvas.DrawRect (rectangle, paint);
        }

        public static void DrawResizeHandle (this SKCanvas canvas, Point point)
        {
            canvas.FillRectangle (point.X - 2, point.Y - 2, 5, 5, SKColors.White);

            canvas.DrawLine (point.X - 2, point.Y - 3, point.X + 3, point.Y - 3, SKColors.Black, 1);
            canvas.DrawLine (point.X - 2, point.Y + 3, point.X + 3, point.Y + 3, SKColors.Black, 1);

            canvas.DrawLine (point.X - 3, point.Y - 2, point.X - 3, point.Y + 3, SKColors.Black, 1);
            canvas.DrawLine (point.X + 3, point.Y - 2, point.X + 3, point.Y + 3, SKColors.Black, 1);
        }

        public static ResizeAdornment? GetResizeAdornmentAtPoint (Rectangle rectangle, Point point, bool resizeOnly = false)
        {
            foreach (var adornment in GetResizeAdornments (rectangle, resizeOnly))
                if (adornment.Bounds.Contains (point))
                    return adornment;

            return null;
        }

        private static IEnumerable<ResizeAdornment> GetResizeAdornments (Rectangle rectangle, bool resizeOnly)
        {
            yield return new ResizeAdornment (SelectionDirection.BottomRight, new Point (rectangle.Right + 2, rectangle.Bottom + 2));

            if (!resizeOnly) {
                yield return new ResizeAdornment (SelectionDirection.TopLeft, new Point (rectangle.X - 3, rectangle.Y - 3));
                yield return new ResizeAdornment (SelectionDirection.BottomLeft, new Point (rectangle.X - 3, rectangle.Bottom + 2));
                yield return new ResizeAdornment (SelectionDirection.TopRight, new Point (rectangle.Right + 2, rectangle.Y - 3));
            }

            if (rectangle.Height > 15) {
                yield return new ResizeAdornment (SelectionDirection.CenterRight, new Point (rectangle.Right + 2, rectangle.Y + (rectangle.Height / 2)));
                
                if (!resizeOnly)
                    yield return new ResizeAdornment (SelectionDirection.CenterLeft, new Point (rectangle.X - 3, rectangle.Y + (rectangle.Height / 2)));
            }

            if (rectangle.Width > 15) {
                yield return new ResizeAdornment (SelectionDirection.BottomMiddle, new Point (rectangle.X + (rectangle.Width / 2), rectangle.Bottom + 2));
                
                if (!resizeOnly)
                    yield return new ResizeAdornment (SelectionDirection.TopMiddle, new Point (rectangle.X + (rectangle.Width / 2), rectangle.Y - 3));
            }
        }

        public static IEnumerable<GrabHandleGlyph> GetGrabHandleGlyphs (Rectangle rectangle, bool resizeOnly)
        {
            yield return new GrabHandleGlyph (rectangle, SelectionDirection.BottomRight);

            if (!resizeOnly) {
                yield return new GrabHandleGlyph (rectangle, SelectionDirection.TopLeft);
                yield return new GrabHandleGlyph (rectangle, SelectionDirection.BottomLeft);
                yield return new GrabHandleGlyph (rectangle, SelectionDirection.TopRight);
            }

            if (rectangle.Height > 15) {
                yield return new GrabHandleGlyph (rectangle, SelectionDirection.CenterRight);

                if (!resizeOnly)
                    yield return new GrabHandleGlyph (rectangle, SelectionDirection.CenterLeft);
            }

            if (rectangle.Width > 15) {
                yield return new GrabHandleGlyph (rectangle, SelectionDirection.BottomMiddle);

                if (!resizeOnly)
                    yield return new GrabHandleGlyph (rectangle, SelectionDirection.TopMiddle);
            }
        }
    }
}
