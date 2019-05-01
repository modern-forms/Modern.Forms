using System;
using SkiaSharp;

namespace Modern.Forms
{
    public class ControlStyle
    {
        private readonly ControlStyle _parent;
        private readonly Action<ControlStyle> _setDefaults;

        // This is used by the static DefaultStyles in Controls 
        public ControlStyle (ControlStyle parent, Action<ControlStyle> setDefaults)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);

            _setDefaults = setDefaults;
            _setDefaults (this);

            Theme.ThemeChanged += (o, e) => _setDefaults (this);
        }

        // This is used by instances of Controls
        public ControlStyle (ControlStyle parent)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);
        }

        public SKColor? ForegroundColor { get; set; }

        public SKColor GetForegroundColor () => ForegroundColor ?? _parent.GetForegroundColor ();

        public SKColor? BackgroundColor { get; set; }

        public SKColor GetBackgroundColor () => BackgroundColor ?? _parent.GetBackgroundColor ();

        public SKTypeface Font { get; set; }

        public SKTypeface GetFont () => Font ?? _parent.Font;

        public int? FontSize { get; set; }

        public int GetFontSize () => FontSize ?? _parent.GetFontSize ();

        public BorderStyle Border { get; }
    }
}
