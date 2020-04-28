using System;
using System.Threading.Tasks;
using Avalonia;

namespace Modern.Forms
{
    /// <summary>
    /// Class for interacting with an operating system's clipboard.
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        /// Gets the contents of the clipboard as text.
        /// </summary>
        public static Task<string> GetTextAsync () 
            => AvaloniaGlobals.ClipboardInterface.GetTextAsync ();

        /// <summary>
        /// Sets the text contents of the clipboard.
        /// </summary>
        public static Task SetTextAsync (string text)
            => AvaloniaGlobals.ClipboardInterface.SetTextAsync (text);

        /// <summary>
        /// Clears the contents of the clipbaord.
        /// </summary>
        public static Task ClearAsync ()
            => AvaloniaGlobals.ClipboardInterface.ClearAsync ();
    }
}
