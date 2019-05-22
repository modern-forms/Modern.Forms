using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteControlAdapter : LiteControl
    {
        private LiteControl selected_control;

        public LiteControlAdapter (IForm parent)
        {
            ParentForm = parent;
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        public IForm ParentForm { get; }

        protected override void OnPaint (SKPaintEventArgs e)
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
                    var args = new SKPaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, control.Left + 1, control.Top + 1);
            }
        }

        internal LiteControl SelectedControl {
            get => selected_control;
            set {
                if (selected_control == value)
                    return;

                selected_control?.Deselect ();

                if (value is LiteControlAdapter)
                    return;

                // Note they could be setting this to null
                selected_control = value;
                selected_control?.Select ();
            }
        }
    }
}
