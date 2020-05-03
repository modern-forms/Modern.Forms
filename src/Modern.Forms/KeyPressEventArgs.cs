// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Modern.Forms
{
    /// <summary>
    ///  Provides data for the KeyPress event.
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        private readonly Keys key_data;

        /// <summary>
        ///  Initializes a new instance of the KeyPressEventArgs class.
        /// </summary>
        public KeyPressEventArgs (string text, Keys keyData = Keys.None)
        {
            Text = text;
            KeyChar = text[0];
            Handled = false;
            key_data = keyData;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the KeyPress event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        ///  Gets the character corresponding to the key pressed.
        /// </summary>
        public char KeyChar { get; set; }

        /// <summary>
        /// Gets or sets the text corresponding to the key press.
        /// </summary>
        public string Text { get; set; }

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
    }
}
