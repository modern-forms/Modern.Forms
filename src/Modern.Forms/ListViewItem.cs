using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ListViewItem.
    /// </summary>
    public class ListViewItem
    {
        /// <summary>
        /// Gets the current bounding box of the item.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets or sets the image displayed on the item.
        /// </summary>
        public SKBitmap? Image { get; set; }

        /// <summary>
        /// Gets the ListView this item is currently a part of.
        /// </summary>
        public ListView? Parent { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating if the item is currently selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Sets the bounding box of the item. This is internal API and should not be called.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        /// <summary>
        /// Gets or sets an object with additional user data about this item.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the text displayed on the item.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
