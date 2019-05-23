using System;
using SkiaSharp;

namespace Modern.Forms
{
    public class PaintEventArgs : EventArgs
    {
        public PaintEventArgs (SKSurface surface, SKImageInfo info)
        {
            Surface = surface;
            Info = info;
            Canvas = surface?.Canvas;
        }

        public PaintEventArgs (SKSurface surface, SKImageInfo info, SKCanvas canvas)
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
