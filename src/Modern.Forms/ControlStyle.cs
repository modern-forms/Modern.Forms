using System;
using SkiaSharp;

namespace Modern.Forms
{
    public class ControlStyle
    {
        private readonly ControlStyle? _parent;

        // This is used by the static DefaultStyles in Controls 
        public ControlStyle (ControlStyle? parent, Action<ControlStyle> setDefaults)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);

            setDefaults (this);

            Theme.ThemeChanged += (o, e) => setDefaults (this);
        }

        // This is used by instances of Controls
        public ControlStyle (ControlStyle parent)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);
        }

        public SKColor? ForegroundColor { get; set; }

        public SKColor GetForegroundColor () => ForegroundColor ?? _parent?.GetForegroundColor () ?? Theme.DarkTextColor;

        public SKColor? BackgroundColor { get; set; }

        public SKColor GetBackgroundColor () => BackgroundColor ?? _parent? .GetBackgroundColor () ?? Theme.NeutralGray;

        public SKTypeface? Font { get; set; }

        public SKTypeface GetFont () => Font ?? _parent?.GetFont () ?? Theme.UIFont;

        public int? FontSize { get; set; }

        public int GetFontSize () => FontSize ?? _parent?.GetFontSize () ?? Theme.FontSize;

        public BorderStyle Border { get; }
    }
}
