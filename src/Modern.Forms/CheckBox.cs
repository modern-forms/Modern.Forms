using System;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a CheckBox control.
    /// </summary>
    public class CheckBox : Control
    {
        private CheckState state;

        /// <summary>
        /// Initializes a new instance of the CheckBox class.
        /// </summary>
        public CheckBox ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        /// <summary>
        /// Gets or sets a valud indicating if the CheckBox will respond to mouse clicks.
        /// </summary>
        public bool AutoCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating if the CheckBox is in the checked state.
        /// </summary>
        public bool Checked {
            get => state != CheckState.Unchecked;
            set => CheckState = value ? CheckState.Checked : CheckState.Unchecked;
        }

        /// <summary>
        /// Raised when the value of the Checked property changes.
        /// </summary>
        public event EventHandler? CheckedChanged;

        /// <summary>
        /// Gets or sets the current state of the CheckBox.
        /// </summary>
        public CheckState CheckState {
            get => state;
            set {
                if (state != value) {
                    state = value;
                    Invalidate ();
                    OnCheckedChanged (EventArgs.Empty);
                    OnCheckStateChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the value of the CheckState property changes.
        /// </summary>
        public event EventHandler? CheckStateChanged;

        /// <inheritdoc/>
        protected override Cursor DefaultCursor => Cursors.Hand;

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (104, 24);

        /// <summary>
        /// The default ControlStyle for all instances of CheckBox.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = Theme.LightNeutralGray);

        /// <summary>
        /// Raises the CheckedChanged event.
        /// </summary>
        protected virtual void OnCheckedChanged (EventArgs e) => CheckedChanged?.Invoke (this, e);

        /// <summary>
        /// Raises the CheckStateChanged event.
        /// </summary>
        protected virtual void OnCheckStateChanged (EventArgs e) => CheckStateChanged?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            if (AutoCheck) {
                if (ThreeState) {
                    // Order: Unchecked -> Checked -> Indeterminate
                    var new_state = ((int)state + 1);

                    if (new_state == 3)
                        new_state = 0;

                    CheckState = (CheckState)new_state;
                } else {
                    Checked = !Checked;
                }
            }

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

        /// <summary>
        /// Gets or sets a value indicating whether the CheckBox will allow three check states rather than two.
        /// </summary>
        public bool ThreeState { get; set; }
    }
}
