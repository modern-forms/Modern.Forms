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
        }

        public SKSurface Surface { get; }

        public SKImageInfo Info { get; }

        public SKCanvas Canvas => Surface?.Canvas;
    }
}
