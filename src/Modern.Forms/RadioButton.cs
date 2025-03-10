using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using Modern.Forms.Layout;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a RadioButton control.
    /// </summary>
    public class RadioButton : Control, IHaveGlyph, IHaveTextAndImageAlign
    {
        private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection (1);
        private static readonly BitVector32.Section s_stateChecked = BitVector32.CreateSection (1, s_stateAutoEllipsis);

        private static readonly int s_propGlyphAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImage = PropertyStore.CreateKey ();
        private static readonly int s_propImageAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImageList = PropertyStore.CreateKey ();
        private static readonly int s_propImageIndex = PropertyStore.CreateKey ();
        private static readonly int s_propImageKey = PropertyStore.CreateKey ();
        private static readonly int s_propTextAlign = PropertyStore.CreateKey ();
        private static readonly int s_propTextImageRelation = PropertyStore.CreateKey ();

        private BitVector32 _radiobuttonState;

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
        /// Gets or sets a value indicating if text will be truncated with an ellipsis if it cannot fully fit in the <see cref='RadioButton'/>.
        /// </summary>
        public bool AutoEllipsis {
            get => _radiobuttonState[s_stateAutoEllipsis] != 0;
            set {
                if (AutoEllipsis != value) {

                    _radiobuttonState[s_stateAutoEllipsis] = value ? 1 : 0;

                    if (Parent is not null)
                        LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.AutoEllipsis);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the radio button glyph on the <see cref='RadioButton'/>.
        /// </summary>
        public ContentAlignment GlyphAlign {
            get => Properties.GetEnum (s_propGlyphAlign, ContentAlignment.MiddleLeft);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (value != GlyphAlign) {
                    Properties.SetEnum (s_propGlyphAlign, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.GlyphAlign);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the RadioButton is in the checked state.
        /// </summary>
        public bool Checked {
            get => _radiobuttonState[s_stateChecked] != 0;
            set {
                if (Checked != value) {
                    _radiobuttonState[s_stateChecked] = value ? 1 : 0;
                    Invalidate ();

                    if (value)
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
        /// Gets or sets the image displayed on the <see cref='RadioButton'/>.
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
        /// Gets or sets the alignment of the image on the <see cref='RadioButton'/>.
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
        /// Gets or sets the index of the image in the <see cref='ImageList'/> associated with the <see cref='RadioButton'/>.
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
        /// Gets or sets the key of the image in the <see cref='ImageList'/> associated with the <see cref='RadioButton'/>.
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
        /// Gets or sets the <see cref='ImageList'/> associated with the <see cref='RadioButton'/>.
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

        /// <summary>
        /// Gets or sets the alignment of the text on the <see cref='RadioButton'/>.
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
        /// Gets or sets the alignment of the text relative to the image on the <see cref='RadioButton'/>.
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
        public override string ToString () => $"{base.ToString ()}, Checked: {Checked}";

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
