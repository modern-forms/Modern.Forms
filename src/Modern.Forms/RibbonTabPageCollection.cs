using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of RibbonTabPages.
    /// </summary>
    public class RibbonTabPageCollection : Collection<RibbonTabPage>
    {
        private readonly Ribbon owner;
        private readonly TabStrip tab_strip;

        internal RibbonTabPageCollection (Ribbon owner, TabStrip tabStrip)
        {
            this.owner = owner;
            tab_strip = tabStrip;
        }

        /// <summary>
        /// Create a new RibbonTabPage and adds it to the collection.
        /// </summary>
        public RibbonTabPage Add () => Add (string.Empty);

        /// <summary>
        /// Create a new RibbonTabPage with the specified text and adds it to the collection.
        /// </summary>
        public RibbonTabPage Add (string text)
        {
            var item = new RibbonTabPage (text, owner);

            base.Add (item);
            return item;
        }

        protected override void InsertItem (int index, RibbonTabPage item)
        {
            base.InsertItem (index, item);

            item.Owner = owner;
            tab_strip.Tabs.Insert (index, item.TabStripItem);

            owner.Invalidate ();
        }

        protected override void RemoveItem (int index)
        {
            base.RemoveItem (index);

            tab_strip.Tabs.RemoveAt (index);

            owner.Invalidate ();
        }

        protected override void SetItem (int index, RibbonTabPage item)
        {
            base.SetItem (index, item);

            item.Owner = owner;
            tab_strip.Tabs[index] = item.TabStripItem;

            owner.Invalidate ();
        }
    }
}
