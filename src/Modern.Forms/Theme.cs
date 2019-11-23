using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public static class Theme
    {
        private static int suspend_count = 0;
        private static bool suspended_raise_waiting = false;

        private static readonly Dictionary<string, object> values = new Dictionary<string, object> ();

        public static event EventHandler? ThemeChanged;

        static Theme ()
        {
            values[nameof (RibbonColor)] = new SKColor (16, 110, 190);
            values[nameof (DarkTextColor)] = SKColors.Black;
            values[nameof (DisabledTextColor)] = new SKColor (190, 190, 190);
            values[nameof (LightTextColor)] = SKColors.White;
            values[nameof (FormBackgroundColor)] = new SKColor (240, 240, 240);
            values[nameof (FormCloseHighlightColor)] = new SKColor (232, 17, 35);
            values[nameof (RibbonTabHighlightColor)] = new SKColor (42, 138, 208);
            values[nameof (RibbonItemHighlightColor)] = new SKColor (198, 198, 198);
            values[nameof (RibbonItemSelectedColor)] = new SKColor (176, 176, 176);
            values[nameof (DarkNeutralGray)] = new SKColor (225, 225, 225);
            values[nameof (NeutralGray)] = new SKColor (240, 240, 240);
            values[nameof (LightNeutralGray)] = new SKColor (251, 251, 251);
            values[nameof (BorderGray)] = new SKColor (171, 171, 171);

            values[nameof (UIFont)] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values[nameof (FontSize)] = 14;
            values[nameof (RibbonItemFontSize)] = 12;
        }

        public static SKColor RibbonColor {
            get => GetValue<SKColor> (nameof (RibbonColor));
            set => SetValue (nameof (RibbonColor), value);
        }

        public static SKColor DarkTextColor {
            get => GetValue<SKColor> (nameof (DarkTextColor));
            set => SetValue (nameof (DarkTextColor), value);
        }

        public static SKColor DisabledTextColor {
            get => GetValue<SKColor> (nameof (DisabledTextColor));
            set => SetValue (nameof (DisabledTextColor), value);
        }

        public static SKColor LightTextColor {
            get => GetValue<SKColor> (nameof (LightTextColor));
            set => SetValue (nameof (LightTextColor), value);
        }

        public static SKColor FormBackgroundColor {
            get => GetValue<SKColor> (nameof (FormBackgroundColor));
            set => SetValue (nameof (FormBackgroundColor), value);
        }

        public static SKColor FormCloseHighlightColor {
            get => GetValue<SKColor> (nameof (FormCloseHighlightColor));
            set => SetValue (nameof (FormCloseHighlightColor), value);
        }

        public static SKColor RibbonTabHighlightColor {
            get => GetValue<SKColor> (nameof (RibbonTabHighlightColor));
            set => SetValue (nameof (RibbonTabHighlightColor), value);
        }

        public static SKColor RibbonItemHighlightColor {
            get => GetValue<SKColor> (nameof (RibbonItemHighlightColor));
            set => SetValue (nameof (RibbonItemHighlightColor), value);
        }

        public static SKColor RibbonItemSelectedColor {
            get => GetValue<SKColor> (nameof (RibbonItemSelectedColor));
            set => SetValue (nameof (RibbonItemSelectedColor), value);
        }

        public static SKColor DarkNeutralGray {
            get => GetValue<SKColor> (nameof (DarkNeutralGray));
            set => SetValue (nameof (DarkNeutralGray), value);
        }

        public static SKColor NeutralGray {
            get => GetValue<SKColor> (nameof (NeutralGray));
            set => SetValue (nameof (NeutralGray), value);
        }

        public static SKColor LightNeutralGray {
            get => GetValue<SKColor> (nameof (LightNeutralGray));
            set => SetValue (nameof (LightNeutralGray), value);
        }

        public static SKColor BorderGray {
            get => GetValue<SKColor> (nameof (BorderGray));
            set => SetValue (nameof (BorderGray), value);
        }

        public static SKTypeface UIFont {
            get => GetValue<SKTypeface> (nameof (UIFont));
            set => SetValue (nameof (UIFont), value);
        }

        public static int FontSize {
            get => GetValue<int> (nameof (FontSize));
            set => SetValue (nameof (FontSize), value);
        }

        public static int RibbonItemFontSize {
            get => GetValue<int> (nameof (RibbonItemFontSize));
            set => SetValue (nameof (RibbonItemFontSize), value);
        }

        private static T GetValue<T> (string name) => (T)values[name];

        private static void SetValue (string key, object value)
        {
            values[key] = value;

            RaiseThemeChanged ();
        }

        public static void BeginUpdate ()
        {
            suspend_count++;
        }

        public static void EndUpdate ()
        {
            if (suspend_count == 0)
                throw new InvalidOperationException ("EndUpdate called without matching BeginUpdate");

            suspend_count--;

            if (suspended_raise_waiting)
                RaiseThemeChanged ();
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
    }
}
