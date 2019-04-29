using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class TreeControl : ModernControl
    {
        public List<TreeItem> Items { get; } = new List<TreeItem> ();

        public event EventHandler<EventArgs<TreeItem>> ItemSelected;

        public TreeControl ()
        {
            Width = 200;
        }

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            e.Surface.Canvas.Clear (Theme.LightNeutralGray);

            LayoutItems ();

            foreach (var item in Items)
                item.DrawItem (e.Surface.Canvas);

            // Side Border
            e.Surface.Canvas.DrawLine (Right - 2, 0, Right - 2, Height, Theme.BorderGray);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            SetSelectedItem (clicked_item);

            if (clicked_item != null)
                ItemSelected?.Invoke (this, new EventArgs<TreeItem> (clicked_item));
        }

        public void SetSelectedItem (TreeItem item)
        {
            if (item == null)
                return;

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
            var x = 0;
            var y = 0;
            var item_width = Width;
            var item_height = 30;

            foreach (var item in Items) {
                item.SetBounds (x, y, item_width, item_height);
                y += item_height;
            }
        }
    }
}
