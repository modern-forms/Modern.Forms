using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class CheckBoxRenderer : Renderer<CheckBox>
    {
        protected const int GLYPH_SIZE = 15;          // Size of the checkbox glyph
        protected const int GLYPH_TEXT_PADDING = 4;   // Padding between glyph and text

        protected override void Render (CheckBox control, PaintEventArgs e)
        {
            var box_size = e.LogicalToDeviceUnits (GLYPH_SIZE);
            var glyph_padding = e.LogicalToDeviceUnits (GLYPH_TEXT_PADDING);

            // Draw the checkbox glyph
            var y = (control.ScaledHeight - box_size) / 2;
            var box_bounds = new Rectangle (e.LogicalToDeviceUnits (3), y, box_size, box_size);

            ControlPaint.DrawCheckBox (e, box_bounds, control.CheckState, !control.Enabled);

            // Draw the text
            if (control.Text.HasValue ()) {
                var text_bounds = new Rectangle (box_bounds.Right + glyph_padding, 0, control.ScaledWidth - box_bounds.Right - glyph_padding, control.ScaledHeight);
                e.Canvas.DrawText (control.Text, text_bounds, control, ContentAlignment.MiddleLeft);
            }
        }
    }
}
