using System;
using System.ComponentModel;
using System.Linq;
using Modern.WindowKit;
using Modern.WindowKit.Controls;
using Modern.WindowKit.Controls.Platform.Surfaces;
using Modern.WindowKit.Input.Raw;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Skia;
using Modern.WindowKit.Win32.Input;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class for windows, like Form and PopupWindow
    /// </summary>
    public abstract class WindowBase : Component
    {
        private const int DOUBLE_CLICK_TIME = 500;
        private const int DOUBLE_CLICK_MOVEMENT = 4;

        internal IWindowBaseImpl window;
        internal ControlAdapter adapter;

        private DateTime last_click_time;
        private Point last_click_point;
        private Cursor? current_cursor;
        internal bool shown;

        /// <summary>
        /// Initializes a new instance of the Window class.
        /// </summary>
        internal WindowBase (IWindowBaseImpl window)
        {
            this.window = window;
            adapter = new ControlAdapter (this);

            window.Input = OnInput;
            window.Paint = DoPaint;
            window.Resized = OnResize;
            window.Closed = () => Closed?.Invoke (this, EventArgs.Empty);
            window.Deactivated = () => {
                // If we're clicking off the form, deactivate any active menus
                Application.ActiveMenu?.Deactivate ();
                Deactivated?.Invoke (this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Gets the bounds of the Window.
        /// </summary>
        public System.Drawing.Rectangle Bounds {
            get => new System.Drawing.Rectangle (Location, Size);
        }

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

        /// <summary>
        /// Closes and destroys the window.
        /// </summary>
        public virtual void Close () 
        {
            // If we just Dispose the window, WM_CLOSE will never get called so OnClosing will not get called
            if (this is Form f) {
                var args = new CancelEventArgs ();

                f.OnClosing (args);

                if (args.Cancel)
                    return;

                Application.OpenForms.Remove (f);
            }
            
            window.Dispose (); 
        }

        /// <summary>
        /// Raised when the window is closed.
        /// </summary>
        public event EventHandler? Closed;

        /// <summary>
        /// Gets the collection of controls contained by the window.
        /// </summary>
        public Control.ControlCollection Controls => adapter.Controls;

        /// <summary>
        /// Gets the current style of this window instance.
        /// </summary>
        public virtual ControlStyle CurrentStyle => Style;

        /// <summary>
        /// Raised when the window is deactivated.
        /// </summary>
        public event EventHandler? Deactivated;

        /// <summary>
        /// Gets the default size of the window.
        /// </summary>
        protected virtual System.Drawing.Size DefaultSize => new System.Drawing.Size (100, 100);

        /// <summary>
        /// Gets the default style for all windows of this type.
        /// </summary>
        public static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = Theme.FormBackgroundColor;
         });

        /// <summary>
        /// Gets the unscaled bounds of the form not including borders.
        /// </summary>
        public System.Drawing.Rectangle DisplayRectangle => new System.Drawing.Rectangle (CurrentStyle.Border.Left.GetWidth (), CurrentStyle.Border.Top.GetWidth (), (int)window.ClientSize.Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth (), (int)window.ClientSize.Height - CurrentStyle.Border.Top.GetWidth () - CurrentStyle.Border.Bottom.GetWidth ());

        internal virtual bool HandleMouseDown (int x, int y)
        {
            return false;
        }

        internal virtual bool HandleMouseMove (int x, int y)
        {
            window.SetCursor (current_cursor?.cursor?.PlatformCursor ?? Cursors.Arrow.cursor.PlatformCursor);
            return false;
        }

        /// <summary>
        /// Hides the window without destroying it.
        /// </summary>
        public void Hide ()
        {
            Visible = false;
            window.Hide ();
            OnVisibleChanged (EventArgs.Empty);
        }

        /// <summary>
        /// Marks the entire window as needing to be redrawn.
        /// </summary>
        public void Invalidate () => window.Invalidate (new Rect (window.ClientSize));

        /// <summary>
        /// Marks the specified portion of the window as needing to be redrawn.
        /// </summary>
        /// <param name="rectangle">The portion of the window to be redrawn.</param>
        public void Invalidate (System.Drawing.Rectangle rectangle) => Invalidate ();

        /// <summary>
        /// Gets the unscaled location of the control.
        /// </summary>
        public System.Drawing.Point Location {
            get => window.Position.ToDrawingPoint ();
        }

        /// <summary>
        /// Raised when the MaximumSize property is changed.
        /// </summary>
        public event EventHandler? MaximumSizeChanged;

        /// <summary>
        /// Raised when the MinimumSize property is changed.
        /// </summary>
        public event EventHandler? MinimumSizeChanged;

        private void OnInput (RawInputEventArgs e)
        {
            if (e is RawPointerEventArgs me) {
                // TODO: How do we want to handle this for real
                me.Position *= window.RenderScaling;

                switch (me.Type) {
                    case RawPointerEventType.LeftButtonDown:
                        if (Resizeable && HandleMouseDown ((int)me.Position.X, (int)me.Position.Y))
                            return;

                        var lbd_e = new MouseEventArgs (MouseButtons.Left, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (lbd_e);
                        break;
                    case RawPointerEventType.LeftButtonUp:
                        var lbu_e = BuildMouseClickArgs (MouseButtons.Left, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (lbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (lbu_e);

                        adapter.RaiseClick (lbu_e);
                        adapter.RaiseMouseUp (lbu_e);
                        break;
                    case RawPointerEventType.MiddleButtonDown:
                        var mbd_e = new MouseEventArgs (MouseButtons.Middle, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (mbd_e);
                        break;
                    case RawPointerEventType.MiddleButtonUp:
                        var mbu_e = BuildMouseClickArgs (MouseButtons.Middle, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (mbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (mbu_e);

                        adapter.RaiseClick (mbu_e);
                        adapter.RaiseMouseUp (mbu_e);
                        break;
                    case RawPointerEventType.RightButtonDown:
                        var rbd_e = new MouseEventArgs (MouseButtons.Right, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (rbd_e);
                        break;
                    case RawPointerEventType.RightButtonUp:
                        var rbu_e = BuildMouseClickArgs (MouseButtons.Right, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (rbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (rbu_e);

                        adapter.RaiseClick (rbu_e);
                        adapter.RaiseMouseUp (rbu_e);
                        break;
                    case RawPointerEventType.LeaveWindow:
                        var lw_e = new MouseEventArgs (me.InputModifiers.ToMouseButtons (), 0, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseLeave (lw_e);
                        break;
                    case RawPointerEventType.Move:
                        if (Resizeable && HandleMouseMove ((int)me.Position.X, (int)me.Position.Y))
                            return;

                        var mea = new MouseEventArgs (me.InputModifiers.ToMouseButtons (), 0, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseMove (mea);
                        break;
                    case RawPointerEventType.Wheel:
                        if (me is RawMouseWheelEventArgs raw) {
                            var we = new MouseEventArgs (me.InputModifiers.ToMouseButtons (), 0, (int)me.Position.X, (int)me.Position.Y, new System.Drawing.Point ((int)raw.Delta.X, (int)raw.Delta.Y), keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                            adapter.RaiseMouseWheel (we);
                        }
                        break;
                }
            } else if (e is RawKeyEventArgs ke) {
                switch (ke.Type) {
                    case RawKeyEventType.KeyDown:
                        var kd_e = new KeyEventArgs (WindowKitExtensions.AddModifiers ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key), ke.Modifiers));
                        adapter.RaiseKeyDown (kd_e);
                        break;
                    case RawKeyEventType.KeyUp:
                        var ku_e = new KeyEventArgs (WindowKitExtensions.AddModifiers ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key), ke.Modifiers));
                        adapter.RaiseKeyUp (ku_e);
                        break;
                }
            } else if (e is RawTextInputEventArgs te) {
                var kp_e = new KeyPressEventArgs (te.Text, KeyEventArgs.FromInputModifiers (te.Modifiers));
                adapter.RaiseKeyPress (kp_e);
                e.Handled = true;
            }
        }

        private void DoPaint (Rect r)
        {
            // Mac tries to give us paints before the Form constructor completes
            if (!shown)
                return;

            var skia_framebuffer = window.Surfaces.OfType<IFramebufferPlatformSurface> ().First ();

            using var framebuffer = skia_framebuffer.Lock ();

            var framebufferImageInfo = new SKImageInfo (framebuffer.Size.Width, framebuffer.Size.Height,
                framebuffer.Format.ToSkColorType (), framebuffer.Format == PixelFormat.Rgb565 ? SKAlphaType.Opaque : SKAlphaType.Premul);

            var scaled_client_size = ScaledClientSize;
            var scaled_display_rect = ScaledDisplayRectangle;

            using var surface = SKSurface.Create (framebufferImageInfo, framebuffer.Address, framebuffer.RowBytes);

            var e = new PaintEventArgs (framebufferImageInfo, surface.Canvas, Scaling);
            OnPaintBackground (e);
            e.Canvas.DrawBorder (new System.Drawing.Rectangle (0, 0, (int)scaled_client_size.Width, (int)scaled_client_size.Height), CurrentStyle);
            OnPaint (e);

            e.Canvas.ClipRect (new SKRect (scaled_display_rect.Left, scaled_display_rect.Top, scaled_display_rect.Width + 1, scaled_display_rect.Height + 1));

            adapter.RaisePaintBackground (e);
            adapter.RaisePaint (e);
        }

        /// <summary>
        /// Raises the MaximumSizeChanged event.
        /// </summary>
        protected virtual void OnMaximumSizeChanged (EventArgs e)
        {
            MaximumSizeChanged?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the MinimumSizeChanged event.
        /// </summary>
        protected virtual void OnMinimumSizeChanged (EventArgs e)
        {
            MinimumSizeChanged?.Invoke (this, e);
        }

        /// <summary>
        /// Paints the Form.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected virtual void OnPaint (PaintEventArgs e)
        {
        }

        /// <summary>
        /// Paints the Form's background.
        /// </summary>
        protected virtual void OnPaintBackground (PaintEventArgs e)
        {
            e.Canvas.DrawBackground (Bounds, CurrentStyle);
        }

        private void OnResize (Size size, PlatformResizeReason reason)
        {
            adapter.SetBounds (DisplayRectangle.Left, DisplayRectangle.Top, Size.Width, Size.Height);
        }

        /// <summary>
        /// Raises the Shown event.
        /// </summary>
        protected virtual void OnShown (EventArgs e) => Shown?.Invoke (this, e);

        private void OnVisibleChanged (EventArgs e)
        {
            adapter.RaiseParentVisibleChanged (e);
        }

        private bool PointInDoubleClickRange (Point point)
        {
            if (Math.Abs (point.X - last_click_point.X) > DOUBLE_CLICK_MOVEMENT)
                return false;

            return Math.Abs (point.Y - last_click_point.Y) <= DOUBLE_CLICK_MOVEMENT;
        }

        /// <summary>
        /// Converts a point from screen coordinates to window coordinates.
        /// </summary>
        public System.Drawing.Point PointToClient (System.Drawing.Point point)
        {
            var pt = window.PointToClient (new PixelPoint (point.X, point.Y));
            return new System.Drawing.Point ((int)pt.X, (int)pt.Y);
        }

        /// <summary>
        /// Converts a point from window coordinates to screen coordinates.
        /// </summary>
        public System.Drawing.Point PointToScreen (System.Drawing.Point point)
        {
            var pt = window.PointToScreen (new Point (point.X, point.Y));
            return new System.Drawing.Point (pt.X, pt.Y);
        }

        /// <summary>
        /// Gets or sets whether the window is resizable.
        /// </summary>
        public bool Resizeable { get; set; }

        private System.Drawing.Size ScaledClientSize => new System.Drawing.Size ((int)(window.ClientSize.Width * window.RenderScaling), (int)(window.ClientSize.Height * window.RenderScaling));

        /// <summary>
        /// Gets the scaled bounds of the form not including borders.
        /// </summary>
        public System.Drawing.Rectangle ScaledDisplayRectangle => new System.Drawing.Rectangle (CurrentStyle.Border.Left.GetWidth (), CurrentStyle.Border.Top.GetWidth (), (int)ScaledClientSize.Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth (), (int)ScaledClientSize.Height - CurrentStyle.Border.Top.GetWidth () - CurrentStyle.Border.Bottom.GetWidth ());

        /// <summary>
        /// Gets or sets the scaled size of the window.
        /// </summary>
        public System.Drawing.Size ScaledSize => new System.Drawing.Size ((int)ScaledClientSize.Width, (int)ScaledClientSize.Height);

        /// <summary>
        /// Gets the current scale factor of the window.
        /// </summary>
        public double Scaling => window.RenderScaling;

        /// <summary>
        /// Gets the current scale factor of the desktop.
        /// </summary>
        public double DesktopScaling => window.DesktopScaling;

        internal Screens Screens => new Screens (window!.Screen);

        internal void SetCursor (Cursor cursor)
        {
            current_cursor = cursor;
        }

        internal virtual void SetWindowStartupLocation (IWindowBaseImpl? owner = null)
        {
        }

        /// <summary>
        /// Displays the window to the user.
        /// </summary>
        public void Show ()
        {
            Visible = true;
            OnVisibleChanged (EventArgs.Empty);

            SetWindowStartupLocation ();
            window.Show (true, false);

            if (this is Form f)
                Application.OpenForms.Add (f);

            if (!shown) {
                shown = true;
                OnShown (EventArgs.Empty);
            }
        }

        internal void ShowDialog (IWindowImpl parent)
        {
            Visible = true;
            OnVisibleChanged (EventArgs.Empty);

            SetWindowStartupLocation (parent);
            parent.SetEnabled (false);
            window.Show (true, true);

            if (this is Form f)
                Application.OpenForms.Add (f);

            if (!shown) {
                shown = true;
                OnShown (EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the window is shown.
        /// </summary>
        public event EventHandler? Shown;

        /// <summary>
        /// Gets or sets the unscaled size of the window.
        /// </summary>
        public System.Drawing.Size Size {
            get => new System.Drawing.Size ((int)window.ClientSize.Width, (int)window.ClientSize.Height);
        }

        /// <summary>
        /// Gets or sets the startup location of the window.
        /// </summary>
        public FormStartPosition StartPosition { get; set; } = FormStartPosition.CenterScreen;

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the window.
        /// </summary>
        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets or sets whether the window is displayed to the user.
        /// </summary>
        public bool Visible { get; private set; }
    }
}
