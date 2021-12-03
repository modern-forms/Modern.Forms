// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.MouseUp'/>, <see cref='Control.MouseDown'/> and
    /// <see cref='Control.MouseMove'/> events.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        private readonly Keys key_data;

        /// <summary>
        ///  Initializes a new instance of the <see cref='MouseEventArgs'/> class.
        /// </summary>
        public MouseEventArgs (MouseButtons button, int clicks, int x, int y, Point delta, int? screenX = null, int? screenY = null, Keys keyData = Keys.None)
        {
            Button = button;
            Clicks = clicks;
            Delta = delta;
            X = x;
            Y = y;
            ScreenLocation = new Point (screenX ?? x, screenY ?? y);
            key_data = keyData;
        }

        /// <summary>
        ///  Gets which mouse button was pressed.
        /// </summary>
        public MouseButtons Button { get; }

        /// <summary>
        ///  Gets the number of times the mouse button was pressed and released.
        /// </summary>
        public int Clicks { get; }

        /// <summary>
        ///  Gets the x-coordinate of a mouse click.
        /// </summary>
        public int X { get; }

        /// <summary>
        ///  Gets the y-coordinate of a mouse click.
        /// </summary>
        public int Y { get; }

        /// <summary>
        ///  Gets a signed count of the number of detents the mouse wheel has rotated in each direction.
        /// </summary>
        public Point Delta { get; }

        /// <summary>
        ///  Gets the location of the mouse during MouseEvent.
        /// </summary>
        public Point Location => new Point (X, Y);

        /// <summary>
        /// Get the mouse location in screen coordinates.
        /// </summary>
        public Point ScreenLocation { get; }

        /// <summary>
        /// Gets whether the Control modifier key was also pressed.
        /// </summary>
        public bool Alt => key_data.HasFlag (Keys.Alt);

        /// <summary>
        /// Gets whether the Alt modifier key was also pressed.
        /// </summary>
        public bool Control => key_data.HasFlag (Keys.Control);

        /// <summary>
        /// Gets the modifier keys that were also pressed.
        /// </summary>
        public Keys Modifiers => key_data & Keys.Modifiers;

        /// <summary>
        /// Gets whether the Shift modifier key was also pressed.
        /// </summary>
        public bool Shift => key_data.HasFlag (Keys.Shift);

        public static MouseEventArgs Empty = new MouseEventArgs (MouseButtons.None, 0, 0, 0, Point.Empty);
    }
}
