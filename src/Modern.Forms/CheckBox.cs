using System;
using System.Collections.Specialized;
using System.Drawing;
using Modern.Forms.Layout;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a CheckBox control.
    /// </summary>
    public class CheckBox : Control, IHaveGlyph, IHaveTextAndImageAlign
    {
        private CheckState state;
        private SKBitmap? _image;
        private int _imageIndex = -1;
        private string _imageKey = string.Empty;
        private ImageList? _imageList;

        private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection (1);

        private static readonly int s_propCheckAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImageAlign = PropertyStore.CreateKey ();
        private static readonly int s_propTextAlign = PropertyStore.CreateKey ();
        private static readonly int s_propTextImageRelation = PropertyStore.CreateKey ();

        private BitVector32 _checkboxState;

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
        /// Gets or sets a value indicating if text will be truncated with an ellipsis if it cannot fully fit in the <see cref='CheckBox'/>.
        /// Value is currently ignored.
        /// </summary>
        public bool AutoEllipsis {
            get => _checkboxState[s_stateAutoEllipsis] != 0;
            set {
                if (AutoEllipsis != value) {

                    _checkboxState[s_stateAutoEllipsis] = value ? 1 : 0;

                    if (Parent is not null)
                        LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.AutoEllipsis);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the checkbox glyph on the <see cref='CheckBox'/>.
        /// </summary>
        public ContentAlignment GlyphAlign {
            get => Properties.GetEnum (s_propCheckAlign, ContentAlignment.MiddleLeft);
            set {
                SourceGenerated.EnumValidator.Validate (value);

                if (value != GlyphAlign) {
                    Properties.SetEnum (s_propCheckAlign, value);
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.GlyphAlign);
                    Invalidate ();
                }
            }
        }

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
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Gets or sets the image displayed on the <see cref='CheckBox'/>.
        /// </summary>
        public SKBitmap? Image {
            get => _image;
            set {
                if (_image != value) {
                    _image = value;
                    LayoutTransaction.DoLayoutIf (AutoSize, Parent, this, PropertyNames.Image);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the image on the <see cref='CheckBox'/>.
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
        /// Gets or sets the index of the image in the <see cref='ImageList'/> to display on the <see cref='CheckBox'/>.
        /// </summary>
        public int ImageIndex {
            get => _imageIndex;
            set {
                if (_imageIndex != value) {
                    _imageIndex = value;

                    // Setting this clears any existing ImageKey and Image
                    if (value >= 0) {
                        _image = null;
                        _imageKey = string.Empty;
                    }

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the key of the image in the <see cref='ImageList'/> to display on the <see cref='CheckBox'/>.
        /// </summary>
        public string ImageKey {
            get => _imageKey;
            set {
                if (_imageKey != value) {
                    _imageKey = value;

                    // Setting this clears any existing ImageIndex and Image
                    if (value is not null) {
                        _image = null;
                        _imageIndex = -1;
                    }

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='ImageList'/> that contains the image to display on the <see cref='CheckBox'/>.
        /// </summary>
        public ImageList? ImageList {
            get => _imageList;
            set {
                if (_imageList != value) {
                    _imageList = value;

                    // If an image list is set, clear any existing image
                    if (value is not null)
                        _image = null;

                    Invalidate ();
                }
            }
        }

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
        /// Gets or sets the alignment of the text on the <see cref='CheckBox'/>.
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
        /// Gets or sets the alignment of the text relative to the image on the <see cref='CheckBox'/>.
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

        /// <summary>
        /// Gets or sets a value indicating whether the CheckBox will allow three check states rather than two.
        /// </summary>
        public bool ThreeState { get; set; }

        /// <inheritdoc/>
        public override string ToString () => $"{base.ToString ()}, CheckState: {(int)CheckState}";
    }
}
