using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    // TODO:
    // Disabled style
    // ThreeState
    // TextAlign/CheckAlign
    // Hover style?
    // Pressed style?
    // GroupKey?
    public class RadioButton : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = Theme.LightNeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public event EventHandler CheckedChanged;

        private bool is_checked;

        public RadioButton ()
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

                    if (is_checked)
                        UpdateSiblings ();

                    OnCheckedChanged (EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCheckedChanged (EventArgs e) => CheckedChanged?.Invoke (this, e);

        protected override void OnClick (MouseEventArgs e)
        {
            if (!Checked)
                Checked = true;

            base.OnClick (e);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            var x = LogicalToDeviceUnits (11);
            var y = ScaledHeight / 2;
            var outer_radius = LogicalToDeviceUnits (8);
            var inner_radius = LogicalToDeviceUnits (5);

            if (Checked) {
                e.Canvas.DrawCircle (x, y, outer_radius, Theme.RibbonColor);
                e.Canvas.FillCircle (x, y, inner_radius, Theme.RibbonColor);
            } else {
                e.Canvas.DrawCircle (x, y, outer_radius, Theme.BorderGray);
            }

            var text_bounds = ClientRectangle;
            text_bounds.X += LogicalToDeviceUnits (24);
            text_bounds.Width -= LogicalToDeviceUnits (24);

            e.Canvas.DrawText (Text, CurrentStyle.GetFont (), LogicalToDeviceUnits (CurrentStyle.GetFontSize ()), text_bounds, CurrentStyle.GetForegroundColor (), ContentAlignment.MiddleLeft);
        }

        // Uncheck any other RadioButtons on the Parent
        private void UpdateSiblings ()
        {
            var siblings = Parent?.Controls.OfType<RadioButton> ().Where (rb => rb != this);

            if (siblings != null)
                foreach (var rb in siblings)
                    rb.Checked = false;
        }
    }
}
