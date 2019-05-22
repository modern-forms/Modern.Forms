using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Platform;
using Avalonia.Shared.PlatformSupport;
using Avalonia.Win32;

namespace Avalonia
{
    public static class AvaloniaGlobals
    {
        public static IRuntimePlatform RuntimePlatform { get; } = new StandardRuntimePlatform ();
        public static IPlatformThreadingInterface PlatformThreadingInterface { get; } = new Win32Platform ();

        static AvaloniaGlobals ()
        {
            Win32Platform.Initialize ();
        }
    }
}
