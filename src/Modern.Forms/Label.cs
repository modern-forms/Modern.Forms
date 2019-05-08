using System;
using System.Drawing;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO:
    // AutoEllipsis
    // Image
    // TextAlign/ImageAlign
    public class Label : LiteControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        protected override Size DefaultSize => new Size (100, 23);

        public Label ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            e.Canvas.DrawCenteredText (Text, Width / 2, 15, CurrentStyle);
        }
    }
}
