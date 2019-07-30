using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Modern.Forms
{
    // TODO: Update selected indexes when adding/removing items
    public class ListBoxItemCollection : ObservableCollection<object>
    {
        private readonly ListBox owner;

        internal ListBoxItemCollection (ListBox owner)
        {
            this.owner = owner;
        }

        public void AddRange (params object[] items)
        {
            owner.SuspendLayout ();

            foreach (var item in items)
                Add (item);

            owner.ResumeLayout (true);
        }

        internal int SelectedIndex {
            get => SelectedIndexes.Count > 0 ? SelectedIndexes[0] : -1;
            set {
                SelectedIndexes.Clear ();
                
                if (value != -1)
                    SelectedIndexes.Add (value);
            }
        }

        internal List<int> SelectedIndexes { get; } = new List<int> ();

        internal object? SelectedItem {
            get => SelectedIndexes.Count > 0 ? this[SelectedIndexes[0]] : null;
            set => SelectedIndex = value == null ? -1 : IndexOf (value);
        }

        internal IEnumerable<object> SelectedItems => SelectedIndexes.Select (i => this[i]);
    }
}
