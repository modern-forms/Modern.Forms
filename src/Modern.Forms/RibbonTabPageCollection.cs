using System;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonTabPageCollection : Collection<RibbonTabPage>
    {
        private readonly Ribbon owner;

        internal RibbonTabPageCollection (Ribbon owner)
        {
            this.owner = owner;
        }

        public RibbonTabPage Add (string text)
        {
            var item = new RibbonTabPage {
                Text = text
            };

            Add (item);

            return item;
        }

        protected override void InsertItem (int index, RibbonTabPage item)
        {
            base.InsertItem (index, item);

            item.Owner = owner;
            owner.TabStrip.Tabs.Insert (index, CreateTabStripItem (item));

            owner.Invalidate ();
        }

        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.Owner = null;
            owner.TabStrip.Tabs.RemoveAt (index);

            owner.Invalidate ();
        }

        protected override void SetItem (int index, RibbonTabPage item)
        {
            var old_item = this.ElementAtOrDefault (index);

            if (old_item != null)
                old_item.Owner = null;

            base.SetItem (index, item);

            item.Owner = owner;
            owner.TabStrip.Tabs[index] = CreateTabStripItem (item);

            owner.Invalidate ();
        }

        private TabStripItem CreateTabStripItem (RibbonTabPage item) => new TabStripItem { Text = item.Text, Tag = item };
    }
}
