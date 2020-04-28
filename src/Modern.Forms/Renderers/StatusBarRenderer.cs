using System;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a StatusBar.
    /// </summary>
    public class StatusBarRenderer : Renderer<StatusBar>
    {
        /// <inheritdoc/>
        protected override void Render (StatusBar control, PaintEventArgs e)
        {
            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, ContentAlignment.MiddleLeft, maxLines: 1);
        }
    }
}
