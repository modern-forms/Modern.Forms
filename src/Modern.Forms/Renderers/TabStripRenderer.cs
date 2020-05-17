using System;

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
            if (item.Selected)
                e.Canvas.FillRectangle (item.Bounds, Theme.NeutralGray);
            else if (item.Hovered && item.Enabled)
                e.Canvas.FillRectangle (item.Bounds, Theme.HighlightColor);

            // Draw focus rectangle
            if (control.Selected && control.ShowFocusCues && control.Tabs.FocusedIndex == control.Tabs.IndexOf (item))
                e.Canvas.DrawFocusRectangle (item.Bounds, e.LogicalToDeviceUnits (1));

            var font_color = !item.Enabled ? Theme.DisabledTextColor:
                             item.Selected ? Theme.PrimaryColor 
                                           : Theme.LightTextColor;

            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);

            e.Canvas.DrawText (item.Text, Theme.UIFont, font_size, item.Bounds, font_color, ContentAlignment.MiddleCenter);
        }
    }
}
