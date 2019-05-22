using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Platform;
using Avalonia.Shared.PlatformSupport;
using Avalonia.Win32;
using Avalonia.X11;

namespace Avalonia
{
    public static class AvaloniaGlobals
    {
        public static IRuntimePlatform RuntimePlatform { get; }
        public static IPlatformThreadingInterface PlatformThreadingInterface { get; }
        public static IWindowingPlatform WindowingInterface { get; }
        public static IStandardCursorFactory StandardCursorFactory { get; }

        static AvaloniaGlobals ()
        {
            RuntimePlatform = new StandardRuntimePlatform ();

            var runtime = RuntimePlatform.GetRuntimeInfo ();

            if (runtime.OperatingSystem == OperatingSystemType.WinNT) {
                Win32Platform.Initialize ();
                PlatformThreadingInterface = Win32Platform.Instance;
                WindowingInterface = Win32Platform.Instance;
            } else if (runtime.OperatingSystem == OperatingSystemType.Linux) {
                var x11 = new AvaloniaX11Platform ();
                x11.Initialize (new X11PlatformOptions ());

                WindowingInterface = x11;
                PlatformThreadingInterface = new X11PlatformThreading (x11);
                StandardCursorFactory = new X11CursorFactory (x11.Display);
            }
        }
    }
}
