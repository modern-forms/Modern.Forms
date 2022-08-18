using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a RibbonItemGroup used in a RibbonTabPage control.
    /// </summary>
    public class RibbonItemGroup : ILayoutable
    {
        /// <summary>
        /// Initializes a new instance of the RibbonItemGroup class.
        /// </summary>
        internal RibbonItemGroup (RibbonTabPage owner)
        {
            Items = new MenuItemCollection (new MenuRootItem (owner.Owner));
            Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the RibbonItemGroup class with the specified text.
        /// </summary>
        internal RibbonItemGroup (string text, RibbonTabPage owner) : this (owner)
        {
            Text = text;
        }

        // This is the bounds for the item group.
        internal Rectangle Bounds { get; private set; }

        /// <summary>
        /// Returns a preferred size the group would like to be.
        /// </summary>
        public Size GetPreferredSize (Size proposedSize)
        {
            var width = Padding.Horizontal;

            foreach (var item in Items)
                width += item.GetPreferredSize (Size.Empty).Width;

            return new Size (width, 0);
        }

        /// <summary>
        /// Gets the collection of ribbon items contained by this group.
        /// </summary>
        public MenuItemCollection Items { get; }

        /// <summary>
        /// Gets the amount of spacing to leave between instances of this group.
        /// </summary>
        public Padding Margin => Padding.Empty;

        // The RibbonTabPage that contains this group.
        internal RibbonTabPage Owner { get; set; }

        // The space available for items, taking Padding into account.
        private Rectangle PaddedBounds {
            get {
                var x = Bounds.Left + Padding.Left;
                var y = Bounds.Top + Padding.Top;
                var w = Bounds.Width - Padding.Horizontal;
                var h = Bounds.Height - Padding.Vertical;
                return new Rectangle (x, y, w, h);
            }
        }

        // The amount of padding to leave between the group's bounds and the contained items' bounds.
        private Padding Padding => new Padding (3, 3, 4, 3);

        /// <summary>
        /// Sets the bounding box of the tab page. This is internal API and should not be called.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);

            // Lay out RibbonItems
            StackLayoutEngine.HorizontalExpand.Layout (PaddedBounds, Items.Cast<ILayoutable> ());
        }

        /// <summary>
        /// Get or sets the text of this RibbonItemGroup.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
