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
        public LiteControlAdapter (ModernForm parent)
        {
            ParentForm = parent;
        }

        public ModernForm ParentForm { get; }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            // We have this special version for now because it needs
            // to take the Form border into account
            foreach (var control in Controls.Where (c => c.Visible)) {
                var info = new SKImageInfo (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
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

        //protected override void OnClick (MouseEventArgs e)
        //{
        //    base.OnClick (e);

        //    var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

        //    child?.RaiseClick (MouseEventsForControl (e, child));
        //}

        //protected override void OnDoubleClick (MouseEventArgs e)
        //{
        //    base.OnDoubleClick (e);

        //    var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

        //    child?.RaiseDoubleClick (MouseEventsForControl (e, child));
        //}

        //protected override void OnMouseDown (MouseEventArgs e)
        //{
        //    base.OnMouseDown (e);

        //    var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

        //    child?.RaiseMouseDown (MouseEventsForControl (e, child));
        //}

        //protected override void OnMouseLeave (EventArgs e)
        //{
        //    base.OnMouseLeave (e);

        //    if (current_mouse_in != null)
        //        current_mouse_in.RaiseMouseLeave (e);

        //    current_mouse_in = null;
        //}

        //protected override void OnMouseMove (MouseEventArgs e)
        //{
        //    base.OnMouseMove (e);

        //    var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

        //    if (current_mouse_in != null && current_mouse_in != child)
        //        current_mouse_in.RaiseMouseLeave (e);

        //    current_mouse_in = child;

        //    child?.RaiseMouseMove (MouseEventsForControl (e, child));
        //}

        //protected override void OnMouseUp (MouseEventArgs e)
        //{
        //    base.OnMouseUp (e);

        //    var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

        //    child?.RaiseMouseUp (MouseEventsForControl (e, child));
        //}
    }
}
