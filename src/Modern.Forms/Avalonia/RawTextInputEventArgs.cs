#nullable disable

// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Avalonia.Input.Raw
{
    internal class RawTextInputEventArgs : RawInputEventArgs
    {
        public RawTextInputEventArgs(
            IKeyboardDevice device,
            ulong timestamp,
            //IInputRoot root,
            string text,
            RawInputModifiers modifiers)
            : base(device, timestamp/*, root*/)
        {
            Text = text;
            Modifiers = modifiers;
        }

        public string Text { get; set; }

        public RawInputModifiers Modifiers { get; set; }
    }
}
