using System;
using System.Collections.Generic;
using System.Linq;
using Modern.WindowKit.Input;

namespace Modern.Forms
{
    static class UtilityExtensions
    {
        public static bool In<T> (this T enumeration, params T[] values) where T : Enum
        {
            foreach (var en in values)
                if (enumeration.Equals (en))
                    return true;

            return false;
        }

        public static bool HasValue (this string str) => !string.IsNullOrEmpty (str);

        public static MouseButtons ToMouseButtons (this RawInputModifiers modifiers)
        {
            var buttons = MouseButtons.None;

            if (modifiers.HasFlag (RawInputModifiers.LeftMouseButton))
                buttons |= MouseButtons.Left;
            if (modifiers.HasFlag (RawInputModifiers.RightMouseButton))
                buttons |= MouseButtons.Right;
            if (modifiers.HasFlag (RawInputModifiers.MiddleMouseButton))
                buttons |= MouseButtons.Middle;
            if (modifiers.HasFlag (RawInputModifiers.XButton1MouseButton))
                buttons |= MouseButtons.XButton1;
            if (modifiers.HasFlag (RawInputModifiers.XButton2MouseButton))
                buttons |= MouseButtons.XButton2;

            return buttons;
        }

        public static IEnumerable<T> WhereNotNull<T> (this IEnumerable<T?> values) where T : class
        {
            return values.Where (p => p != null).Cast<T> ();
        }
    }
}
