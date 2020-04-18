using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms.Renderers
{
    public class TreeViewRenderer : Renderer<TreeView>
    {
        protected const int INDENT_SIZE = 18;
        protected const int IMAGE_SIZE = 16;
        protected const int GLYPH_SIZE = 10;

        protected override void Render (TreeView control, PaintEventArgs e)
        {
            e.Canvas.Save ();
            e.Canvas.Clip (control.ClientRectangle);

            var visible_item_count = control.ScaledHeight / control.ScaledItemHeight;

            foreach (var item in control.GetVisibleItems ().Take (visible_item_count + 1))
                RenderItem (control, item, e);

            e.Canvas.Restore ();
        }

        protected virtual void RenderItem (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            var background_color = item.Selected ? Theme.RibbonItemHighlightColor : Theme.LightNeutralGray;
            var foreground_color = control.Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor;

            e.Canvas.FillRectangle (item.Bounds, background_color);

            if (control.ShowDropdownGlyph == true) {
                var glyph_bounds = GetGlyphBounds (control, item);

                if (GetShouldDrawDropdownGlyph (control, item))
                    ControlPaint.DrawArrowGlyph (e, glyph_bounds, foreground_color, item.Expanded ? ArrowDirection.Down : ArrowDirection.Right);
            }

            if (control.ShowItemImages == true && item.Image != null) {
                var image_bounds = GetImageBounds (control, item, e);

                e.Canvas.DrawBitmap (item.Image!, image_bounds);
            }

            if (string.IsNullOrWhiteSpace (item.Text))
                return;

            var text_bounds = GetTextBounds (control, item, e);

            e.Canvas.DrawText (item.Text.Trim (), Theme.UIFont, e.LogicalToDeviceUnits (Theme.FontSize), text_bounds, foreground_color, ContentAlignment.MiddleLeft, maxLines: 1);
        }

        internal Rectangle GetGlyphBounds (TreeView control, TreeViewItem item)
        {
            if (!control.ShowDropdownGlyph)
                return Rectangle.Empty;

            var glyph_area = new Rectangle (GetIndentStart (control, item), item.Bounds.Top, control.LogicalToDeviceUnits (GLYPH_SIZE), item.Bounds.Height);
            var glyph_bounds = DrawingExtensions.CenterSquare (glyph_area, control.LogicalToDeviceUnits (GLYPH_SIZE));

            glyph_bounds.Width = control.LogicalToDeviceUnits (GLYPH_SIZE);

            return glyph_bounds;
        }

        protected Rectangle GetImageBounds (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            if (!control.ShowItemImages || item.Image is null)
                return Rectangle.Empty;

            var left_index = control.ShowDropdownGlyph ? GetGlyphBounds (control, item).Right : GetIndentStart (control, item);
            var image_area = new Rectangle (left_index, item.Bounds.Top, item.Bounds.Height, item.Bounds.Height);

            return DrawingExtensions.CenterSquare (image_area, e.LogicalToDeviceUnits (IMAGE_SIZE));
        }

        protected Rectangle GetTextBounds (TreeView control, TreeViewItem item, PaintEventArgs e)
        {
            var show_glyph = control.ShowDropdownGlyph;
            var show_image = control.ShowItemImages;

            if (!show_glyph && !show_image)
                return new Rectangle (GetIndentStart (control, item), item.Bounds.Top, item.Bounds.Width - GetIndentStart (control, item), item.Bounds.Height);

            // One of these will be valid because we handled the other case above
            var padding = e.LogicalToDeviceUnits (6);
            var used_bounds = show_image ? GetImageBounds (control, item, e) : GetGlyphBounds (control, item);
            return new Rectangle (used_bounds.Right + padding, item.Bounds.Top, item.Bounds.Right - used_bounds.Right - padding, item.Bounds.Height);
        }

        protected int GetIndentStart (TreeView control, TreeViewItem item) => item.Bounds.Left + item.IndentLevel * control.LogicalToDeviceUnits (INDENT_SIZE) + 2;

        protected bool GetShouldDrawDropdownGlyph (TreeView control, TreeViewItem item) => control.ShowDropdownGlyph && (item.HasChildren || (control.VirtualMode && item.items == null));
    }
}
