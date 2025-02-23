using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms;

/// <summary>
/// Represents a collection of images used by an <see cref="ImageList"/>.
/// </summary>
public class ImageCollection : IDictionary<string, SKBitmap>
{
    // We can't use a normal Dictionary because we need to be able to access images by index
    private OrderedDictionary _images = [];

    internal SKSize ImageSize { get; private set; }

    internal ImageCollection (SKSize imageSize)
    {
        ImageSize = imageSize;
    }

    /// <summary>
    /// Adds an image to the collection. Note the image will be resized to the correct size if it is not already.
    /// Also note that a copy of the image is stored, you are responsible for disposing of the original image when no longer needed.
    /// </summary>
    public void Add (string key, SKBitmap value)
    {
        if (_images.Contains (key))
            throw new ArgumentException ($"An element with the same key already exists: {key}");

        // The image is the correct size, simply make a copy of it
        if (value.GetSize ().ToSKSize () == ImageSize) {
            var copy = value.Copy ();
            _images.Add (key, copy);
            return;
        }

        // The image is not the correct size, resize it
        var resized = value.Resize (ImageSize.ToSizeI (), SKFilterQuality.High);
        _images.Add (key, resized);
    }

    /// <summary>
    /// Adds an image to the collection. Note the image will be resized to the correct size if it is not already.
    /// Also note that a copy of the image is stored, you are responsible for disposing of the original image when no longer needed.
    /// </summary>
    public void Add (KeyValuePair<string, SKBitmap> item)
    {
        Add (item.Key, item.Value);
    }

    /// <summary>
    /// Removes all images from the collection.
    /// </summary>
    public void Clear ()
    {
        // Do it this way so the images get disposed.
        foreach (var key in _images.Keys.OfType<string> ().ToArray ())
            Remove (key);
    }

    /// <summary>
    /// Checks if the collection contains the specified image.
    /// </summary>
    public bool Contains (KeyValuePair<string, SKBitmap> item)
    {
        return _images.Contains (item);
    }

    /// <summary>
    /// Checks if the collection contains the specified image key.
    /// </summary>
    public bool ContainsKey (string key)
    {
        return _images.Keys.Cast<string> ().Contains (key);
    }

    private static SKBitmap ConvertToBitmap (object? image)
    {
        return (image as SKBitmap) ?? throw new InvalidOperationException ("Key not found");
    }

    /// <summary>
    /// Copies the collection to an array.
    /// </summary>
    public void CopyTo (KeyValuePair<string, SKBitmap>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, SKBitmap>>)_images).CopyTo (array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of images in the collection.
    /// </summary>
    public int Count => _images.Count;

    internal void Dispose ()
    {
        foreach (var image in _images.Values.OfType<SKBitmap> ())
            image.Dispose ();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<KeyValuePair<string, SKBitmap>> GetEnumerator ()
    {
        return (IEnumerator<KeyValuePair<string, SKBitmap>>)_images.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator ();
    }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only. This is always false.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the image keys in the collection.
    /// </summary>
    public ICollection<string> Keys => _images.Keys.Cast<string> ().ToArray ();

    /// <summary>
    /// Removes the image with the specified key.
    /// </summary>
    public bool Remove (string key)
    {
        if (TryGetValue (key, out var image)) {
            image.Dispose ();
            _images.Remove (key);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the specified image.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove (KeyValuePair<string, SKBitmap> item)
    {
        return Remove (item.Key);
    }

    internal void SetImageSize (SKSize imageSize)
    {
        if (imageSize == ImageSize)
            return;

        if (_images.Count > 0)
            throw new InvalidOperationException ("Cannot set ImageSize after Images are already added.");

        ImageSize = imageSize;
    }

    /// <summary>
    /// Gets or sets the image with the specified key.
    /// </summary>
    public SKBitmap this[string key] {
        get => ConvertToBitmap (_images[key]);
        set {
            // If an existing image is going to be replaced, dispose of the old one
            if (TryGetValue (key, out var image))
                image.Dispose ();

            _images[key] = value;
        }
    }

    /// <summary>
    /// Gets or sets the image at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public SKBitmap this[int index] {
        get => ConvertToBitmap (_images[index]);
        set {
            // If an existing image is going to be replaced, dispose of the old one
            if (_images.Count >= index) {
                var image = this[index];
                image.Dispose ();
            }

            _images[index] = value;
        }
    }

    /// <summary>
    /// Gets the image with the specified key.
    /// </summary>
    public bool TryGetValue (string key, [MaybeNullWhen (false)] out SKBitmap value)
    {
        if (!ContainsKey (key)) {
            value = null;
            return false;
        }

        value = ConvertToBitmap (_images[key]);
        return true;
    }

    /// <summary>
    /// Gets the images in the collection.
    /// </summary>
    public ICollection<SKBitmap> Values => _images.Values.Cast<SKBitmap> ().ToArray ();
}
