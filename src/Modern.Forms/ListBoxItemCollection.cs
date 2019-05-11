using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class ListBoxItemCollection : IList<object>
    {
        private readonly List<object> items = new List<object> ();
        private readonly ListBox owner;

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
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (object item)
        {
            items.Add (item);
        }

        public TreeViewItem Add (string text, SKBitmap image)
        {
            var item = new TreeViewItem {
                Text = text,
                Image = image
            };

            Add (item);

            return item;
        }

        public void Clear ()
        {
            while (items.Count > 0)
                RemoveAt (0);
        }

        public bool Contains (object item) => items.Contains (item);

        public void CopyTo (object[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<object> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (object item) => items.IndexOf (item);

        public void Insert (int index, object item)
        {
            items.Insert (index, item);
        }

        public bool Remove (object item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            items.RemoveAt (index);
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();
    }
}
