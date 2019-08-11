using System;
using System.Drawing;

namespace Modern.Forms
{
    // TODO:
    // Disabled styles
    // Pressed styles
    // Image
    // IsDefault?
    // TextAlign/ImageAlign
    public class Button : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.Border.Width = 1);

        public new static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.RibbonTabHighlightColor;
                style.Border.Color = Theme.RibbonColor;
                style.ForegroundColor = Theme.LightTextColor;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
        public override ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        private ContentAlignment text_align = ContentAlignment.MiddleCenter;

        protected override Size DefaultSize => new Size (100, 30);

        public Button ()
        {
            SetControlBehavior (ControlBehaviors.Hoverable);
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        public ContentAlignment TextAlign {
            get => text_align;
            set {
                if (text_align != value) {
                    text_align = value;
                    Invalidate ();
                }
            }
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            e.Canvas.DrawText (Text,PaddedClientRectangle, this, text_align);
        }

        public void PerformClick ()
        {
            OnClick (new MouseEventArgs(MouseButtons.Left, 1, 0, 0, Point.Empty));
        }

        public DialogResult DialogResult { get; set; }
    }
}
