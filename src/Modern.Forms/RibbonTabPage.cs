using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a RibbonTabPage used in a Ribbon control.
    /// </summary>
    public class RibbonTabPage
    {
        /// <summary>
        /// Initializes a new instance of the RibbonTabPage class.
        /// </summary>
        internal RibbonTabPage (string text, Ribbon owner)
        {
            Groups = new RibbonItemGroupCollection (this);
            TabStripItem = new TabStripItem (text);
            Owner = owner;
        }

        // This is the bounds for the tab page.
        internal Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets the collection of ribbon item groups contained by this tab page.
        /// </summary>
        public RibbonItemGroupCollection Groups { get; }

        // Lays out each group.
        internal void LayoutItems ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (Bounds, Groups.Cast<ILayoutable> ());
        }

        // The ribbon that contains this page.
        internal Ribbon Owner { get; set; }

        /// <summary>
        /// Gets a value indicating this RibbonTabPage is currently selected. 
        /// </summary>
        public bool Selected { get; internal set; }

        /// <summary>
        /// Sets the bounding box of the tab page. This is internal API and should not be called.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        // The TabStripItem that accompanies this RibbonTabPage.
        internal TabStripItem TabStripItem { get; }

        /// <summary>
        /// Get or sets the text of this RibbonTabPage's tab.
        /// </summary>
        public string Text {
            get => TabStripItem.Text;
            set => TabStripItem.Text = value;
        }
    }
}
