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
        private int focused_index = 0;
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

        internal void AddSelectedIndex (int index, bool single)
        {
            if (single)
                SelectedIndexes.Clear ();

            focused_index = Math.Max (index, 0);

            if (index != -1)
                SelectedIndexes.Add (index);

            owner.Invalidate ();
        }

        internal int FocusedIndex {
            get => focused_index;
            set {
                if (focused_index != value) {
                    focused_index = value;
                    owner.Invalidate ();
                }
            }
        }

        internal (int start, int end) GetSingleContiguousSelection ()
        {
            if (SelectedIndexes.Count == 0)
                return (-1, -1);

            if (SelectedIndexes.Count == 1)
                return (SelectedIndex, SelectedIndex);

            var indexes = SelectedIndexes.OrderBy (p => p).ToList ();

            if (indexes.Last () - indexes.First () + 1 == indexes.Count)
                return (indexes.First (), indexes.Last ());

            return (-1, -1);
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

        internal void RemoveSelectedIndex (int index)
        {
            focused_index = Math.Max (index, 0);

            SelectedIndexes.Remove (index);

            owner.Invalidate ();
        }

        internal int SelectedIndex {
            get => SelectedIndexes.Count > 0 ? SelectedIndexes[0] : -1;
            set {
                if (value < -1 || value >= Count)
                    throw new ArgumentOutOfRangeException ("Index out of range");

                AddSelectedIndex (value, true);
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

        internal void ToggleSelectedIndex (int index)
        {
            if (SelectedIndexes.Contains (index))
                RemoveSelectedIndex (index);
            else
                AddSelectedIndex (index, false);
        }
    }
}
