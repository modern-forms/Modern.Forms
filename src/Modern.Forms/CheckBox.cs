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
        private const int BOX_FILL_SIZE = 11;
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

            var y = (Height - 16) / 2;
            var box_size = LogicalToDeviceUnits (BOX_BORDER_SIZE);
            var box_fill_size = LogicalToDeviceUnits (BOX_FILL_SIZE);

            var box_bounds = new Rectangle (3, y, box_size, box_size);
            var fill_size = new Rectangle (Point.Empty, new Size (box_fill_size, box_fill_size));
            var fill_bounds = DrawingExtensions.CenterRectangle (box_bounds, fill_size);

            if (Checked) {
                e.Canvas.DrawRectangle (box_bounds, Theme.RibbonColor, LogicalToDeviceUnits (1));
                e.Canvas.FillRectangle (fill_bounds, Theme.RibbonColor);
            } else {
                e.Canvas.DrawRectangle (box_bounds, Theme.BorderGray, LogicalToDeviceUnits (1));
            }

            var glyph_padding = LogicalToDeviceUnits (GLYPH_TEXT_PADDING);
            e.Canvas.DrawText (Text, CurrentStyle.GetFont (), LogicalToDeviceUnits (CurrentStyle.GetFontSize ()), new Rectangle (box_bounds.Right + glyph_padding, 0, ScaledWidth - box_bounds.Right - glyph_padding, ScaledHeight), CurrentStyle.GetForegroundColor (), ContentAlignment.MiddleLeft);
        }
    }
}
