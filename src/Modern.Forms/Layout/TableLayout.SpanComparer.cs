// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    private abstract class SpanComparer : IComparer
    {
        public abstract int GetSpan (LayoutInfo layoutInfo);

        public int Compare (object? x, object? y)
        {
            var xInfo = (LayoutInfo)x!;
            var yInfo = (LayoutInfo)y!;

            return GetSpan (xInfo) - GetSpan (yInfo);
        }
    }
}
