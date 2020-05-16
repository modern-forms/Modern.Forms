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
            if (control.Selected && control.ShowFocusCues)
                e.Canvas.DrawFocusRectangle (control.ClientRectangle, e.LogicalToDeviceUnits (3));

            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, control.TextAlign);
        }
    }
}
