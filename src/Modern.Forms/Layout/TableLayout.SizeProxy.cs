// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms.Layout;

internal partial class TableLayout
{
    //sizeProxy. Takes a strip and return its minSize or maxSize accordingly
    private abstract class SizeProxy
    {
        protected Strip strip;

        public Strip Strip {
            get => strip;
            set => strip = value;
        }

        public abstract int Size { get; set; }
    }
}
