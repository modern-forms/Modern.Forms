using System.Collections.Specialized;
using System.Drawing;
using Modern.Forms.Layout;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a Button control.
    /// </summary>
    public class Button : Control, IHaveTextAndImageAlign
    {
        private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection (1);

        private static readonly int s_propImage = PropertyStore.CreateKey ();
        private static readonly int s_propImageAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImageList = PropertyStore.CreateKey ();
        private static readonly int s_propImageIndex = PropertyStore.CreateKey ();
        private static readonly int s_propImageKey = PropertyStore.CreateKey ();
        private static readonly int s_propTextAlign = PropertyStore.CreateKey ();
        private static readonly int s_propTextImageRelation = PropertyStore.CreateKey ();

        private BitVector32 _buttonState;

        /// <summary>
        /// Initializes a new instance of the Button class.
        /// </summary>
        public Button ()
        {
            SetControlBehavior (ControlBehaviors.Hoverable);
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        /// <summary>
        /// Gets or sets a value indicating if text will be truncated with an ellipsis if it cannot fully fit in the <see cref='Button'/>.
        /// </summary>
        public bool AutoEllipsis {
            get => _buttonState[s_stateAutoEllipsis] != 0;
            set {
                if (AutoEllipsis != value) {

                    _buttonState[s_stateAutoEllipsis] = value ? 1 : 0;

                    if (Parent is not null)
                        LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.AutoEllipsis);

                    Invalidate ();
                }
            }
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
                style.BackgroundColor = Theme.AccentColor;
                style.Border.Color = Theme.AccentColor2;
                style.ForegroundColor = Theme.ForegroundColorOnAccent;
            });

        /// <summary>
        /// Gets or sets a value that is returned to the parent form when the button is clicked.
        /// </summary>
        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// Gets or sets the image displayed on the <see cref='Button'/>.
        /// </summary>
        public SKBitmap? Image {
            get => Properties.GetObject<SKBitmap> (s_propImage);
            set {
                if (Image != value) {
                    Properties.SetObject (s_propImage, value);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the image on the <see cref='Button'/>.
        /// </summary>
        public ContentAlignment ImageAlign {
            get => Properties.GetEnum (s_propImageAlign, ContentAlignment.MiddleLeft);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (value != ImageAlign) {
                    Properties.SetEnum (s_propImageAlign, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.ImageAlign);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the image in the <see cref='ImageList'/> to display on the <see cref='Button'/>.
        /// </summary>
        public int ImageIndex {
            get => Properties.GetInteger (s_propImageIndex, -1);
            set {
                if (ImageIndex != value) {
                    Properties.SetInteger (s_propImageIndex, value);

                    // Setting this clears any existing ImageKey and Image
                    if (value >= 0) {
                        Properties.RemoveObject (s_propImage);
                        Properties.RemoveObject (s_propImageKey);
                    }

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the key of the image in the <see cref='ImageList'/> to display on the <see cref='Button'/>.
        /// </summary>
        public string ImageKey {
            get => Properties.GetObject<string> (s_propImageKey) ?? string.Empty;
            set {
                if (ImageKey != value) {
                    Properties.SetObject (s_propImageKey, value);

                    // Setting this clears any existing ImageIndex and Image
                    if (value is not null) {
                        Properties.RemoveObject (s_propImage);
                        Properties.RemoveInteger (s_propImageIndex);
                    }

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='ImageList'/> that contains the image to display on the <see cref='Button'/>.
        /// </summary>
        public ImageList? ImageList {
            get => Properties.GetObject<ImageList> (s_propImageList);
            set {
                if (ImageList != value) {
                    Properties.SetObject (s_propImageList, value);

                    // If an image list is set, clear any existing image
                    if (value is not null)
                        Properties.RemoveObject (s_propImage);

                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            if (FindForm () is Form form)
                form.DialogResult = DialogResult;

            base.OnClick (e);
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

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
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
        /// Gets or sets the alignment of the text on the <see cref='Button'/>.
        /// </summary>
        public ContentAlignment TextAlign {
            get => Properties.GetEnum (s_propTextAlign, ContentAlignment.MiddleLeft);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (value != TextAlign) {
                    Properties.SetEnum (s_propTextAlign, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.TextAlign);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text relative to the image on the <see cref='Button'/>.
        /// </summary>
        public TextImageRelation TextImageRelation {
            get => Properties.GetEnum (s_propTextImageRelation, TextImageRelation.ImageBeforeText);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (value != TextImageRelation) {
                    Properties.SetEnum (s_propTextImageRelation, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.TextImageRelation);
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString () => $"{base.ToString ()}, Text: {Text}";
    }
}
