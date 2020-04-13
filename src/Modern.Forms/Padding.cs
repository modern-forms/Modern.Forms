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
    public struct Padding
    {
        public Padding (int all)
        {
            Left = all;
            Right = all;
            Top = all;
            Bottom = all;
        }

        public Padding (int left, int top, int right, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public static readonly Padding Empty = new Padding (0);

        public int All {
            get => (Left != Top) || (Left != Right) || (Left != Bottom) ? -1 : Top;
            set => Left = Top = Right = Bottom = value;
        }

        public int Bottom { get; set; }

        public int Horizontal => Left + Right;

        public int Left { get; set; }

        public int Right { get; set; }

        public Size Size => new Size (Horizontal, Vertical);

        public int Top { get; set; }

        public int Vertical => Top + Bottom;

        public static Padding Add (Padding p1, Padding p2)
        {
            return p1 + p2;
        }

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

        public override int GetHashCode () => Top ^ Bottom ^ Left ^ Right;

        public static Padding operator + (Padding p1, Padding p2)
        {
            return new Padding (p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }

        public static bool operator == (Padding p1, Padding p2) => p1.Equals (p2);

        public static bool operator != (Padding p1, Padding p2) => !p1.Equals (p2);

        public static Padding operator - (Padding p1, Padding p2)
        {
            return new Padding (p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }

        public static Padding Subtract (Padding p1, Padding p2) => p1 - p2;

        public override string ToString () => $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";
    }
}
