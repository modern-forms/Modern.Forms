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
        public static async Task<string?> GetTextAsync ()
            => await AvaloniaGlobals.GetRequiredService<IClipboard> ().GetTextAsync ().ConfigureAwait (false);

        /// <summary>
        /// Sets the text contents of the clipboard.
        /// </summary>
        public static async Task SetTextAsync (string text)
            => await AvaloniaGlobals.GetRequiredService<IClipboard> ().SetTextAsync (text).ConfigureAwait (false);

        /// <summary>
        /// Clears the contents of the clipboard.
        /// </summary>
        public static async Task ClearAsync ()
            => await AvaloniaGlobals.GetRequiredService<IClipboard> ().ClearAsync ().ConfigureAwait (false);
    }
}
