using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Modern.Forms
{
    // TODO: Update selected indexes when adding/removing items
    public class ListBoxItemCollection : IList<object>
    {
        private readonly List<object> items = new List<object> ();
        private readonly ListBox owner;
        private int suspend_update;

        internal ListBoxItemCollection (ListBox owner)
        {
            this.owner = owner;
        }

        public object this[int index] {
            get => items[index];
            set {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index));

                items[index] = value;

                OnCollectionChanged ();
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (object item)
        {
            Insert (Count, item);
        }

        public void AddRange (params object[] items)
        {
            SuspendUpdate ();

            foreach (var item in items)
                Insert (Count, item);

            ResumeUpdate ();
        }

        public void Clear ()
        {
            SuspendUpdate ();

            while (items.Count > 0)
                RemoveAt (0);

            ResumeUpdate ();
        }

        public bool Contains (object item) => items.Contains (item);

        public void CopyTo (object[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<object> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (object item) => items.IndexOf (item);

        public void Insert (int index, object item)
        {
            items.Insert (index, item);

            OnCollectionChanged ();
        }

        public bool Remove (object item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            OnCollectionChanged ();

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            items.RemoveAt (index);
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

        internal object SelectedItem {
            get => SelectedIndexes.Count > 0 ? items[SelectedIndexes[0]] : null;
            set => SelectedIndex = value == null ? -1 : items.IndexOf (value);
        }

        internal IEnumerable<object> SelectedItems => SelectedIndexes.Select (i => items[i]);

        private void OnCollectionChanged ()
        {
            if (suspend_update == 0)
                owner.CollectionChanged ();
        }

        private void ResumeUpdate ()
        {
            suspend_update--;

            OnCollectionChanged ();
        }

        private void SuspendUpdate ()
        {
            suspend_update++;
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();
    }
}
