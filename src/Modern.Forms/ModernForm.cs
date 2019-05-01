using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    [DefaultEvent ("PaintSurface")]
    [DefaultProperty ("Name")]
    public class ModernForm : Form
    {
        private readonly bool designMode;

        private Bitmap bitmap;

        public ModernForm ()
        {
            DoubleBuffered = true;
            SetStyle (ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding (1);

            designMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        protected override Size DefaultSize => new Size (1080, 720);

        [Bindable (false)]
        [Browsable (false)]
        [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable (EditorBrowsableState.Never)]
        public SKSize CanvasSize => bitmap == null ? SKSize.Empty : new SKSize (bitmap.Width, bitmap.Height);

        [Category ("Appearance")]
        public event EventHandler<SKPaintEventArgs> PaintSurface;

        protected override void OnPaint (PaintEventArgs e)
        {
            if (designMode)
                return;

            base.OnPaint (e);

            // get the bitmap
            CreateBitmap ();
            var data = bitmap.LockBits (new Rectangle (0, 0, Width, Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // create the surface
            var info = new SKImageInfo (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using (var surface = SKSurface.Create (info, data.Scan0, data.Stride)) {

                surface.Canvas.DrawRectangle (0, 0, Width - 1, Height - 1, Theme.RibbonColor);

                // start drawing
                OnPaintSurface (new SKPaintEventArgs (surface, info));

                surface.Canvas.Flush ();
            }

            // write the bitmap to the graphics
            bitmap.UnlockBits (data);
            e.Graphics.DrawImage (bitmap, 0, 0);
        }

        protected virtual void OnPaintSurface (SKPaintEventArgs e)
        {
            // invoke the event
            PaintSurface?.Invoke (this, e);
        }

        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);

            FreeBitmap ();
        }

        private void CreateBitmap ()
        {
            if (bitmap == null || bitmap.Width != Width || bitmap.Height != Height) {
                FreeBitmap ();

                bitmap = new Bitmap (Width, Height, PixelFormat.Format32bppPArgb);
            }
        }

        private void FreeBitmap ()
        {
            if (bitmap != null) {
                bitmap.Dispose ();
                bitmap = null;
            }
        }
    }
}
