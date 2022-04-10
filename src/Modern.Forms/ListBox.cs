using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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

        private void EnsureItemVisible (int index)
        {
            // If there aren't enough items to need scrolling, things are good
            if (VisibleItemCount >= Items.Count)
                return;

            if (index < FirstVisibleIndex)
                FirstVisibleIndex = index;
            else if (index >= FirstVisibleIndex + VisibleItemCount)
                FirstVisibleIndex = index - VisibleItemCount + 1;
        }

        /// <summary>
        /// Finds the index of the next item after startIndex that begins with the specified string. This search is case-insensitive.
        /// </summary>
        public int FindString (string s, int startIndex = -1)
        {
            if (s is null || Items.Count == 0)
                return -1;

            if (startIndex < -1 || startIndex >= Items.Count)
                throw new ArgumentOutOfRangeException (nameof (startIndex));

            // We actually look for matches AFTER the start index
            startIndex = (startIndex == Items.Count - 1) ? 0 : startIndex + 1;
            var current = startIndex;

            while (true) {
                var item = Items[current]?.ToString ();

                if (string.Compare (s, 0, item, 0, s.Length, true, CultureInfo.CurrentCulture) == 0)
                    return current;

                current++;

                if (current == Items.Count)
                    current = 0;

                if (current == startIndex)
                    return -1;
            }
        }

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
        protected override void OnKeyUp (KeyEventArgs e)
        {
            // In "None" mode, the focus goes up and down
            // In "MultiSimple" mode, the focus goes up and down, and space selects or deselects
            if (selection_mode.In (SelectionMode.None, SelectionMode.MultiSimple)) {
                if (e.KeyCode.In (Keys.Down, Keys.Right)) {
                    if (Items.FocusedIndex < Items.Count - 1) {
                        Items.FocusedIndex++;
                        EnsureItemVisible (Items.FocusedIndex);
                        e.Handled = true;
                        return;
                    }
                }

                if (e.KeyCode.In (Keys.Up, Keys.Left)) {
                    if (Items.FocusedIndex > 0) {
                        Items.FocusedIndex--;
                        EnsureItemVisible (Items.FocusedIndex);
                        e.Handled = true;
                        return;
                    }
                }

                if (e.KeyCode == Keys.Space && selection_mode == SelectionMode.MultiSimple) {
                    Items.ToggleSelectedIndex (Items.FocusedIndex);
                    e.Handled = true;
                    return;
                }
            }

            // In "One" mode, the selection goes up and down
            // In "MultiExtended" mode, the selection goes up and down, and SHIFT adds or subtracts to a contiguous selection
            if (selection_mode.In (SelectionMode.One, SelectionMode.MultiExtended)) {

                if (selection_mode == SelectionMode.MultiExtended && e.Shift) {
                    // Find contiguous selection index is part of
                    // - If at top or bottom, add or subtract based towards or away from selection
                    // - If not in contiguous, or middle of contiguous, or more than one contiguous: start new selection, adding item in direction
                    var (start, end) = Items.GetSingleContiguousSelection ();

                    if (start == -1 || (start != Items.FocusedIndex && end != Items.FocusedIndex)) {
                        // No existing selection, make a new one
                        SelectedIndex = Items.FocusedIndex;

                        if (e.KeyCode.In (Keys.Down, Keys.Right) && SelectedIndex < Items.Count - 1) {
                            Items.AddSelectedIndex (SelectedIndex + 1, false);
                            EnsureItemVisible (Items.FocusedIndex);
                            e.Handled = true;
                            return;
                        }

                        if (e.KeyCode.In (Keys.Up, Keys.Left) && SelectedIndex > 0) {
                            Items.AddSelectedIndex (SelectedIndex - 1, false);
                            EnsureItemVisible (Items.FocusedIndex);
                            e.Handled = true;
                            return;
                        }
                    } else {
                        // At the top and moving up, add to top of the selection
                        if (start == Items.FocusedIndex && e.KeyCode.In (Keys.Up, Keys.Left)) {
                            if (Items.FocusedIndex > 0) {
                                Items.AddSelectedIndex (start - 1, false);
                                EnsureItemVisible (Items.FocusedIndex);
                            }
                            e.Handled = true;
                            return;
                        }

                        // At the top and moving down, remove from top of the selection
                        if (start == Items.FocusedIndex && start != end && e.KeyCode.In (Keys.Down, Keys.Right)) {
                            Items.RemoveSelectedIndex (start);
                            Items.FocusedIndex++;
                            EnsureItemVisible (Items.FocusedIndex);
                            e.Handled = true;
                            return;
                        }

                        // At the bottom and moving down, add to bottom of the selection
                        if (end == Items.FocusedIndex && e.KeyCode.In (Keys.Down, Keys.Right)) {
                            if (Items.FocusedIndex < Items.Count - 1) {
                                Items.AddSelectedIndex (end + 1, false);
                                EnsureItemVisible (Items.FocusedIndex);
                            }
                            e.Handled = true;
                            return;
                        }

                        // At the bottom and moving up, remove from bottom of the selection
                        if (end == Items.FocusedIndex && start != end && e.KeyCode.In (Keys.Up, Keys.Left)) {
                            Items.RemoveSelectedIndex (end);
                            Items.FocusedIndex--;
                            EnsureItemVisible (Items.FocusedIndex);
                            e.Handled = true;
                            return;
                        }
                    }
                }

                // If any direction is pressed and nothing is selected, select the item with focus.
                // This is generally used to select the first item when you tab into an listbox
                // with nothing currently selected.
                if (e.KeyCode.In (Keys.Down, Keys.Right, Keys.Up, Keys.Left) && SelectedIndex == -1) {
                    SelectedIndex = Items.FocusedIndex;
                    EnsureItemVisible (Items.FocusedIndex);
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode.In (Keys.Down, Keys.Right)) {
                    if (SelectedIndex < Items.Count - 1) {
                        SelectedIndex = Items.FocusedIndex + 1;
                        EnsureItemVisible (Items.FocusedIndex);
                        e.Handled = true;
                        return;
                    }
                }


                if (e.KeyCode.In (Keys.Up, Keys.Left)) {
                    if (SelectedIndex > 0) {
                        SelectedIndex = Items.FocusedIndex - 1;
                        EnsureItemVisible (Items.FocusedIndex);
                        e.Handled = true;
                        return;
                    }
                }

                if (char.IsLetterOrDigit ((char)e.KeyCode)) {
                    var index = FindString (((char)e.KeyCode).ToString (), SelectedIndex);

                    if (index >= 0) {
                        SelectedIndex = index;
                        EnsureItemVisible (Items.FocusedIndex);
                        e.Handled = true;
                        return;
                    }
                }
            }

            base.OnKeyUp (e);
        }

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
                case SelectionMode.None:
                    Items.FocusedIndex = index;
                    break;

                case SelectionMode.One:
                    SelectedIndex = index;
                    break;

                case SelectionMode.MultiSimple:
                    Items.ToggleSelectedIndex (index);
                    break;

                case SelectionMode.MultiExtended:
                    // TODO: Shift

                    // When Control is held we treat this like MultiSimple
                    if (e.Control) {
                        Items.ToggleSelectedIndex (index);
                        break;
                    }

                    // Else we treat this like SelectionMode.One
                    SelectedIndex = index;

                    break;
            }

            EnsureItemVisible (index);
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

                if (Items.SelectedIndex != value || Items.SelectedIndexes.Count > 1) {
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
        /// Gets all currently selected items.
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
                vscrollbar.LargeChange = Math.Max (0, VisibleItemCount);
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
