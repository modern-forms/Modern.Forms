using System;
using System.Drawing;

namespace Modern.Forms
{
    public class ProgressBar : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.Border.Width = 1);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private int maximum = 100;
        private int minimum = 0;
        private int current_value;

        public ProgressBar ()
        {
        }

        public void Increment (int? value = null)
        {
            var new_value = Value + value.GetValueOrDefault (Step);

            new_value = new_value.Clamp (minimum, maximum);

            Value = new_value;
        }

        public int Maximum {
            get => maximum;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (Maximum),
                        string.Format ("Value '{0}' must be greater than or equal to 0.", value));

                if (maximum != value) {
                    maximum = value;
                    minimum = Math.Min (minimum, maximum);
                    current_value = Math.Min (current_value, maximum);
                    Invalidate ();
                }
            }
        }

        public int Minimum {
            get => minimum;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (Minimum),
                        string.Format ("Value '{0}' must be greater than or equal to 0.", value));

                if (minimum != value) {
                    minimum = value;
                    maximum = Math.Max (minimum, maximum);
                    current_value = Math.Max (current_value, minimum);
                    Invalidate ();
                }
            }
        }

        public int Step { get; set; } = 10;

        public int Value {
            get => current_value;
            set {
                if (value < Minimum || value > Maximum)
                    throw new ArgumentOutOfRangeException (nameof (Value), string.Format ("'{0}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'", value));

                if (current_value != value) {
                    current_value = value;
                    Invalidate ();
                }
            }
        }

        protected override Padding DefaultPadding => new Padding (1);

        protected override Size DefaultSize => new Size (100, 23);

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            // Prevent divide by zero
            if (maximum == minimum)
                return;

            var percent = (float)(current_value - minimum) / (maximum - minimum);
            var client_area = PaddedClientRectangle;
            var filled_pixels = (int)(percent * client_area.Width);

            if (filled_pixels > 0)
                e.Canvas.FillRectangle (client_area.X, client_area.Y, filled_pixels, client_area.Height, Theme.RibbonColor);
        }
    }
}
