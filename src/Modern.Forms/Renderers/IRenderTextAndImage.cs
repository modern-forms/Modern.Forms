namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Interface that indicates the Control can render aligned text and an image.
    /// </summary>
    interface IRenderTextAndImage
    {
        /// <summary>
        /// Margin between image and text.
        /// </summary>
        int ImageTextMargin { get; }
    }
}
