using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class RibbonRenderer : Renderer<Ribbon>
    {
        protected const int IMAGE_SIZE = 32;
        protected const int MINIMUM_ITEM_SIZE = 40;

        protected override void Render (Ribbon control, PaintEventArgs e)
        {
            var selected_tab = control.SelectedTabPage;

            if (selected_tab != null)
                RenderTabPage (control, selected_tab, e);
        }

        protected virtual void RenderTabPage (Ribbon control, RibbonTabPage tabPage, PaintEventArgs e)
        {
            LayoutTabPage (tabPage);

            e.Canvas.FillRectangle (tabPage.Bounds, Theme.NeutralGray);

            foreach (var group in tabPage.Groups)
                RenderTabPageGroup (control, tabPage, group, e);
        }

        protected virtual void RenderTabPageGroup (Ribbon control, RibbonTabPage tabPage, RibbonItemGroup group, PaintEventArgs e)
        {
            // Draw each ribbon item
            foreach (var item in group.Items)
                RenderItem (control, tabPage, group, item, e);

            // Right border (group separator)
            e.Canvas.DrawLine (group.Bounds.Right - 1, group.Bounds.Top + 4, group.Bounds.Right - 1, group.Bounds.Bottom - 4, Theme.BorderGray);
        }

        protected virtual void RenderItem (Ribbon control, RibbonTabPage tabPage, RibbonItemGroup group, RibbonItem item, PaintEventArgs e)
        {
            var canvas = e.Canvas;
            var padding = e.LogicalToDeviceUnits (item.Padding);
            var background_color = item.Selected ? Theme.RibbonItemSelectedColor : item.Hovered ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;

            canvas.FillRectangle (item.Bounds, background_color);

            var image_area_bounds = new Rectangle (item.Bounds.Left + padding.Left, item.Bounds.Top + padding.Top, item.Bounds.Width - padding.Horizontal, e.LogicalToDeviceUnits (MINIMUM_ITEM_SIZE));
            var final_image_bounds = DrawingExtensions.CenterSquare (image_area_bounds, e.LogicalToDeviceUnits (IMAGE_SIZE));

            if (item.Image != null) {
                if (item.Enabled)
                    canvas.DrawBitmap (item.Image, final_image_bounds);
                else
                    canvas.DrawDisabledBitmap (item.Image, image_area_bounds);
            }

            if (!string.IsNullOrWhiteSpace (item.Text)) {
                var font_size = e.LogicalToDeviceUnits (Theme.RibbonItemFontSize);

                canvas.Save ();
                canvas.Clip (item.Bounds);

                var text_bounds = new Rectangle (item.Bounds.Left, image_area_bounds.Bottom, item.Bounds.Width, item.Bounds.Bottom - image_area_bounds.Bottom);
                canvas.DrawText (item.Text, Theme.UIFont, font_size, text_bounds, item.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor, ContentAlignment.MiddleCenter);

                canvas.Restore ();
            }
        }

        // TODO: This should not be done during the paint process
        protected void LayoutTabPage (RibbonTabPage tabPage) => tabPage.LayoutItems ();
    }
}
