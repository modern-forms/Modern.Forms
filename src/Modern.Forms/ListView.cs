using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ListView control.
    /// Note the ListView control has not been fully developed, and probably does not contain enough functionality to be useful yet.
    /// </summary>
    public class ListView : Control
    {
        /// <summary>
        /// Initializes a new instance of the ListView class.
        /// </summary>
        public ListView ()
        {
            Items = new ListViewItemCollection (this);
        }

        /// <inheritdoc/>
        protected override Padding DefaultPadding => new Padding (3);

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (450, 450);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => style.BackgroundColor = Theme.ControlLowColor);

        /// <summary>
        /// Raised when a list view item is double-clicked.
        /// </summary>
        public event EventHandler<EventArgs<ListViewItem>>? ItemDoubleClicked;

        /// <summary>
        /// Gets the collection of items contained by this ListView.
        /// </summary>
        public ListViewItemCollection Items { get; }

        // Lays out the ListViewItems.
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

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            SelectedItem = clicked_item;
        }

        /// <inheritdoc/>
        protected override void OnDoubleClick (MouseEventArgs e)
        {
            base.OnDoubleClick (e);

            var clicked_item = Items.FirstOrDefault (tp => tp.Bounds.Contains (e.Location));

            if (clicked_item != null)
                ItemDoubleClicked?.Invoke (this, new EventArgs<ListViewItem> (clicked_item));
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutItems ();

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Gets or sets the currently selected item, if any. If there are multiple selected items, the first selected item will be returned.
        /// </summary>
        public ListViewItem? SelectedItem {
            get => Items.FirstOrDefault (i => i.Selected);
            set {
                var current_item = Items.FirstOrDefault (i => i.Selected);

                if (current_item == value)
                    return;

                if (current_item != null)
                    current_item.Selected = false;

                if (value != null)
                    value.Selected = true;

                Invalidate ();
            }
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
