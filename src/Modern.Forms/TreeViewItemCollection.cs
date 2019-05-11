using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItemCollection : IList<TreeViewItem>
    {
        private readonly List<TreeViewItem> items = new List<TreeViewItem> ();
        private readonly TreeView owner;

        internal TreeViewItemCollection (TreeView owner)
        {
            this.owner = owner;
        }

        public TreeViewItem this[int index] {
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

        public void Add (TreeViewItem item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public TreeViewItem Add (string text)
        {
            var item = new TreeViewItem {
                Text = text
            };

            Add (item);

            return item;
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

        public bool Contains (TreeViewItem item) => items.Contains (item);

        public void CopyTo (TreeViewItem[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<TreeViewItem> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (TreeViewItem item) => items.IndexOf (item);

        public void Insert (int index, TreeViewItem item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (TreeViewItem item)
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

        private void SetUpItem (TreeViewItem item)
        {
            item.Parent = owner;

            owner.Invalidate ();
        }
    }
}
