using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;

namespace Modern.Forms
{
    public class Cursor
    {
        internal Avalonia.Input.Cursor cursor;

        internal Cursor (StandardCursorType type)
        {
            cursor = new Avalonia.Input.Cursor (type);
        }

        public static Cursor Default => Cursors.Arrow;
    }
}
