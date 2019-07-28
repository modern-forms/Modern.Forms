using System.Collections.Generic;

namespace Avalonia.Platform
{
    interface IScreenImpl
    {
        int ScreenCount { get; }

        IReadOnlyList<Screen> AllScreens { get; }
    }
}
