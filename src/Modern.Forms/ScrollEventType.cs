// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms
{
    /// <summary>
    ///  Specifies the type of action used to raise the <see cref='ScrollBar.Scroll'/> event.
    /// </summary>
    public enum ScrollEventType
    {
        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  left (horizontal) or top (vertical) scroll arrow or pressed
        ///  the UP ARROW
        /// </summary>
        SmallDecrement = 0,

        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  right (horizontal) or bottom (vertical) scroll arrow or pressed
        ///  the DOWN ARROW key.
        /// </summary>
        SmallIncrement = 1,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the left (horizontal) or above (vertical) the scroll box, or pressed
        ///  the PAGE UP key.
        /// </summary>
        LargeDecrement = 2,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the right (horizontal) or below (vertical) the scroll box, or pressed
        ///  the PAGE DOWN key.
        /// </summary>
        LargeIncrement = 3,

        /// <summary>
        ///  The scroll box was moved.
        /// </summary>
        ThumbPosition = 4,

        /// <summary>
        ///  The scroll box is currently being moved.
        /// </summary>
        ThumbTrack = 5,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Minimum'/>
        ///  position.
        /// </summary>
        First = 6,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Maximum'/>
        ///  position.
        /// </summary>
        Last = 7,

        /// <summary>
        ///  The scroll box has stopped moving.
        /// </summary>
        EndScroll = 8
    }
}
