namespace Modern.Forms;

// Used by TextImageLayoutEngine to lay out check boxes, radio buttons, and other controls with a glyph
interface IHaveGlyph
{
    ContentAlignment GlyphAlign { get; set; }
}
