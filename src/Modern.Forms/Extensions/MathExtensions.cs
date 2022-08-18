﻿using System;

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
    }
}
