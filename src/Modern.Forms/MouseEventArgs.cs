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
// Copyright (c) 2004-2005 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//

using System;
using System.Drawing;

namespace Modern.Forms
{
    public class MouseEventArgs : EventArgs
    {
        private readonly Keys key_data;

        public MouseEventArgs (MouseButtons button, int clicks, int x, int y, Point delta, Keys keyData = Keys.None)
        {
            Button = button;
            Clicks = clicks;
            Delta = delta;
            X = x;
            Y = y;
            key_data = keyData;
        }

        public MouseButtons Button { get; }

        public int Clicks { get; }

        public Point Delta { get; }

        public int X { get; }

        public int Y { get; }

        public Point Location => new Point (X, Y);

        public bool Alt => key_data.HasFlag (Keys.Alt);

        public bool Control => key_data.HasFlag (Keys.Control);

        public Keys Modifiers => key_data & Keys.Modifiers;

        public bool Shift => key_data.HasFlag (Keys.Shift);
    }
}
