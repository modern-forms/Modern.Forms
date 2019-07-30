using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Win32.Input;
using SkiaSharp;

namespace Modern.Forms
{
    public abstract class Window
    {
        private const int DOUBLE_CLICK_TIME = 500;
        private const int DOUBLE_CLICK_MOVEMENT = 4;

        public static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = Theme.FormBackgroundColor;
         });

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public virtual ControlStyle CurrentStyle => Style;

        internal IWindowBaseImpl window;
        public ControlAdapter adapter;

        private DateTime last_click_time;
        private Point last_click_point;

        internal Window (IWindowBaseImpl window)
        {
            this.window = window;
            adapter = new ControlAdapter (this);

            window.Input = OnInput;
            window.Paint = OnPaint;
            window.Resized = OnResize;
            window.Closed = () => Closed?.Invoke (this, EventArgs.Empty);
            window.Deactivated = () => Deactivated?.Invoke (this, EventArgs.Empty);
            window.Resize (new Size (DefaultSize.Width, DefaultSize.Height));
        }

        public event EventHandler Closed;
        public event EventHandler Deactivated;

        public void BeginMoveDrag () => window.BeginMoveDrag ();

        public void Close () => window.Dispose ();

        public ControlCollection Controls => adapter.Controls;

        protected virtual System.Drawing.Size DefaultSize => new System.Drawing.Size (100, 100);

        public System.Drawing.Rectangle DisplayRectangle => new System.Drawing.Rectangle (CurrentStyle.Border.Left.GetWidth (), CurrentStyle.Border.Top.GetWidth (), (int)window.ClientSize.Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth (), (int)window.ClientSize.Height - CurrentStyle.Border.Top.GetWidth () - CurrentStyle.Border.Bottom.GetWidth ());

        public System.Drawing.Rectangle ScaledDisplayRectangle => new System.Drawing.Rectangle (CurrentStyle.Border.Left.GetWidth (), CurrentStyle.Border.Top.GetWidth (), (int)window.ScaledClientSize.Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth (), (int)window.ScaledClientSize.Height - CurrentStyle.Border.Top.GetWidth () - CurrentStyle.Border.Bottom.GetWidth ());

        public void Hide ()
        {
            Visible = false;
            window.Hide ();
            OnVisibleChanged (EventArgs.Empty);
        }

        public void Invalidate () => window.Invalidate (new Rect (window.ClientSize));

        public void Invalidate (System.Drawing.Rectangle rect) => Invalidate ();

        public PixelPoint Location {
            get => window.Position;
            set {
                if (window.Position != value) {
                    window.Position = value;
                }
            }
        }

        public System.Drawing.Point PointToScreen (System.Drawing.Point point)
        {
            var pt = window.PointToScreen (new Point (point.X, point.Y));
            return new System.Drawing.Point (pt.X, pt.Y);
        }

        public double Scaling => window.Scaling;

        public System.Drawing.Size Size {
            get => new System.Drawing.Size ((int)window.ClientSize.Width, (int)window.ClientSize.Height);
            set => window.Resize (new Avalonia.Size (value.Width, value.Height));
        }


        public System.Drawing.Size ScaledSize {
            get => new System.Drawing.Size ((int)window.ScaledClientSize.Width, (int)window.ScaledClientSize.Height);
        }

        internal Screens Screens => new Screens (window!.Screen);

        public void Show ()
        {
            Visible = true;
            OnVisibleChanged (EventArgs.Empty);

            window.Show ();
        }

        public bool Visible { get; private set; }

        private MouseEventArgs BuildMouseClickArgs (MouseButtons buttons, Point point, Keys keyData)
        {
            var click_count = 1;

            if (DateTime.Now.Subtract (last_click_time).TotalMilliseconds < DOUBLE_CLICK_TIME && PointInDoubleClickRange (point))
                click_count = 2;

            var e = new MouseEventArgs (buttons, click_count, (int)point.X, (int)point.Y, System.Drawing.Point.Empty, keyData: keyData);

            last_click_time = click_count > 1 ? DateTime.MinValue : DateTime.Now;
            last_click_point = click_count > 1 ? Point.Empty : point;

            return e;
        }

        private bool PointInDoubleClickRange (Point point)
        {
            if (Math.Abs (point.X - last_click_point.X) > DOUBLE_CLICK_MOVEMENT)
                return false;

            return Math.Abs (point.Y - last_click_point.Y) <= DOUBLE_CLICK_MOVEMENT;
        }

        private void OnInput (RawInputEventArgs e)
        {
            if (e is RawMouseEventArgs me) {
                switch (me.Type) {
                    case RawMouseEventType.LeftButtonDown:
                        var lbd_e = new MouseEventArgs (MouseButtons.Left, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (lbd_e);
                        break;
                    case RawMouseEventType.LeftButtonUp:
                        var lbu_e = BuildMouseClickArgs (MouseButtons.Left, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (lbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (lbu_e);

                        adapter.RaiseClick (lbu_e);
                        adapter.RaiseMouseUp (lbu_e);
                        break;
                    case RawMouseEventType.MiddleButtonDown:
                        var mbd_e = new MouseEventArgs (MouseButtons.Middle, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (mbd_e);
                        break;
                    case RawMouseEventType.MiddleButtonUp:
                        var mbu_e = BuildMouseClickArgs (MouseButtons.Middle, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (mbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (mbu_e);

                        adapter.RaiseClick (mbu_e);
                        adapter.RaiseMouseUp (mbu_e);
                        break;
                    case RawMouseEventType.RightButtonDown:
                        var rbd_e = new MouseEventArgs (MouseButtons.Right, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (rbd_e);
                        break;
                    case RawMouseEventType.RightButtonUp:
                        var rbu_e = BuildMouseClickArgs (MouseButtons.Right, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (rbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (rbu_e);

                        adapter.RaiseClick (rbu_e);
                        adapter.RaiseMouseUp (rbu_e);
                        break;
                    case RawMouseEventType.LeaveWindow:
                        var lw_e = new MouseEventArgs (MouseButtons.None, 0, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseLeave (lw_e);
                        break;
                    case RawMouseEventType.Move:
                        var mea = new MouseEventArgs (MouseButtons.None, 0, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseMove (mea);
                        break;
                    case RawMouseEventType.Wheel:
                        if (me is RawMouseWheelEventArgs raw) {
                            var we = new MouseEventArgs (MouseButtons.None, 0, (int)me.Position.X, (int)me.Position.Y, new System.Drawing.Point ((int)raw.Delta.X, (int)raw.Delta.Y), keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                            adapter.RaiseMouseWheel (we);
                        }
                        break;
                }
            } else if (e is RawKeyEventArgs ke) {
                switch (ke.Type) {
                    case RawKeyEventType.KeyDown:
                        var kd_e = new KeyEventArgs ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key));
                        adapter.RaiseKeyDown (kd_e);
                        break;
                    case RawKeyEventType.KeyUp:
                        var ku_e = new KeyEventArgs ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key));
                        adapter.RaiseKeyUp (ku_e);
                        break;
                }
            } else if (e is RawTextInputEventArgs te) {
                var kp_e = new KeyPressEventArgs (te.Text[0], KeyEventArgs.FromInputModifiers (te.Modifiers));
                adapter.RaiseKeyPress (kp_e);
            }
        }

        private void OnPaint (Rect r)
        {
            var skia_framebuffer = window.Surfaces.OfType<IFramebufferPlatformSurface> ().First ();

            using var framebuffer = skia_framebuffer.Lock ();

            var framebufferImageInfo = new SKImageInfo (framebuffer.Size.Width, framebuffer.Size.Height,
                framebuffer.Format.ToSkColorType (), framebuffer.Format == PixelFormat.Rgb565 ? SKAlphaType.Opaque : SKAlphaType.Premul);

            var scaled_client_size = window.ScaledClientSize;
            var scaled_display_rect = ScaledDisplayRectangle;

            using var surface = SKSurface.Create (framebufferImageInfo, framebuffer.Address, framebuffer.RowBytes);

            var e = new PaintEventArgs (framebufferImageInfo, surface.Canvas, Scaling);
            e.Canvas.DrawBackground (CurrentStyle);
            e.Canvas.DrawBorder (new System.Drawing.Rectangle (0, 0, (int)scaled_client_size.Width, (int)scaled_client_size.Height), CurrentStyle);

            e.Canvas.ClipRect (new SKRect (scaled_display_rect.Left, scaled_display_rect.Top, scaled_display_rect.Width + 1, scaled_display_rect.Height + 1));

            adapter.RaisePaintBackground (e);
            adapter.RaisePaint (e);
        }

        private void OnResize (Size size)
        {
            adapter.SetBounds (DisplayRectangle.Left, DisplayRectangle.Top, ScaledSize.Width, ScaledSize.Height);
        }

        protected virtual void OnVisibleChanged (EventArgs e)
        {
            adapter.RaiseParentVisibleChanged (EventArgs.Empty);
        }
    }
}
