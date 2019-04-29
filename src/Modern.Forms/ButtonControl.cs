using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class ButtonControl : ModernControl
    {
        public ControlTheme Style { get; } = new ControlTheme ();
        public ControlTheme StyleHover { get; } = new ControlTheme ();

        public ControlTheme CurrentStyle => is_hover ? StyleHover : Style;

        private bool is_hover;

        public ButtonControl ()
        {
            Width = 100;
            Height = 30;

            StyleHover.BackgroundColor = Theme.RibbonTabHighlightColor;
        }

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            var canvas = e.Surface.Canvas;

            if (CurrentStyle.BorderRadius > 0) {
                canvas.Clear (SKColors.Transparent);
                canvas.Save ();
                canvas.ClipRoundRect (new SKRoundRect (new SKRect (1.5f, 1.5f , Width - 1, Height - 1), CurrentStyle.BorderRadius, CurrentStyle.BorderRadius));
                canvas.Clear (CurrentStyle.BackgroundColor);
                canvas.Restore ();
            } else {
                canvas.Clear (CurrentStyle.BackgroundColor);
            }

            canvas.DrawCenteredText (Text, Theme.UIFont, 14, Width / 2, 20, CurrentStyle.ForeColor);
            if (CurrentStyle.BorderRadius > 0)
                canvas.DrawRoundedRectangle (1, 1, Width - (CurrentStyle.BorderWidth * 2), Height - (CurrentStyle.BorderWidth * 2), CurrentStyle.BorderColor, CurrentStyle.BorderRadius, CurrentStyle.BorderRadius, .5f);
            else
                canvas.DrawRectangle (0, 0, Width - CurrentStyle.BorderWidth, Height - CurrentStyle.BorderWidth, CurrentStyle.BorderColor, CurrentStyle.BorderWidth);
        }

        protected override void OnMouseEnter (EventArgs e)
        {
            base.OnMouseEnter (e);

            is_hover = true;
            Invalidate ();
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            is_hover = false;
            Invalidate ();
        }
    }

    public class ControlTheme
    {
        public SKColor ForeColor { get; set; } = Theme.LightText;
        public SKColor BackgroundColor { get; set; } = Theme.RibbonColor;
        public SKColor BorderColor { get; set; } = Theme.RibbonColor;
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 0;
    }
}
