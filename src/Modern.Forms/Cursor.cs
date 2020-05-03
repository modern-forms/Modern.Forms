using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a mouse cursor.
    /// </summary>
    public class Cursor
    {
        internal Avalonia.Input.Cursor cursor;

        internal Cursor (StandardCursorType type)
        {
            cursor = new Avalonia.Input.Cursor (type);
        }

        /// <summary>
        /// The default cursor provided by the operating system.
        /// </summary>
        public static Cursor Default => Cursors.Arrow;
    }
}
