using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of RibbonItemGroups.
    /// </summary>
    public class RibbonItemGroupCollection : Collection<RibbonItemGroup>
    {
        private readonly RibbonTabPage owner;

        internal RibbonItemGroupCollection (RibbonTabPage owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Creates a new RibbonItemGroup and add it to the collection.
        /// </summary>
        public RibbonItemGroup Add () => Add (string.Empty);

        /// <summary>
        /// Creates a new RibbonItemGroup with the specified text and adds it to the collection.
        /// </summary>
        public RibbonItemGroup Add (string text)
        {
            var item = new RibbonItemGroup (text, owner);

            base.Add (item);
            return item;
        }

        protected override void InsertItem (int index, RibbonItemGroup item)
        {
            base.InsertItem (index, item);

            item.Owner = owner;
        }

        protected override void SetItem (int index, RibbonItemGroup item)
        {
            base.SetItem (index, item);

            item.Owner = owner;
        }
    }
}
