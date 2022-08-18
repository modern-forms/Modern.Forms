using System.Threading.Tasks;
using Modern.WindowKit;
using Modern.WindowKit.Input.Platform;

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
            => AvaloniaGlobals.GetRequiredService<IClipboard> ().GetTextAsync ();

        /// <summary>
        /// Sets the text contents of the clipboard.
        /// </summary>
        public static Task SetTextAsync (string text)
            => AvaloniaGlobals.GetRequiredService<IClipboard> ().SetTextAsync (text);

        /// <summary>
        /// Clears the contents of the clipbaord.
        /// </summary>
        public static Task ClearAsync ()
            => AvaloniaGlobals.GetRequiredService<IClipboard> ().ClearAsync ();
    }
}
