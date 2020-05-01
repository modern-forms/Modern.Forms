#nullable disable

// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a class that can be closed.
    /// </summary>
    public interface ICloseable
    {
        /// <summary>
        /// Raised when the class is closed.
        /// </summary>
        event EventHandler Closed;
    }
}
