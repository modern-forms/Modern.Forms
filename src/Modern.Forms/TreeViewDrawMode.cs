// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
///  Specifies responsibility for drawing TreeView nodes.
/// </summary>
public enum TreeViewDrawMode
{
    /// <summary>
    ///  The toolkit paints the nodes of the TreeView.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///  The user needs to paint the text and other content only.
    /// </summary>
    OwnerDrawContent = 1,

    ///// <summary>
    /////  The user paints the entire row corresponding to a node, including backgrounds, lines, and boxes.
    ///// </summary>
    //OwnerDrawAll = 2,
}
