using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItem
    {
        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public bool Highlighted { get; set; }

        public Rectangle Bounds { get; private set; }

        public event EventHandler Click;

        public RibbonItem ()
        {
        }

        public RibbonItem (string text, SKBitmap image = null, EventHandler onClick = null)
        {
            Text = text;
            Image = image;

            Click += onClick;
        }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public void DrawItem (SKCanvas canvas)
        {
            var background_color = Selected ? ModernTheme.RibbonItemSelectedColor : Highlighted ? ModernTheme.RibbonItemHighlightColor : ModernTheme.NeutralGray;

            canvas.FillRectangle (Bounds, background_color);

            if (Image != null)
                canvas.DrawBitmap (Image, Bounds.Left + 6, Bounds.Top + 3);

            canvas.Save ();
            canvas.ClipRect (new SKRect (Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom));

            var lines = Text.Split (' ');

            canvas.DrawCenteredText (lines[0].Trim (), ModernTheme.UIFont, 12, Bounds.Left + Bounds.Width / 2, Bounds.Top + 50, ModernTheme.DarkTextColor);

            if (lines.Length > 1)
                canvas.DrawCenteredText (lines[1].Trim (), ModernTheme.UIFont, 12, Bounds.Left + Bounds.Width / 2, Bounds.Top + 66, ModernTheme.DarkTextColor);

            canvas.Restore ();
        }

        public void PerformClick ()
        {
            Click?.Invoke (this, EventArgs.Empty);
        }
    }
}
