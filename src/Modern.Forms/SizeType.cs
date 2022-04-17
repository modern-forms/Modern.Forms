// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
/// Represents the size type of a table column or row.
/// </summary>
public enum SizeType
{
    /// <summary>
    /// The row or column will be automatically sized as needed to fit the contents.
    /// </summary>
    AutoSize,

    /// <summary>
    /// The row or column will be assigned a fixed absolute size.
    /// </summary>
    Absolute,

    /// <summary>
    /// The row or column will be assigned a percentage of the total available space.
    /// </summary>
    Percent
}
