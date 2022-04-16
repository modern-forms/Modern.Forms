// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms;

/// <summary>
///  Defined in such a way that you can cast the relation to an AnchorStyle and
///  the direction of the AnchorStyle points to where the image goes.
///  (e.g., (AnchorStyle)ImageBeforeText -> Left))
/// </summary>
public enum TextImageRelation
{
    /// <summary>
    /// Image and text should occupy the same space.
    /// </summary>
    Overlay = AnchorStyles.None,

    /// <summary>
    /// Image should be positioned before the text.
    /// </summary>
    ImageBeforeText = AnchorStyles.Left,

    /// <summary>
    /// Text should be postitioned before the image.
    /// </summary>
    TextBeforeImage = AnchorStyles.Right,

    /// <summary>
    /// Image should be positioned above the text.
    /// </summary>
    ImageAboveText = AnchorStyles.Top,

    /// <summary>
    /// Text should be positioned above the image.
    /// </summary>
    TextAboveImage = AnchorStyles.Bottom
}
