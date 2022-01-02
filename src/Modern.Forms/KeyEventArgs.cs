// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Modern.WindowKit.Input;

namespace Modern.Forms
{
    /// <summary>
    ///  Provides data for the KeyDown or KeyUp event.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress = false;

        /// <summary>
        ///  Initializes a new instance of the KeyEventArgs class.
        /// </summary>
        public KeyEventArgs (Keys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        ///  Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public virtual bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

        /// <summary>
        ///  Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        /// <summary>
        ///  Gets or sets a value indicating whether the event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        ///  Gets the keyboard code for a KeyDown or KeyUp event.
        /// </summary>
        public Keys KeyCode {
            get {
                var keyGenerated = KeyData & Keys.KeyCode;

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined (typeof (Keys), (int)keyGenerated)) {
                    return Keys.None;
                }
                return keyGenerated;
            }
        }

        /// <summary>
        ///  Gets the keyboard value for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        /// </summary>
        public int KeyValue => (int)(KeyData & Keys.KeyCode);

        /// <summary>
        ///  Gets the key data for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        /// </summary>
        public Keys KeyData { get; }

        /// <summary>
        ///  Gets the modifier flags for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        ///  This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        /// </summary>
        public Keys Modifiers => KeyData & Keys.Modifiers;

        /// <summary>
        ///  Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        public virtual bool Shift => (KeyData & Keys.Shift) == Keys.Shift;

        /// <summary>
        /// Gets or sets a value indicating the key press should be suppressed.
        /// </summary>
        public bool SuppressKeyPress {
            get => _suppressKeyPress;
            set {
                _suppressKeyPress = value;
                Handled = value;
            }
        }

        internal static Keys FromInputModifiers (RawInputModifiers modifiers)
        {
            var keys = Keys.None;

            if (modifiers.HasFlag (RawInputModifiers.Alt))
                keys |= Keys.Alt;
            if (modifiers.HasFlag (RawInputModifiers.Control))
                keys |= Keys.Control;
            if (modifiers.HasFlag (RawInputModifiers.Shift))
                keys |= Keys.Shift;

            return keys;
        }
    }
}
