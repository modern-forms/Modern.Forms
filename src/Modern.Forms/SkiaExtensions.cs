using System;
using System.Drawing;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Modern.Forms
{
    public static class SkiaExtensions
    {
        private static SKColorFilter disabled_matrix = SKColorFilter.CreateColorMatrix (new float[]
                {
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0,     0,     0,     1, 0
                });

        public static void DrawText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, int x, int y, SKColor color)
        {
            using (var paint = new SKPaint {
                Color = color,
                Typeface = font,
                IsAntialias = true,
                LcdRenderText = true,
                TextSize = fontsize,
                SubpixelText = true,
                DeviceKerningEnabled = true,
                FilterQuality = SKFilterQuality.High,
                HintingLevel = SKPaintHinting.Full,
                IsAutohinted = true
            })
                canvas.DrawText (text, x, y, paint);
        }

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

            using (var paint = CreateTextPaint (font, fontsize, color)) {
                var font_height = new SKRect ();
                paint.MeasureText ("Bg", ref font_height);

                var x = bounds.Left + 1;

                switch (alignment) {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        x += bounds.Width / 2;
                        paint.TextAlign = SKTextAlign.Center;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        x = bounds.Right - 1;
                        paint.TextAlign = SKTextAlign.Right;
                        break;
                }

                var y = bounds.Top + (int)font_height.Height;

                switch (alignment) {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        y = bounds.Bottom - (int)font_height.Bottom - 1;
                        break;
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        y = (int)(bounds.Top + ((bounds.Height - (font_height.Height)) / 2) + font_height.Height) - 1;
                        break;
                }

                canvas.Save ();
                canvas.ClipRect (bounds.ToSKRect ());
                canvas.DrawText (text, x, y, paint);
                canvas.Restore ();
            }
        }

        public static void DrawText (this SKCanvas canvas, string text, Rectangle bounds, ControlStyle style, ContentAlignment alignment)
            => canvas.DrawText (text, style.GetFont (), style.GetFontSize (), bounds, style.GetForegroundColor (), alignment);

        public static void DrawText (this SKCanvas canvas, string text, int x, int y, ControlStyle style)
            => canvas.DrawText (text, style.GetFont (), style.GetFontSize (), x, y, style.GetForegroundColor ());

        public static void DrawCenteredText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, int x, int y, SKColor color)
        {
            if (string.IsNullOrWhiteSpace (text))
                return;

            using (var paint = CreateTextPaint (font, fontsize, color, SKTextAlign.Center))
                canvas.DrawText (text, x, y, paint);
        }

        public static void DrawCenteredText (this SKCanvas canvas, string text, int x, int y, ControlStyle style)
            => canvas.DrawCenteredText (text, style.GetFont (), style.GetFontSize (), x, y, style.GetForegroundColor ());

        public static void DrawCenteredText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, Rectangle bounds, SKColor color)
        {
            using (var paint = CreateTextPaint (font, fontsize, color, SKTextAlign.Center)) {
                var b = new SKRect ();
                paint.MeasureText (text, ref b);

                var y = (int)(bounds.Top + ((bounds.Height - (b.Height)) / 2) + b.Height) - 1;
                DrawCenteredText (canvas, text, font, fontsize, bounds.Left + (bounds.Width / 2), y, color);
            }
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

        public static SKPaint CreateTextPaint (ControlStyle style, SKTextAlign align = SKTextAlign.Left)
            => CreateTextPaint (style.GetFont (), style.GetFontSize (), style.GetForegroundColor (), align);

        public static void DrawLine (this SKCanvas canvas, float x1, float y1, float x2, float y2, SKColor color, int thickness = 1)
        {
            using (var paint = new SKPaint { Color = color, StrokeWidth = thickness })
                canvas.DrawLine (x1, y1, x2, y2, paint);
        }

        public static void FillRectangle (this SKCanvas canvas, System.Drawing.Rectangle rectangle, SKColor color)
        {
            using (var paint = new SKPaint { Color = color })
                canvas.DrawRect (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, paint);
        }

        public static void FillRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color)
        {
            using (var paint = new SKPaint { Color = color })
                canvas.DrawRect (x, y, width, height, paint);
        }

        public static void DrawRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color, int strokeWidth = 1)
        {
            using (var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth })
                canvas.DrawRect (x, y, width, height, paint);
        }

        public static void DrawRectangle (this SKCanvas canvas, System.Drawing.Rectangle rectangle, SKColor color, int strokeWidth = 1)
        {
            using (var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth })
                canvas.DrawRect (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, paint);
        }

        public static void DrawCircle (this SKCanvas canvas, int x, int y, int radius, SKColor color, int strokeWidth = 1)
        {
            using (var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth, IsAntialias = true })
                canvas.DrawCircle (x, y, radius, paint);
        }

        public static void FillCircle (this SKCanvas canvas, int x, int y, int radius, SKColor color)
        {
            using (var paint = new SKPaint { Color = color, IsAntialias = true })
                canvas.DrawCircle (x, y, radius, paint);
        }

        public static void DrawRoundedRectangle (this SKCanvas canvas, int x, int y, int width, int height, SKColor color, int rx = 3, int ry = 3, float strokeWidth = 1)
        {
            using (var paint = new SKPaint {
                Color = color,
                IsStroke = true,
                IsAntialias = true,
                LcdRenderText = true,
                StrokeWidth = 1f,
                SubpixelText = true,
                DeviceKerningEnabled = true,
                FilterQuality = SKFilterQuality.High,
                HintingLevel = SKPaintHinting.Full,
                IsAutohinted = true,
                TextAlign = SKTextAlign.Center
            })
                canvas.DrawRoundRect (x + .5f, y + .5f, width, height, rx, ry, paint);
        }

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

        public static void DrawBackground (this SKCanvas canvas, Rectangle bounds, ControlStyle style)
        {

            //if (CurrentStyle.BorderRadius > 0) {
            //    canvas.Clear (SKColors.Transparent);
            //    canvas.Save ();
            //    canvas.ClipRoundRect (new SKRoundRect (new SKRect (1.5f, 1.5f , Width - 1, Height - 1), CurrentStyle.BorderRadius, CurrentStyle.BorderRadius));
            //    canvas.Clear (CurrentStyle.BackgroundColor);
            //    canvas.Restore ();
            //} else {
            //    canvas.Clear (CurrentStyle.BackgroundColor);
            //}
            canvas.Clear (style.GetBackgroundColor ());
        }

        public static void DrawArrow (this SKCanvas canvas, Rectangle bounds, SKColor color, ArrowDirection direction)
        {
            switch (direction) {
                case ArrowDirection.Left: {
                        var y = bounds.Y + (bounds.Height / 2);
                        var x = bounds.X + (bounds.Width / 2) - 2;

                        canvas.DrawLine (x, y, x + 1, y, color);
                        canvas.DrawLine (x + 1, y - 1, x + 1, y + 2, color);
                        canvas.DrawLine (x + 2, y - 2, x + 2, y + 3, color);
                        canvas.DrawLine (x + 3, y - 3, x + 3, y + 4, color);
                        break;
                    }
                case ArrowDirection.Up: {
                        var y = bounds.Y + (bounds.Height / 2) - 2;
                        var x = bounds.X + (bounds.Width / 2);

                        canvas.DrawLine (x, y, x, y + 1, color);
                        canvas.DrawLine (x - 1, y + 1, x + 2, y + 1, color);
                        canvas.DrawLine (x - 2, y + 2, x + 3, y + 2, color);
                        canvas.DrawLine (x - 3, y + 3, x + 4, y + 3, color);
                        break;
                    }
                case ArrowDirection.Right: {
                        var y = bounds.Y + (bounds.Height / 2);
                        var x = bounds.X + (bounds.Width / 2) - 1;

                        canvas.DrawLine (x, y - 3, x + 1, y + 4, color);
                        canvas.DrawLine (x + 1, y - 2, x + 1, y + 3, color);
                        canvas.DrawLine (x + 2, y - 1, x + 2, y + 2, color);
                        canvas.DrawLine (x + 3, y, x + 3, y + 1, color);
                        break;
                    }
                case ArrowDirection.Down: {
                        var y = bounds.Y + (bounds.Height / 2) - 1;
                        var x = bounds.X + (bounds.Width / 2);

                        canvas.DrawLine (x - 3, y, x + 4, y + 1, color);
                        canvas.DrawLine (x - 2, y + 1, x + 3, y + 1, color);
                        canvas.DrawLine (x - 1, y + 2, x + 2, y + 2, color);
                        canvas.DrawLine (x, y + 3, x + 1, y + 3, color);
                        break;
                    }
            }
        }

        public static void DrawDisabledBitmap (this SKCanvas canvas, SKBitmap bitmap, float x, float y)
        {
            using (var paint = new SKPaint { ColorFilter = disabled_matrix })
                canvas.DrawBitmap (bitmap, x, y, paint);
        }

        public static SKRect ToSKRect (this Rectangle rect) => new SKRect (rect.X, rect.Y, rect.Right, rect.Bottom);
    }
}
