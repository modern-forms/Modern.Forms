using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Provides a repository of color resources.
    /// </summary>
    public static class Theme
    {
        private static int suspend_count = 0;
        private static bool suspended_raise_waiting = false;

        private static readonly Dictionary<string, object> values = new Dictionary<string, object> ();

        static Theme ()
        {
            values[nameof (PrimaryColor)] = new SKColor (0, 120, 212);
            values[nameof (PrimaryTextColor)] = SKColors.Black;
            values[nameof (DisabledTextColor)] = new SKColor (190, 190, 190);
            values[nameof (LightTextColor)] = SKColors.White;
            values[nameof (FormBackgroundColor)] = new SKColor (240, 240, 240);
            values[nameof (FormCloseHighlightColor)] = new SKColor (232, 17, 35);
            values[nameof (HighlightColor)] = new SKColor (42, 138, 208);
            values[nameof (ItemHighlightColor)] = new SKColor (198, 198, 198);
            values[nameof (ItemSelectedColor)] = new SKColor (176, 176, 176);
            values[nameof (DarkNeutralGray)] = new SKColor (225, 225, 225);
            values[nameof (NeutralGray)] = new SKColor (243, 242, 241);
            values[nameof (LightNeutralGray)] = new SKColor (251, 251, 251);
            values[nameof (BorderGray)] = new SKColor (171, 171, 171);

            values[nameof (UIFont)] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values[nameof (UIFontBold)] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values[nameof (FontSize)] = 14;
            values[nameof (ItemFontSize)] = 12;
        }

        /// <summary>
        /// Pause raising ThemeChanged for better performance if changing many Theme elements.
        /// Resume with EndUpdate ().
        /// </summary>
        public static void BeginUpdate ()
        {
            suspend_count++;
        }

        /// <summary>
        /// A darker gray used as the default control border color.
        /// </summary>
        public static SKColor BorderGray {
            get => GetValue<SKColor> (nameof (BorderGray));
            set => SetValue (nameof (BorderGray), value);
        }

        /// <summary>
        /// A darker gray generally used for showing an item is currently pressed.
        /// </summary>
        public static SKColor DarkNeutralGray {
            get => GetValue<SKColor> (nameof (DarkNeutralGray));
            set => SetValue (nameof (DarkNeutralGray), value);
        }

        /// <summary>
        /// The color used for disabled text.
        /// </summary>
        public static SKColor DisabledTextColor {
            get => GetValue<SKColor> (nameof (DisabledTextColor));
            set => SetValue (nameof (DisabledTextColor), value);
        }

        /// <summary>
        /// Resume raising the ThemeChanged event after pausing with BeginUpdate ().
        /// </summary>
        public static void EndUpdate ()
        {
            if (suspend_count == 0)
                throw new InvalidOperationException ("EndUpdate called without matching BeginUpdate");

            suspend_count--;

            if (suspended_raise_waiting)
                RaiseThemeChanged ();
        }

        /// <summary>
        /// The default font size used by control.
        /// </summary>
        public static int FontSize {
            get => GetValue<int> (nameof (FontSize));
            set => SetValue (nameof (FontSize), value);
        }

        /// <summary>
        /// The background color of a form.
        /// </summary>
        public static SKColor FormBackgroundColor {
            get => GetValue<SKColor> (nameof (FormBackgroundColor));
            set => SetValue (nameof (FormBackgroundColor), value);
        }

        /// <summary>
        /// The color used to draw a form's close button when highlighted.
        /// </summary>
        public static SKColor FormCloseHighlightColor {
            get => GetValue<SKColor> (nameof (FormCloseHighlightColor));
            set => SetValue (nameof (FormCloseHighlightColor), value);
        }

        private static T GetValue<T> (string name) => (T)values[name];

        /// <summary>
        /// The color used for highlighted elements, like hovered tabs or buttons.
        /// </summary>
        public static SKColor HighlightColor {
            get => GetValue<SKColor> (nameof (HighlightColor));
            set => SetValue (nameof (HighlightColor), value);
        }

        /// <summary>
        /// A smaller font size generally used for lists of items.
        /// </summary>
        public static int ItemFontSize {
            get => GetValue<int> (nameof (ItemFontSize));
            set => SetValue (nameof (ItemFontSize), value);
        }

        /// <summary>
        /// The color used for a highlighted item's background.
        /// </summary>
        public static SKColor ItemHighlightColor {
            get => GetValue<SKColor> (nameof (ItemHighlightColor));
            set => SetValue (nameof (ItemHighlightColor), value);
        }

        /// <summary>
        /// The color used for a selected item's background.
        /// </summary>
        public static SKColor ItemSelectedColor {
            get => GetValue<SKColor> (nameof (ItemSelectedColor));
            set => SetValue (nameof (ItemSelectedColor), value);
        }

        /// <summary>
        /// A lighter gray used primarily used as the background of list items.
        /// </summary>
        public static SKColor LightNeutralGray {
            get => GetValue<SKColor> (nameof (LightNeutralGray));
            set => SetValue (nameof (LightNeutralGray), value);
        }

        /// <summary>
        /// A lighter text color, often used when an element is selected with a darker background.
        /// </summary>
        public static SKColor LightTextColor {
            get => GetValue<SKColor> (nameof (LightTextColor));
            set => SetValue (nameof (LightTextColor), value);
        }

        /// <summary>
        /// A neutral gray used as the default background of controls.
        /// </summary>
        public static SKColor NeutralGray {
            get => GetValue<SKColor> (nameof (NeutralGray));
            set => SetValue (nameof (NeutralGray), value);
        }

        /// <summary>
        /// The primary color for elements such as the title bar, tabs, checkboxes and radio button glyph.
        /// </summary>
        public static SKColor PrimaryColor {
            get => GetValue<SKColor> (nameof (PrimaryColor));
            set => SetValue (nameof (PrimaryColor), value);
        }

        /// <summary>
        /// The primary text color.
        /// </summary>
        public static SKColor PrimaryTextColor {
            get => GetValue<SKColor> (nameof (PrimaryTextColor));
            set => SetValue (nameof (PrimaryTextColor), value);
        }

        private static void RaiseThemeChanged ()
        {
            if (suspend_count > 0) {
                suspended_raise_waiting = true;
                return;
            }

            ThemeChanged?.Invoke (null, EventArgs.Empty);
            suspended_raise_waiting = false;
        }

        private static void SetValue (string key, object value)
        {
            values[key] = value;

            RaiseThemeChanged ();
        }

        /// <summary>
        /// Raised when a theme color is changed. Controls listen
        /// for this event to know when to repaint themselves.
        /// </summary>
        public static event EventHandler? ThemeChanged;

        /// <summary>
        /// The default font used by controls.
        /// </summary>
        public static SKTypeface UIFont {
            get => GetValue<SKTypeface> (nameof (UIFont));
            set => SetValue (nameof (UIFont), value);
        }

        /// <summary>
        /// The default bold font used by controls.
        /// </summary>
        public static SKTypeface UIFontBold {
            get => GetValue<SKTypeface> (nameof (UIFontBold));
            set => SetValue (nameof (UIFontBold), value);
        }
    }
}
