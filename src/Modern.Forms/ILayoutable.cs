using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Provides functionality needed to be laid out by a LayoutEngine.
    /// </summary>
    public interface ILayoutable
    {
        /// <summary>
        /// Gets the size the control would prefer to be.
        /// </summary>
        /// <param name="proposedSize">A size the layout engine is proposing for the control.</param>
        Size GetPreferredSize (Size proposedSize);

        /// <summary>
        /// Gets or sets how much space there should be between the control and other controls.
        /// </summary>
        Padding Margin { get; }

        /// <summary>
        /// Sets the unscaled bounds of the control.
        /// </summary>
        void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All);
    }
}
