using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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

        protected override Padding DefaultPadding => new Padding (3);

        protected override Size DefaultSize => new Size (600, 25);

        public StatusBar ()
        {
            Dock = DockStyle.Bottom;
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            if (!string.IsNullOrWhiteSpace (Text)) {
                var padding = LogicalToDeviceUnits (Padding);
                var text_bounds = new Rectangle (ClientRectangle.Left + padding.Left, ClientRectangle.Top, ClientRectangle.Width - padding.Horizontal, ClientRectangle.Height);
                e.Canvas.DrawText (Text, text_bounds, this, ContentAlignment.MiddleLeft);
            }
        }
    }
}
