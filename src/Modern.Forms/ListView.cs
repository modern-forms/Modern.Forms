using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class ListView : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => style.BackgroundColor = Theme.LightNeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public List<ListViewItem> Items { get; } = new List<ListViewItem> ();

        public event EventHandler<EventArgs<ListViewItem>> ItemDoubleClicked;

        protected override Size DefaultSize => new Size (450, 450);

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutItems ();

            foreach (var item in Items)
                item.DrawItem (e.Canvas);
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            SetSelectedItem (clicked_item);
        }

        protected override void OnDoubleClick (MouseEventArgs e)
        {
            base.OnDoubleClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            if (clicked_item != null)
                ItemDoubleClicked?.Invoke (this, new EventArgs<ListViewItem> (clicked_item));
        }

        public void SetSelectedItem (ListViewItem item)
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
