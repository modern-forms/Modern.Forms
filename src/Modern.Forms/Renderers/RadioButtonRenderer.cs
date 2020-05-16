using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a RadioButton.
    /// </summary>
    public class RadioButtonRenderer : Renderer<RadioButton>
    {
        /// <inheritdoc/>
        protected override void Render (RadioButton control, PaintEventArgs e)
        {
            var x = e.LogicalToDeviceUnits (11);
            var y = control.ScaledHeight / 2;

            ControlPaint.DrawRadioButton (e, new Point (x, y), control.Checked ? CheckState.Checked : CheckState.Unchecked, !control.Enabled);

            var text_bounds = control.ClientRectangle;
            var unit_24 = e.LogicalToDeviceUnits (24);
            var unit_5 = e.LogicalToDeviceUnits (5);

            text_bounds.X += unit_24;
            text_bounds.Width -= unit_24;

            if (control.Selected && control.ShowFocusCues) {
                var focus_bounds = new Rectangle (text_bounds.X - unit_5, 0, text_bounds.Width + unit_5, text_bounds.Height);
                e.Canvas.DrawFocusRectangle (focus_bounds, e.LogicalToDeviceUnits (3));
            }

            e.Canvas.DrawText (control.Text, text_bounds, control, ContentAlignment.MiddleLeft);
        }
    }
}
