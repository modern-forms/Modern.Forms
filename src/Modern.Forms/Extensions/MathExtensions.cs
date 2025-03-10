using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    static class MathExtensions
    {
        public static int Clamp (this int value, int minimum, int maximum)
        {
            value = Math.Min (value, maximum);
            value = Math.Max (value, minimum);

            return value;
        }

        public static Size Clamp (this Size value, Size maximum)
        {
            value.Width = Math.Min (value.Width, maximum.Width);
            value.Height = Math.Min (value.Height, maximum.Height);

            return value;
        }
    }
}
