using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class ListBoxRenderer : Renderer<ListBox>
    {
        public override void Render (ListBox control, PaintEventArgs e)
        {
            for (var i = control.FirstVisibleIndex; i < Math.Min (control.Items.Count, control.FirstVisibleIndex + control.VisibleItemCount + 1); i++) {
                var item = control.Items[i];
                var bounds = control.GetItemRectangle (i);

                RenderItem (control, item, i, bounds, e);
            }
        }

        public virtual void RenderItem (ListBox control, object item, int index, Rectangle bounds, PaintEventArgs e)
        {
            // Draw selected background
            if (control.Items.SelectedIndexes.Contains (index))
                e.Canvas.FillRectangle (bounds, Theme.RibbonItemHighlightColor);

            // Draw hover background
            else if (control.ShowHover && control.Items.HoveredIndex == index)
                e.Canvas.FillRectangle (bounds, Theme.NeutralGray);

            // This fixes text positioning for partially shown items
            bounds.Height = control.ScaledItemHeight;
            bounds.Inflate (-4, 0);

            // Draw text
            e.Canvas.DrawText (item.ToString (), bounds, control, ContentAlignment.MiddleLeft, maxLines: 1);
        }
    }
}
