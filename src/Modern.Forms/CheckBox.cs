using System;
using System.Collections.Specialized;
using System.Diagnostics;
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
        private CheckState _state;

        private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection (1);

        private static readonly int s_propCheckAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImage = PropertyStore.CreateKey ();
        private static readonly int s_propImageAlign = PropertyStore.CreateKey ();
        private static readonly int s_propImageList = PropertyStore.CreateKey ();
        private static readonly int s_propImageIndex = PropertyStore.CreateKey ();
        private static readonly int s_propImageKey = PropertyStore.CreateKey ();
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

        public override bool AutoSize {
            get => base.AutoSize;
            set {
                if (base.AutoSize != value) {
                    base.AutoSize = value;
                    SetState (States.IsDirty, true);
                }
            }
        }

        public override string Text {
            get => base.Text;
            set {
                if (base.Text != value) {
                    base.Text = value;
                    Trigger_Resizing ();
                }
            }
        }

        private void Trigger_Resizing (bool isEnabled = true)
        {
            if(!isEnabled)
            return;

            //required
            //SetState (States.IsDirty, true);
            var old = AutoSize;
            AutoSize = !old;
            AutoSize = old;
        }
        private void Trigger_Resizing_forEnums (bool isEnabled = false)
        {
            if (!isEnabled)
                return;

            //required
            //SetState (States.IsDirty, true);
            var old = AutoSize;
            AutoSize = !old;
            AutoSize = old;
        }

        Size calculate_AutosizeArea_old () 
        {
            //the layout calculation is corrent - we just need to make controls.width bigger to show all the clipped texts
            //find biggest right Coord
            var newSize = new Size (DefaultSize.Width, DefaultSize.Height);

            var layout = TextImageLayoutEngine.Layout (this);

            var rightMax = 0;
            rightMax = Math.Max (rightMax, layout.GlyphBounds.Right);
            rightMax = Math.Max (rightMax, layout.ImageBounds.Right);
            rightMax = Math.Max (rightMax, layout.TextBounds.Right);
            
            var bottomMax = 0;
            bottomMax = Math.Max (bottomMax, layout.GlyphBounds.Bottom);
            bottomMax = Math.Max (bottomMax, layout.ImageBounds.Bottom);
            bottomMax = Math.Max (bottomMax, layout.TextBounds.Bottom);

            return new Size (rightMax, bottomMax);
        }

        /// <summary>
        /// this doesnt uses X,Y of TextImageLayoutEngine.Layout . only sizes. to prevent element getting bigger on each .Layout()
        /// </summary>
        /// <returns></returns>
        int Bounds_Width_Sum (TextImageLayoutData layout)
        {
            return
              //layout.GlyphBounds.Width
            + layout.ImageBounds.Width
            + layout.TextBounds.Width;
        }
        int Bounds_Height_Sum (TextImageLayoutData layout)
        {
            return
              //layout.GlyphBounds.Height 
            + layout.ImageBounds.Height 
            + layout.TextBounds.Height;
        }
        int Bounds_Width_Max (TextImageLayoutData layout)
        {
            var widthMax = 0;
            widthMax = Math.Max (layout.TextBounds.Width, layout.ImageBounds.Width);
            //widthMax = Math.Max (widthMax, layout.GlyphBounds.Width);
            return widthMax;
        }
        int Bounds_Height_Max (TextImageLayoutData layout)
        {
            var heightMax = 0;
            heightMax = Math.Max (layout.TextBounds.Height, layout.ImageBounds.Height);
            //heightMax = Math.Max (heightMax, layout.GlyphBounds.Height);
            return heightMax;
        }

        Size calculate_AutosizeArea ()
        {
            //var newSize = new Size (DefaultSize.Width, DefaultSize.Height);

            //newSize.Width = 100;
            //newSize.Height = 30;

            //return newSize;

            var layout = TextImageLayoutEngine.Layout(this);
            Debug.WriteLine ("GlyphBounds: " + layout.GlyphBounds);
            Debug.WriteLine ("ImageBounds: "+layout.ImageBounds);
            Debug.WriteLine ("TextBounds: "+layout.TextBounds);

            var total_width = 0;
            var total_height = 0;
            //total_width = Bounds_Width_Sum (layout);
            //total_height = Bounds_Height_Sum (layout);

            switch (this.TextImageRelation) {
                case TextImageRelation.TextBeforeImage:
                case TextImageRelation.ImageBeforeText:
                    total_width = Bounds_Width_Sum (layout);
                    total_height = Bounds_Height_Max(layout);
                    break;
                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    total_width = Bounds_Width_Max (layout);
                    total_height = Bounds_Height_Sum (layout);
                    break;
                case TextImageRelation.Overlay:
                    total_width = Bounds_Width_Max (layout);
                    total_height = Bounds_Height_Max (layout);
                    break;
            }

            total_width +=
                + layout.GlyphBounds.Width
                + this.Padding.Horizontal
                + (this.Style.Border.Width ?? 0) * 2
                ;
            total_height +=
                + layout.GlyphBounds.Height
                + this.Padding.Vertical
                + (this.Style.Border.Width ?? 0) * 2
                ;

            return new Size (total_width, total_height);

            //height . sum 
            // (bounds width max) + pad + border


        }


        internal override Size GetPreferredSizeCore (Size proposedSize)
        {
            //return base.GetPreferredSizeCore (proposedSize);
            if (AutoSize) 
            {
                return calculate_AutosizeArea ();
            }
            return DefaultSize;
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
                    Trigger_Resizing_forEnums ();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the CheckBox is in the checked state.
        /// </summary>
        public bool Checked {
            get => _state != CheckState.Unchecked;
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
            get => _state;
            set {
                if (_state != value) {
                    _state = value;
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
            get => Properties.GetObject<SKBitmap> (s_propImage);
            set {
                if (Image != value) {
                    Properties.SetObject (s_propImage, value);
                    Invalidate ();
                    Trigger_Resizing ();
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
                    Trigger_Resizing_forEnums ();
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
                    var new_state = ((int)_state + 1);

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
                    Trigger_Resizing_forEnums ();
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
                    Trigger_Resizing_forEnums ();
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
