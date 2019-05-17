using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItemGroupCollection : IList<RibbonItemGroup>
    {
        private readonly List<RibbonItemGroup> items = new List<RibbonItemGroup> ();
        private readonly RibbonTabPage owner;

        internal RibbonItemGroupCollection (RibbonTabPage owner)
        {
            this.owner = owner;
        }

        public RibbonItemGroup this[int index] {
            get => items[index];
            set {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index));

                items[index] = value;
                SetUpItem (value);
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (RibbonItemGroup item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public RibbonItemGroup Add (string text)
        {
            var item = new RibbonItemGroup {
                Text = text
            };

            Add (item);

            return item;
        }

        public void Clear ()
        {
            while (items.Count > 0)
                RemoveAt (0);
        }

        public bool Contains (RibbonItemGroup item) => items.Contains (item);

        public void CopyTo (RibbonItemGroup[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<RibbonItemGroup> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (RibbonItemGroup item) => items.IndexOf (item);

        public void Insert (int index, RibbonItemGroup item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (RibbonItemGroup item)
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

        private void SetUpItem (RibbonItemGroup item)
        {
            item.Owner = owner;
        }
    }
}
