using SkiaSharp;

namespace Modern.Forms;

// Used by TextImageLayoutEngine to lay out text and image
interface IHaveTextAndImageAlign
{
    ContentAlignment ImageAlign { get; set; }
    ContentAlignment TextAlign { get; set; }
    TextImageRelation TextImageRelation { get; set; }
    SKBitmap? Image { get; set; }
    ImageList? ImageList { get; set; }
    int ImageIndex { get; set; }
    string ImageKey { get; set; }

    public SKBitmap? GetImage ()
    {
        if (Image is not null)
            return Image;

        if (ImageList is null)
            return null;

        if (ImageIndex >= 0)
            return ImageList.Images[ImageIndex];

        if (ImageKey.Length > 0)
            return ImageList.Images[ImageKey];

        return null;
    }
}
