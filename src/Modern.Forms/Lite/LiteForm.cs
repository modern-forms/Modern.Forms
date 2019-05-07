using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteForm : ModernForm
    {
        private LiteControl current_mouse_in;

        public LiteControlCollection LiteControls { get; }

        public LiteForm ()
        {
            LiteControls = new LiteControlCollection (this);
        }

        public void DoLayout ()
        {
            new System.Windows.Forms.Layout.ModernDefaultLayout ().Layout (this, null);
        }

        public override Rectangle DisplayRectangle => new Rectangle (1, 1, Width - 2, Height - 2);

        public MouseEventArgs MouseEventsForControl (MouseEventArgs e, LiteControl control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.Left, e.Location.Y - control.Top, e.Delta);
        }

        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;

                cp.ClassStyle |= (int)XplatUIWin32.ClassStyle.CS_DBLCLKS;

                return cp;
            }
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            foreach (var control in LiteControls) {
                var info = new SKImageInfo (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                using (var canvas = new SKCanvas (buffer)) {
                    // start drawing
                    var args = new SKPaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, control.Left, control.Top);
            }
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            var child = LiteControls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            child?.RaiseClick (MouseEventsForControl (e, child));
        }

        protected override void OnMouseDoubleClick (MouseEventArgs e)
        {
            base.OnMouseDoubleClick (e);

            var child = LiteControls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            child?.RaiseDoubleClick (MouseEventsForControl (e, child));
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            var child = LiteControls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            child?.RaiseMouseDown (MouseEventsForControl (e, child));
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (current_mouse_in != null)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = null;
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            var child = LiteControls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (current_mouse_in != null && current_mouse_in != child)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = child;

            child?.RaiseMouseMove (MouseEventsForControl (e, child));
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            var child = LiteControls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            child?.RaiseMouseUp (MouseEventsForControl (e, child));
        }
    }
}
