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
        private int hovered_index = -1;
        private int selected_index = -1;

        internal TabStripItemCollection (TabStrip owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds a new TabStripItem to the collection with the specified text.
        /// </summary>
        public TabStripItem Add (string text) => Add (new TabStripItem { Text = text });

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
            this[index].Parent = null;

            base.RemoveItem (index);

            if (owner.SelectedTab == null && Count > 0)
                owner.SelectedTab = this[0];
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

                if (selected_index != value)
                    selected_index = value;
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
