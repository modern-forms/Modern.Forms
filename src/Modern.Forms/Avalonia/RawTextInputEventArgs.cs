#nullable disable

// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Avalonia.Input.Raw
{
    class RawTextInputEventArgs : RawInputEventArgs
    {
        public string Text { get; set; }

        public InputModifiers Modifiers { get; set; }

        public RawTextInputEventArgs(IKeyboardDevice device, ulong timestamp, string text, InputModifiers modifiers) : base(device, timestamp)
        {
            Text = text;
            Modifiers = modifiers;
        }
    }
}
