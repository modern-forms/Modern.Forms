using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class StatusBar : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.Border.Top.Width = 1;
                style.FontSize = 13;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        protected override Size DefaultSize => new Size (600, 25);

        public StatusBar ()
        {
            Dock = System.Windows.Forms.DockStyle.Bottom;
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            if (!string.IsNullOrWhiteSpace (Text))
                e.Canvas.DrawText (Text, CurrentStyle.GetFont (), CurrentStyle.GetFontSize (), 6, 17, CurrentStyle.GetForegroundColor ());
        }
    }
}
