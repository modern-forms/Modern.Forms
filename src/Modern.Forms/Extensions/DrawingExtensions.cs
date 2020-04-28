using System;
using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// A collection of extension methods to facilitate drawing operations.
    /// </summary>
    public static class DrawingExtensions
    {
        /// <summary>
        /// Centers a rectangle inside a larger rectangle.
        /// </summary>
        public static Rectangle CenterRectangle (this Rectangle outer, Size inner)
        {
            return new Rectangle (outer.X + ((outer.Width - inner.Width) / 2), outer.Y + ((outer.Height - inner.Height) / 2), inner.Width, inner.Height);
        }

        /// <summary>
        /// Centers a square inside a larger rectangle.
        /// </summary>
        public static Rectangle CenterSquare (this Rectangle outer, int inner)
            => CenterRectangle (outer, new Size (inner, inner));

        /// <summary>
        /// Gets the center point of a rectangle.
        /// </summary>
        public static Point GetCenter (this Rectangle rectangle)
            => new Point (rectangle.Left + ((rectangle.Right - rectangle.Left) / 2),  rectangle.Top + ((rectangle.Bottom - rectangle.Top) / 2));

        internal static Avalonia.PixelPoint ToPixelPoint (this Point point) => new Avalonia.PixelPoint (point.X, point.Y);
    }
}
