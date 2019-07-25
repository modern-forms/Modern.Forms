using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public static class ControlPaint
    {
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

        public static void DrawCheckBox (PaintEventArgs e, Rectangle rectangle, CheckState state)
        {
            var border_color = state == CheckState.Checked ? Theme.RibbonColor : Theme.BorderGray;
            e.Canvas.DrawRectangle (rectangle, border_color, e.LogicalToDeviceUnits (1));

            if (state == CheckState.Checked) {
                var fill_bounds = new Rectangle (rectangle.Left + 1 + e.LogicalToDeviceUnits (2), rectangle.Top + 1 + e.LogicalToDeviceUnits (2), rectangle.Width - e.LogicalToDeviceUnits (5), rectangle.Height - e.LogicalToDeviceUnits (5));
                e.Canvas.FillRectangle (fill_bounds, Theme.RibbonColor);
            }
        }

        public static void DrawCloseGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawLine (rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom, Theme.LightTextColor);
            e.Canvas.DrawLine (rectangle.X, rectangle.Bottom, rectangle.Right, rectangle.Y, Theme.LightTextColor);
        }

        public static void DrawMaximizeGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawRectangle (rectangle, Theme.LightTextColor);
        }

        public static void DrawMinimizeGlyph (PaintEventArgs e, Rectangle rectangle)
        {
            e.Canvas.DrawLine (rectangle.X, rectangle.Y, rectangle.Right, rectangle.Y, Theme.LightTextColor);
        }

        public static void DrawRadioButton (PaintEventArgs e, Point origin, CheckState state)
        {
            var outer_radius = e.LogicalToDeviceUnits (8);
            var inner_radius = e.LogicalToDeviceUnits (5);
            var border_color = state == CheckState.Checked ? Theme.RibbonColor : Theme.BorderGray;

            e.Canvas.DrawCircle (origin.X, origin.Y, outer_radius, border_color, e.LogicalToDeviceUnits (1));

            if (state == CheckState.Checked)
                e.Canvas.FillCircle (origin.X, origin.Y, inner_radius, Theme.RibbonColor);
        }
    }
}
