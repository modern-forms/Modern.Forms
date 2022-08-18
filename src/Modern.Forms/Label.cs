using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    // TODO:
    // AutoEllipsis
    // Image
    // TextAlign/ImageAlign

    /// <summary>
    /// Represents a Label control.
    /// </summary>
    public class Label : Control
    {
        private bool auto_ellipsis;
        private bool multiline;
        private ContentAlignment text_align = ContentAlignment.MiddleLeft;

        /// <summary>
        /// Initializes a new instance of the Label class.
        /// </summary>
        public Label ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        /// <summary>
        /// Gets or sets a value indicating if text will be truncated with an ellipsis if it cannot fully fit in the Label.
        /// </summary>
        public bool AutoEllipsis {
            get => auto_ellipsis;
            set {
                if (auto_ellipsis != value) {
                    auto_ellipsis = value;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 23);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Gets or sets a value indicating if text should wrap.
        /// </summary>
        public bool Multiline {
            get => multiline;
            set {
                if (multiline != value) {
                    multiline = value;
                    Invalidate ();
                }
            }
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
        /// Gets or sets a value indicating how text will be aligned within the Label.
        /// </summary>
        public ContentAlignment TextAlign {
            get => text_align;
            set {
                if (text_align == value)
                    return;

                text_align = value;

                Invalidate ();
            }
        }
    }
}
