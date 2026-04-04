namespace Modern.Forms
{
    /// <summary>
    /// Specifies how link text should be underlined in a <see cref="LinkLabel"/>.
    /// </summary>
    public enum LinkBehavior
    {
        /// <summary>
        /// Uses the framework default behavior.
        /// </summary>
        /// <remarks>
        /// In this implementation, <see cref="SystemDefault"/> behaves the same as
        /// <see cref="AlwaysUnderline"/>.
        /// </remarks>
        SystemDefault,

        /// <summary>
        /// Link text is always underlined.
        /// </summary>
        AlwaysUnderline,

        /// <summary>
        /// Link text is underlined only while the pointer hovers over it.
        /// </summary>
        HoverUnderline,

        /// <summary>
        /// Link text is never underlined.
        /// </summary>
        NeverUnderline
    }
}
