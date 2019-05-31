using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItemCollection : Collection<RibbonItem>
    {
        private readonly RibbonItemGroup owner;

        internal RibbonItemCollection (RibbonItemGroup owner)
        {
            this.owner = owner;
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
