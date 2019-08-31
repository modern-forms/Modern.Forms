#nullable disable

// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Input;

namespace Avalonia.Platform
{
    internal interface IStandardCursorFactory
    {
        IPlatformHandle GetCursor(StandardCursorType cursorType);
    }
}
