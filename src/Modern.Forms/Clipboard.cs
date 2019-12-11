using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Modern.Forms
{
    public static class Clipboard
    {
        public static Task<string> GetTextAsync () 
            => AvaloniaGlobals.ClipboardInterface.GetTextAsync ();

        public static Task SetTextAsync (string text)
            => AvaloniaGlobals.ClipboardInterface.SetTextAsync (text);

        public static Task ClearAsync ()
            => AvaloniaGlobals.ClipboardInterface.ClearAsync ();
    }
}
