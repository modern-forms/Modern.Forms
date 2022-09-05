using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms;

/// <summary>
/// Represents a NavigationPaneItem.
/// </summary>
public class NavigationPaneItem : ILayoutable
{
    private bool enabled = true;
    private string text;
    private SKBitmap? image;

    /// <summary>
    /// Initializes a new instance of the NavigationPaneItem class.
    /// </summary>
    public NavigationPaneItem (SKBitmap image, string? text = null)
    {
        this.image = image;
        this.text = text ?? string.Empty;
        Margin = new Padding (4);
    }

    /// <summary>
    /// Gets the current bounding box of the item.
    /// </summary>
    public Rectangle Bounds { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is enabled.
    /// </summary>
    public bool Enabled {
        get => enabled && Parent?.Enabled == true;
        set {
            if (enabled != value) {
                enabled = value;
                Parent?.Invalidate ();
            }
        }
    }

    /// <summary>
    /// Gets the preferred size of the item.
    /// </summary>
    public Size GetPreferredSize (Size proposedSize)
    {
        var size = Parent?.LogicalToDeviceUnits (40) ?? 40;

        return new Size (size, size);
    }

    /// <summary>
    /// Gets a value indicating if the item currently has the mouse hovered over it.
    /// </summary>
    public bool Hovered => Parent?.Items.HoveredIndex == Index;

    /// <summary>
    /// Gets or sets the image displayed on the item.
    /// </summary>
    public SKBitmap? Image {
        get => image;
        set {
            if (image != value) {
                image = value;
                Parent?.Invalidate ();
            }
        }
    }

    // Gets the current index in the parent NavigationPane, if parented to a NavigationPane.
    private int Index => Parent?.Items.IndexOf (this) ?? -1;

    /// <summary>
    /// Gets or sets the amount of space to leave between this item and other elements.
    /// </summary>
    public Padding Margin { get; set; } = Padding.Empty;

    /// <summary>
    /// Gets or sets the amount of space to leave between the text and the border of the item.
    /// </summary>
    public Padding Padding { get; set; } = new Padding (14, 0, 14, 0);

    /// <summary>
    /// Gets the NavigationPane this item is currently a part of.
    /// </summary>
    public NavigationPane? Parent { get; internal set; }

    /// <summary>
    /// Gets a value indicating if the item is currently the selected item.
    /// </summary>
    public bool Selected => Parent?.SelectedItem == this;

    /// <summary>
    /// Sets the bounding box of the item. This is internal API and should not be called.
    /// </summary>
    public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
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
    public string Text {
        get => text;
        set {
            if (text != value) {
                text = value;
                Parent?.Invalidate ();
            }
        }
    }
}
