using System;
using System.Drawing;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a FormTitleBar control.
    /// </summary>
    public class FormTitleBar : Control
    {
        private readonly TitleBarButton minimize_button;
        private readonly TitleBarButton maximize_button;
        private readonly TitleBarButton close_button;
        private readonly PictureBox form_image;
        
        private bool show_image = true;

        /// <summary>
        /// Initializes a new instance of the FormTitleBar class.
        /// </summary>
        public FormTitleBar ()
        {
            Dock = DockStyle.Top;

            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
            SetControlBehavior (ControlBehaviors.Selectable, false);

            minimize_button = Controls.AddImplicitControl (new TitleBarButton (TitleBarButton.TitleBarButtonGlyph.Minimize));
            minimize_button.Click += (o, e) => {
                var form_min = FindForm ();

                if (form_min != null)
                    form_min.WindowState = FormWindowState.Minimized;
            };

            maximize_button = Controls.AddImplicitControl (new TitleBarButton (TitleBarButton.TitleBarButtonGlyph.Maximize));
            maximize_button.Click += (o, e) => {
                var form = FindForm ();

                if (form != null)
                    form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            };

            close_button = Controls.AddImplicitControl (new TitleBarButton (TitleBarButton.TitleBarButtonGlyph.Close));
            close_button.Click += (o, e) => { FindForm ()?.Close (); };

            form_image = Controls.AddImplicitControl (new PictureBox {
                Width = DefaultSize.Height,
                Dock = DockStyle.Left,
                Visible = false,
                SizeMode = PictureBoxSizeMode.CenterImage
            });

            form_image.Style.BackgroundColor = SKColors.Transparent;
            form_image.SetControlBehavior (ControlBehaviors.ReceivesMouseEvents, false);
        }

        /// <summary>
        /// Gets or sets whenther the Maximize button is shown.
        /// </summary>
        public bool AllowMaximize {
            get => maximize_button.Visible;
            set {
                maximize_button.Visible = value;
                Invalidate (); // TODO: Shouldn't be necessary, should automatically be triggered
            }
        }

        /// <summary>
        /// Gets or sets whether the Minimize button is shown.
        /// </summary>
        public bool AllowMinimize {
            get => minimize_button.Visible;
            set {
                minimize_button.Visible = value;
                Invalidate (); // TODO: Shouldn't be necessary, should automatically be triggered
            }
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 34);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
           (style) => style.BackgroundColor = Theme.PrimaryColor);

        /// <summary>
        /// Gets or sets the image used as the upper-left icon of the titlebar.
        /// </summary>
        public SKBitmap? Image {
            get => form_image.Image;
            set {
                if (form_image.Image != value) {
                    form_image.Image = value;

                    form_image.Visible = form_image.Image is not null && show_image;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            // We won't get a MouseUp from the system for this, so don't capture the mouse
            Capture = false;
            FindForm ()?.BeginMoveDrag ();
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <inheritdoc/>
        protected override void OnSizeChanged (EventArgs e)
        {
            base.OnSizeChanged (e);

            // Keep our form image a square
            form_image.Width = Height;
        }

        /// <summary>
        /// Specifies whether the Form's Image should be shown in the left corner.
        /// </summary>
        public bool ShowImage {
            get => show_image;
            set {
                if (show_image != value) {
                    show_image = value;
                    form_image.Visible = value && form_image is not null;
                    Invalidate (); // TODO: Shouldn't be required
                }
            }
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        internal class TitleBarButton : Button
        {
            protected const int BUTTON_PADDING = 10;

            private TitleBarButtonGlyph glyph;

            public TitleBarButton (TitleBarButtonGlyph glyph)
            {
                this.glyph = glyph;
                Width = 46;
                Dock = DockStyle.Right;

                Style.BackgroundColor = SKColors.Transparent;
                Style.Border.Width = 0;
                StyleHover.Border.Width = 0;
            }

            protected override void OnPaint (PaintEventArgs e)
            {
                base.OnPaint (e);

                if (IsHovering)
                    e.Canvas.Clear (glyph == TitleBarButtonGlyph.Close ? Theme.FormCloseHighlightColor : Theme.HighlightColor);

                var glyph_bounds = glyph == TitleBarButtonGlyph.Minimize ?
                    DrawingExtensions.CenterRectangle (DisplayRectangle, e.LogicalToDeviceUnits (new Size (BUTTON_PADDING, 1))) :
                    DrawingExtensions.CenterSquare (DisplayRectangle, e.LogicalToDeviceUnits (BUTTON_PADDING));

                switch (glyph) {
                    case TitleBarButtonGlyph.Close:
                        ControlPaint.DrawCloseGlyph (e, glyph_bounds);
                        break;
                    case TitleBarButtonGlyph.Minimize:
                        ControlPaint.DrawMinimizeGlyph (e, glyph_bounds);
                        break;
                    case TitleBarButtonGlyph.Maximize:
                        ControlPaint.DrawMaximizeGlyph (e, glyph_bounds);
                        break;
                }
            }

            public enum TitleBarButtonGlyph
            {
                Close,
                Minimize,
                Maximize
            }
        }
    }
}
