using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class TreeView : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.LightNeutralGray;
                style.Border.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private readonly TreeViewItem root_item;
        private int top_index = 0;
        private bool show_dropdown_glyph = true;
        private bool show_item_images = true;
        private readonly VerticalScrollBar vscrollbar;

        public TreeView ()
        {
            root_item = new TreeViewItem (this) {
                Expanded = true
            };

            vscrollbar = new VerticalScrollBar {
                Minimum = 0,
                Maximum = 0,
                SmallChange = 1,
                LargeChange = 1,
                Visible = false,
                Dock = DockStyle.Right
            };

            vscrollbar.ValueChanged += VerticalScrollBar_ValueChanged;

            Controls.AddImplicitControl (vscrollbar);
        }

        public event EventHandler<EventArgs<TreeViewItem>> ItemSelected;

        public TreeViewItem GetItemAtLocation (Point location) => root_item.GetVisibleItems ().FirstOrDefault (tp => tp.Bounds.Contains (location));

        public TreeViewItemCollection Items => root_item.Items;

        public TreeViewItem SelectedItem {
            get => GetAllItems ().FirstOrDefault (i => i.Selected);
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

        public bool ShowDropdownGlyph {
            get => show_dropdown_glyph;
            set {
                if (show_dropdown_glyph != value) {
                    show_dropdown_glyph = value;
                    Invalidate ();
                }
            }
        }

        public bool ShowItemImages {
            get => show_item_images;
            set {
                if (show_item_images != value) {
                    show_item_images = value;
                    Invalidate ();
                }
            }
        }

        protected override Size DefaultSize => new Size (250, 500);

        protected virtual void OnItemSelected (EventArgs<TreeViewItem> e) => ItemSelected?.Invoke (this, e);

        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (vscrollbar.Visible)
                vscrollbar.RaiseMouseWheel (e);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            var visible_items = LayoutItems ();

            base.OnPaint (e);

            e.Canvas.Save ();
            e.Canvas.Clip (ClientRectangle);

            foreach (var item in visible_items.Take (VisibleItemCount + 1))
                item.OnPaint (e.Canvas);

            e.Canvas.Restore ();
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            var item = GetItemAtLocation (e.Location);

            if (item == null)
                return;

            var element = item.GetElementAtLocation (e.Location);

            if (element == TreeViewItem.TreeViewItemElement.Glyph)
                item.Expanded = !item.Expanded;
            else
                SelectedItem = GetItemAtLocation (e.Location);
        }

        protected override void OnDoubleClick (MouseEventArgs e)
        {
            base.OnDoubleClick (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            var item = GetItemAtLocation (e.Location);

            if (item == null)
                return;

            var element = item.GetElementAtLocation (e.Location);

            if (element != TreeViewItem.TreeViewItemElement.Glyph)
                item.Expanded = !item.Expanded;
        }

        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateVerticalScrollBar ();
        }

        private IEnumerable<TreeViewItem> GetAllItems () => root_item.GetAllItems ();

        private int ItemHeight => root_item.GetPreferredSize (Size.Empty).Height;

        private List<TreeViewItem> LayoutItems ()
        {
            UpdateVerticalScrollBar ();

            var visible_items = root_item.GetVisibleItems ().Skip (1 + top_index).ToList ();  // Skip the root element
            var client_rect = ClientRectangle;

            if (vscrollbar.Visible)
                client_rect.Width -= vscrollbar.Width;

            StackLayoutEngine.VerticalExpand.Layout (client_rect, visible_items.Cast<ILayoutable> ());

            return visible_items;
        }

        private int NeededHeightForItems => ScaledItemHeight * root_item.GetVisibleChildrenCount ();

        private int ScaledItemHeight => LogicalToDeviceUnits (ItemHeight);

        private void UpdateVerticalScrollBar ()
        {
            if (Items.Count == 0)
                vscrollbar.Visible = false;

            if (NeededHeightForItems > Bounds.Height) {
                if (!vscrollbar.Visible)
                    vscrollbar.Value = 0;

                vscrollbar.Visible = true;
                vscrollbar.Maximum = root_item.GetVisibleChildrenCount () - VisibleItemCount;
                vscrollbar.LargeChange = VisibleItemCount;
            } else {
                vscrollbar.Visible = false;
                top_index = 0;
            }
        }

        private void VerticalScrollBar_ValueChanged (object sender, EventArgs e)
        {
            top_index = vscrollbar.Value;

            Invalidate ();
        }

        private int VisibleItemCount => (int)(ClientRectangle.Height / ScaledItemHeight);
    }
}
