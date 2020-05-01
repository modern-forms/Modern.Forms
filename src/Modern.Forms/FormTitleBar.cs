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
        private SKBitmap? image;

        /// <summary>
        /// Initializes a new instance of the FormTitleBar class.
        /// </summary>
        public FormTitleBar ()
        {
            Dock = DockStyle.Top;

            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);
        }

        /// <summary>
        /// Gets or sets whenther the Maximize button is shown.
        /// </summary>
        public bool AllowMaximize { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the Minimize button is shown.
        /// </summary>
        public bool AllowMinimize { get; set; } = true;

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 34);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
           (style) => style.BackgroundColor = Theme.RibbonColor);

        /// <summary>
        /// Gets the element at the specified location.
        /// </summary>
        public FormTitleBarElement GetElementAtLocation (Point location)
        {
            var renderer = RenderManager.GetRenderer<FormTitleBarRenderer> ()!;

            if (renderer.GetCloseButtonBounds (this).Contains (location))
                return FormTitleBarElement.Close;
            if (Image != null && renderer.GetIconBounds (this).Contains (location))
                return FormTitleBarElement.Icon;
            if (AllowMaximize && renderer.GetMaximizeButtonBounds (this).Contains (location))
                return FormTitleBarElement.Maximize;
            if (AllowMinimize && renderer.GetMinimizeButtonBounds (this).Contains (location))
                return FormTitleBarElement.Minimize;

            return FormTitleBarElement.Title;
        }

        /// <summary>
        /// The element the mouse pointer is currently hovering over, if any.
        /// </summary>
        public FormTitleBarElement HoverElement { get; private set; }

        /// <summary>
        /// Gets or sets the image used as the upper-left icon of the titlebar.
        /// </summary>
        public SKBitmap? Image {
            get => image;
            set {
                if (image != value) {
                    image = value;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            switch (GetElementAtLocation (e.Location)) {
                case FormTitleBarElement.Maximize:
                    var form_max = FindForm ();

                    if (form_max != null)
                        form_max.WindowState = form_max.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

                    break;
                case FormTitleBarElement.Minimize:
                    var form_min = FindForm ();

                    if (form_min != null)
                        form_min.WindowState = FormWindowState.Minimized;

                    break;
                case FormTitleBarElement.Close:
                    FindForm ()?.Close ();

                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (GetElementAtLocation (e.Location).In (FormTitleBarElement.Title, FormTitleBarElement.Icon)) {
                // We won't get a MouseUp from the system for this, so don't capture the mouse
                Capture = false;
                FindForm ()?.BeginMoveDrag ();
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetHover (FormTitleBarElement.None);
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetHover (GetElementAtLocation (e.Location));
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        // Update the element that the mouse is currently hovered over
        private void SetHover (FormTitleBarElement element)
        {
            if (element.In (FormTitleBarElement.Icon, FormTitleBarElement.Title))
                element = FormTitleBarElement.None;

            if (HoverElement == element)
                return;

            HoverElement = element;

            Invalidate ();
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Elements of a FormTitleBar.
        /// </summary>
        public enum FormTitleBarElement
        {
            /// <summary>
            /// No element. Not used.
            /// </summary>
            None,

            /// <summary>
            /// The title portion of the titlebar. Basically anything that isn't another element.
            /// </summary>
            Title,

            /// <summary>
            /// The icon of the titlebar.
            /// </summary>
            Icon,

            /// <summary>
            /// The maximize button of the titlebar.
            /// </summary>
            Maximize,

            /// <summary>
            /// The minimize button of the titlebar.
            /// </summary>
            Minimize,

            /// <summary>
            /// The close button of the titlebar.
            /// </summary>
            Close
        }
    }
}
