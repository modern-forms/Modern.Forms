using System;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a ProgressBar.
    /// </summary>
    public class ProgressBarRenderer : Renderer<ProgressBar>
    {
        /// <inheritdoc/>
        protected override void Render (ProgressBar control, PaintEventArgs e)
        {
            // Prevent divide by zero
            if (control.Maximum == control.Minimum)
                return;

            var percent = (float)(control.Value - control.Minimum) / (control.Maximum - control.Minimum);
            var client_area = control.PaddedClientRectangle;
            var filled_pixels = (int)(percent * client_area.Width);

            if (filled_pixels > 0)
                e.Canvas.FillRectangle (client_area.X, client_area.Y, filled_pixels, client_area.Height, control.Enabled ? Theme.AccentColor2 : Theme.ForegroundDisabledColor);
        }
    }
}
