namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Interface that indicates the Control can render a glyph like a check box or radio button.
    /// </summary>
    interface IRenderGlyph
    {
        /// <summary>
        /// Size of the checkbox glyph.
        /// </summary>
        int GlyphSize { get; }

        /// <summary>
        /// Padding between glyph and text.
        /// </summary>
        int GlyphTextPadding { get; }
    }
}
