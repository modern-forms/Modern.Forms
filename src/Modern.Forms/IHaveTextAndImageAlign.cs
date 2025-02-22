using SkiaSharp;

namespace Modern.Forms;

// Used by TextImageLayoutEngine to lay out text and image
interface IHaveTextAndImageAlign
{
    ContentAlignment ImageAlign { get; set; }
    ContentAlignment TextAlign { get; set; }
    TextImageRelation TextImageRelation { get; set; }
    SKBitmap? Image { get; set; }
}
