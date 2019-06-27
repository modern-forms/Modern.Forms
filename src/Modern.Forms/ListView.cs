using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class ListView : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => style.BackgroundColor = Theme.LightNeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public ListViewItemCollection Items { get; }

        public ListView ()
        {
            Items = new ListViewItemCollection (this);
        }

        public event EventHandler<EventArgs<ListViewItem>> ItemDoubleClicked;

        protected override Padding DefaultPadding => new Padding (3);

        protected override Size DefaultSize => new Size (450, 450);

        protected override void OnPaint (PaintEventArgs e)
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
            var bounds = PaddedClientRectangle;
            var item_size = LogicalToDeviceUnits (70);
            var item_margin = LogicalToDeviceUnits (6);

            var x = bounds.Left;
            var y = bounds.Top;

            foreach (var item in Items) {
                item.SetBounds (x, y, item_size, item_size);
                x += item_size + item_margin;

                if (x + item_size > bounds.Width) {
                    x = bounds.Left;
                    y += item_size + item_margin;
                }
            }
        }
    }
}
