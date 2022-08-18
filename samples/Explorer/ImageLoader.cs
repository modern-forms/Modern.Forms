﻿using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace Explore
{
    public static class ImageLoader
    {
        private static Dictionary<string, SKBitmap> _cache = new Dictionary<string, SKBitmap> ();
        private static string _defaultLocation = "Images";

        public static SKBitmap Get (string filename)
        {
            if (!_cache.ContainsKey (filename.ToLowerInvariant ()))
                _cache.Add (filename.ToLowerInvariant (), SKBitmap.Decode (Path.Combine (_defaultLocation, filename)));

            return _cache[filename.ToLowerInvariant ()];
        }
    }
}
