namespace Modern.Forms
{
    /// <summary>
    /// Specifies identifiers to indicate how a control is docked to its parent.
    /// </summary>
    public enum DockStyle
    {
        /// <summary>
        /// The control is not docked to its parent.
        /// </summary>
        None = 0,

        /// <summary>
        /// The control is docked to the top of its parent.
        /// </summary>
        Top = 1,

        /// <summary>
        /// The control is docked to the bottom of its parent.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// The control is docked to the left of its parent.
        /// </summary>
        Left = 3,

        /// <summary>
        /// The control is docked to the right of its parent.
        /// </summary>
        Right = 4,

        /// <summary>
        /// The control will fill the area of its parent.
        /// </summary>
        Fill = 5
    }
}
