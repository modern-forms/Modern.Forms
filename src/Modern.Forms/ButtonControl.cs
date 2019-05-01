using System;
using System.Drawing;
using System.Windows.Forms;

namespace Modern.Forms
{
    // TODO:
    // Disabled styles
    // Pressed styles
    // Image
    // IsDefault?
    // TextAlign/ImageAlign
    public class ButtonControl : ModernControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (ModernControl.DefaultStyle, 
            (style) => style.Border.Width = 1);

        public new static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle, 
            (style) => {
                style.BackgroundColor = Theme.RibbonTabHighlightColor;
                style.Border.Color = Theme.RibbonColor;
                style.ForegroundColor = Theme.LightTextColor;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
        public override ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        protected override Size DefaultSize => new Size (100, 30);

        public ButtonControl ()
        {
            SetControlBehavior (ControlBehaviors.Hoverable, true);

            Cursor = Cursors.Hand;
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            e.Canvas.DrawCenteredText (Text, Width / 2, 20, CurrentStyle);
        }
    }
}
