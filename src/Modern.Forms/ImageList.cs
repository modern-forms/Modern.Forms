using System.ComponentModel;
using SkiaSharp;

namespace Modern.Forms;

/// <summary>
/// Represents a collection of images that can be used by controls.
/// </summary>
public class ImageList : Component
{
    private static readonly SKSize s_defaultImageSize = new (32, 32);

    /// <summary>
    /// Initializes a new instance of the ImageList class.
    /// </summary>
    public ImageList ()
    {
        Images = new (s_defaultImageSize);
    }

    /// <summary>
    /// Gets the collection of images in the ImageList.
    /// </summary>
    public ImageCollection Images { get; }

    /// <summary>
    /// Gets or sets the size of the images in the ImageList. Note this cannot be set once images have been added.
    /// </summary>
    public SKSize ImageSize {
        get => Images.ImageSize;
        set => Images.SetImageSize (value);
    }

    /// <inheritdoc/>
    protected override void Dispose (bool disposing)
    {
        if (disposing)
            Images.Dispose ();

        base.Dispose (disposing);
    }
}
