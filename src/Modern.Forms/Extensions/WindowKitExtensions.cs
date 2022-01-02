using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.WindowKit.Input;

namespace Modern.Forms
{
    internal static class WindowKitExtensions
    {
        public static Keys AddModifiers (Keys keys, RawInputModifiers modifiers)
        {
            if (modifiers.HasFlag (RawInputModifiers.Alt))
                keys |= Modern.Forms.Keys.Alt;
            if (modifiers.HasFlag (RawInputModifiers.Control))
                keys |= Modern.Forms.Keys.Control;
            if (modifiers.HasFlag (RawInputModifiers.Shift))
                keys |= Modern.Forms.Keys.Shift;

            return keys;
        }
    }
}
