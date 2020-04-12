using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class RadioButtonRenderer : Renderer<RadioButton>
    {
        protected override void Render (RadioButton control, PaintEventArgs e)
        {
            var x = e.LogicalToDeviceUnits (11);
            var y = control.ScaledHeight / 2;

            ControlPaint.DrawRadioButton (e, new Point (x, y), control.Checked ? CheckState.Checked : CheckState.Unchecked, !control.Enabled);

            var text_bounds = control.ClientRectangle;
            var unit_24 = e.LogicalToDeviceUnits (24);

            text_bounds.X += unit_24;
            text_bounds.Width -= unit_24;

            e.Canvas.DrawText (control.Text, text_bounds, control, ContentAlignment.MiddleLeft);
        }
    }
}
