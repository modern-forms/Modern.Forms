using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteControl : ILayoutable, IDisposable
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

        public static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle);

        private SKBitmap back_buffer;
        private ControlBehaviors behaviors;
        private LiteControl current_mouse_in;

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        public bool IsHovering { get; private set; }

        public virtual ControlStyle CurrentStyle => IsHovering ? StyleHover : Style;

        public LiteControl ()
        {
            Controls = new LiteControlCollection (this);

            Width = DefaultSize.Width;
            Height = DefaultSize.Height;
        }

        public virtual AnchorStyles Anchor { get; set; } = AnchorStyles.Left | AnchorStyles.Top;

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

        public void DoLayout ()
        {
            new System.Windows.Forms.Layout.ModernDefaultLayout ().Layout (this, null);

            foreach (var child in Controls)
                child.DoLayout ();
        }

        public bool Enabled { get; set; } = true;

        public int Height { get; set; }

        public void Invalidate ()
        {
            FindForm ()?.Invalidate ();
        }

        public void Invalidate (Rectangle rectangle)
        {
            Invalidate ();
        }

        public LiteControlCollection Controls { get; }

        public Cursor Cursor { get; set; }

        public ModernForm FindForm ()
        {
            if (this is LiteControlAdapter adapter)
                return adapter.ParentForm;

            return Parent?.FindForm ();
        }

        public int Left { get; set; }

        public LiteControl Parent { get; set; }

        public Size Size => new Size (Width, Height);

        public bool TabStop { get; set; } = true;

        public int Top { get; set; }

        public string Text { get; set; }

        public int Width { get; set; }

        protected virtual Size DefaultSize => Size.Empty;

        public virtual Padding Margin => new Padding (3);

        public MouseEventArgs MouseEventsForControl (MouseEventArgs e, LiteControl control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.Left, e.Location.Y - control.Top, e.Delta);
        }

        public bool Visible { get; set; } = true;

        internal void RaiseClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseClick (MouseEventsForControl (e, child));
            else
                OnClick (e);
        }

        protected virtual void OnClick (MouseEventArgs e)
        {
        }

        internal void RaiseDoubleClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseDoubleClick (MouseEventsForControl (e, child));
            else
                OnDoubleClick (e);
        }

        protected virtual void OnDoubleClick (MouseEventArgs e)
        {
        }

        internal void RaiseMouseDown (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (MouseEventsForControl (e, child));
            else
                OnMouseDown (e);
        }

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
        }

        internal void RaiseMouseLeave (EventArgs e)
        {
            if (current_mouse_in != null)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = null;

            OnMouseLeave (e);
        }

        protected virtual void OnMouseLeave (EventArgs e)
        {
        }

        internal void RaiseMouseMove (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (current_mouse_in != null && current_mouse_in != child)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = child;

            if (child != null)
                child?.RaiseMouseMove (MouseEventsForControl (e, child));
            else
                OnMouseMove (e);
        }

        protected virtual void OnMouseMove (MouseEventArgs e)
        {
        }

        internal void RaiseMouseUp (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseUp (MouseEventsForControl (e, child));
            else
                OnMouseUp (e);
        }

        protected virtual void OnMouseUp (MouseEventArgs e)
        {
        }

        internal void RaisePaint (SKPaintEventArgs e) => OnPaint (e);

        protected virtual void OnPaint (SKPaintEventArgs e)
        {
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

                e.Canvas.DrawBitmap (buffer, control.Left, control.Top);
            }
        }

        protected void SetControlBehavior (ControlBehaviors behavior, bool value = true)
        {
            if (value)
                behaviors |= behavior;
            else
                behaviors &= behavior;
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

        public virtual Size GetPreferredSize (Size proposedSize) => new Size (Width, Height);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                FreeBitmap ();

                foreach (var c in Controls)
                    c.Dispose (disposing);

                disposedValue = true;
            }
        }

        ~LiteControl ()
        {
            Dispose (false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose ()
        {
            Dispose (true);
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
