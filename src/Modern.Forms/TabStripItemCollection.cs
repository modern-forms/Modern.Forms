using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    public class TabStripItemCollection : Collection<TabStripItem>
    {
        private TabStrip tab_strip;

        internal TabStripItemCollection (TabStrip tabStrip)
        {
            tab_strip = tabStrip;
        }

        public TabStripItem Add (string text)
        {
            var item = new TabStripItem { Text = text };
            Add (item);

            return item;
        }

        protected override void InsertItem (int index, TabStripItem item)
        {
            base.InsertItem (index, item);

            if (Count == 1)
                tab_strip.SelectedTab = item;
            else
                tab_strip.Invalidate ();
        }

        protected override void RemoveItem (int index)
        {
            base.RemoveItem (index);

            if (tab_strip.SelectedTab == null && Count > 0)
                tab_strip.SelectedTab = this[0];
            else
                tab_strip.Invalidate ();
        }
    }
}
