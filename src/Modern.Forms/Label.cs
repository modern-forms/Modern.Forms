using System;
using System.Collections.Specialized;
using System.Drawing;
using Modern.Forms.Layout;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO:
    // AutoEllipsis

    /// <summary>
    /// Represents a Label control.
    /// </summary>
    public class Label : Control, IHaveTextAndImageAlign
    {
        private static readonly object s_eventTextAlignChanged = new ();

        private static readonly BitVector32.Section s_stateUseMnemonic = BitVector32.CreateSection (1);
        private static readonly BitVector32.Section s_stateAutoSize = BitVector32.CreateSection (1, s_stateUseMnemonic);
        private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection (1, s_stateAutoSize);
        private static readonly BitVector32.Section s_stateMultiline = BitVector32.CreateSection (1, s_stateAutoEllipsis);

        private static readonly int s_propImage = PropertyStore.CreateKey ();
        private static readonly int s_propImageAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImageList = PropertyStore.CreateKey ();
        private static readonly int s_propImageIndex = PropertyStore.CreateKey ();
        private static readonly int s_propImageKey = PropertyStore.CreateKey ();
        private static readonly int s_propTextAlign = PropertyStore.CreateKey ();
        private static readonly int s_propTextImageRelation = PropertyStore.CreateKey ();

        private BitVector32 _labelState;

        /// <summary>
        /// Initializes a new instance of the Label class.
        /// </summary>
        public Label ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
            SetControlBehavior (ControlBehaviors.Selectable, false);

            _labelState[s_stateUseMnemonic] = 1;

            TabStop = false;
        }

        /// <summary>
        /// Gets or sets a value indicating if text will be truncated with an ellipsis if it cannot fully fit in the Label.
        /// </summary>
        public bool AutoEllipsis {
            get => _labelState[s_stateAutoEllipsis] != 0;
            set {
                if (AutoEllipsis != value) {

                    _labelState[s_stateAutoEllipsis] = value ? 1 : 0;

                    if (Parent is not null)
                        LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.AutoEllipsis);

                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override Padding DefaultMargin => new Padding (3, 0, 3, 0);

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 23);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Gets or sets the image that is displayed on a <see cref='Label'/>.
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
        /// Gets or sets the alignment of the image on the <see cref='Label'/>.
        /// </summary>
        public ContentAlignment ImageAlign {
            get => Properties.GetEnum (s_propImageAlign, ContentAlignment.MiddleCenter);
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
        /// Gets or sets the index of the image in the <see cref='ImageList'/> to display on the <see cref='CheckBox'/>.
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
        /// Gets or sets the key of the image in the <see cref='ImageList'/> to display on the <see cref='CheckBox'/>.
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
        /// Gets or sets the <see cref='ImageList'/> that contains the image to display on the <see cref='CheckBox'/>.
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
        /// Gets or sets a value indicating if text should wrap.
        /// </summary>
        public bool Multiline {
            get => _labelState[s_stateMultiline] != 0;
            set {
                if (Multiline != value) {
                    _labelState[s_stateMultiline] = value ? 1 : 0;

                    if (Parent is not null)
                        LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.Multiline);

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
            get => Properties.GetEnum (s_propTextAlign, ContentAlignment.TopLeft);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (TextAlign != value) {
                    Properties.SetEnum (s_propTextAlign, value);
                    Invalidate ();

                    OnTextAlignChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the TextAlign property is changed.
        /// </summary>
        public event EventHandler TextAlignChanged {
            add => Events.AddHandler (s_eventTextAlignChanged, value);
            remove => Events.RemoveHandler (s_eventTextAlignChanged, value);
        }

        /// <summary>
        /// Gets or sets a value indicating how the Label's Image and Text are layed out relative to each other.
        /// </summary>
        public TextImageRelation TextImageRelation {
            get => Properties.GetEnum (s_propTextImageRelation, TextImageRelation.Overlay);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (TextImageRelation != value) {
                    Properties.SetEnum (s_propTextImageRelation, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.TextImageRelation);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether an ampersand (&amp;) included in the text of the control.
        /// </summary>
        public bool UseMnemonic {
            get => _labelState[s_stateUseMnemonic] != 0;
            set {
                if (UseMnemonic == value)
                    return;

                _labelState[s_stateUseMnemonic] = value ? 1 : 0;

                // The size of the label need to be adjusted when the Mnemonic is set irrespective of auto-sizing.
                using (LayoutTransaction.CreateTransactionIf (AutoSize, Parent, this, PropertyNames.Text)) {
                    //AdjustSize ();
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void Dispose (bool disposing)
        {
            if (disposing && Image is not null)
                Properties.SetObject (s_propImage, null);

            base.Dispose (disposing);
        }

        /// <summary>
        /// Called when the TextAlign property is changed.
        /// </summary>
        protected virtual void OnTextAlignChanged (EventArgs e) => (Events[s_eventTextAlignChanged] as EventHandler)?.Invoke (this, e);
    }
}
