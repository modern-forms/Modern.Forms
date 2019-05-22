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
//

using System;

namespace Modern.Forms
{
    public class KeyEventArgs : EventArgs
    {
        private bool supress_key_press;

        public KeyEventArgs (Keys keyData)
        {
            KeyData = keyData;
            Handled = false;
        }

        public virtual bool Alt => KeyData.HasFlag (Keys.Alt);

        public virtual bool Control => KeyData.HasFlag (Keys.Control);

        public bool Handled { get; set; }

        public Keys KeyCode => KeyData & Keys.KeyCode;

        public Keys KeyData { get; }

        public int KeyValue => Convert.ToInt32 (KeyData);

        public Keys Modifiers => KeyData & Keys.Modifiers;

        public virtual bool Shift => KeyData.HasFlag (Keys.Shift);

        public bool SuppressKeyPress {
            get => supress_key_press;
            set {
                supress_key_press = value;
                Handled = value;
            }
        }
    }
}
