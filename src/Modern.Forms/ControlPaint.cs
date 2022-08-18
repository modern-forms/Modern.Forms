using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Contains methods for drawing elements of controls.
    /// </summary>
    public static class ControlPaint
    {
        /// <summary>
        /// Draws an arrow glyph, as seen on ComboBoxes and TreeView dropdowns.
        /// </summary>
        public static void DrawArrowGlyph (PaintEventArgs e, Rectangle rectangle, SKColor color, ArrowDirection direction)
        {
            var lines = e.LogicalToDeviceUnits (4);

            switch (direction) {
                case ArrowDirection.Left: {
                        var y = rectangle.Y + (rectangle.Height / 2);
                        var x = rectangle.X + (rectangle.Width / 2) - e.LogicalToDeviceUnits (2);

                        for (var i = 0; i < lines; i++)
                            e.Canvas.DrawLine (x + i, y - i, x + i, y + i + 1, color);

                        break;
                    }
                case ArrowDirection.Up: {
                        var y = rectangle.Y + (rectangle.Height / 2) - e.LogicalToDeviceUnits (2);
                        var x = rectangle.X + (rectangle.Width / 2);

                        for (var i = 0; i < lines; i++)
                            e.Canvas.DrawLine (x - i, y + i, x + i + 1, y + i, color);

                        break;
                    }
                case ArrowDirection.Right: {
                        var y = rectangle.Y + (rectangle.Height / 2);
                        var x = rectangle.X + (rectangle.Width / 2) - e.LogicalToDeviceUnits (1);

                        for (var i = 0; i < lines; i++)
                            e.Canvas.DrawLine (x + i, y - (lines - 1 - i), x + i, y + lines - i, color);

                        break;
                    }
                case ArrowDirection.Down: {
                        var y = rectangle.Y + (rectangle.Height / 2) - e.LogicalToDeviceUnits (1);
                        var x = rectangle.X + (rectangle.Width / 2);

                        for (var i = 0; i < lines; i++)
                            e.Canvas.DrawLine (x - (lines - 1 - i), y + i, x + lines - i, y + i, color);

                        break;
                    }
            }
        }

        /// <summary>
        /// Draws a CheckBox glyph.
        /// </summary>
        public static void DrawCheckBox (PaintEventArgs e, Rectangle rectangle, CheckState state, bool disabled = false)
        {
            var color = disabled ? Theme.DisabledTextColor
                            : state == CheckState.Checked && !disabled ? Theme.PrimaryColor
                            : Theme.BorderGray;
            var unit_1 = e.LogicalToDeviceUnits (1);

            // Draw the border
            e.Canvas.DrawRectangle (rectangle, color, unit_1);

            // Draw the checked glyph if needed
            if (state == CheckState.Checked) {
                var unit_2 = e.LogicalToDeviceUnits (2);
                var unit_5 = e.LogicalToDeviceUnits (5);
                var fill_bounds = new Rectangle (rectangle.Left + 1 + unit_2, rectangle.Top + 1 + unit_2, rectangle.Width - unit_5, rectangle.Height - unit_5);

                e.Canvas.FillRectangle (fill_bounds, color);
            }

            // Draw the indeterminate glyph if needed
            if (state == CheckState.Indeterminate) {
                var unit_2 = e.LogicalToDeviceUnits (2);
                var unit_5 = e.LogicalToDeviceUnits (5);
                var center_y = rectangle.GetCenter ().Y;

                var fill_bounds = new Rectangle (rectangle.Left + 1 + unit_2, center_y, rectangle.Width - unit_5, unit_1 + unit_2);

                e.Canvas.FillRectangle (fill_bounds, color);
            }
        }

        /// <summary>
        /// Draws a close glyph, as seen on FormTitleBar.
        /// </summary>
        public static void DrawCloseGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawLine (rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom, Theme.LightTextColor);
            e.Canvas.DrawLine (rectangle.X, rectangle.Bottom, rectangle.Right, rectangle.Y, Theme.LightTextColor);
        }

        /// <summary>
        /// Draws a maximize glyph, as seen on FormTitleBar.
        /// </summary>
        public static void DrawMaximizeGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawRectangle (rectangle, Theme.LightTextColor);
        }

        /// <summary>
        /// Draws a minimize glyph, as seen on FormTitleBar.
        /// </summary>
        public static void DrawMinimizeGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawLine (rectangle.X, rectangle.Y, rectangle.Right, rectangle.Y, Theme.LightTextColor);
        }


        /// <summary>
        /// Draws a RadioButton glyph.
        /// </summary>
        public static void DrawRadioButton (PaintEventArgs e, Point origin, CheckState state, bool disabled = false)
        {
            var outer_radius = e.LogicalToDeviceUnits (8);
            var inner_radius = e.LogicalToDeviceUnits (5);
            var border_color = disabled ? Theme.DisabledTextColor :
                               state == CheckState.Checked ? Theme.PrimaryColor :
                               Theme.BorderGray;

            e.Canvas.DrawCircle (origin.X, origin.Y, outer_radius, border_color, e.LogicalToDeviceUnits (1));

            if (state == CheckState.Checked)
                e.Canvas.FillCircle (origin.X, origin.Y, inner_radius, disabled ? Theme.DisabledTextColor : Theme.PrimaryColor);
        }
    }
}
