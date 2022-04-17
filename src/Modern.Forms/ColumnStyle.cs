// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
/// Represents a column style for a TableLayoutPanel.
/// </summary>
public class ColumnStyle : TableLayoutStyle
{
    /// <summary>
    /// Initializes a new instance of the ColumnStyle class.
    /// </summary>
    public ColumnStyle ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ColumnStyle class.
    /// </summary>
    public ColumnStyle (SizeType sizeType)
    {
        SizeType = sizeType;
    }

    /// <summary>
    /// Initializes a new instance of the ColumnStyle class.
    /// </summary>
    public ColumnStyle (SizeType sizeType, float width)
    {
        SizeType = sizeType;
        Width = width;
    }

    /// <summary>
    /// The width of the column.
    /// </summary>
    public float Width {
        get => Size;
        set => Size = value;
    }
}
