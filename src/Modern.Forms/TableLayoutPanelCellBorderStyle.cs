// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
/// Specifies the available types of TableLayoutPanel cell border styles.
/// </summary>
public enum TableLayoutPanelCellBorderStyle
{
    /// <summary>
    /// No border
    /// </summary>
    None = 0,

    /// <summary>
    /// A single line border
    /// </summary>
    Single = 1,

    /// <summary>
    /// A border that appears inset
    /// </summary>
    Inset = 2,

    /// <summary>
    /// A double border that appears inset
    /// </summary>
    InsetDouble = 3,

    /// <summary>
    /// A border that appears outset
    /// </summary>
    Outset = 4,

    /// <summary>
    /// A double border that appears outset
    /// </summary>
    OutsetDouble = 5,

    /// <summary>
    /// A partial border that appears outset
    /// </summary>
    OutsetPartial = 6
}
