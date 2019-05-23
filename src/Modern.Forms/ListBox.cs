using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

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
        private List<int> selected_items = new List<int> ();
        private SelectionMode selection_mode = SelectionMode.One;

        public ListBox ()
        {
            Items = new ListBoxItemCollection (this);
        }

        public event EventHandler<EventArgs<TreeViewItem>> ItemSelected;

        public ListBoxItemCollection Items { get; }

        public Rectangle GetItemRectangle (int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException ("Index out of range.");

            var client = ClientRectangle;

            return new Rectangle (client.Left, index * ItemHeight + client.Top, client.Width, ItemHeight);
        }

        public int GetIndexAtLocation (Point location)
        {
            // Inefficient?
            for (var i = 0; i < Items.Count; i++)
                if (GetItemRectangle (i).Contains (location))
                    return i;

            return -1;
        }

        public int ItemHeight {
            get {
                if (item_height == -1)
                    item_height = (int)TextMeasurer.MeasureText ("The quick brown Fox", Style).Height + 7;

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
            get => selected_items.Count > 0 ? selected_items[0] : -1;
            set {
                if (value < -1 || value >= Items.Count)
                    throw new ArgumentOutOfRangeException ("Index of out range");

                if (SelectionMode == SelectionMode.None)
                    throw new ArgumentException ("cannot call this method if SelectionMode is SelectionMode.None");

                selected_items.Clear ();

                if (value != -1)
                    selected_items.Add (value);

                Invalidate ();
            }
        }

        public object SelectedItem {
            get => selected_items.Count > 0 ? Items[selected_items[0]] : null;
            set {
                if (value != null && !Items.Contains (value))
                    throw new ArgumentException ("Item is not part of this list");

                SelectedIndex = value == null ? -1 : Items.IndexOf (value);
            }
        }

        public IEnumerable<object> SelectedItems => selected_items.Select (i => Items[i]);

        public SelectionMode SelectionMode {
            get => selection_mode;
            set {
                if (!Enum.IsDefined (typeof (SelectionMode), value))
                    throw new InvalidEnumArgumentException ($"Enum argument value '{value}' is not valid for SelectionMode");

                if (selection_mode == value)
                    return;

                selection_mode = value;

                // TODO: May need to clear selections if now allowing fewer
            }
        }

        protected override Size DefaultSize => new Size (120, 96);

        protected virtual void OnItemSelected (EventArgs<TreeViewItem> e) => ItemSelected?.Invoke (this, e);

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
                    if (selected_items.Contains (index))
                        selected_items.Remove (index);
                    else
                        selected_items.Add (index);

                    Invalidate ();

                    break;
                case SelectionMode.MultiExtended:
                    // TODO: Shift

                    // When Control is held we treat this like MultiSimple
                    if (e.Control) {
                        if (selected_items.Contains (index))
                            selected_items.Remove (index);
                        else
                            selected_items.Add (index);

                        Invalidate ();
                        break;
                    }

                    // Else we treat this like SelectionMode.One
                    SelectedIndex = index;

                    break;
            }
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            for (var i = 0; i < Items.Count; i++) {
                var item = Items[i];
                var bounds = GetItemRectangle (i);

                if (selected_items.Contains (i))
                    e.Canvas.FillRectangle (bounds, Theme.NeutralGray);

                bounds.Inflate (-4, 0);
                e.Canvas.DrawText (item.ToString (), bounds, Style, ContentAlignment.MiddleLeft);
            }
        }
    }
}
