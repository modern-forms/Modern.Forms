using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class FormTitleBar : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
           (style) => style.BackgroundColor = Theme.RibbonColor);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private bool close_button_hover;
        private bool maximize_button_hover;
        private bool minimize_button_hover;
        private SKBitmap form_icon;
        private SKBitmap sized_icon;

        public SKBitmap Image {
            get => form_icon;
            set {
                if (form_icon == value)
                    return;

                form_icon = value;

                if (form_icon.Height > 16) {
                    sized_icon?.Dispose ();
                    sized_icon = new SKBitmap (16, 16);
                    form_icon.ScalePixels (sized_icon, SKFilterQuality.High);
                } else {
                    sized_icon = value;
                }

                Invalidate ();
            }
        }

        public bool AllowMaximize { get; set; } = true;
        public bool AllowMinimize { get; set; } = true;

        protected override Size DefaultSize => new Size (600, 34);

        public FormTitleBar ()
        {
            Dock = DockStyle.Top;
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            if (CloseButtonBounds.Contains (e.Location))
                FindForm ().Close ();
            else if (AllowMinimize && MinimizeButtonBounds.Contains (e.Location))
                FindForm ().WindowState = FormWindowState.Minimized;
            else if (AllowMaximize && MaximizeButtonBounds.Contains (e.Location)) {
                var form = FindForm ();
                form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            }
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!CloseButtonBounds.Contains (e.Location) && !(AllowMaximize && MaximizeButtonBounds.Contains (e.Location)) && !(AllowMinimize && MinimizeButtonBounds.Contains (e.Location))) {
                // We won't get a MouseUp from the system for this, so don't capture the mouse
                Capture = false;
                FindForm ().BeginMoveDrag ();
            }
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetCloseHover (CloseButtonBounds.Contains (e.Location));
            SetMaximizeHover (MaximizeButtonBounds.Contains (e.Location));
            SetMinimizeHover (MinimizeButtonBounds.Contains (e.Location));
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetCloseHover (false);
            SetMaximizeHover (false);
            SetMinimizeHover (false);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            // Form icon
            if (sized_icon != null)
                e.Canvas.DrawBitmap (sized_icon, Bounds.Left + 7, Bounds.Top + 7);

            // Form text
            if (!string.IsNullOrWhiteSpace (Text))
                e.Canvas.DrawCenteredText (Text.Trim (), Theme.UIFont, 14, Left + Width / 2, Top + 21, Theme.LightTextColor);

            // Minimize button
            if (AllowMinimize) {
                var minimize_button_bounds = MinimizeButtonBounds;

                if (minimize_button_hover)
                    e.Canvas.FillRectangle (minimize_button_bounds, Theme.RibbonTabHighlightColor);

                e.Canvas.DrawLine (minimize_button_bounds.X + 18, minimize_button_bounds.Y + 17, minimize_button_bounds.X + 28, minimize_button_bounds.Y + 17, Theme.LightTextColor);
            }

            // Maximize button
            if (AllowMaximize) {
                var maximize_button_bounds = MaximizeButtonBounds;

                if (maximize_button_hover)
                    e.Canvas.FillRectangle (maximize_button_bounds, Theme.RibbonTabHighlightColor);

                e.Canvas.DrawRectangle (maximize_button_bounds.X + 18, maximize_button_bounds.Y + 11, 10, 10, Theme.LightTextColor);
            }

            // Close button
            var close_button_bounds = CloseButtonBounds;

            if (close_button_hover)
                e.Canvas.FillRectangle (close_button_bounds, Theme.FormCloseHighlightColor);

            e.Canvas.DrawLine (close_button_bounds.X + 18, close_button_bounds.Y + 12, close_button_bounds.X + 28, close_button_bounds.Y + 22, Theme.LightTextColor);
            e.Canvas.DrawLine (close_button_bounds.X + 18, close_button_bounds.Y + 22, close_button_bounds.X + 28, close_button_bounds.Y + 12, Theme.LightTextColor);
        }

        private Rectangle CloseButtonBounds => new Rectangle (Width - 46, 0, 46, Height);
        private Rectangle MaximizeButtonBounds => AllowMaximize ? new Rectangle (Width - 92, 0, 46, Height) : Rectangle.Empty;

        private Rectangle MinimizeButtonBounds {
            get {
                if (AllowMinimize && AllowMaximize)
                    return new Rectangle (Width - 138, 0, 46, Height);

                if (AllowMinimize)
                    return new Rectangle (Width - 92, 0, 46, Height);

                return Rectangle.Empty;
            }
        }

        private void SetCloseHover (bool hover)
        {
            if (close_button_hover == hover)
                return;

            close_button_hover = hover;
            Invalidate (CloseButtonBounds);
        }

        private void SetMaximizeHover (bool hover)
        {
            if (!AllowMaximize || maximize_button_hover == hover)
                return;

            maximize_button_hover = hover;
            Invalidate (MaximizeButtonBounds);
        }

        private void SetMinimizeHover (bool hover)
        {
            if (!AllowMinimize || minimize_button_hover == hover)
                return;

            minimize_button_hover = hover;
            Invalidate (MinimizeButtonBounds);
        }
    }
}
