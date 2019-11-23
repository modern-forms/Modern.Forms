using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Modern.Forms
{
    public class ListBox : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.LightNeutralGray;
                style.Border.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private int item_height = -1;
        private SelectionMode selection_mode = SelectionMode.One;
        private int top_index = 0;
        private readonly VerticalScrollBar vscrollbar;

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

        public event EventHandler? SelectedIndexChanged;

        public ListBoxItemCollection Items { get; }

        public Rectangle GetItemRectangle (int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException ("Index out of range.");

            var client = ClientRectangle;

            return new Rectangle (client.Left, index * ScaledItemHeight + client.Top, client.Width - (vscrollbar.Visible ? vscrollbar.ScaledWidth : 0), ScaledItemHeight);
        }

        public int GetIndexAtLocation (Point location)
        {
            for (var i = top_index; i < Math.Min (Items.Count, top_index + VisibleItemCount + 1); i++)
                if (GetItemDisplayRectangle (i).Contains (location))
                    return i;

            return -1;
        }

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

        public int SelectedIndex {
            get => Items.SelectedIndex;
            set {
                if (value < -1 || value >= Items.Count)
                    throw new ArgumentOutOfRangeException ("Index of out range");

                if (SelectionMode == SelectionMode.None)
                    throw new ArgumentException ("Cannot call this method if SelectionMode is SelectionMode.None");

                Items.SelectedIndex = value;
                OnSelectedIndexChanged (EventArgs.Empty);

                Invalidate ();
            }
        }

        public object? SelectedItem {
            get => Items.SelectedItem;
            set {
                if (value != null && !Items.Contains (value))
                    throw new ArgumentException ("Item is not part of this list");

                Items.SelectedItem = value;
            }
        }

        public IEnumerable<object> SelectedItems => Items.SelectedItems;

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

        protected override Size DefaultSize => new Size (120, 96);

        protected virtual void OnSelectedIndexChanged (EventArgs e) => SelectedIndexChanged?.Invoke (this, e);

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

        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (vscrollbar.Visible)
                vscrollbar.RaiseMouseWheel (e);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            for (var i = top_index; i < Math.Min (Items.Count, top_index + VisibleItemCount + 1); i++) {
                var item = Items[i];
                var bounds = GetItemDisplayRectangle (i);

                if (Items.SelectedIndexes.Contains (i))
                    e.Canvas.FillRectangle (bounds, Theme.NeutralGray);

                // This fixes text positioning for partially shown items
                bounds.Height = ScaledItemHeight;

                bounds.Inflate (-4, 0);

                e.Canvas.DrawText (item.ToString (), bounds, this, ContentAlignment.MiddleLeft);
            }
        }

        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateVerticalScrollBar ();
        }

        private Rectangle GetItemDisplayRectangle (int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException ("Index out of range.");

            var client = ClientRectangle;

            index -= top_index;

            var top = index * ScaledItemHeight + client.Top;
            return new Rectangle (client.Left, top, client.Width - (vscrollbar.Visible ? vscrollbar.ScaledWidth : 0), Math.Min (ScaledItemHeight, client.Bottom - top));
        }

        private int NeededHeightForItems => ScaledItemHeight * Items.Count;

        private int ScaledItemHeight => LogicalToDeviceUnits (ItemHeight);

        private void UpdateVerticalScrollBar ()
        {
            if (Items.Count == 0)
                vscrollbar.Visible = false;

            if (NeededHeightForItems > Bounds.Height) {
                vscrollbar.Visible = true;
                vscrollbar.Maximum = Items.Count - VisibleItemCount;
                vscrollbar.LargeChange = VisibleItemCount;
            } else {
                vscrollbar.Visible = false;
            }
        }

        private void VerticalScrollBar_ValueChanged (object sender, EventArgs e)
        {
            top_index = Math.Max (vscrollbar.Value, 0);

            Invalidate ();
        }

        private int VisibleItemCount => ClientRectangle.Height / ScaledItemHeight;
    }
}
