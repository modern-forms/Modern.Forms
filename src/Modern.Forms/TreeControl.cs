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
        public new static ControlStyle DefaultStyle = new ControlStyle (ModernControl.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.LightNeutralGray;
                style.Border.Right.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public List<TreeItem> Items { get; } = new List<TreeItem> ();

        public event EventHandler<EventArgs<TreeItem>> ItemSelected;

        protected override Size DefaultSize => new Size (250, 500);

        public TreeControl ()
        {
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutItems ();

            foreach (var item in Items)
                item.DrawItem (e.Canvas);
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
            var item_width = Width - 1;
            var item_height = 30;

            foreach (var item in Items) {
                item.SetBounds (x, y, item_width, item_height);
                y += item_height;
            }
        }
    }
}
