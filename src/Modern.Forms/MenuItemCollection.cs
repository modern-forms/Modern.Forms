using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class MenuItemCollection : Collection<MenuItem>
    {
        private readonly MenuItem owner;

        internal MenuItemCollection (MenuItem owner)
        {
            this.owner = owner;
        }

        public T Add<T> (T item) where T : MenuItem
        {
            base.Add (item);
            return item;
        }

        public MenuItem Add (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            return Add (new MenuItem (text, image, onClick));
        }

        protected override void InsertItem (int index, MenuItem item)
        {
            base.InsertItem (index, item);

            item.Parent = owner;
        }

        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.Parent = null;
        }

        protected override void SetItem (int index, MenuItem item)
        {
            var old_item = this.ElementAtOrDefault (index);

            if (old_item != null)
                old_item.Parent = null;

            base.SetItem (index, item);

            item.Parent = owner;
        }
    }
}
