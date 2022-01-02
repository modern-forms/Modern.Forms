using System;
using System.Collections.Generic;
using System.Text;
using Modern.WindowKit.Input;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a mouse cursor.
    /// </summary>
    public class Cursor
    {
        internal Modern.WindowKit.Input.Cursor cursor;

        internal Cursor (StandardCursorType type)
        {
            cursor = new Modern.WindowKit.Input.Cursor (type);
        }

        /// <summary>
        /// The default cursor provided by the operating system.
        /// </summary>
        public static Cursor Default => Cursors.Arrow;
    }
}
