using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItemCollection : Collection<TreeViewItem>
    {
        private readonly TreeViewItem owner;

        internal TreeViewItemCollection (TreeViewItem owner)
        {
            this.owner = owner;
        }

        public T Add<T> (T item) where T : TreeViewItem
        {
            base.Add (item);
            return item;
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

        public void AddRange (IEnumerable<TreeViewItem> children)
        {
            foreach (var item in children)
                Add (item);
        }

        protected override void InsertItem (int index, TreeViewItem item)
        {
            base.InsertItem (index, item);

            item.Parent = owner;
            owner.Invalidate ();
        }

        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.Parent = null;
            owner.Invalidate ();
        }

        protected override void SetItem (int index, TreeViewItem item)
        {
            var old_item = this.ElementAtOrDefault (index);

            if (old_item != null)
                old_item.Parent = null;

            base.SetItem (index, item);

            item.Parent = owner;
            owner.Invalidate ();
        }
    }
}
