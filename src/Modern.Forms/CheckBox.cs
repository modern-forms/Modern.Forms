using System;
using System.Drawing;

namespace Modern.Forms
{
    // TODO:
    // Disabled style
    // ThreeState
    // TextAlign/CheckAlign
    // Hover style?
    // Pressed style?
    public class CheckBox : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = Theme.LightNeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public event EventHandler CheckedChanged;

        private bool is_checked;

        public CheckBox ()
        {
            //Cursor = Cursors.Hand;
        }

        protected override Size DefaultSize => new Size (104, 24);

        public bool Checked {
            get => is_checked;
            set {
                if (is_checked != value) {
                    is_checked = value;
                    Invalidate ();
                    OnCheckedChanged (EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCheckedChanged (EventArgs e) => CheckedChanged?.Invoke (this, e);

        protected override void OnClick (MouseEventArgs e)
        {
            Checked = !Checked;

            base.OnClick (e);
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            var y = (Height - 16) / 2;

            if (Checked) {
                e.Canvas.DrawRectangle (3, y, 15, 15, Theme.RibbonColor);
                e.Canvas.FillRectangle (6, y + 3, 10, 10, Theme.RibbonColor);
            } else {
                e.Canvas.DrawRectangle (3, y, 15, 15, Theme.BorderGray);
            }

            e.Canvas.DrawText (Text, new Rectangle (24, 0, Width - 24, Height), CurrentStyle, ContentAlignment.MiddleLeft);
        }
    }
}
