#nullable disable

using Avalonia.Platform;

namespace Avalonia.Controls.Platform.Surfaces
{
    interface IFramebufferPlatformSurface
    {
        /// <summary>
        /// Provides a framebuffer descriptor for drawing.
        /// </summary>
        /// <remarks>
        /// Contents should be drawn on actual window after disposing
        /// </remarks>
        ILockedFramebuffer Lock();
    }
}
