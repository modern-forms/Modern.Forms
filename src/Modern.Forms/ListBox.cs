using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ListBox control.
    /// </summary>
    public class ListBox : Control
    {
        private int item_height = -1;
        private SelectionMode selection_mode = SelectionMode.One;
        private bool scrollbar_always_visible;
        private int top_index = 0;
        private readonly VerticalScrollBar vscrollbar;

        /// <summary>
        /// Initializes a new instance of the ListBox class.
        /// </summary>
        public ListBox ()
        {
            Items = new ListBoxItemCollection (this);

            Items.CollectionChanged += (o, e) => UpdateVerticalScrollBar ();

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

        /// <inheritdoc/>
        protected override Cursor DefaultCursor => Cursors.Hand;

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (120, 96);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.LightNeutralGray;
                style.Border.Width = 1;
            });

        /// <summary>
        /// Gets or sets the index of the first visible item.
        /// </summary>
        public int FirstVisibleIndex {
            get => top_index;
            set {
                if (top_index == value)
                    return;

                if (value < 0 || value >= Items.Count)
                    return;

                vscrollbar.Value = Math.Min (value, vscrollbar.Maximum);
            }
        }
        /// <summary>
        /// Gets the index of the item currently at the specified location.
        /// </summary>
        /// <param name="location">The coordinates used to determine the index.</param>
        public int GetIndexAtLocation (Point location)
        {
            for (var i = top_index; i < Math.Min (Items.Count, top_index + VisibleItemCount + 1); i++)
                if (GetItemRectangle (i).Contains (location))
                    return i;

            return -1;
        }

        /// <summary>
        /// Gets the bounding rectangle for the specified item.
        /// </summary>
        /// <param name="index">The zero-based index of the desired item.</param>
        public Rectangle GetItemRectangle (int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException ("Index out of range.");

            var client = ClientRectangle;

            index -= top_index;

            var top = index * ScaledItemHeight + client.Top;
            return new Rectangle (client.Left, top, client.Width - (vscrollbar.Visible ? vscrollbar.ScaledWidth : 0), Math.Min (ScaledItemHeight, client.Bottom - top));
        }

        /// <summary>
        /// Gets or sets the unscaled height each item will use.
        /// </summary>
        public int ItemHeight {
            get {
                if (item_height == -1)
                    item_height = (int)TextMeasurer.MeasureText ("The quick brown Fox", this).Height + 3;

                return item_height;
            }
            set {
                if (value > 255)
                    throw new ArgumentOutOfRangeException ("The ItemHeight property was set beyond 255 pixels");

                item_height = value;

                Invalidate ();
            }
        }

        /// <summary>
        /// Gets the collection of items contained by this ListBox.
        /// </summary>
        public ListBoxItemCollection Items { get; }

        // The height that would be needed to display all items.
        private int NeededHeightForItems => ScaledItemHeight * Items.Count;

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            var index = GetIndexAtLocation (e.Location);

            if (index == -1)
                return;

            switch (SelectionMode) {
                case SelectionMode.One:
                    SelectedIndex = index;
                    break;

                case SelectionMode.MultiSimple:
                    if (Items.SelectedIndexes.Contains (index))
                        Items.SelectedIndexes.Remove (index);
                    else
                        Items.SelectedIndexes.Add (index);

                    Invalidate ();

                    break;
                case SelectionMode.MultiExtended:
                    // TODO: Shift

                    // When Control is held we treat this like MultiSimple
                    if (e.Control) {
                        if (Items.SelectedIndexes.Contains (index))
                            Items.SelectedIndexes.Remove (index);
                        else
                            Items.SelectedIndexes.Add (index);

                        Invalidate ();
                        break;
                    }

                    // Else we treat this like SelectionMode.One
                    SelectedIndex = index;

                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (ShowHover)
                Items.HoveredIndex = -1;
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (ShowHover)
                Items.HoveredIndex = GetIndexAtLocation (e.Location);
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
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        protected virtual void OnSelectedIndexChanged (EventArgs e) => SelectedIndexChanged?.Invoke (this, e);

        /// <summary>
        /// Gets the scaled height each item occupies.
        /// </summary>
        public int ScaledItemHeight => LogicalToDeviceUnits (ItemHeight);

        /// <summary>
        /// Gets or sets a value indicating if the vertical scrollbar is always shown.
        /// </summary>
        public bool ScrollbarAlwaysVisible {
            get => scrollbar_always_visible;
            set {
                if (scrollbar_always_visible != value) {
                    scrollbar_always_visible = value;
                    UpdateVerticalScrollBar ();
                }
            }
        }
        /// <summary>
        /// Gets or sets the index of the currently selected item.  If there are multiple selected items, the first item's index will be returned.
        /// </summary>
        public int SelectedIndex {
            get => Items.SelectedIndex;
            set {
                if (SelectionMode == SelectionMode.None)
                    throw new ArgumentException ("Cannot call this method if SelectionMode is SelectionMode.None");

                if (Items.SelectedIndex != value) {
                    Items.SelectedIndex = value;
                    OnSelectedIndexChanged (EventArgs.Empty);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Raised when the value of the SelectedIndex property changes.
        /// </summary>
        public event EventHandler? SelectedIndexChanged;

        /// <summary>
        /// Gets or sets the currently selected item, if any.  If there are multiple selected items, the first selected item will be returned.
        /// </summary>
        public object? SelectedItem {
            get => Items.SelectedItem;
            set => Items.SelectedItem = value;
        }

        /// <summary>
        /// Gets all currently select items.
        /// </summary>
        public IEnumerable<object> SelectedItems => Items.SelectedItems;

        /// <summary>
        /// Gets or set the selection mode of the ListBox.
        /// </summary>
        public SelectionMode SelectionMode {
            get => selection_mode;
            set {
                if (!Enum.IsDefined (typeof (SelectionMode), value))
                    throw new InvalidEnumArgumentException ($"Enum argument value '{value}' is not valid for SelectionMode");

                if (selection_mode == value)
                    return;

                selection_mode = value;

                if (selection_mode == SelectionMode.None)
                    Items.SelectedIndex = -1;
                else if (selection_mode == SelectionMode.One)
                    Items.SelectedIndex = Items.SelectedIndex;  // Yes this does something  ;)
            }
        }

        /// <inheritdoc/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateVerticalScrollBar ();
        }

        /// <summary>
        /// Gets or sets whether the item currently containing the mouse pointer should be highlighted.
        /// </summary>
        public bool ShowHover { get; set; }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        // Update the vertical scroll bar to match the number of current items.
        private void UpdateVerticalScrollBar ()
        {
            if (Items.Count == 0)
                vscrollbar.Visible = ScrollbarAlwaysVisible;

            if (NeededHeightForItems > Bounds.Height) {
                vscrollbar.Visible = true;
                vscrollbar.Maximum = Items.Count - VisibleItemCount;
                vscrollbar.LargeChange = VisibleItemCount;
            } else {
                vscrollbar.Visible = ScrollbarAlwaysVisible;
            }
        }

        // Handle changes to the vertical scroll bar.
        private void VerticalScrollBar_ValueChanged (object? sender, EventArgs e)
        {
            top_index = Math.Max (vscrollbar.Value, 0);

            Invalidate ();
        }

        /// <summary>
        /// The number of full items that can be shown at a time.
        /// </summary>
        public int VisibleItemCount => ClientRectangle.Height / ScaledItemHeight;
    }
}
