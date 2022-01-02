using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        private TreeViewItem selected_item;
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

            selected_item = root_item;

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

        internal void EnsureItemVisible (TreeViewItem item)
        {
            // Make sure all parent are expanded so this node is shown
            var parent = item.Parent;

            while (parent != null && parent != root_item) {
                parent.Expand ();
                parent = parent.Parent;
            }

            var all_items = root_item.GetVisibleItems ().Skip (1).ToList ();

            if (all_items.Count <= VisibleItemCount)
                return;

            var index = all_items.IndexOf (item);

            if (index < top_index) {
                top_index = index;
                vscrollbar.Value = top_index;
                return;
            }

            if (index >= top_index + VisibleItemCount - 1) {
               // top_index = index - (VisibleItemCount - 1);
                vscrollbar.Value = index - (VisibleItemCount - 1);
                return;
            }
        }

        /// <summary>
        /// Finds the index of the next item after startIndex that begins with the specified string. This search is case-insensitive.
        /// </summary>
        private TreeViewItem? FindString (string s, TreeViewItem startItem)
        {
            var all_items = GetVisibleItems ().ToList ();
            var start_index = all_items.IndexOf (startItem);

            if (s is null || all_items.Count == 0)
                return null;

            // We actually look for matches AFTER the start index
            start_index = (start_index == all_items.Count - 1) ? 0 : start_index + 1;
            var current = start_index;

            while (true) {
                var item = all_items[current];

                if (string.Compare (s, 0, item.Text, 0, s.Length, true, CultureInfo.CurrentCulture) == 0)
                    return item;

                current++;

                if (current == all_items.Count)
                    current = 0;

                if (current == start_index)
                    return null;
            }
        }

        /// <summary>
        /// Returns the TreeViewItem at the specified location.
        /// </summary>
        public TreeViewItem? GetItemAtLocation (Point location) => root_item.GetVisibleItems ().FirstOrDefault (tp => tp.Bounds.Contains (location));

        // Enumerates through every visible TreeViewItem. Note items may not be in the currently shown part.
        internal IEnumerable<TreeViewItem> GetVisibleItems (bool skipOffscreen = false) => root_item.GetVisibleItems ().Skip (1 + (skipOffscreen ? top_index : 0));

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
                    item.ContextMenu.Show (this, PointToScreen (e.Location));
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
        protected override void OnKeyDown (KeyEventArgs e)
        {
            // PERF: Anything using GetVisibleItems () could probably be written more efficiently
            // Down moves down one visible node
            if (e.KeyCode == Keys.Down) {
                var all = GetVisibleItems ().ToList ();
                var index = all.IndexOf (selected_item);

                if (index + 1 < all.Count)
                    SelectedItem = all[index + 1];

                e.Handled = true;
                return;
            }

            // Up moves up one visible node
            if (e.KeyCode == Keys.Up) {
                var all = GetVisibleItems ().ToList ();
                var index = all.IndexOf (selected_item);

                if (index > 0)
                    SelectedItem = all[index - 1];

                e.Handled = true;
                return;
            }

            // Home moves to first expanded node
            if (e.KeyCode == Keys.End) {
                var all = GetVisibleItems ().ToList ();

                if (all.Count == 0)
                    return;

                SelectedItem = all.Last ();

                e.Handled = true;
                return;
            }

            // End moves to last expanded node
            if (e.KeyCode == Keys.Home) {
                var all = GetVisibleItems ().ToList ();

                if (all.Count == 0)
                    return;

                SelectedItem = all.First ();

                e.Handled = true;
                return;
            }

            // PgDown moves down by amount of visible nodes
            if (e.KeyCode == Keys.PageDown) {
                var all = GetVisibleItems ().ToList ();

                if (all.Count == 0)
                    return;

                var index = all.IndexOf (selected_item);
                var new_index = Math.Min (index + VisibleItemCount - 1, all.Count - 1);

                SelectedItem = all[new_index];

                e.Handled = true;
                return;
            }

            // PgUp moves up by amount of visible nodes
            if (e.KeyCode == Keys.PageUp) {
                var all = GetVisibleItems ().ToList ();

                if (all.Count == 0)
                    return;

                var index = all.IndexOf (selected_item);
                var new_index = Math.Max (index - (VisibleItemCount - 1), 0);

                SelectedItem = all[new_index];

                e.Handled = true;
                return;
            }

            // Right when HasChildren expands node (if needed) and selects first child
            if (e.KeyCode == Keys.Right) {
                selected_item.Expand ();

                if (selected_item.HasChildren)
                    SelectedItem = selected_item.Items.First ();

                e.Handled = true;
                return;
            }

            // Left with expanded children collapses children
            if (e.KeyCode == Keys.Left && selected_item.HasChildren && selected_item.Expanded) {
                selected_item.Collapse ();
                e.Handled = true;
                return;
            }

            // Left with no children or collapsed selects parent
            if (e.KeyCode == Keys.Left && !selected_item.Expanded) {
                if (selected_item.Parent is TreeViewItem parent && parent != root_item)
                    SelectedItem = parent;

                e.Handled = true;
                return;
            }

            // First letter toggles between all expanded nodes
            if (char.IsLetterOrDigit ((char)e.KeyCode)) {
                var item = FindString (((char)e.KeyCode).ToString (), selected_item);

                if (item != null) {
                    SelectedItem = item;
                    e.Handled = true;
                    return;
                }
            }

            // TODO: If checkboxes, space toggles checkbox
            base.OnKeyDown (e);
        }

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
            get => selected_item;
            set {
                // Don't allow user to unselect items
                if (value is null)
                    return;

                var current_selection = selected_item;

                if (current_selection == value)
                    return;

                selected_item = value;

                EnsureItemVisible (value);
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
