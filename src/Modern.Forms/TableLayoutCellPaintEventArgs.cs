// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using SkiaSharp;

namespace Modern.Forms;

/// <summary>
///  This is the overridden PaintEventArgs for painting the cell of the table. It contains additional information
///  indicating the row/column of the cell as well as the bounds of the cell.
/// </summary>
public class TableLayoutCellPaintEventArgs : PaintEventArgs
{
    /// <summary>
    /// Initializes a new instance of the TableLayoutCellPaintEventArgs class.
    /// </summary>
    public TableLayoutCellPaintEventArgs (SKImageInfo info, SKCanvas canvas, double scaling, Rectangle clipRectangle, Rectangle cellBounds, int column, int row)
        : base (info, canvas, scaling)
    {
        CellBounds = cellBounds;
        Column = column;
        Row = row;
    }

    //internal TableLayoutCellPaintEventArgs(
    //    PaintEventArgs e,
    //    Rectangle clipRectangle,
    //    Rectangle cellBounds,
    //    int column,
    //    int row)
    //    : base(e, clipRectangle)
    //{
    //    CellBounds = cellBounds;
    //    Column = column;
    //    Row = row;
    //}

    /// <summary>
    /// The bounds of the cell to paint.
    /// </summary>
    public Rectangle CellBounds { get; }

    /// <summary>
    /// The column index of the cell.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// The row index of the cell.
    /// </summary>
    public int Row { get; }
}
