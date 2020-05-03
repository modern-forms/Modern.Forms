// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2005,2006 Novell, Inc. (http://www.novell.com)
//
// Author:
//	Pedro Martínez Juliá <pedromj@gmail.com>
//	Daniel Nauck    (dna(at)mono-project(dot)de)
//

using System;
using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the amount of spacing between a control's edges and its content.
    /// </summary>
    public struct Padding
    {
        /// <summary>
        /// Initializes a new instance of the Padding class with the specified padding for all sides.
        /// </summary>
        public Padding (int all)
        {
            Left = all;
            Right = all;
            Top = all;
            Bottom = all;
        }

        /// <summary>
        /// Initializes a new instance of the Padding class with the specified padding for each side.
        /// </summary>
        public Padding (int left, int top, int right, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets or sets the amount of padding for all sides. If all sides are not the same, -1 is returned.
        /// </summary>
        public int All {
            get => (Left != Top) || (Left != Right) || (Left != Bottom) ? -1 : Top;
            set => Left = Top = Right = Bottom = value;
        }

        /// <summary>
        /// Gets or sets the amount of padding on the bottom.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Represents padding with all sides set to 0.
        /// </summary>
        public static readonly Padding Empty = new Padding (0);

        /// <inheritdoc/>
        public override bool Equals (object? other)
        {
            if (other is Padding other_aux) {
                return Left == other_aux.Left &&
                    Top == other_aux.Top &&
                    Right == other_aux.Right &&
                    Bottom == other_aux.Bottom;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode () => Top ^ Bottom ^ Left ^ Right;

        /// <summary>
        /// Gets the total amount of horizontal padding.
        /// </summary>
        public int Horizontal => Left + Right;

        /// <summary>
        /// Gets or sets the amount of padding on the left side.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the amount of padding on the right side.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the amount of padding on the top.
        /// </summary>
        public int Top { get; set; }

        /// <inheritdoc/>
        public override string ToString () => $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";

        /// <summary>
        /// Gets the total amount of vertical padding.
        /// </summary>
        public int Vertical => Top + Bottom;

        /// <summary>
        /// Implements the == operator to determine if 2 Padding objects are the same.
        /// </summary>
        public static bool operator == (Padding p1, Padding p2) => p1.Equals (p2);

        /// <summary>
        /// Implements the != operator to determine if 2 Padding objects are not the same.
        /// </summary>
        public static bool operator != (Padding p1, Padding p2) => !p1.Equals (p2);
    }
}
