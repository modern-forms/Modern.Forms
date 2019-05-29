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

                owner.CollectionChanged ();
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (object item)
        {
            items.Add (item);

            owner.CollectionChanged ();
        }

        public void AddRange (params object[] item)
        {
            items.AddRange (item);

            owner.CollectionChanged ();
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

            owner.CollectionChanged ();
        }

        public bool Remove (object item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            owner.CollectionChanged ();

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            items.RemoveAt (index);
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();
    }
}
