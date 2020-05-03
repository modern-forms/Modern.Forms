using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Modern.Forms
{
    // TODO: Update selected indexes when adding/removing items
    /// <summary>
    /// Represents a collection of items for ListBox.
    /// </summary>
    public class ListBoxItemCollection : ObservableCollection<object>
    {
        private readonly ListBox owner;
        private int hovered_index = -1;

        internal ListBoxItemCollection (ListBox owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds a collection of items to the collection.
        /// </summary>
        public void AddRange (params object[] items)
        {
            owner.SuspendLayout ();

            foreach (var item in items)
                Add (item);

            owner.ResumeLayout (true);
        }

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
        protected override void OnCollectionChanged (NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged (e);

            owner.Invalidate ();
        }

        internal int SelectedIndex {
            get => SelectedIndexes.Count > 0 ? SelectedIndexes[0] : -1;
            set {
                if (value < -1 || value >= Count)
                    throw new ArgumentOutOfRangeException ("Index out of range");

                SelectedIndexes.Clear ();
                
                if (value != -1)
                    SelectedIndexes.Add (value);
            }
        }

        internal List<int> SelectedIndexes { get; } = new List<int> ();

        internal object? SelectedItem {
            get => SelectedIndexes.Count > 0 ? this[SelectedIndexes[0]] : null;
            set {
                if (value is null) {
                    SelectedIndex = -1;
                    return;
                }

                var index = IndexOf (value);

                if (index == -1)
                    throw new ArgumentException ("Item is not part of this list");

                SelectedIndex = index;
            }
        }

        internal IEnumerable<object> SelectedItems => SelectedIndexes.Select (i => this[i]);
    }
}
