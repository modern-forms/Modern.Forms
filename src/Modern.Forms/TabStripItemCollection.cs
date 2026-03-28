using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of TabStripItems.
    /// </summary>
    public class TabStripItemCollection : Collection<TabStripItem>
    {
        private readonly TabStrip owner;
        private int focused_index = 0;
        private int hovered_index = -1;
        private int selected_index = -1;

        internal TabStripItemCollection (TabStrip owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds the TabStripItem to the collection.
        /// </summary>
        public new TabStripItem Add (TabStripItem item)
        {
            item.Parent = owner;
            base.Add (item);

            return item;
        }

        /// <summary>
        /// Adds a new TabStripItem to the collection with the specified text.
        /// </summary>
        public TabStripItem Add (string text) => Add (new TabStripItem { Text = text });

        internal int FocusedIndex {
            get => focused_index;
            set {
                if (focused_index != value) {
                    focused_index = value;
                    owner.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the tab the mouse is currently hovered over.
        /// </summary>
        internal int HoveredIndex {
            get => hovered_index;
            set {
                if (hovered_index != value) {
                    hovered_index = value;
                    owner.Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, TabStripItem item)
        {
            item.Parent = owner;

            base.InsertItem (index, item);

            if (Count == 1)
                owner.SelectedTab = item;
            else
                owner.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            var item = this[index];

            item.Parent = null;

            var selected_tab = owner.SelectedTab;

            base.RemoveItem (index);

            if (selected_tab == item && Count > 0) {
                // Need to temporarily set this to nothing in case the index doesn't change,
                // we still want to force it to be treated like a new selection.
                selected_index = -1;
                owner.SelectedIndex = Math.Max (index - 1, 0);
            }

            if (selected_tab is null && Count > 0)
                owner.SelectedIndex = 0;
            else
                owner.Invalidate ();
        }

        /// <summary>
        /// Gets or sets the index of the currently selected tab.
        /// </summary>
        internal int SelectedIndex {
            get => selected_index;
            set {
                if (value < -1 || value >= Count)
                    throw new ArgumentOutOfRangeException ("Index out of range");

                if (selected_index != value) {
                    selected_index = value;
                    focused_index = value;
                }
            }
        }

        /// <inheritdoc/>
        protected override void SetItem (int index, TabStripItem item)
        {
            this[index].Parent = null;
            item.Parent = owner;

            base.SetItem (index, item);
        }
    }
}
