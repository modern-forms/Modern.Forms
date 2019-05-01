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
    public class ModernControl : Control
    {
        private readonly bool designMode;

        private Bitmap bitmap;
        private ControlBehaviors behaviors;

        public ModernControl ()
        {
            DoubleBuffered = true;
            SetStyle (ControlStyles.ResizeRedraw, true);

            designMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        public static ControlStyle DefaultStyle = new ControlStyle (ModernControl.DefaultStyle,
            (style) => {
                style.ForegroundColor = Theme.DarkTextColor;
                style.BackgroundColor = Theme.NeutralGray;
                style.Font = Theme.UIFont;
                style.FontSize = Theme.FontSize;
                style.Border.Radius = 0;
                style.Border.Color = Theme.BorderGray;
                style.Border.Width = 0;
            });

        public static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle);

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        public virtual ControlStyle CurrentStyle => IsHovering ? StyleHover : Style;

        protected void SetControlBehavior (ControlBehaviors behavior, bool value)
        {
            if (value)
                behaviors |= behavior;
            else
                behaviors &= behavior;
        }
        //[Bindable (false)]
        //[Browsable (false)]
        //[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable (EditorBrowsableState.Never)]
        //public SKSize CanvasSize => bitmap == null ? SKSize.Empty : new SKSize (bitmap.Width, bitmap.Height);

        // TODO: Wire up Paint/PaintBackground events to Skia
        //[Category ("Appearance")]
        //public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

        public bool IsHovering { get; private set; }

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
                // start drawing
                var args = new SKPaintEventArgs (surface, info);

                OnPaintBackground (args);
                OnPaint (args);

                surface.Canvas.Flush ();
            }

            // write the bitmap to the graphics
            bitmap.UnlockBits (data);
            e.Graphics.DrawImage (bitmap, 0, 0);
        }

        protected virtual void OnPaintBackground (SKPaintEventArgs e)
        {
            e.Canvas.DrawBackground (Bounds, CurrentStyle);
            e.Canvas.DrawBorder (Bounds, CurrentStyle);
        }

        protected virtual void OnPaint (SKPaintEventArgs e)
        {
        }

        protected override void OnMouseEnter (EventArgs e)
        {
            base.OnMouseEnter (e);

            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = true;
                Invalidate ();
            }
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = false;
                Invalidate ();
            }
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
