using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    public class StatusBarControl : ModernControl
    {
        public StatusBarControl ()
        {
            Height = 25;
            Dock = System.Windows.Forms.DockStyle.Bottom;
        }

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            e.Surface.Canvas.Clear (Theme.NeutralGray);
            e.Surface.Canvas.DrawLine (0, 0, Width, 0, Theme.BorderGray);

            if (!string.IsNullOrWhiteSpace (Text))
                e.Surface.Canvas.DrawText (Text, Theme.UIFont, 13, 6, 17, Theme.DarkText);
        }
    }
}
