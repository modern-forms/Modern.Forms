namespace Modern.Forms
{
    /// <summary>
    /// Specifies the visual state of a link within a <see cref="LinkLabel"/>.
    /// </summary>
    [System.Flags]
    public enum LinkState
    {
        /// <summary>
        /// The link is in its normal state.
        /// </summary>
        Normal = 0x00,

        /// <summary>
        /// The pointer is hovering over the link.
        /// </summary>
        Hover = 0x01,

        /// <summary>
        /// The link is currently pressed.
        /// </summary>
        Active = 0x02,

        /// <summary>
        /// The link has been marked as visited.
        /// </summary>
        Visited = 0x04
    }
}
