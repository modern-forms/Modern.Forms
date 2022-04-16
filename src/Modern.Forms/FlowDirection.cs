// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
/// Defines directions a layout can flow the controls it is positioning.
/// </summary>
public enum FlowDirection
{
    /// <summary>
    /// Controls are positioned left to right.
    /// </summary>
    LeftToRight,

    /// <summary>
    /// Controls are positioned top to bottom.
    /// </summary>
    TopDown,

    /// <summary>
    /// Controls are positioned right to left.
    /// </summary>
    RightToLeft,

    /// <summary>
    /// Controls are positioned bottom to top.
    /// </summary>
    BottomUp
}
