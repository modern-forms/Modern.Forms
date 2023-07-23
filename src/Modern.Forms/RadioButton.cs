using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a RadioButton control.
    /// </summary>
    public class RadioButton : Control
    {
        private bool is_checked;

        /// <summary>
        /// Initializes a new instance of the RadioButton class.
        /// </summary>
        public RadioButton ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        /// <summary>
        /// Gets or sets a valud indicating if the RadioButton will respond to mouse clicks.
        /// </summary>
        public bool AutoCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating if the RadioButton is in the checked state.
        /// </summary>
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

        /// <summary>
        /// Raised when the value of the Checked property changes.
        /// </summary>
        public event EventHandler? CheckedChanged;

        /// <inheritdoc/>
        protected override Cursor DefaultCursor => Cursors.Hand;

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (104, 24);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Raises the CheckedChanged event.
        /// </summary>
        protected virtual void OnCheckedChanged (EventArgs e) => CheckedChanged?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            if (AutoCheck && !Checked)
                Checked = true;

            base.OnClick (e);
        }

        /// <inheritdoc/>
        protected override void OnKeyUp (KeyEventArgs e)
        {
            if (e.KeyCode.In (Keys.Space, Keys.Enter)) {
                OnClick (new MouseEventArgs (MouseButtons.Left, 1, 0, 0, Point.Empty));
                e.Handled = true;
                return;
            }

            base.OnKeyUp (e);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

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
