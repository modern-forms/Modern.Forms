using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a ComboBox.
    /// </summary>
    public class ComboBoxRenderer : Renderer<ComboBox>
    {
        /// <summary>
        /// Size of the drop down glyph.
        /// </summary>
        protected const int GLYPH_SIZE = 15;

        /// <inheritdoc/>
        protected override void Render (ComboBox control, PaintEventArgs e)
        {
            var text_area = GetTextArea (control, e);
            var unit_3 = e.LogicalToDeviceUnits (3);

            if (control.Selected && control.ShowFocusCues) {
                var focus_bounds = new Rectangle (text_area.Left - unit_3, text_area.Top, text_area.Width + unit_3, text_area.Height);
                e.Canvas.DrawFocusRectangle (focus_bounds, unit_3);
            }

            // Draw the text of the selected item
            if (control.Items.SelectedItem != null)
                e.Canvas.DrawText (control.Items.SelectedItem.ToString ()!, text_area, control, ContentAlignment.MiddleLeft, maxLines: 1);

            // Draw the drop down glyph
            var button_bounds = GetDropDownButtonArea (control, e);

            ControlPaint.DrawArrowGlyph (e, button_bounds, control.Enabled ? Theme.PrimaryTextColor : Theme.DisabledTextColor, ArrowDirection.Down);
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
