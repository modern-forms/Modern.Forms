// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    private class PostAssignedPositionComparer : IComparer
    {
        public static PostAssignedPositionComparer GetInstance { get; } = new PostAssignedPositionComparer ();

        public int Compare (object? x, object? y)
        {
            var xInfo = (LayoutInfo)x!;
            var yInfo = (LayoutInfo)y!;

            if (xInfo.RowStart < yInfo.RowStart)
                return -1;

            if (xInfo.RowStart > yInfo.RowStart)
                return 1;

            if (xInfo.ColumnStart < yInfo.ColumnStart)
                return -1;

            if (xInfo.ColumnStart > yInfo.ColumnStart)
                return 1;

            return 0;
        }
    }
}
