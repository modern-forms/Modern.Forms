using System;
using System.Drawing;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class ModernForm : Form
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = ModernTheme.FormBackgroundColor;
             style.Border.Color = ModernTheme.RibbonColor;
             style.Border.Width = 1;
         });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public ModernForm ()
        {
            DoubleBuffered = true;
            SetStyle (ControlStyles.ResizeRedraw, true);

            StartPosition = FormStartPosition.CenterScreen;
            //FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding (1);
        }

        protected override Size DefaultSize => new Size (1080, 720);
    }
}
