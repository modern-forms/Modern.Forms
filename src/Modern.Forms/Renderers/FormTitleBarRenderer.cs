using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class FormTitleBarRenderer : Renderer<FormTitleBar>
    {
        protected const int BUTTON_PADDING = 10;
        protected const int BUTTON_SIZE = 46;
        protected const int FORM_ICON_SIZE = 16;

        protected override void Render (FormTitleBar control, PaintEventArgs e)
        {
            // Form icon
            if (control.Image != null) {
                var icon_glyph_bounds = DrawingExtensions.CenterSquare (GetIconBounds (control), e.LogicalToDeviceUnits (FORM_ICON_SIZE));

                e.Canvas.DrawBitmap (control.Image, icon_glyph_bounds);
            }

            // Form text
            e.Canvas.DrawText (control.Text.Trim (), Theme.UIFont, e.LogicalToDeviceUnits (Theme.FontSize), GetTitleBounds (control), Theme.LightTextColor, ContentAlignment.MiddleCenter);

            // Minimize button
            if (control.AllowMinimize) {
                var minimize_button_bounds = GetMinimizeButtonBounds (control);

                if (control.HoverElement == FormTitleBar.FormTitleBarElement.Minimize)
                    e.Canvas.FillRectangle (minimize_button_bounds, Theme.RibbonTabHighlightColor);

                var min_glyph_bounds = DrawingExtensions.CenterRectangle (minimize_button_bounds, e.LogicalToDeviceUnits (new Size (BUTTON_PADDING, 1)));
                ControlPaint.DrawMinimizeGlyph (e, min_glyph_bounds);
            }

            // Maximize button
            if (control.AllowMaximize) {
                var maximize_button_bounds = GetMaximizeButtonBounds (control);

                if (control.HoverElement == FormTitleBar.FormTitleBarElement.Maximize)
                    e.Canvas.FillRectangle (maximize_button_bounds, Theme.RibbonTabHighlightColor);

                var max_glyph_bounds = DrawingExtensions.CenterSquare (maximize_button_bounds, e.LogicalToDeviceUnits (BUTTON_PADDING));
                ControlPaint.DrawMaximizeGlyph (e, max_glyph_bounds);
            }

            // Close button
            var close_button_bounds = GetCloseButtonBounds (control);

            if (control.HoverElement == FormTitleBar.FormTitleBarElement.Close)
                e.Canvas.FillRectangle (close_button_bounds, Theme.FormCloseHighlightColor);

            var close_glyph_bounds = DrawingExtensions.CenterSquare (close_button_bounds, e.LogicalToDeviceUnits (BUTTON_PADDING));
            ControlPaint.DrawCloseGlyph (e, close_glyph_bounds);

        }

        public virtual Rectangle GetCloseButtonBounds (FormTitleBar control)
        {
            var button_width = GetScaledButtonWidth (control);

            return new Rectangle (control.ScaledWidth - button_width, 0, button_width, control.ScaledHeight);
        }

        public virtual Rectangle GetIconBounds (FormTitleBar control) => new Rectangle (0, 0, control.ScaledHeight, control.ScaledHeight);

        public virtual Rectangle GetMaximizeButtonBounds (FormTitleBar control)
        {
            var button_width = GetScaledButtonWidth (control);

            return control.AllowMaximize ? new Rectangle (control.ScaledWidth - (button_width * 2), 0, button_width, control.ScaledHeight) : Rectangle.Empty;
        }

        public virtual Rectangle GetMinimizeButtonBounds (FormTitleBar control)
        {
            var button_width = GetScaledButtonWidth (control);

            if (control.AllowMinimize && control.AllowMaximize)
                return new Rectangle (control.ScaledWidth - (button_width * 3), 0, button_width, control.ScaledHeight);

            if (control.AllowMinimize)
                return new Rectangle (control.ScaledWidth - (button_width * 2), 0, button_width, control.ScaledHeight);

            return Rectangle.Empty;
        }

        public virtual int GetScaledButtonWidth (FormTitleBar control) => control.LogicalToDeviceUnits (BUTTON_SIZE);

        public virtual Rectangle GetTitleBounds (FormTitleBar control)
        {
            var x = control.Image == null ? 0 : GetIconBounds (control).Right;
            var right = control.AllowMinimize ? GetMinimizeButtonBounds (control).Left : control.AllowMaximize ? GetMaximizeButtonBounds (control).Left : GetCloseButtonBounds (control).Left;

            return Rectangle.FromLTRB (x, control.Top, right, control.ScaledBounds.Bottom);
        }
    }
}
