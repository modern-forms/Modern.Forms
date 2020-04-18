using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class MenuRenderer : Renderer<Menu>
    {
        protected override void Render (Menu control, PaintEventArgs e)
        {
            foreach (var item in control.Items)
                if (item is MenuSeparatorItem msi)
                    RenderMenuSeparatorItem (control, msi, e);
                else
                    RenderItem (control, item, e);
        }

        protected virtual void RenderItem (Menu control, MenuItem item, PaintEventArgs e)
        {
            // Background
            var background_color = item.Hovered || item.IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;
            e.Canvas.FillRectangle (item.Bounds, background_color);

            // Text
            var font_color = item.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor;
            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);

            e.Canvas.DrawText (item.Text, Theme.UIFont, font_size, item.Bounds, font_color, ContentAlignment.MiddleCenter);
        }

        protected virtual void RenderMenuSeparatorItem (Menu control, MenuSeparatorItem item, PaintEventArgs e)
        {
            // Background
            e.Canvas.FillRectangle (item.Bounds, Theme.NeutralGray);

            var center = item.Bounds.GetCenter ();
            var thickness = e.LogicalToDeviceUnits (1);
            var padding = e.LogicalToDeviceUnits (item.Padding);

            e.Canvas.DrawLine (center.X, item.Bounds.Top + padding.Top, center.X, item.Bounds.Bottom - padding.Bottom, item.Enabled ? Theme.RibbonItemHighlightColor : Theme.DisabledTextColor, thickness);
        }

        public virtual Size GetPreferredItemSize (Menu control, MenuItem item, Size proposedSize)
        {
            if (item is MenuSeparatorItem msi)
                return GetPreferredSeparatorItemSize (control, msi, proposedSize);

            var padding = control.LogicalToDeviceUnits (item.Padding.Horizontal);
            var font_size = control.LogicalToDeviceUnits (Theme.FontSize);
            var text_size = (int)Math.Round (TextMeasurer.MeasureText (item.Text, Theme.UIFont, font_size).Width);

            return new Size (text_size + padding, item.Bounds.Height);
        }

        protected virtual Size GetPreferredSeparatorItemSize (Menu control, MenuSeparatorItem item, Size proposedSize)
        {
            var padding = control.LogicalToDeviceUnits (item.Padding.Horizontal);
            var thickness = control.LogicalToDeviceUnits (1);

            return new Size (thickness + padding, item.Bounds.Height);
        }
    }
}
