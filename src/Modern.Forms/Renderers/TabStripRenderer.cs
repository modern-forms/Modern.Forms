using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class TabStripRenderer : Renderer<TabStrip>
    {
        protected override void Render (TabStrip control, PaintEventArgs e)
        {
            foreach (var item in control.Tabs)
                RenderItem (control, item, e);
        }

        protected virtual void RenderItem (TabStrip control, TabStripItem item, PaintEventArgs e)
        {
            if (item.Selected)
                e.Canvas.FillRectangle (item.Bounds, Theme.NeutralGray);
            else if (item.Hovered && item.Enabled)
                e.Canvas.FillRectangle (item.Bounds, Theme.RibbonTabHighlightColor);

            var font_color = !item.Enabled ? Theme.DisabledTextColor:
                             item.Selected ? Theme.RibbonColor 
                                           : Theme.LightTextColor;

            var font_size = e.LogicalToDeviceUnits (Theme.FontSize);

            e.Canvas.DrawText (item.Text, Theme.UIFont, font_size, item.Bounds, font_color, ContentAlignment.MiddleCenter);
        }
    }
}
