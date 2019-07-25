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
        private const int BOX_BORDER_SIZE = 15;
        private const int GLYPH_TEXT_PADDING = 3;

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

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            var box_size = LogicalToDeviceUnits (BOX_BORDER_SIZE);
            var y = (ScaledHeight - box_size) / 2;

            var box_bounds = new Rectangle (3, y, box_size, box_size);

            ControlPaint.DrawCheckBox (e, box_bounds, Checked ? CheckState.Checked : CheckState.Unchecked);

            var glyph_padding = LogicalToDeviceUnits (GLYPH_TEXT_PADDING);
            e.Canvas.DrawText (Text, new Rectangle (box_bounds.Right + glyph_padding, 0, ScaledWidth - box_bounds.Right - glyph_padding, ScaledHeight), this, ContentAlignment.MiddleLeft);
        }
    }
}
