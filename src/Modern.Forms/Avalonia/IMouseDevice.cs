#nullable disable

// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia.Input
{
    /// <summary>
    /// Represents a mouse device.
    /// </summary>
    internal interface IMouseDevice : IPointerDevice
    {
        /// <summary>
        /// Gets the mouse position, in screen coordinates.
        /// </summary>
        [Obsolete("Use PointerEventArgs.GetPosition")]
        PixelPoint Position { get; }
    }
}
