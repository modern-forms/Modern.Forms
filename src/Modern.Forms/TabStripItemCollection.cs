using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    public class TabStripItemCollection : Collection<TabStripItem>
    {
        private readonly TabStrip tab_strip;

        internal TabStripItemCollection (TabStrip tabStrip)
        {
            tab_strip = tabStrip;
        }

        public TabStripItem Add (string text)
        {
            var item = new TabStripItem { Parent = tab_strip, Text = text };
            Add (item);

            return item;
        }

        protected override void InsertItem (int index, TabStripItem item)
        {
            item.Parent = tab_strip;

            base.InsertItem (index, item);

            if (Count == 1)
                tab_strip.SelectedTab = item;
            else
                tab_strip.Invalidate ();
        }

        protected override void RemoveItem (int index)
        {
            this[index].Parent = null;

            base.RemoveItem (index);

            if (tab_strip.SelectedTab == null && Count > 0)
                tab_strip.SelectedTab = this[0];
            else
                tab_strip.Invalidate ();
        }

        protected override void SetItem (int index, TabStripItem item)
        {
            this[index].Parent = null;
            item.Parent = tab_strip;

            base.SetItem (index, item);
        }
    }
}
