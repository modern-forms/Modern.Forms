using System;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a TabPage control.
    /// </summary>
    public class TabPage : Panel
    {
        /// <summary>
        /// Initializes a new instance of the TabPage class.
        /// </summary>
        public TabPage ()
        {
            Dock = DockStyle.Fill;
            TabStripItem = new TabStripItem ();
        }

        /// <summary>
        /// Initializes a new instance of the TabPage class with the specified text.
        /// </summary>
        public TabPage (string text) : this ()
        {
            TabStripItem.Text = text;
        }

        // The TabStripItem that accompanies this TabPage.
        internal TabStripItem TabStripItem { get; }

        /// <inheritdoc/>
        public override string Text { 
            get => TabStripItem.Text; 
            set => TabStripItem.Text = value;
        }
    }
}
