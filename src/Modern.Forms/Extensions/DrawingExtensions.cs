using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    public static class DrawingExtensions
    {
        public static Rectangle CenterRectangle (this Rectangle outer, Size inner)
        {
            return new Rectangle (outer.X + ((outer.Width - inner.Width) / 2), outer.Y + ((outer.Height - inner.Height) / 2), inner.Width, inner.Height);
        }

        public static Rectangle CenterSquare (this Rectangle outer, int inner)
            => CenterRectangle (outer, new Size (inner, inner));
    }
}
