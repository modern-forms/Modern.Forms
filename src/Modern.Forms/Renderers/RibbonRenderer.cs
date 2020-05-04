using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a Ribbon.
    /// </summary>
    public class RibbonRenderer : Renderer<Ribbon>
    {
        /// <summary>
        /// Size of a ribbon item image.
        /// </summary>
        protected const int IMAGE_SIZE = 32;
        /// <summary>
        /// Minimum size of a ribbon item.
        /// </summary>
        protected const int MINIMUM_ITEM_SIZE = 40;

        /// <inheritdoc/>
        protected override void Render (Ribbon control, PaintEventArgs e)
        {
            var selected_tab = control.SelectedTabPage;

            if (selected_tab != null)
                RenderTabPage (control, selected_tab, e);
        }

        /// <summary>
        /// Renders a RibbonTabPage.
        /// </summary>
        protected virtual void RenderTabPage (Ribbon control, RibbonTabPage tabPage, PaintEventArgs e)
        {
            LayoutTabPage (tabPage);

            e.Canvas.FillRectangle (tabPage.Bounds, Theme.NeutralGray);

            foreach (var group in tabPage.Groups)
                RenderItemGroup (control, tabPage, group, e);
        }

        /// <summary>
        /// Renders a RibbonItemGroup.
        /// </summary>
        protected virtual void RenderItemGroup (Ribbon control, RibbonTabPage tabPage, RibbonItemGroup group, PaintEventArgs e)
        {
            // Draw each ribbon item
            foreach (var item in group.Items)
                if (item is MenuSeparatorItem msi)
                    RenderMenuSeparatorItem (control, tabPage, group, msi, e);
                else
                    RenderItem (control, tabPage, group, item, e);

            // Right border (group separator)
            e.Canvas.DrawLine (group.Bounds.Right - 1, group.Bounds.Top + 4, group.Bounds.Right - 1, group.Bounds.Bottom - 4, Theme.BorderGray);
        }

        /// <summary>
        /// Renders a MenuItem.
        /// </summary>
        protected virtual void RenderItem (Ribbon control, RibbonTabPage tabPage, RibbonItemGroup group, MenuItem item, PaintEventArgs e)
        {
            var canvas = e.Canvas;
            var padding = e.LogicalToDeviceUnits (item.Padding);
            var background_color = item.Selected ? Theme.ItemSelectedColor : item.Hovered ? Theme.ItemHighlightColor : Theme.NeutralGray;

            canvas.FillRectangle (item.Bounds, background_color);

            var image_area_bounds = new Rectangle (item.Bounds.Left + padding.Left, item.Bounds.Top + padding.Top, item.Bounds.Width - padding.Horizontal, e.LogicalToDeviceUnits (MINIMUM_ITEM_SIZE));
            var final_image_bounds = DrawingExtensions.CenterSquare (image_area_bounds, e.LogicalToDeviceUnits (IMAGE_SIZE));

            if (item.Image != null)
                    canvas.DrawBitmap (item.Image, final_image_bounds, !item.Enabled);

            if (!string.IsNullOrWhiteSpace (item.Text)) {
                var font_size = e.LogicalToDeviceUnits (Theme.ItemFontSize);

                canvas.Save ();
                canvas.Clip (item.Bounds);

                var text_bounds = new Rectangle (item.Bounds.Left, image_area_bounds.Bottom, item.Bounds.Width, item.Bounds.Bottom - image_area_bounds.Bottom);
                canvas.DrawText (item.Text, Theme.UIFont, font_size, text_bounds, item.Enabled ? Theme.PrimaryTextColor : Theme.DisabledTextColor, ContentAlignment.MiddleCenter);

                canvas.Restore ();
            }
        }

        /// <summary>
        /// Renders a MenuSeparatorItem.
        /// </summary>
        protected virtual void RenderMenuSeparatorItem (Ribbon control, RibbonTabPage tabPage, RibbonItemGroup group, MenuSeparatorItem item, PaintEventArgs e)
        {
            // Background
            e.Canvas.FillRectangle (item.Bounds, Theme.NeutralGray);

            var center = item.Bounds.GetCenter ();
            var thickness = e.LogicalToDeviceUnits (1);
            var padding = e.LogicalToDeviceUnits (item.Padding);

            e.Canvas.DrawLine (center.X, item.Bounds.Y + padding.Top, center.X, item.Bounds.Bottom - padding.Bottom, item.Enabled ? Theme.ItemHighlightColor : Theme.DisabledTextColor, thickness);
        }

        /// <summary>
        /// Gets the preferred size of a MenuItem.
        /// </summary>
        public virtual Size GetPreferredItemSize (Ribbon control, MenuItem item, Size proposedSize)
        {
            if (item is MenuSeparatorItem msi)
                return GetPreferredSeparatorItemSize (control, msi, proposedSize);

            var padding = control.LogicalToDeviceUnits (6);
            var font_size = control.LogicalToDeviceUnits (Theme.ItemFontSize);
            var proposed_size = control.LogicalToDeviceUnits (new Size (40, 30));
            var text_size = (int)Math.Round (TextMeasurer.MeasureText (item.Text ?? string.Empty, Theme.UIFont, font_size, proposed_size).Width);

            return new Size (Math.Max (text_size + padding, control.LogicalToDeviceUnits (MINIMUM_ITEM_SIZE)), 0);
        }

        /// <summary>
        /// Gets the preferred size of a MenuSeparatorItem.
        /// </summary>
        protected virtual Size GetPreferredSeparatorItemSize (Ribbon control, MenuSeparatorItem item, Size proposedSize)
        {
            var padding = control.LogicalToDeviceUnits (6);
            var thickness = control.LogicalToDeviceUnits (1);

            return new Size (thickness + padding, proposedSize.Height);
        }

        /// <summary>
        /// Performs a layout of the tab page's items.
        /// </summary>
        // TODO: This should not be done during the paint process
        protected void LayoutTabPage (RibbonTabPage tabPage) => tabPage.LayoutItems ();
    }
}
