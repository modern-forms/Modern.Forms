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
        public static IPlatformThreadingInterface PlatformThreadingInterface { get; private set; }
        public static IWindowingPlatform WindowingInterface { get; private set; }
        public static IStandardCursorFactory StandardCursorFactory { get; private set; }

        static AvaloniaGlobals ()
        {
            RuntimePlatform = new StandardRuntimePlatform ();

            var runtime = RuntimePlatform.GetRuntimeInfo ();

            if (runtime.OperatingSystem == OperatingSystemType.WinNT)
                InitializeWindows ();
            else if (runtime.OperatingSystem == OperatingSystemType.Linux)
                InitializeLinux ();
            else if (runtime.OperatingSystem == OperatingSystemType.OSX)
                InitializeOSX ();
            else
                throw new InvalidOperationException ("Unrecognized Operating System");
        }

        private static void InitializeLinux ()
        {
            var x11 = new AvaloniaX11Platform ();
            x11.Initialize (new X11PlatformOptions ());

            WindowingInterface = x11;
            PlatformThreadingInterface = new X11PlatformThreading (x11);
            StandardCursorFactory = new X11CursorFactory (x11.Display);
        }

        private static void InitializeOSX ()
        {
            var platform = Native.AvaloniaNativePlatform.Initialize ();

            WindowingInterface = platform;
            PlatformThreadingInterface = new Native.PlatformThreadingInterface (platform.Factory.CreatePlatformThreadingInterface ());
            StandardCursorFactory = new Native.CursorFactory (platform.Factory.CreateCursorFactory ());
        }
 
        private static void InitializeWindows ()
        {
                Win32Platform.Initialize ();
                PlatformThreadingInterface = Win32Platform.Instance;
                WindowingInterface = Win32Platform.Instance;
                StandardCursorFactory = CursorFactory.Instance;
        }
    }
}
