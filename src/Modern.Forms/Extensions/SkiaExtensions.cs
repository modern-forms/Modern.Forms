using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// A collection of extension methods to facilitate working with Skia.
    /// </summary>
    public static class SkiaExtensions
    {
        private static readonly SKColorFilter disabled_matrix = SKColorFilter.CreateColorMatrix (new float[]
                {
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0,     0,     0,     1, 0
                });

        /// <summary>
        /// Clips a canvas to the specified rectangle.
        /// </summary>
        public static void Clip (this SKCanvas canvas, Rectangle rectangle) => canvas.ClipRect (rectangle.ToSKRect ());

        /// <summary>
        /// Draws a control's background.
        /// </summary>
        public static void DrawBackground (this SKCanvas canvas, ControlStyle style) =>
            canvas.Clear (style.GetBackgroundColor ());

        /// <summary>
        /// Draws a bitmap.
        /// </summary>
        public static void DrawBitmap (this SKCanvas canvas, SKBitmap bitmap, Rectangle rect, bool disabled = false)
        {
            using var paint = new SKPaint { FilterQuality = SKFilterQuality.High };

            if (disabled)
                paint.ColorFilter = disabled_matrix;

            canvas.DrawBitmap (bitmap, rect.ToSKRect (), paint);
        }

        /// <summary>
        /// Draws a bitmap.
        /// </summary>
        public static void DrawBitmap (this SKCanvas canvas, SKBitmap bitmap, float x, float y, bool disabled = false)
        {
            using var paint = new SKPaint { FilterQuality = SKFilterQuality.High };

            if (disabled)
                paint.ColorFilter = disabled_matrix;

            canvas.DrawBitmap (bitmap, x, y, paint);
        }

        /// <summary>
        /// Draws a control's border.
        /// </summary>
        public static void DrawBorder (this SKCanvas canvas, Rectangle bounds, ControlStyle style)
        {
            // Left Border
            if (style.Border.Left.GetWidth () > 0) {
                var left_offset = style.Border.Left.GetWidth () / 2f;
                canvas.DrawLine (left_offset, 0, left_offset, bounds.Height, style.Border.Left.GetColor (), style.Border.Left.GetWidth ());
            }

            // Right Border
            if (style.Border.Right.GetWidth () > 0) {
                var right_offset = style.Border.Right.GetWidth () / 2f;
                canvas.DrawLine (bounds.Width - right_offset, 0, bounds.Width - right_offset, bounds.Height, style.Border.Right.GetColor (), style.Border.Right.GetWidth ());
            }

            // Top Border
            if (style.Border.Top.GetWidth () > 0) {
                var top_offset = style.Border.Top.GetWidth () / 2f;
                canvas.DrawLine (0, top_offset, bounds.Width, top_offset, style.Border.Top.GetColor (), style.Border.Top.GetWidth ());
            }

            // Bottom Border
            if (style.Border.Bottom.GetWidth () > 0) {
                var bottom_offset = style.Border.Bottom.GetWidth () / 2f;
                canvas.DrawLine (0, bounds.Height - bottom_offset, bounds.Width, bounds.Height - bottom_offset, style.Border.Bottom.GetColor (), style.Border.Bottom.GetWidth ());
            }

            //if (CurrentStyle.BorderRadius > 0) { }
            ////canvas.DrawRoundedRectangle (1, 1, Width - (CurrentStyle.BorderWidth * 2), Height - (CurrentStyle.BorderWidth * 2), CurrentStyle.BorderColor, CurrentStyle.BorderRadius, CurrentStyle.BorderRadius, .5f);
            //else {
            //    //canvas.DrawRectangle (0, 0, Width - CurrentStyle.BorderWidth, Height - CurrentStyle.BorderWidth, CurrentStyle.BorderColor, CurrentStyle.BorderWidth);
            //    canvas.DrawBorder (Bounds, CurrentStyle);
            //}
        }

        /// <summary>
        /// Draws an unfilled circle.
        /// </summary>
        public static void DrawCircle (this SKCanvas canvas, int x, int y, int radius, SKColor color, int strokeWidth = 1)
        {
            using var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth, IsAntialias = true };

            canvas.DrawCircle (x, y, radius, paint);
        }

        /// <summary>
        /// Draws a focus rectangle.
        /// </summary>
        public static void DrawFocusRectangle (this SKCanvas canvas, int x, int y, int width, int height, int inset = 0)
        {
            var effect = SKPathEffect.CreateDash (new[] { 1f, 1f }, 0);
            using var paint = new SKPaint { Color = SKColors.DarkGray, IsStroke = true, StrokeWidth = 1, PathEffect = effect };

            canvas.DrawRect (x + inset, y + inset, width - (2 * inset) - 1, height - (2 * inset) - 1, paint);
        }

        /// <summary>
        /// Draws an focus rectangle.
        /// </summary>
        public static void DrawFocusRectangle (this SKCanvas canvas, Rectangle rectangle, int inset = 0)
            => DrawFocusRectangle (canvas, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, inset);

        /// <summary>
        /// Draws a line.
        /// </summary>
        public static void DrawLine (this SKCanvas canvas, float x1, float y1, float x2, float y2, SKColor color, int thickness = 1)
        {
            using var paint = new SKPaint { Color = color, StrokeWidth = thickness };

            canvas.DrawLine (x1, y1, x2, y2, paint);
        }

        /// <summary>
        /// Draws an unfilled rectangle.
        /// </summary>
        public static void DrawRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color, int strokeWidth = 1)
        {
            using var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth };

            canvas.DrawRect (x, y, width, height, paint);
        }

        /// <summary>
        /// Draws an unfilled rectangle.
        /// </summary>
        public static void DrawRectangle (this SKCanvas canvas, Rectangle rectangle, SKColor color, int strokeWidth = 1)
            => DrawRectangle (canvas, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color, strokeWidth);

        //public static void DrawRoundedRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color, int rx = 3, int ry = 3, float strokeWidth = 1)
        //{
        //    using var paint = new SKPaint {
        //        Color = color,
        //        IsStroke = true,
        //        IsAntialias = true,
        //        LcdRenderText = true,
        //        StrokeWidth = 1f,
        //        SubpixelText = true,
        //        DeviceKerningEnabled = true,
        //        FilterQuality = SKFilterQuality.High,
        //        HintingLevel = SKPaintHinting.Full,
        //        IsAutohinted = true,
        //        TextAlign = SKTextAlign.Center
        //    };
        //    canvas.DrawRoundRect (x + .5f, y + .5f, width, height, rx, ry, paint);
        //}

        /// <summary>
        /// Draws a filled circle.
        /// </summary>
        public static void FillCircle (this SKCanvas canvas, int x, int y, int radius, SKColor color)
        {
            using var paint = new SKPaint { Color = color, IsAntialias = true };

            canvas.DrawCircle (x, y, radius, paint);
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// </summary>
        public static void FillRectangle (this SKCanvas canvas, Rectangle rectangle, SKColor color)
            => FillRectangle (canvas, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);

        /// <summary>
        /// Draws a filled rectangle.
        /// </summary>
        public static void FillRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color)
        {
            using var paint = new SKPaint { Color = color };

            canvas.DrawRect (x, y, width, height, paint);
        }

        /// <summary>
        /// Convers an SKImage to a Bitmap.
        /// </summary>
        public static Bitmap ToBitmap (this SKImage skiaImage)
        {
            // TODO: maybe keep the same color types where we can, instead of just going to the platform default
            var bitmap = new Bitmap (skiaImage.Width, skiaImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            var data = bitmap.LockBits (new Rectangle (0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // copy
            using (var pixmap = new SKPixmap (new SKImageInfo (data.Width, data.Height), data.Scan0, data.Stride))
                skiaImage.ReadPixels (pixmap, 0, 0);

            bitmap.UnlockBits (data);
            return bitmap;
        }

        /// <summary>
        /// Convers an SKBitmap to a Bitmap.
        /// </summary>
        public static Bitmap ToBitmap (this SKBitmap skiaBitmap)
        {
            using (var image = SKImage.FromPixels (skiaBitmap.PeekPixels ()))
                return ToBitmap (image);
        }

        /// <summary>
        /// Converts an SKRect to a Rectangle.
        /// </summary>
        public static Rectangle ToRectangle (this SKRect rect) => new Rectangle ((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);

        /// <summary>
        /// Converts a Rectangle to an SKRect.
        /// </summary>
        public static SKRect ToSKRect (this Rectangle rect) => new SKRect (rect.X, rect.Y, rect.Right, rect.Bottom);

        /// <summary>
        /// Converts a Size to an SKSize.
        /// </summary>
        public static SKSize ToSKSize (this Size size) => new SKSize (size.Width, size.Height);
    }
}
