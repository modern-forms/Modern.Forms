using System;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Defines the style of a control.
    /// </summary>
    public class ControlStyle
    {
        private readonly ControlStyle? _parent;

        /// <summary>
        /// Initializes a new instance of the ControlStyle class.  This constructor is
        /// generally used by the static DefaultStyle property.
        /// </summary>
        public ControlStyle (ControlStyle? parent, Action<ControlStyle> setDefaults)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);

            setDefaults (this);

            Theme.ThemeChanged += (o, e) => setDefaults (this);
        }

        /// <summary>
        /// Initializes a new instance of the ControlStyle class.  This constructor is
        /// generally used by the instance Style property.
        /// </summary>
        public ControlStyle (ControlStyle parent)
        {
            _parent = parent;

            Border = new BorderStyle (parent?.Border);
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public SKColor? BackgroundColor { get; set; }

        /// <summary>
        /// Provides access to border style properties.
        /// </summary>
        public BorderStyle Border { get; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public SKTypeface? Font { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public int? FontSize { get; set; }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public SKColor? ForegroundColor { get; set; }

        /// <summary>
        /// Gets the computed background color.
        /// </summary>
        public SKColor GetBackgroundColor () => BackgroundColor ?? _parent? .GetBackgroundColor () ?? Theme.NeutralGray;

        /// <summary>
        /// Gets the computed font.
        /// </summary>
        public SKTypeface GetFont () => Font ?? _parent?.GetFont () ?? Theme.UIFont;

        /// <summary>
        /// Gets the computed font size.
        /// </summary>
        public int GetFontSize () => FontSize ?? _parent?.GetFontSize () ?? Theme.FontSize;

        /// <summary>
        /// Gets the computed foreground color.
        /// </summary>
        public SKColor GetForegroundColor () => ForegroundColor ?? _parent?.GetForegroundColor () ?? Theme.DarkTextColor;
    }
}
