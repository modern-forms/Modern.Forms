using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class ComboBoxRenderer : Renderer<ComboBox>
    {
        protected const int GLYPH_SIZE = 15;    // Size of the drop down glyph

        public override void Render (ComboBox control, PaintEventArgs e)
        {
            // Draw the text of the selected item
            if (control.Items.SelectedItem != null)
                e.Canvas.DrawText (control.Items.SelectedItem.ToString (), GetTextArea (control, e), control, ContentAlignment.MiddleLeft, maxLines: 1);

            // Draw the drop down glyph
            var button_bounds = GetDropDownButtonArea (control, e);

            ControlPaint.DrawArrowGlyph (e, button_bounds, control.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor, ArrowDirection.Down);
        }

        /// <summary>
        /// Gets the bounding box of the area to draw the drop down glyph.
        /// </summary>
        protected Rectangle GetDropDownButtonArea (ComboBox control, PaintEventArgs e)
        {
            var glyph_size = e.LogicalToDeviceUnits (GLYPH_SIZE);

            return new Rectangle (control.ScaledWidth - glyph_size, 0, glyph_size - e.LogicalToDeviceUnits (control.Padding.Right), control.ScaledHeight);
        }

        /// <summary>
        /// Gets the bounding box of the area to draw the ComboBox text.
        /// </summary>
        protected Rectangle GetTextArea (ComboBox control, PaintEventArgs e)
        {
            var area = control.PaddedClientRectangle;

            area.Width -= e.LogicalToDeviceUnits (GLYPH_SIZE);

            return area;
        }
    }
}
