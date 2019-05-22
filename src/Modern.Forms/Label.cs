using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO:
    // AutoEllipsis
    // Image
    // TextAlign/ImageAlign
    public class Label : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private ContentAlignment text_align;

        protected override Size DefaultSize => new Size (100, 23);

        public Label ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        public ContentAlignment TextAlign {
            get => text_align;
            set {
                if (text_align == value)
                    return;

                text_align = value;

                Invalidate ();
            }
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            if (!string.IsNullOrWhiteSpace (Text))
                e.Canvas.DrawText (Text, ClientRectangle, CurrentStyle, TextAlign);
        }
    }
}
