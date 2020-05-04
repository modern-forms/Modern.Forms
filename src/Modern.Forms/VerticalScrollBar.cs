using System;
using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a VerticalScrollBar control.
    /// </summary>
    public class VerticalScrollBar : ScrollBar
    {
        /// <summary>
        /// Initializes a new instance of the VerticalScrollBar class.
        /// </summary>
        public VerticalScrollBar () : base (true)
        {
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (15, 80);
    }
}
