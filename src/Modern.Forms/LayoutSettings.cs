// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// A base class for settings needed by a LayoutEngine.
/// </summary>
public abstract class LayoutSettings
{
    /// <summary>
    /// Initializes a new instance of the LayoutSettings class.
    /// </summary>
    internal LayoutSettings (IArrangedElement owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Gets the LayoutEngine for the current control.
    /// </summary>
    public virtual LayoutEngine? LayoutEngine => null;

    internal IArrangedElement Owner { get; }
}
