using Modern.Forms.Layout;
using SkiaSharp;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a Button.
    /// </summary>
    public class ButtonRenderer : Renderer<Button>, IRenderTextAndImage
    {
        /// <inheritdoc/>
        public int ImageTextMargin { get; } = 4;

        /// <inheritdoc/>
        protected override void Render (Button control, PaintEventArgs e)
        {
            var layout = TextImageLayoutEngine.Layout (control);

            // Draw the image
            if ((control as IHaveTextAndImageAlign).GetImage () is SKBitmap image)
                e.Canvas.DrawBitmap (image, layout.ImageBounds, !control.Enabled);

            // Draw the focus rectangle
            if (control.Selected && control.ShowFocusCues)
                e.Canvas.DrawFocusRectangle (layout.Focus, 0);

            // Draw the text
            if (control.Text.HasValue ())
                e.Canvas.DrawText (control.Text, layout.TextBounds, control, control.TextAlign, maxLines: 1, ellipsis: control.AutoEllipsis);
        }
    }
}
