using System;
using System.Collections.Generic;
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
            SetBuiltInTheme (BuiltInTheme.Default);

            values[nameof (UIFont)] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values[nameof (UIFontBold)] = SKTypeface.FromFamilyName ("Segoe UI Emoji", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            values[nameof (FontSize)] = 14;
            values[nameof (ItemFontSize)] = 12;
        }

        /// <summary>
        /// The primary color uses for accents.
        /// </summary>
        public static SKColor AccentColor {
            get => GetValue<SKColor> (nameof (AccentColor));
            set => SetValue (nameof (AccentColor), value);
        }

        /// <summary>
        /// The secondary color uses for accents.
        /// </summary>
        public static SKColor AccentColor2 {
            get => GetValue<SKColor> (nameof (AccentColor2));
            set => SetValue (nameof (AccentColor2), value);
        }

        /// <summary>
        /// The default background color of a Form and Control.
        /// </summary>
        public static SKColor BackgroundColor {
            get => GetValue<SKColor> (nameof (BackgroundColor));
            set => SetValue (nameof (BackgroundColor), value);
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
        /// The low color used for Control borders.
        /// </summary>
        public static SKColor BorderLowColor {
            get => GetValue<SKColor> (nameof (BorderLowColor));
            set => SetValue (nameof (BorderLowColor), value);
        }

        /// <summary>
        /// The mid color used for Control borders.
        /// </summary>
        public static SKColor BorderMidColor {
            get => GetValue<SKColor> (nameof (BorderMidColor));
            set => SetValue (nameof (BorderMidColor), value);
        }

        /// <summary>
        /// The high color used for Control borders.
        /// </summary>
        public static SKColor BorderHighColor {
            get => GetValue<SKColor> (nameof (BorderHighColor));
            set => SetValue (nameof (BorderHighColor), value);
        }

        /// <summary>
        /// The low color used for Controls.
        /// </summary>
        public static SKColor ControlLowColor {
            get => GetValue<SKColor> (nameof (ControlLowColor));
            set => SetValue (nameof (ControlLowColor), value);
        }

        /// <summary>
        /// The mid color used for Controls.
        /// </summary>
        public static SKColor ControlMidColor {
            get => GetValue<SKColor> (nameof (ControlMidColor));
            set => SetValue (nameof (ControlMidColor), value);
        }

        /// <summary>
        /// The mid-high color used for Controls.
        /// </summary>
        public static SKColor ControlMidHighColor {
            get => GetValue<SKColor> (nameof (ControlMidHighColor));
            set => SetValue (nameof (ControlMidHighColor), value);
        }

        /// <summary>
        /// The high color used for Controls.
        /// </summary>
        public static SKColor ControlHighColor {
            get => GetValue<SKColor> (nameof (ControlHighColor));
            set => SetValue (nameof (ControlHighColor), value);
        }

        /// <summary>
        /// The very high color used for Controls.
        /// </summary>
        public static SKColor ControlVeryHighColor {
            get => GetValue<SKColor> (nameof (ControlVeryHighColor));
            set => SetValue (nameof (ControlVeryHighColor), value);
        }

        /// <summary>
        /// The low color used for highlighted elements, like hovered tabs or buttons.
        /// </summary>
        public static SKColor ControlHighlightLowColor {
            get => GetValue<SKColor> (nameof (ControlHighlightLowColor));
            set => SetValue (nameof (ControlHighlightLowColor), value);
        }

        /// <summary>
        /// The mid color used for highlighted elements, like hovered tabs or buttons.
        /// </summary>
        public static SKColor ControlHighlightMidColor {
            get => GetValue<SKColor> (nameof (ControlHighlightMidColor));
            set => SetValue (nameof (ControlHighlightMidColor), value);
        }

        /// <summary>
        /// The high color used for highlighted elements, like hovered tabs or buttons.
        /// </summary>
        public static SKColor ControlHighlightHighColor {
            get => GetValue<SKColor> (nameof (ControlHighlightHighColor));
            set => SetValue (nameof (ControlHighlightHighColor), value);
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
        /// The foreground color used for text.
        /// </summary>
        public static SKColor ForegroundColor {
            get => GetValue<SKColor> (nameof (ForegroundColor));
            set => SetValue (nameof (ForegroundColor), value);
        }

        /// <summary>
        /// A text color that is visible on an AccentColor background.
        /// </summary>
        public static SKColor ForegroundColorOnAccent {
            get => GetValue<SKColor> (nameof (ForegroundColorOnAccent));
            set => SetValue (nameof (ForegroundColorOnAccent), value);
        }

        /// <summary>
        /// The color used for disabled text.
        /// </summary>
        public static SKColor ForegroundDisabledColor {
            get => GetValue<SKColor> (nameof (ForegroundDisabledColor));
            set => SetValue (nameof (ForegroundDisabledColor), value);
        }

        private static T GetValue<T> (string name) => (T)values[name];

        /// <summary>
        /// A smaller font size generally used for lists of items.
        /// </summary>
        public static int ItemFontSize {
            get => GetValue<int> (nameof (ItemFontSize));
            set => SetValue (nameof (ItemFontSize), value);
        }

        /// <summary>
        /// The color used to highlight a potentially destructive action.
        /// </summary>
        public static SKColor WarningHighlightColor {
            get => GetValue<SKColor> (nameof (WarningHighlightColor));
            set => SetValue (nameof (WarningHighlightColor), value);
        }

        /// <summary>
        /// Changes or resets the application theme to a set of built-in defaults.
        /// </summary>
        /// <param name="theme"></param>
        public static void SetBuiltInTheme (BuiltInTheme theme)
        {
            // We always reset the colors, even if we were already using the current theme.
            // This resets any modification the user made, which feels like the expected behavior.

            BeginUpdate ();

            // TODO: BuiltInTheme.Default should detect the OS setting. Currently it just uses Light.
            switch (theme) {
                case BuiltInTheme.Dark:
                    values[nameof (BackgroundColor)] = SKColor.Parse ("#FF282828");
                    values[nameof (BorderLowColor)] = SKColor.Parse ("#FF505050");
                    values[nameof (BorderMidColor)] = SKColor.Parse ("#FF808080");
                    values[nameof (BorderHighColor)] = SKColor.Parse ("#FFA0A0A0");
                    values[nameof (ControlLowColor)] = SKColor.Parse ("#FF282828");
                    values[nameof (ControlMidColor)] = SKColor.Parse ("#FF505050");
                    values[nameof (ControlMidHighColor)] = SKColor.Parse ("#FF686868");
                    values[nameof (ControlHighColor)] = SKColor.Parse ("#FF808080");
                    values[nameof (ControlVeryHighColor)] = SKColor.Parse ("#FFEFEBEF");
                    values[nameof (ControlHighlightLowColor)] = SKColor.Parse ("#FFA8A8A8");
                    values[nameof (ControlHighlightMidColor)] = SKColor.Parse ("#FF828282");
                    values[nameof (ControlHighlightHighColor)] = SKColor.Parse ("#FF505050");
                    values[nameof (ForegroundColor)] = SKColor.Parse ("#FFDEDEDE");
                    values[nameof (ForegroundDisabledColor)] = new SKColor (150, 150, 150);
                    values[nameof (ForegroundColorOnAccent)] = SKColors.White;
                    values[nameof (AccentColor)] = SKColor.Parse ("#FF096085");
                    values[nameof (AccentColor2)] = new SKColor (0, 120, 212);
                    values[nameof (WarningHighlightColor)] = new SKColor (232, 17, 35);
                    break;
                default:
                    values[nameof (BackgroundColor)] = new SKColor (240, 240, 240);
                    values[nameof (BorderLowColor)] = SKColor.Parse ("#FFABABAB");
                    values[nameof (BorderMidColor)] = SKColor.Parse ("#FF808080");
                    values[nameof (BorderHighColor)] = SKColor.Parse ("#FF333333");
                    values[nameof (ControlLowColor)] = SKColor.Parse ("#FFFBFBFB");
                    values[nameof (ControlMidColor)] = SKColor.Parse ("#FFF3F3F3");
                    values[nameof (ControlMidHighColor)] = SKColor.Parse ("#FFE1E1E1");
                    values[nameof (ControlHighColor)] = SKColor.Parse ("#FFC2C3C9");
                    values[nameof (ControlVeryHighColor)] = SKColor.Parse ("#FF686868");
                    values[nameof (ControlHighlightLowColor)] = SKColor.Parse ("#FFC6C6C6");
                    values[nameof (ControlHighlightMidColor)] = SKColor.Parse ("#FFB0B0B0");
                    values[nameof (ControlHighlightHighColor)] = SKColor.Parse ("#FF808080");
                    values[nameof (ForegroundColor)] = SKColor.Parse ("#FF000000");
                    values[nameof (ForegroundDisabledColor)] = new SKColor (170, 170, 170);
                    values[nameof (ForegroundColorOnAccent)] = SKColors.White;
                    values[nameof (AccentColor)] = new SKColor (42, 138, 208);
                    values[nameof (AccentColor2)] = new SKColor (0, 120, 212);
                    values[nameof (WarningHighlightColor)] = new SKColor (232, 17, 35);
                    break;
            }

            RaiseThemeChanged ();
            EndUpdate ();
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
