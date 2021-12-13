using System;

namespace Modern.Forms
{
    /// <summary>
    /// Specifies some behaviors descendents of Control can request.
    /// </summary>
    [Flags]
    public enum ControlBehaviors
    {
        /// <summary>
        /// The control should be redrawn when the mouse is over it.
        /// </summary>
        Hoverable = 1,

        /// <summary>
        /// The control should be redrawn when the Text property changes.
        /// </summary>
        InvalidateOnTextChanged = 2,

        /// <summary>
        /// The control can receive focus.
        /// </summary>
        Selectable = 4,

        /// <summary>
        /// The control's background is transparent.
        /// </summary>
        Transparent = 8,

        /// <summary>
        /// The control receives mouse events. If not, mouse events will go
        /// to control(s) behind it.
        /// </summary>
        ReceivesMouseEvents = 16
    }
}
