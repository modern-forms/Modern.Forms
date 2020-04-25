using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class ToolBarRenderer : Renderer<ToolBar>
    {
        protected override void Render (ToolBar control, PaintEventArgs e)
        {
            foreach (var item in control.Items)
                if (item is MenuSeparatorItem msi)
                    RenderMenuSeparatorItem (control, msi, e);
                else
                    RenderItem (control, item, e);
        }

        protected virtual void RenderItem (ToolBar control, MenuItem item, PaintEventArgs e)
        {
            // Background
            var background_color = item.Hovered || item.IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;
            e.Canvas.FillRectangle (item.Bounds, background_color);

            var bounds = item.Bounds;
            bounds.X += e.LogicalToDeviceUnits (8);

            // Image
            if (item.Image != null) {
                var image_size = e.LogicalToDeviceUnits (20);
                var image_bounds = DrawingExtensions.CenterSquare (item.Bounds, image_size);
                var image_rect = new Rectangle (bounds.Left, image_bounds.Top, image_size, image_size);
                e.Canvas.DrawBitmap (item.Image, image_rect, !item.Enabled);

                bounds.X += e.LogicalToDeviceUnits (28);
            } else {
                bounds.X += e.LogicalToDeviceUnits (4);
            }

            // Text
            var font_color = item.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor;
            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);

            bounds.Y += 1;
            e.Canvas.DrawText (item.Text, Theme.UIFont, font_size, bounds, font_color, ContentAlignment.MiddleLeft);
            bounds.Y -= 1;

            // Dropdown Arrow
            if (item.HasItems) {
                var arrow_bounds = DrawingExtensions.CenterSquare (item.Bounds, 16);
                var arrow_area = new Rectangle (item.Bounds.Right - e.LogicalToDeviceUnits (16) - 4, arrow_bounds.Top, 16, 16);
                ControlPaint.DrawArrowGlyph (e, arrow_area, font_color, ArrowDirection.Down);
            }
        }

        protected virtual void RenderMenuSeparatorItem (ToolBar control, MenuSeparatorItem item, PaintEventArgs e)
        {
            // Background
            e.Canvas.FillRectangle (item.Bounds, Theme.NeutralGray);

            var center = item.Bounds.GetCenter ();
            var thickness = e.LogicalToDeviceUnits (1);
            var padding = e.LogicalToDeviceUnits (item.Padding);

            e.Canvas.DrawLine (center.X, item.Bounds.Top + padding.Top + thickness, center.X, item.Bounds.Bottom - padding.Bottom - thickness, item.Enabled ? Theme.RibbonItemHighlightColor : Theme.DisabledTextColor, thickness);
        }

        public virtual Size GetPreferredItemSize (ToolBar control, MenuItem item, Size proposedSize)
        {
            if (item is MenuSeparatorItem msi)
                return GetPreferredSeparatorItemSize (control, msi, proposedSize);

            var width = control.LogicalToDeviceUnits (item.Padding.Horizontal);
            var font_size = control.LogicalToDeviceUnits (Theme.FontSize);
            width += (int)Math.Round (TextMeasurer.MeasureText (item.Text, Theme.UIFont, font_size).Width);

            if (!(item.Image is null))
                width += control.LogicalToDeviceUnits (20);

            if (item.HasItems)
                width += control.LogicalToDeviceUnits (14);

            return new Size (width, item.Bounds.Height);
        }

        protected virtual Size GetPreferredSeparatorItemSize (ToolBar control, MenuSeparatorItem item, Size proposedSize)
        {
            var padding = control.LogicalToDeviceUnits (item.Padding.Horizontal);
            var thickness = control.LogicalToDeviceUnits (1);

            return new Size (thickness + padding, item.Bounds.Height);
        }
    }
}
