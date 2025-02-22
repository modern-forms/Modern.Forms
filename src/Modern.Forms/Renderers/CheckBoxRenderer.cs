﻿using Modern.Forms.Layout;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a CheckBox.
    /// </summary>
    public class CheckBoxRenderer : Renderer<CheckBox>, IRenderGlyph
    {
        /// <inheritdoc/>
        public int GlyphSize { get; } = 15;

        /// <inheritdoc/>
        public int GlyphTextPadding { get; } = 5;

        /// <inheritdoc/>
        protected override void Render (CheckBox control, PaintEventArgs e)
        {
            var layout = TextImageLayoutEngine.Layout (control);

            ControlPaint.DrawCheckBox (e, layout.GlyphBounds, control.CheckState, !control.Enabled);

            // Draw the image
            if (control.Image is not null)
                e.Canvas.DrawBitmap (control.Image, layout.ImageBounds, !control.Enabled);

            // Draw the focus rectangle
            if (control.Selected && control.ShowFocusCues) {
                //var focus_bounds = new Rectangle (box_bounds.Right, 0, text_bounds.Width + glyph_padding, text_bounds.Height);
                e.Canvas.DrawFocusRectangle (layout.TextBounds, e.LogicalToDeviceUnits (-3));
            }

            // Draw the text
            if (control.Text.HasValue ())
                e.Canvas.DrawText (control.Text, layout.TextBounds, control, control.TextAlign, maxLines: 1, ellipsis: control.AutoEllipsis);
        }
    }
}
