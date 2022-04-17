// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    private class PreAssignedPositionComparer : IComparer
    {
        public static PreAssignedPositionComparer GetInstance { get; } = new PreAssignedPositionComparer ();

        public int Compare (object? x, object? y)
        {
            var xInfo = (LayoutInfo)x!;
            var yInfo = (LayoutInfo)y!;

            if (xInfo.RowPosition < yInfo.RowPosition)
                return -1;

            if (xInfo.RowPosition > yInfo.RowPosition)
                return 1;

            if (xInfo.ColumnPosition < yInfo.ColumnPosition)
                return -1;

            if (xInfo.ColumnPosition > yInfo.ColumnPosition)
                return 1;

            return 0;
        }
    }
}
