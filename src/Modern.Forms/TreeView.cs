using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class TreeView : LiteControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = ModernTheme.LightNeutralGray;
                style.Border.Right.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public TreeViewItemCollection Items { get; }

        private StackLayoutEngine layout_engine = new StackLayoutEngine (Orientation.Vertical, true);

        public TreeView ()
        {
            Items = new TreeViewItemCollection (this);
        }

        public event EventHandler<EventArgs<TreeViewItem>> ItemSelected;

        public TreeViewItem GetItemAtLocation (Point location) => Items.FirstOrDefault (tp => tp.Bounds.Contains (location));

        public TreeViewItem SelectedItem {
            get => Items.FirstOrDefault (i => i.Selected);
            set {
                // Don't allow user to unselect items
                if (value == null)
                    return;

                var old = SelectedItem;

                if (old == value)
                    return;

                if (old != null)
                    old.Selected = false;

                value.Selected = true;

                Invalidate ();

                OnItemSelected (new EventArgs<TreeViewItem> (value));
            }
        }

        protected override Size DefaultSize => new Size (250, 500);

        protected virtual void OnItemSelected (EventArgs<TreeViewItem> e) => ItemSelected?.Invoke (this, e);

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutItems ();

            foreach (var item in Items)
                item.OnPaint (e.Canvas);
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            SelectedItem = GetItemAtLocation (e.Location);
        }

        private void LayoutItems ()
        {
            var item_bounds = ClientBounds;

            item_bounds.Width -= 1;

            layout_engine.Layout (item_bounds, Items.Cast<ILayoutable> ());
        }
    }
}
