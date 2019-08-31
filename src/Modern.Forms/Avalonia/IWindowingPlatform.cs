#nullable disable

namespace Avalonia.Platform
{
    internal interface IWindowingPlatform
    {
        IWindowImpl CreateWindow();
        //IEmbeddableWindowImpl CreateEmbeddableWindow();
        IPopupImpl CreatePopup();
    }
}
