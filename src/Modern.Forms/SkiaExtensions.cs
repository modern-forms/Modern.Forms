using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public static class SkiaExtensions
    {
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

        public static void DrawCenteredText (this SKCanvas canvas, string text, SKTypeface font, int fontsize, int x, int y, SKColor color)
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
                IsAutohinted = true,
                TextAlign = SKTextAlign.Center
            })
                canvas.DrawText (text, x, y, paint);
        }

        public static void DrawLine (this SKCanvas canvas, float x1, float y1, float x2, float y2, SKColor color)
        {
            using (var paint = new SKPaint { Color = color })
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
            //using (var paint = new SKPaint { Color = color, IsStroke = true, StrokeWidth = strokeWidth })
                canvas.DrawRoundRect (x + .5f, y + .5f, width, height, rx, ry, paint);
        }
    }
}
