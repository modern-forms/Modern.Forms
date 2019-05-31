using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItemGroupCollection : Collection<RibbonItemGroup>
    {
        private readonly RibbonTabPage owner;

        internal RibbonItemGroupCollection (RibbonTabPage owner)
        {
            this.owner = owner;
        }

        public RibbonItemGroup Add (string text)
        {
            var item = new RibbonItemGroup {
                Text = text
            };

            Add (item);

            return item;
        }

        protected override void InsertItem (int index, RibbonItemGroup item)
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

        protected override void SetItem (int index, RibbonItemGroup item)
        {
            var old_item = this.ElementAtOrDefault (index);

            if (old_item != null)
                old_item.Owner = null;

            base.SetItem (index, item);

            item.Owner = owner;
        }
    }
}
