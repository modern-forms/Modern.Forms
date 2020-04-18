using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class MenuDropDownRenderer : Renderer<MenuDropDown>
    {
        protected override void Render (MenuDropDown control, PaintEventArgs e)
        {
            foreach (var item in control.Items)
                if (item is MenuSeparatorItem msi)
                    RenderMenuSeparatorItem (control, msi, e);
                else
                    RenderItem (control, item, e);
        }

        protected virtual void RenderItem (MenuDropDown control, MenuItem item, PaintEventArgs e)
        {
            // Background
            var background_color = item.Hovered || item.IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.LightTextColor;
            e.Canvas.FillRectangle (item.Bounds, background_color);

            // Image
            if (item.Image != null) {
                var image_size = e.LogicalToDeviceUnits (16);
                var image_bounds = DrawingExtensions.CenterSquare (item.Bounds, image_size);
                var image_rect = new Rectangle (item.Bounds.Left + e.LogicalToDeviceUnits (6), image_bounds.Top, image_size, image_size);
                e.Canvas.DrawBitmap (item.Image, image_rect);
            }

            // Text
            var font_color = item.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor;
            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);
            var bounds = item.Bounds;
            bounds.X += e.LogicalToDeviceUnits (28);
            e.Canvas.DrawText (item.Text, Theme.UIFont, font_size, bounds, font_color, ContentAlignment.MiddleLeft);

            // Dropdown Arrow
            if (item.HasItems) {
                var arrow_bounds = DrawingExtensions.CenterSquare (item.Bounds, 16);
                var arrow_area = new Rectangle (item.Bounds.Right - e.LogicalToDeviceUnits (16) - 4, arrow_bounds.Top, 16, 16);
                ControlPaint.DrawArrowGlyph (e, arrow_area, font_color, ArrowDirection.Right);
            }
        }

        protected virtual void RenderMenuSeparatorItem (MenuDropDown control, MenuSeparatorItem item, PaintEventArgs e)
        {
            // Background
            e.Canvas.FillRectangle (item.Bounds, Theme.LightTextColor);

            var center = item.Bounds.GetCenter ();
            var thickness = e.LogicalToDeviceUnits (1);
            var padding = e.LogicalToDeviceUnits (item.Padding);

            e.Canvas.DrawLine (item.Bounds.X + padding.Top, center.Y, item.Bounds.Right - padding.Right, center.Y, item.Enabled ? Theme.RibbonItemHighlightColor : Theme.DisabledTextColor, thickness);
        }

        public virtual Size GetPreferredItemSize (MenuDropDown control, MenuItem item, Size proposedSize)
        {
            if (item is MenuSeparatorItem msi)
                return GetPreferredSeparatorItemSize (control, msi, proposedSize);

            var padding = control.LogicalToDeviceUnits (item.Padding);
            var font_size = control.LogicalToDeviceUnits (Theme.FontSize);
            var text_size = TextMeasurer.MeasureText (item.Text, Theme.UIFont, font_size);

            return new Size ((int)Math.Round (text_size.Width, 0, MidpointRounding.AwayFromZero) + padding.Horizontal + control.LogicalToDeviceUnits (70), (int)Math.Round (text_size.Height, 0, MidpointRounding.AwayFromZero) + control.LogicalToDeviceUnits (8));
        }

        protected virtual Size GetPreferredSeparatorItemSize (MenuDropDown control, MenuSeparatorItem item, Size proposedSize)
        {
            var padding = control.LogicalToDeviceUnits (item.Padding.Vertical);
            var thickness = control.LogicalToDeviceUnits (1);

            return new Size (item.Bounds.Width, thickness + padding);
        }
    }
}
