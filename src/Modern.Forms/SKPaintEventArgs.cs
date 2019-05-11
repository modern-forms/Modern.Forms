using System;
using SkiaSharp;

namespace Modern.Forms
{
    public class SKPaintEventArgs : EventArgs
    {
        public SKPaintEventArgs (SKSurface surface, SKImageInfo info)
        {
            Surface = surface;
            Info = info;
            Canvas = surface?.Canvas;
        }

        public SKPaintEventArgs (SKSurface surface, SKImageInfo info, SKCanvas canvas)
        {
            Surface = surface;
            Info = info;
            Canvas = canvas;
        }

        public SKSurface Surface { get; }

        public SKImageInfo Info { get; }

        public SKCanvas Canvas { get; set; }
    }
}
