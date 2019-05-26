using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class ControlAdapter : ScrollableControl
    {
        private Control selected_control;

        public ControlAdapter (Form parent)
        {
            ParentForm = parent;
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        public Form ParentForm { get; }

        protected override void OnPaint (PaintEventArgs e)
        {
            // We have this special version for now because it needs
            // to take the Form border into account
            foreach (var control in Controls.Where (c => c.Visible)) {
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

                e.Canvas.DrawBitmap (buffer, control.Left + 1, control.Top + 1);
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
