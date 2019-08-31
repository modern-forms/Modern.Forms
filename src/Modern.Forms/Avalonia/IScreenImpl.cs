#nullable disable

using System.Collections.Generic;

namespace Avalonia.Platform
{
    internal interface IScreenImpl
    {
        int ScreenCount { get; }

        IReadOnlyList<Screen> AllScreens { get; }
    }
}
