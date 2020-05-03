using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of MenuItems.
    /// </summary>
    public class MenuItemCollection : Collection<MenuItem>
    {
        private readonly MenuItem owner;

        internal MenuItemCollection (MenuItem owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds the MenuItem to the collection.
        /// </summary>
        public T Add<T> (T item) where T : MenuItem
        {
            base.Add (item);
            return item;
        }

        /// <summary>
        /// Adds a new MenuItem to the collection with the specified text, image, and Click handler.
        /// </summary>
        public MenuItem Add (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            return Add (new MenuItem (text, image, onClick));
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, MenuItem item)
        {
            base.InsertItem (index, item);

            item.Parent = owner;
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.Parent = null;
        }

        /// <inheritdoc/>
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
