using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a TreeView control.
    /// </summary>
    public class TreeView : Control
    {
        private readonly TreeViewItem root_item;
        private int top_index = 0;
        private bool show_dropdown_glyph = true;
        private bool show_item_images = true;
        private bool virtual_mode;
        private readonly VerticalScrollBar vscrollbar;

        /// <summary>
        /// Initializes a new instance of the TreeView class.
        /// </summary>
        public TreeView ()
        {
            root_item = new TreeViewItem (this) {
                Expanded = true
            };

            vscrollbar = Controls.AddImplicitControl (new VerticalScrollBar {
                Minimum = 0,
                Maximum = 0,
                SmallChange = 1,
                LargeChange = 1,
                Visible = false,
                Dock = DockStyle.Right
            });

            vscrollbar.ValueChanged += VerticalScrollBar_ValueChanged;
        }

        /// <summary>
        /// Raised before a node is expanded.
        /// </summary>
        public event EventHandler<EventArgs<TreeViewItem>>? BeforeExpand;

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.LightNeutralGray;
                style.Border.Width = 1;
            });

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (250, 500);

        /// <summary>
        /// Returns the TreeViewItem at the specified location.
        /// </summary>
        public TreeViewItem? GetItemAtLocation (Point location) => root_item.GetVisibleItems ().FirstOrDefault (tp => tp.Bounds.Contains (location));

        // Enumerates through every visible TreeViewItem. Note items may not be in the currently shown part.
        internal IEnumerable<TreeViewItem> GetVisibleItems () => root_item.GetVisibleItems ().Skip (1 + top_index);

        /// <summary>
        /// Gets the collection of items contained by this TreeView.
        /// </summary>
        public TreeViewItemCollection Items => root_item.Items;

        /// <summary>
        /// Raised when an item is selected.
        /// </summary>
        public event EventHandler<EventArgs<TreeViewItem>>? ItemSelected;

        // Runs a layout pass on all TreeViewItems.
        private List<TreeViewItem> LayoutItems ()
        {
            UpdateVerticalScrollBar ();

            var visible_items = root_item.GetVisibleItems ().Skip (1 + top_index).ToList ();  // Skip the root element
            var client_rect = ClientRectangle;

            if (vscrollbar.Visible)
                client_rect.Width -= (client_rect.Width - vscrollbar.ScaledLeft + 1);

            StackLayoutEngine.VerticalExpand.Layout (client_rect, visible_items.Cast<ILayoutable> ());

            return visible_items;
        }

        /// <summary>
        /// Raises the BeforeExpand event.
        /// </summary>
        public void OnBeforeExpand (EventArgs<TreeViewItem> e) => BeforeExpand?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            var item = GetItemAtLocation (e.Location);

            // If an item wasn't clicked, let the base run and nothing else
            if (item is null) {
                base.OnClick (e);
                return;
            }

            // If an item with a ContextMenu was right-clicked, show its ContextMenu
            if (e.Button == MouseButtons.Right) {
                if (item.ContextMenu != null) {
                    item.ContextMenu.Show (PointToScreen (e.Location));
                    return;
                }

                // Otherwise let the base handle any right-click
                base.OnClick (e);
                return;
            }

            base.OnClick (e);

            var element = item.GetElementAtLocation (e.Location);

            if (element == TreeViewItem.TreeViewItemElement.Glyph)
                item.Expanded = !item.Expanded;
            else
                SelectedItem = item;
        }

        /// <inheritdoc/>
        protected override void OnDoubleClick (MouseEventArgs e)
        {
            base.OnDoubleClick (e);

            if (!e.Button.HasFlag (MouseButtons.Left))
                return;

            var item = GetItemAtLocation (e.Location);

            if (item is null)
                return;

            var element = item.GetElementAtLocation (e.Location);

            if (element != TreeViewItem.TreeViewItemElement.Glyph)
                item.Expanded = !item.Expanded;
        }

        /// <summary>
        /// Raises the ItemSelected event.
        /// </summary>
        protected virtual void OnItemSelected (EventArgs<TreeViewItem> e) => ItemSelected?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (vscrollbar.Visible)
                vscrollbar.RaiseMouseWheel (e);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            LayoutItems ();

            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        // The scaled height of each TreeViewItem.
        internal int ScaledItemHeight => root_item.GetPreferredSize (Size.Empty).Height;

        /// <summary>
        /// Gets or sets the currently selected TreeViewItem.
        /// </summary>
        public TreeViewItem SelectedItem {
            get => root_item.GetAllItems ().FirstOrDefault (i => i.Selected);
            set {
                // Don't allow user to unselect items
                if (value is null)
                    return;

                var current_selection = SelectedItem;

                if (current_selection == value)
                    return;

                if (current_selection != null)
                    current_selection.Selected = false;

                value.Selected = true;

                Invalidate ();

                OnItemSelected (new EventArgs<TreeViewItem> (value));
            }
        }

        /// <inheritdoc/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateVerticalScrollBar ();
        }

        /// <summary>
        /// Gets or sets a value indicating the drop down glyph should be shown.
        /// </summary>
        public bool ShowDropdownGlyph {
            get => show_dropdown_glyph;
            set {
                if (show_dropdown_glyph != value) {
                    show_dropdown_glyph = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating item images should be shown.
        /// </summary>
        public bool ShowItemImages {
            get => show_item_images;
            set {
                if (show_item_images != value) {
                    show_item_images = value;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        // Determines scrollbar visibility and scrollbar values.
        private void UpdateVerticalScrollBar ()
        {
            if (Items.Count == 0)
                vscrollbar.Visible = false;

            // See if we need more height than we have.
            if (ScaledItemHeight * root_item.GetVisibleChildrenCount () > ScaledHeight) {
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

        // Handles scrollbar scrolling.
        private void VerticalScrollBar_ValueChanged (object? sender, EventArgs e)
        {
            top_index = vscrollbar.Value;

            Invalidate ();
        }

        /// <summary>
        /// Gets or sets a value indicating if TreeViewItem nodes will be resolved when expanded.
        /// </summary>
        public bool VirtualMode {
            get => virtual_mode;
            set {
                if (virtual_mode != value) {
                    virtual_mode = value;
                    Invalidate ();
                }
            }
        }

        // The number of items that can be shown with the current height.
        private int VisibleItemCount => ScaledHeight / ScaledItemHeight;
    }
}
