using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class ModernFormTitleBar : ModernControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (ModernControl.DefaultStyle,
           (style) => style.BackgroundColor = Theme.RibbonColor);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private Rectangle close_button_bounds;
        private bool close_button_hover;
        private Rectangle minimize_button_bounds;
        private bool minimize_button_hover;

        private Point drag_start_location;
        private Point drag_start_mouse_location;
        private bool is_dragging;

        public SKBitmap Image { get; set; }
        public bool AllowMinimize { get; set; } = true;

        protected override Size DefaultSize => new Size (600, 34);

        public ModernFormTitleBar ()
        {
            Dock = DockStyle.Top;
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            if (close_button_bounds.Contains (e.Location))
                FindForm ().Close ();
            if (AllowMinimize && minimize_button_bounds.Contains (e.Location))
                FindForm ().WindowState = FormWindowState.Minimized;
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!close_button_bounds.Contains (e.Location) && !(AllowMinimize && minimize_button_bounds.Contains (e.Location))) {
                // Start drag-moving the window
                is_dragging = true;
                drag_start_location = FindForm ().Location;
                drag_start_mouse_location = PointToScreen (e.Location);
            }
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetCloseHover (close_button_bounds.Contains (e.Location));
            SetMinimizeHover (minimize_button_bounds.Contains (e.Location));

            if (is_dragging) {
                var screen = PointToScreen (e.Location);
                var delta_x = screen.X - drag_start_mouse_location.X;
                var delta_y = screen.Y - drag_start_mouse_location.Y;

                FindForm ().Location = new Point (drag_start_location.X + delta_x, drag_start_location.Y + delta_y);
            }
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            is_dragging = false;
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetCloseHover (false);
            SetMinimizeHover (false);
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            close_button_bounds = new Rectangle (Width - 46, 0, 46, Height);
            minimize_button_bounds = new Rectangle (Width - 92, 0, 46, Height);

            if (!string.IsNullOrWhiteSpace (Text))
                e.Canvas.DrawCenteredText (Text.Trim (), Theme.UIFont, 14, Left + Width / 2, Top + 21, Theme.LightTextColor);

            if (Image != null)
                e.Canvas.DrawBitmap (Image, Bounds.Left + 7, Bounds.Top + 7);

            if (close_button_hover)
                e.Canvas.FillRectangle (close_button_bounds, Theme.FormCloseHighlightColor);

            if (AllowMinimize && minimize_button_hover)
                e.Canvas.FillRectangle (minimize_button_bounds, Theme.RibbonTabHighlightColor);

            // Draw the close X
            e.Canvas.DrawLine (close_button_bounds.X + 18, close_button_bounds.Y + 12, close_button_bounds.X + 28, close_button_bounds.Y + 22, Theme.LightTextColor);
            e.Canvas.DrawLine (close_button_bounds.X + 18, close_button_bounds.Y + 22, close_button_bounds.X + 28, close_button_bounds.Y + 12, Theme.LightTextColor);

            // Draw the minimize -
            if (AllowMinimize)
                e.Canvas.DrawLine (minimize_button_bounds.X + 18, minimize_button_bounds.Y + 17, minimize_button_bounds.X + 28, minimize_button_bounds.Y + 17, Theme.LightTextColor);
        }

        private void SetCloseHover (bool hover)
        {
            if (close_button_hover == hover)
                return;

            close_button_hover = hover;
            Invalidate (close_button_bounds);
        }

        private void SetMinimizeHover (bool hover)
        {
            if (!AllowMinimize || minimize_button_hover == hover)
                return;

            minimize_button_hover = hover;
            Invalidate (minimize_button_bounds);
        }
    }
}
