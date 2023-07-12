using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform.Storage;
using Modern.WindowKit.Platform.Storage.FileIO;

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

        public static string? GetFullPath (this IStorageFile file)
        {
            if (file is BclStorageFile path)
                return path.FileInfo.FullName;

            return null;
        }

        public static string? GetFullPath (this IStorageFolder file)
        {
            if (file is BclStorageFolder path)
                return path.DirectoryInfo.FullName;

            return null;
        }
    }
}
