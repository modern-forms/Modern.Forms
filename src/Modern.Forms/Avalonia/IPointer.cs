#nullable disable

namespace Avalonia.Input
{
    internal interface IPointer
    {
        int Id { get; }
        //void Capture(IInputElement control);
        //IInputElement Captured { get; }
        PointerType Type { get; }
        bool IsPrimary { get; }
        
    }

    internal enum PointerType
    {
        Mouse,
        Touch
    }
}
