using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Platform;
using SkiaSharp;

namespace Modern.Forms
{
    public class ControlAdapter : ScrollableControl
    {
        private Control selected_control;

        public ControlAdapter (Window parent)
        {
            ParentForm = parent;
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        public Window ParentForm { get; }

        protected override void OnPaint (PaintEventArgs e)
        {
            // We have this special version for the Adapter because it is
            // given the Form's native surface including any managed Form
            // borders, and it needs to not draw on top of those borders.
            // That is, this often needs to start drawing at (1, 1) instead of (0, 0)
            // This could probably eliminated in the future with Canvas.Translate.
            var form_border = ParentForm.CurrentStyle.Border;

            var form_x = form_border.Left.GetWidth ();
            var form_y = form_border.Top.GetWidth ();

            foreach (var control in Controls.GetAllControls ().Where (c => c.Visible)) {
                if (control.Width <= 0 || control.Height <= 0)
                    continue;

                var info = new SKImageInfo (control.Width, control.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                using (var canvas = new SKCanvas (buffer)) {
                    // start drawing
                    var args = new PaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, form_x + control.Left, form_y + control.Top);
            }
        }

        internal Control SelectedControl {
            get => selected_control;
            set {
                if (selected_control == value)
                    return;

                selected_control?.Deselect ();

                if (value is ControlAdapter)
                    return;

                // Note they could be setting this to null
                selected_control = value;
                selected_control?.Select ();
            }
        }
    }
}
