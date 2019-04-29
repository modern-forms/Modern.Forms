using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public static class Theme
    {
        public static SKColor DarkText = SKColors.Black;
        public static SKColor LightText = SKColors.White;
        public static SKColor FormBackColor = SKColors.White;
        public static SKColor FormCloseHighlight = new SKColor (232, 17, 35);

        public static SKColor RibbonColor = new SKColor (16, 110, 190);
        public static SKColor RibbonTabHighlightColor = new SKColor (42, 138, 208);
        public static SKColor RibbonItemHighlightColor = new SKColor (198, 198, 198);
        public static SKColor RibbonItemSelectedColor = new SKColor (176, 176, 176);
        public static SKColor NeutralGray = new SKColor (240, 240, 240);
        public static SKColor LightNeutralGray = new SKColor (251, 251, 251);
        public static SKColor BorderGray = new SKColor (171, 171, 171);

        public static SKTypeface UIFont = SKTypeface.FromFamilyName ("Segoe UI", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
    }
}
