#nullable disable

namespace Avalonia.Platform
{
    /// <summary>
    /// Defines the platform-specific interface for a <see cref="Avalonia.Media.Imaging.WriteableBitmap"/>.
    /// </summary>
    interface IWriteableBitmapImpl : IBitmapImpl
    {
        ILockedFramebuffer Lock();
    }
}
