using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of RibbonItems.
    /// </summary>
    public class RibbonItemCollection : Collection<RibbonItem>
    {
        private readonly RibbonItemGroup owner;

        internal RibbonItemCollection (RibbonItemGroup owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds the RibbonItem to the collection.
        /// </summary>
        public T Add<T> (T item) where T : RibbonItem
        {
            base.Add (item);
            return item;
        }

        /// <summary>
        /// Adds a new RibbonItem to the collection with the specified text and optional image.
        /// </summary>
        public RibbonItem Add (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            return Add (new RibbonItem (text, image, onClick));
        }

        protected override void InsertItem (int index, RibbonItem item)
        {
            base.InsertItem (index, item);

            item.Owner = owner;
        }

        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.Owner = null;
        }

        protected override void SetItem (int index, RibbonItem item)
        {
            var old_item = this.ElementAtOrDefault (index);

            if (old_item != null)
                old_item.Owner = null;

            base.SetItem (index, item);

            item.Owner = owner;
        }
    }
}
