// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
/// Represents a row style for a TableLayoutPanel.
/// </summary>
public class RowStyle : TableLayoutStyle
{
    /// <summary>
    /// Initializes a new instance of the RowStyle class.
    /// </summary>
    public RowStyle ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RowStyle class.
    /// </summary>
    public RowStyle (SizeType sizeType)
    {
        SizeType = sizeType;
    }

    /// <summary>
    /// Initializes a new instance of the RowStyle class.
    /// </summary>
    public RowStyle (SizeType sizeType, float height)
    {
        SizeType = sizeType;
        Height = height;
    }

    /// <summary>
    /// The height of the row.
    /// </summary>
    public float Height {
        get => Size;
        set => Size = value;
    }
}
