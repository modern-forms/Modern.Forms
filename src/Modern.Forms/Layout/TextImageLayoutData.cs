using System.Drawing;

namespace Modern.Forms.Layout;

internal class TextImageLayoutData
{
    /// <summary>
    /// Control's ClientRectangle.
    /// </summary>
    public Rectangle Client;

    /// <summary>
    /// Control's ClientRectangle minus the border.
    /// </summary>
    public Rectangle Face;

    /// <summary>
    /// The total area the glyph can be put in.
    /// </summary>
    public Rectangle GlyphArea;

    /// <summary>
    /// The actual area to put the Glyph in. This size will be the size of the glyph.
    /// </summary>
    public Rectangle GlyphBounds;

    /// <summary>
    /// The bouding rectangle for the area the text can be put in.
    /// </summary>
    public Rectangle TextBounds;

    /// <summary>
    /// Scratch property for layout that represents the remaining unallocated space.
    /// </summary>
    public Rectangle Field;

    /// <summary>
    /// The bounding rectangle for the focus rectangle.
    /// </summary>
    public Rectangle Focus;

    /// <summary>
    /// The bounding rectangle for the image.
    /// </summary>
    public Rectangle ImageBounds;
}
