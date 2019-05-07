using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteTreeViewItemCollection : IList<LiteTreeViewItem>
    {
        private readonly List<LiteTreeViewItem> items = new List<LiteTreeViewItem> ();
        private readonly LiteTreeView owner;

        internal LiteTreeViewItemCollection (LiteTreeView owner)
        {
            this.owner = owner;
        }

        public LiteTreeViewItem this[int index] {
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

        public void Add (LiteTreeViewItem item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public LiteTreeViewItem Add (string text)
        {
            var item = new LiteTreeViewItem {
                Text = text
            };

            Add (item);

            return item;
        }

        public LiteTreeViewItem Add (string text, SKBitmap image)
        {
            var item = new LiteTreeViewItem {
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

        public bool Contains (LiteTreeViewItem item) => items.Contains (item);

        public void CopyTo (LiteTreeViewItem[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<LiteTreeViewItem> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (LiteTreeViewItem item) => items.IndexOf (item);

        public void Insert (int index, LiteTreeViewItem item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (LiteTreeViewItem item)
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
            var item = items[index];
            var parent = item.Parent;

            item.Parent = null;
            items.RemoveAt (index);

            if (parent != null)
                parent.Invalidate ();
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();

        private void SetUpItem (LiteTreeViewItem item)
        {
            item.Parent = owner;

            owner.Invalidate ();
        }
    }
}
