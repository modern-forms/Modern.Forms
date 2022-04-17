// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    private class ColumnSpanComparer : SpanComparer
    {
        public override int GetSpan (LayoutInfo layoutInfo) => layoutInfo.ColumnSpan;

        public static ColumnSpanComparer GetInstance { get; } = new ColumnSpanComparer ();
    }
}
