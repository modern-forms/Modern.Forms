#nullable disable

using System;
using System.Reflection;

namespace Avalonia.Platform
{
    internal interface IRuntimePlatform
    {
        IDisposable StartSystemTimer(TimeSpan interval, Action tick);
        RuntimePlatformInfo GetRuntimeInfo();
        IUnmanagedBlob AllocBlob(int size);
    }

    internal interface IUnmanagedBlob : IDisposable
    {
        IntPtr Address { get; }
        int Size { get; }
        bool IsDisposed { get; }
        
    }

    internal struct RuntimePlatformInfo
    {
        public OperatingSystemType OperatingSystem { get; set; }
        public bool IsDesktop { get; set; }
        public bool IsMobile { get; set; }
        public bool IsCoreClr { get; set; }
        public bool IsMono { get; set; }
        public bool IsDotNetFramework { get; set; }
        public bool IsUnix { get; set; }
    }

    internal enum OperatingSystemType
    {
        Unknown,
        WinNT,
        Linux,
        OSX,
        Android,
        iOS
    }
}
