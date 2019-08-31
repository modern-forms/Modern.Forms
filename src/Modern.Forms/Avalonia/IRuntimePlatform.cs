#nullable disable

using System;
using System.Reflection;

namespace Avalonia.Platform
{
    interface IRuntimePlatform
    {
        IDisposable StartSystemTimer(TimeSpan interval, Action tick);
        RuntimePlatformInfo GetRuntimeInfo();
        IUnmanagedBlob AllocBlob(int size);
    }

    interface IUnmanagedBlob : IDisposable
    {
        IntPtr Address { get; }
        int Size { get; }
        bool IsDisposed { get; }
        
    }

    struct RuntimePlatformInfo
    {
        public OperatingSystemType OperatingSystem { get; set; }
        public bool IsDesktop { get; set; }
        public bool IsMobile { get; set; }
        public bool IsCoreClr { get; set; }
        public bool IsMono { get; set; }
        public bool IsDotNetFramework { get; set; }
        public bool IsUnix { get; set; }
    }

    enum OperatingSystemType
    {
        Unknown,
        WinNT,
        Linux,
        OSX,
        Android,
        iOS
    }
}
