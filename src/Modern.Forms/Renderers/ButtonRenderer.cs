using System;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a Button.
    /// </summary>
    public class ButtonRenderer : Renderer<Button>
    {
        /// <inheritdoc/>
        protected override void Render (Button control, PaintEventArgs e)
        {
            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, control.TextAlign);
        }
    }
}
