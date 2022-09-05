using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a TabStrip.
    /// </summary>
    public class TabStripRenderer : Renderer<TabStrip>
    {
        /// <inheritdoc/>
        protected override void Render (TabStrip control, PaintEventArgs e)
        {
            foreach (var item in control.Tabs)
                RenderItem (control, item, e);
        }

        /// <summary>
        /// Renders a TabStripItem.
        /// </summary>
        protected virtual void RenderItem (TabStrip control, TabStripItem item, PaintEventArgs e)
        {
            // Hover background
            if (item.Hovered && item.Enabled)
                e.Canvas.FillRectangle (item.Bounds, Theme.LightNeutralGray);

            // Draw focus rectangle
            if (control.Selected && control.ShowFocusCues && control.Tabs.FocusedIndex == control.Tabs.IndexOf (item))
                e.Canvas.DrawFocusRectangle (item.Bounds, e.LogicalToDeviceUnits (1));

            var font_color = !item.Enabled ? Theme.DisabledTextColor : Theme.PrimaryTextColor;
            var font = item.Selected || item.Hovered ? Theme.UIFontBold : Theme.UIFont;
            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);

            e.Canvas.DrawText (item.Text, font, font_size, item.Bounds, font_color, ContentAlignment.MiddleCenter);

            if (item.Selected) {
                var highlight_padding = e.LogicalToDeviceUnits (10);
                var highlight_height = e.LogicalToDeviceUnits (3);
                var highlight_bounds = new Rectangle (item.Bounds.Left + highlight_padding, item.Bounds.Bottom - highlight_height, item.Bounds.Width - (2 * highlight_padding), highlight_height);
                
                e.Canvas.FillRectangle (highlight_bounds, Theme.PrimaryColor);
            }
        }
    }
}
