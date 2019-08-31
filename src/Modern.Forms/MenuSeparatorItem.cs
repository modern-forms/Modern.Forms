using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class MenuSeparatorItem : MenuItem
    {
        public MenuSeparatorItem ()
        {
            Padding = new Padding (3);
        }

        public override Size GetPreferredSize (Size proposedSize)
        {
            if (OwnerControl is Menu menu) {
                var padding = menu.LogicalToDeviceUnits (Padding.Horizontal);
                var thickness = menu.LogicalToDeviceUnits (1);

                return new Size (thickness + padding, Bounds.Height);
            }

            if (OwnerControl is ToolBar tb) {
                var padding = tb.LogicalToDeviceUnits (Padding.Horizontal);
                var thickness = tb.LogicalToDeviceUnits (1);

                return new Size (thickness + padding, Bounds.Height);
            }

            if (OwnerControl is MenuDropDown dropdown) {
                var padding = dropdown.LogicalToDeviceUnits (Padding.Vertical);
                var thickness = dropdown.LogicalToDeviceUnits (1);

                return new Size (Bounds.Width, thickness + padding);
            }

            return proposedSize;
        }

        public override void OnPaint (SKCanvas canvas)
        {
            if (OwnerControl is Menu menu) {
                // Background
                canvas.FillRectangle (Bounds, Theme.NeutralGray);

                var center = Bounds.GetCenter ();
                var thickness = menu.LogicalToDeviceUnits (1);
                var padding = menu.LogicalToDeviceUnits (Padding);

                canvas.DrawLine (center.X, Bounds.Top + padding.Top, center.X, Bounds.Bottom - padding.Bottom, Theme.RibbonItemHighlightColor, thickness);

                return;
            }

            if (OwnerControl is ToolBar tb) {
                // Background
                canvas.FillRectangle (Bounds, Theme.NeutralGray);

                var center = Bounds.GetCenter ();
                var thickness = tb.LogicalToDeviceUnits (1);
                var padding = tb.LogicalToDeviceUnits (Padding);

                canvas.DrawLine (center.X, Bounds.Top + padding.Top + thickness, center.X, Bounds.Bottom - padding.Bottom - thickness, Theme.RibbonItemHighlightColor, thickness);

                return;
            }

            if (OwnerControl is MenuDropDown dropdown) {
                // Background
                canvas.FillRectangle (Bounds, Theme.LightTextColor);

                var center = Bounds.GetCenter ();
                var thickness = dropdown.LogicalToDeviceUnits (1);
                var padding = dropdown.LogicalToDeviceUnits (Padding);

                canvas.DrawLine (Bounds.X + padding.Top, center.Y, Bounds.Right - padding.Right, center.Y, Theme.RibbonItemHighlightColor, thickness);

                return;
            }
        }
    }
}
