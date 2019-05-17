using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItemCollection : IList<RibbonItem>
    {
        private readonly List<RibbonItem> items = new List<RibbonItem> ();
        private readonly RibbonItemGroup owner;

        internal RibbonItemCollection (RibbonItemGroup owner)
        {
            this.owner = owner;
        }

        public RibbonItem this[int index] {
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

        public void Add (RibbonItem item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public RibbonItem Add (string text, SKBitmap image = null)
        {
            var item = new RibbonItem {
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

        public bool Contains (RibbonItem item) => items.Contains (item);

        public void CopyTo (RibbonItem[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<RibbonItem> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (RibbonItem item) => items.IndexOf (item);

        public void Insert (int index, RibbonItem item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (RibbonItem item)
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

        private void SetUpItem (RibbonItem item)
        {
            item.Owner = owner;
        }
    }
}
