using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;

namespace Designer
{
    public class ModernControlHost : Control
    {
        private readonly Modern.Forms.Control control;

        public ModernControlHost (Modern.Forms.Control control)
        {
            this.control = control;

            HandleResize ();

            SizeChanged += (o, e) => HandleResize ();
            control.Invalidated += (o, e) => Invalidate ();
            DoubleBuffered = true;
            control.Visible = true;
            
        }

        protected override void OnMouseEnter (EventArgs e)
        {
            base.OnMouseEnter (e);

            control.RaiseMouseEnter (Modern.Forms.MouseEventArgs.Empty);
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            control.RaiseMouseLeave (EventArgs.Empty);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            var mea = new Modern.Forms.MouseEventArgs (Modern.Forms.MouseButtons.Left, 0, e.X, e.Y, new System.Drawing.Point (0, e.Delta));

            control.RaiseMouseMove (mea);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);
            var mea = new Modern.Forms.MouseEventArgs (Modern.Forms.MouseButtons.Left, 0, e.X, e.Y, new System.Drawing.Point (0, e.Delta));

            control.RaiseClick (mea);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            using var bitmap = new SkiaSharp.SKBitmap (Width, Height);
            using var canvas = new SkiaSharp.SKCanvas (bitmap);

            var pea = new Modern.Forms.PaintEventArgs (SkiaSharp.SKImageInfo.Empty, canvas, 1);

            control.DoPaint (pea);

            using var image = bitmap.ToBitmap ();

            e.Graphics.DrawImage (image, System.Drawing.Point.Empty);
        }

        private void HandleResize ()
        {
            control.Size = Size;
        }
    }
}
