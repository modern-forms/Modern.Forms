using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public static class ModernTheme
    {
        private static int suspend_count = 0;
        private static bool suspended_raise_waiting = false;

        private static Dictionary<string, object> values = new Dictionary<string, object> ();

        public static event EventHandler ThemeChanged;

        static ModernTheme ()
        {
            values["RibbonColor"] = new SKColor (16, 110, 190);
            values["DarkTextColor"] = SKColors.Black;
            values["DisabledTextColor"] = new SKColor (190, 190, 190);
            values["LightTextColor"] = SKColors.White;
            values["FormBackgroundColor"] = new SKColor (240, 240, 240);
            values["FormCloseHighlightColor"] = new SKColor (232, 17, 35);
            values["RibbonTabHighlightColor"] = new SKColor (42, 138, 208);
            values["RibbonItemHighlightColor"] = new SKColor (198, 198, 198);
            values["RibbonItemSelectedColor"] = new SKColor (176, 176, 176);
            values["DarkNeutralGray"] = new SKColor (225, 225, 225);
            values["NeutralGray"] = new SKColor (240, 240, 240);
            values["LightNeutralGray"] = new SKColor (251, 251, 251);
            values["BorderGray"] = new SKColor (171, 171, 171);

            values["UIFont"] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values["FontSize"] = 14;
        }

        public static SKColor RibbonColor {
            get => GetValue<SKColor> ("RibbonColor");
            set => SetValue ("RibbonColor", value);
        }

        public static SKColor DarkTextColor {
            get => GetValue<SKColor> ("DarkTextColor");
            set => SetValue ("DarkTextColor", value);
        }

        public static SKColor DisabledTextColor {
            get => GetValue<SKColor> ("DisabledTextColor");
            set => SetValue ("DisabledTextColor", value);
        }

        public static SKColor LightTextColor {
            get => GetValue<SKColor> ("LightTextColor");
            set => SetValue ("LightTextColor", value);
        }

        public static SKColor FormBackgroundColor {
            get => GetValue<SKColor> ("FormBackgroundColor");
            set => SetValue ("FormBackgroundColor", value);
        }

        public static SKColor FormCloseHighlightColor {
            get => GetValue<SKColor> ("FormCloseHighlightColor");
            set => SetValue ("FormCloseHighlightColor", value);
        }

        public static SKColor RibbonTabHighlightColor {
            get => GetValue<SKColor> ("RibbonTabHighlightColor");
            set => SetValue ("RibbonTabHighlightColor", value);
        }

        public static SKColor RibbonItemHighlightColor {
            get => GetValue<SKColor> ("RibbonItemHighlightColor");
            set => SetValue ("RibbonItemHighlightColor", value);
        }

        public static SKColor RibbonItemSelectedColor {
            get => GetValue<SKColor> ("RibbonItemSelectedColor");
            set => SetValue ("RibbonItemSelectedColor", value);
        }

        public static SKColor DarkNeutralGray {
            get => GetValue<SKColor> ("DarkNeutralGray");
            set => SetValue ("DarkNeutralGray", value);
        }

        public static SKColor NeutralGray {
            get => GetValue<SKColor> ("NeutralGray");
            set => SetValue ("NeutralGray", value);
        }

        public static SKColor LightNeutralGray {
            get => GetValue<SKColor> ("LightNeutralGray");
            set => SetValue ("LightNeutralGray", value);
        }

        public static SKColor BorderGray {
            get => GetValue<SKColor> ("BorderGray");
            set => SetValue ("BorderGray", value);
        }

        public static SKTypeface UIFont {
            get => GetValue<SKTypeface> ("UIFont");
            set => SetValue ("UIFont", value);
        }

        public static int FontSize {
            get => GetValue<int> ("FontSize");
            set => SetValue ("FontSize", value);
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
