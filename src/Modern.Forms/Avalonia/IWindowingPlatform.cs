#nullable disable

namespace Avalonia.Platform
{
    interface IWindowingPlatform
    {
        IWindowImpl CreateWindow();
        //IEmbeddableWindowImpl CreateEmbeddableWindow();
        IPopupImpl CreatePopup();
    }
}
