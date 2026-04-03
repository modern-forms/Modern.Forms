namespace Modern.Forms
{
    /// <summary>
    /// Specifies where tick marks are drawn on a <see cref="TrackBar"/>.
    /// </summary>
    public enum TickStyle
    {
        /// <summary>
        /// No tick marks are displayed.
        /// </summary>
        None,

        /// <summary>
        /// Tick marks are displayed on the top side of a horizontal <see cref="TrackBar"/>
        /// or on the left side of a vertical <see cref="TrackBar"/>.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Tick marks are displayed on the bottom side of a horizontal <see cref="TrackBar"/>
        /// or on the right side of a vertical <see cref="TrackBar"/>.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Tick marks are displayed on both sides of the track.
        /// </summary>
        Both
    }
}
