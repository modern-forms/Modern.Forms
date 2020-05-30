using System;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a Button control.
    /// </summary>
    public class Button : Control
    {
        private ContentAlignment text_align = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Initializes a new instance of the Button class.
        /// </summary>
        public Button ()
        {
            SetControlBehavior (ControlBehaviors.Hoverable);
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        /// <inheritdoc/>
        protected override Cursor DefaultCursor => Cursors.Hand;

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 30);

        /// <summary>
        /// The default ControlStyle for all instances of Button.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.Border.Width = 1);

        /// <summary>
        /// The default hover ControlStyle for all instances of Button.
        /// </summary>
        public new static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.HighlightColor;
                style.Border.Color = Theme.PrimaryColor;
                style.ForegroundColor = Theme.LightTextColor;
            });

        /// <summary>
        /// Gets or sets a value that is returned to the parent form when the button is clicked.
        /// </summary>
        public DialogResult DialogResult { get; set; }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <inheritdoc/>
        protected override void OnKeyUp (KeyEventArgs e)
        {
            if (e.KeyCode.In (Keys.Space, Keys.Enter)) {
                PerformClick ();
                e.Handled = true;
                return;
            }

            base.OnKeyUp (e);
        }

        /// <summary>
        /// Generates a Click event for the Button.
        /// </summary>
        public void PerformClick ()
        {
            OnClick (new MouseEventArgs (MouseButtons.Left, 1, 0, 0, Point.Empty));
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <inheritdoc/>
        public override ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        /// <summary>
        /// Gets or sets the text alignment of the Button.
        /// </summary>
        public ContentAlignment TextAlign {
            get => text_align;
            set {
                if (text_align != value) {
                    text_align = value;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        public override string? ToString () => $"{base.ToString ()}, Text: {Text}";
    }
}
