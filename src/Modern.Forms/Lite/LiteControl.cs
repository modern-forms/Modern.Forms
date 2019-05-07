using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteControl : ILayoutable
    {
        public static ControlStyle DefaultStyle = new ControlStyle (null,
            (style) => {
                style.ForegroundColor = ModernTheme.DarkTextColor;
                style.BackgroundColor = ModernTheme.NeutralGray;
                style.Font = ModernTheme.UIFont;
                style.FontSize = ModernTheme.FontSize;
                style.Border.Radius = 0;
                style.Border.Color = ModernTheme.BorderGray;
                style.Border.Width = 0;
            });

        private SKBitmap back_buffer;

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public virtual ControlStyle CurrentStyle => Style;

        public LiteControl ()
        {
            LiteControls = new LiteControlCollection (this);

            Width = DefaultSize.Width;
            Height = DefaultSize.Height;
        }

        public Rectangle Bounds => new Rectangle (Left, Top, Width, Height);

        public Rectangle ClientBounds {
            get {
                var x = Style.Border.Left.GetWidth ();
                var y = Style.Border.Top.GetWidth ();
                var w = Width - Style.Border.Right.GetWidth () - x;
                var h = Height - Style.Border.Bottom.GetWidth () - y;
                return new Rectangle (x, y, w, h);
            }
        }

        public DockStyle Dock { get; set; }

        public bool Enabled { get; set; } = true;

        public int Height { get; set; }

        public void Invalidate ()
        {
            ParentForm?.Invalidate (Bounds);
        }

        public void Invalidate (Rectangle rectangle)
        {
            Invalidate ();
        }

        public LiteControlCollection LiteControls { get; }

        public int Left { get; set; }

        public LiteForm ParentForm { get; set; }

        public Size Size => new Size (Width, Height);

        public int Top { get; set; }

        public string Text { get; set; }

        public int Width { get; set; }

        protected virtual Size DefaultSize => Size.Empty;

        public virtual Padding Margin => new Padding (3);

        public bool Visible { get; set; } = true;

        internal void RaiseClick (MouseEventArgs e) => OnClick (e);

        protected virtual void OnClick (MouseEventArgs e)
        {
        }

        internal void RaiseDoubleClick (MouseEventArgs e) => OnDoubleClick (e);

        protected virtual void OnDoubleClick (MouseEventArgs e)
        {
        }

        internal void RaiseMouseDown (MouseEventArgs e) => OnMouseDown (e);

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
        }

        internal void RaiseMouseLeave (EventArgs e) => OnMouseLeave (e);

        protected virtual void OnMouseLeave (EventArgs e)
        {
        }

        internal void RaiseMouseMove (MouseEventArgs e) => OnMouseMove (e);

        protected virtual void OnMouseMove (MouseEventArgs e)
        {
        }

        internal void RaiseMouseUp (MouseEventArgs e) => OnMouseUp (e);

        protected virtual void OnMouseUp (MouseEventArgs e)
        {
        }

        internal void RaisePaint (SKPaintEventArgs e) => OnPaint (e);

        protected virtual void OnPaint (SKPaintEventArgs e)
        {
        }

        internal void RaisePaintBackground (SKPaintEventArgs e) => OnPaintBackground (e);

        protected virtual void OnPaintBackground (SKPaintEventArgs e)
        {
            e.Canvas.DrawBackground (Bounds, CurrentStyle);
            e.Canvas.DrawBorder (Bounds, CurrentStyle);
        }

        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer == null || back_buffer.Width != Width || back_buffer.Height != Height) {
                FreeBitmap ();
                back_buffer = new SKBitmap (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            }

            return back_buffer;
        }

        private void FreeBitmap ()
        {
            if (back_buffer != null) {
                back_buffer.Dispose ();
                back_buffer = null;
            }
        }

        public Size GetPreferredSize (Size proposedSize) => new Size (Width, Height);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }
    }
}
