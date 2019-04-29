using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class ListViewControl : ModernControl
    {
        public List<ListViewControlItem> Items { get; } = new List<ListViewControlItem> ();

        public event EventHandler<EventArgs<ListViewControlItem>> ItemDoubleClicked;

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            e.Surface.Canvas.Clear (Theme.FormBackColor);

            LayoutItems ();

            foreach (var item in Items)
                item.DrawItem (e.Surface.Canvas);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            SetSelectedItem (clicked_item);
        }

        protected override void OnMouseDoubleClick (MouseEventArgs e)
        {
            base.OnMouseDoubleClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            if (clicked_item != null)
                ItemDoubleClicked?.Invoke (this, new EventArgs<ListViewControlItem> (clicked_item));
        }

        public void SetSelectedItem (ListViewControlItem item)
        {
            var old = Items.FirstOrDefault (i => i.Selected);

            if (old == item)
                return;

            if (old != null)
                old.Selected = false;

            if (item != null)
                item.Selected = true;

            Invalidate ();
        }

        private void LayoutItems ()
        {
            var x = 3;
            var y = 3;
            var item_width = 70;
            var item_height = 70;
            var item_padding = 6;

            foreach (var item in Items) {
                item.SetBounds (x, y, item_width, item_height);
                x += item_width + item_padding;

                if (x + item_width > Width) {
                    x = 3;
                    y += item_height + item_padding;
                }
            }
        }
    }
}
