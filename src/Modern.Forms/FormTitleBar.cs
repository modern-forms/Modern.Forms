using System;
using System.Drawing;
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

        private const int BUTTON_SIZE = 46;
        private const int BUTTON_PADDING = 10;
        private const int FORM_ICON_SIZE = 16;

        public SKBitmap Image {
            get => form_icon;
            set {
                if (form_icon != value) {
                    form_icon = value;
                    Invalidate ();
                }
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
            if (form_icon != null) {
                var icon_glyph_bounds = DrawingExtensions.CenterSquare (IconBounds, LogicalToDeviceUnits (FORM_ICON_SIZE));

                e.Canvas.DrawBitmap (form_icon, icon_glyph_bounds);
            }

            // Form text
            if (!string.IsNullOrWhiteSpace (Text))
                e.Canvas.DrawText (Text.Trim (), Theme.UIFont, LogicalToDeviceUnits (Theme.FontSize), TitleBounds, Theme.LightTextColor, ContentAlignment.MiddleCenter);

            // Minimize button
            if (AllowMinimize) {
                var minimize_button_bounds = MinimizeButtonBounds;

                if (minimize_button_hover)
                    e.Canvas.FillRectangle (minimize_button_bounds, Theme.RibbonTabHighlightColor);

                var min_glyph_bounds = DrawingExtensions.CenterRectangle (minimize_button_bounds, LogicalToDeviceUnits (new Size (BUTTON_PADDING, 1)));
                e.Canvas.DrawLine (min_glyph_bounds.X, min_glyph_bounds.Y, min_glyph_bounds.Right, min_glyph_bounds.Y, Theme.LightTextColor);
            }

            // Maximize button
            if (AllowMaximize) {
                var maximize_button_bounds = MaximizeButtonBounds;

                if (maximize_button_hover)
                    e.Canvas.FillRectangle (maximize_button_bounds, Theme.RibbonTabHighlightColor);

                var max_glyph_bounds = DrawingExtensions.CenterSquare (maximize_button_bounds, LogicalToDeviceUnits (BUTTON_PADDING));
                e.Canvas.DrawRectangle (max_glyph_bounds, Theme.LightTextColor);
            }

            // Close button
            var close_button_bounds = CloseButtonBounds;

            if (close_button_hover)
                e.Canvas.FillRectangle (close_button_bounds, Theme.FormCloseHighlightColor);

            var close_glyph_bounds = DrawingExtensions.CenterSquare (close_button_bounds, LogicalToDeviceUnits (BUTTON_PADDING));
            e.Canvas.DrawLine (close_glyph_bounds.X, close_glyph_bounds.Y, close_glyph_bounds.Right, close_glyph_bounds.Bottom, Theme.LightTextColor);
            e.Canvas.DrawLine (close_glyph_bounds.X, close_glyph_bounds.Bottom, close_glyph_bounds.Right, close_glyph_bounds.Y, Theme.LightTextColor);
        }

        private int ScaledButtonWidth => LogicalToDeviceUnits (BUTTON_SIZE);

        private Rectangle IconBounds => new Rectangle (0, 0, ScaledHeight, ScaledHeight);
        private Rectangle CloseButtonBounds => new Rectangle (ScaledWidth - ScaledButtonWidth, 0, ScaledButtonWidth, ScaledHeight);
        private Rectangle MaximizeButtonBounds => AllowMaximize ? new Rectangle (ScaledWidth - (ScaledButtonWidth * 2), 0, ScaledButtonWidth, ScaledHeight) : Rectangle.Empty;

        private Rectangle MinimizeButtonBounds {
            get {
                if (AllowMinimize && AllowMaximize)
                    return new Rectangle (ScaledWidth - (ScaledButtonWidth * 3), 0, ScaledButtonWidth, ScaledHeight);

                if (AllowMinimize)
                    return new Rectangle (ScaledWidth - (ScaledButtonWidth * 2), 0, ScaledButtonWidth, ScaledHeight);

                return Rectangle.Empty;
            }
        }

        private Rectangle TitleBounds {
            get {
                var x = form_icon == null ? 0 : IconBounds.Right;
                var right = AllowMinimize ? MinimizeButtonBounds.Left : AllowMaximize ? MaximizeButtonBounds.Left : CloseButtonBounds.Left;

                return Rectangle.FromLTRB (x, Top, right, ScaledBounds.Bottom);
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
