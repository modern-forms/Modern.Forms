using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of TabPages.
    /// </summary>
    public class TabPageCollection : Collection<TabPage>
    {
        private readonly TabControl owner;
        private readonly TabStrip tab_strip;

        internal TabPageCollection (TabControl owner, TabStrip tabStrip)
        {
            this.owner = owner;
            tab_strip = tabStrip;
        }

        /// <summary>
        /// Adds the TabPage to the collection.
        /// </summary>
        public new TabPage Add (TabPage item)
        {
            base.Add (item);

            return item;
        }

        /// <summary>
        /// Adds a new TabPage to the collection with the specified text.
        /// </summary>
        public TabPage Add (string text) => Add (new TabPage (text));

        protected override void InsertItem (int index, TabPage item)
        {
            base.InsertItem (index, item);

            item.Visible = false;
            owner.Controls.Insert (index, item);
            tab_strip.Tabs.Insert (index, item.TabStripItem);
        }

        protected override void RemoveItem (int index)
        {
            base.RemoveItem (index);
            
            owner.Controls.RemoveAt (index);
            tab_strip.Tabs.RemoveAt (index);
        }

        protected override void SetItem (int index, TabPage item)
        {
            base.SetItem (index, item);

            item.Visible = false;
            owner.Controls[index] = item;
            tab_strip.Tabs[index] = item.TabStripItem;
        }
    }
}
