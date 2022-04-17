// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    internal struct Strip
    {
        public int MinSize { get; set; }

        public int MaxSize { get; set; }

        public bool IsStart { get; set; } //whether there is an element starting in this strip
    }
}
