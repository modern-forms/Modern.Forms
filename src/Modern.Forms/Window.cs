﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Win32.Input;
using Avalonia.Win32.Interop;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class for windows, like Form and PopupWindow
    /// </summary>
    public abstract class Window : Component
    {
        private const int DOUBLE_CLICK_TIME = 500;
        private const int DOUBLE_CLICK_MOVEMENT = 4;

        // If the border is only 1 pixel it's too hard to resize, so we may steal some pixels from the client area
        private const int MINIMUM_RESIZE_PIXELS = 4;

        internal IWindowBaseImpl window;
        internal ControlAdapter adapter;

        private DateTime last_click_time;
        private Point last_click_point;
        private Cursor? current_cursor;
        private IWindowImpl? dialog_parent;
        internal bool shown;

        /// <summary>
        /// Initializes a new instance of the Window class.
        /// </summary>
        internal Window (IWindowBaseImpl window)
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

            window.Resize (new Size (DefaultSize.Width, DefaultSize.Height));
        }

        /// <summary>
        /// Begins dragging the window to move it.
        /// </summary>
        public void BeginMoveDrag () => window.BeginMoveDrag (new Avalonia.Input.PointerPressedEventArgs ());

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
        public void Close () 
        {
            // If we just Dispose the window, WM_CLOSE will never get called so OnClosing will not get called
            if (this is Form f) {
                var args = new CancelEventArgs ();

                f.OnClosing (args);

                if (args.Cancel)
                    return;
            }

            // If this was a dialog box we need to reactivate the parent
            if (!(dialog_parent is null)) {
                dialog_parent.Activate ();
                dialog_parent = null;
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
        public ControlCollection Controls => adapter.Controls;

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

        private WindowElement GetElementAtLocation (int x, int y)
        {
            var left = false;
            var right = false;

            if (x < Math.Max (Style.Border.Left.GetWidth (), MINIMUM_RESIZE_PIXELS))
                left = true;
            else if (x >= ScaledSize.Width - Math.Max (Style.Border.Right.GetWidth (), MINIMUM_RESIZE_PIXELS))
                right = true;

            if (y < Math.Max (Style.Border.Top.GetWidth (), MINIMUM_RESIZE_PIXELS))
                return left ? WindowElement.TopLeftCorner : right ? WindowElement.TopRightCorner : WindowElement.TopBorder;
            else if (y >= ScaledSize.Height - Math.Max (Style.Border.Bottom.GetWidth (), MINIMUM_RESIZE_PIXELS))
                return left ? WindowElement.BottomLeftCorner : right ? WindowElement.BottomRightCorner : WindowElement.BottomBorder;

            return left ? WindowElement.LeftBorder : right ? WindowElement.RightBorder : WindowElement.Client;
        }

        private bool HandleMouseDown (int x, int y)
        {
            var element = GetElementAtLocation (x, y);

            switch (element) {
                case WindowElement.TopBorder:
                    window.BeginResizeDrag (WindowEdge.North, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.RightBorder:
                    window.BeginResizeDrag (WindowEdge.East, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomBorder:
                    window.BeginResizeDrag (WindowEdge.South, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.LeftBorder:
                    window.BeginResizeDrag (WindowEdge.West, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopLeftCorner:
                    window.BeginResizeDrag (WindowEdge.NorthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopRightCorner:
                    window.BeginResizeDrag (WindowEdge.NorthEast, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomLeftCorner:
                    window.BeginResizeDrag (WindowEdge.SouthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomRightCorner:
                    window.BeginResizeDrag (WindowEdge.SouthEast, new PointerPressedEventArgs ());
                    return true;
            }

            return false;
        }

        private bool HandleMouseMove (int x, int y)
        {
            var element = GetElementAtLocation (x, y);

            switch (element) {
                case WindowElement.TopBorder:
                    window.SetCursor (Cursors.TopSide.cursor.PlatformCursor);
                    return true;
                case WindowElement.RightBorder:
                    window.SetCursor (Cursors.RightSide.cursor.PlatformCursor);
                    return true;
                case WindowElement.BottomBorder:
                    window.SetCursor (Cursors.BottomSide.cursor.PlatformCursor);
                    return true;
                case WindowElement.LeftBorder:
                    window.SetCursor (Cursors.LeftSide.cursor.PlatformCursor);
                    return true;
                case WindowElement.TopLeftCorner:
                    window.SetCursor (Cursors.TopLeftCorner.cursor.PlatformCursor);
                    return true;
                case WindowElement.TopRightCorner:
                    window.SetCursor (Cursors.TopRightCorner.cursor.PlatformCursor);
                    return true;
                case WindowElement.BottomLeftCorner:
                    window.SetCursor (Cursors.BottomLeftCorner.cursor.PlatformCursor);
                    return true;
                case WindowElement.BottomRightCorner:
                    window.SetCursor (Cursors.BottomRightCorner.cursor.PlatformCursor);
                    return true;
            }

            window.SetCursor (current_cursor?.cursor?.PlatformCursor);
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
        /// Gets or sets the unscaled location of the control.
        /// </summary>
        public System.Drawing.Point Location {
            get => window.Position.ToDrawingPoint ();
            set {
                if (window.Position.ToDrawingPoint () != value) {
                    window.Position = value.ToPixelPoint ();
                }
            }
        }
        private bool client_captured;

        private void OnInput (RawInputEventArgs e)
        {
            if (e is RawPointerEventArgs me) {
                switch (me.Type) {
                    case RawPointerEventType.LeftButtonDown:
                        if (Resizeable && HandleMouseDown ((int)me.Position.X, (int)me.Position.Y))
                            return;
                        UnmanagedMethods.SetCapture (window.Handle.Handle);
                        client_captured = true;

                        var lbd_e = new MouseEventArgs (MouseButtons.Left, 1, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseDown (lbd_e);
                        break;
                    case RawPointerEventType.LeftButtonUp:
                        var lbu_e = BuildMouseClickArgs (MouseButtons.Left, me.Position, KeyEventArgs.FromInputModifiers (me.InputModifiers));

                        if (lbu_e.Clicks > 1)
                            adapter.RaiseDoubleClick (lbu_e);

                        adapter.RaiseClick (lbu_e);
                        adapter.RaiseMouseUp (lbu_e);

                        UnmanagedMethods.ReleaseCapture ();
                        client_captured = false;
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
                        //Debug.WriteLine ("mouse leave");
                        var lw_e = new MouseEventArgs (me.InputModifiers.ToMouseButtons (), 0, (int)me.Position.X, (int)me.Position.Y, System.Drawing.Point.Empty, keyData: KeyEventArgs.FromInputModifiers (me.InputModifiers));
                        adapter.RaiseMouseLeave (lw_e);
                        break;
                    case RawPointerEventType.Move:
                        //Debug.WriteLine ($"Window mouse move: {me.Position}");

                        if (me.Position.X > 1500)
                            Console.WriteLine ();
                        if (!client_captured && Resizeable && HandleMouseMove ((int)me.Position.X, (int)me.Position.Y))
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
                        var kd_e = new KeyEventArgs (KeyInterop.AddModifiers ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key), ke.Modifiers));
                        adapter.RaiseKeyDown (kd_e);
                        break;
                    case RawKeyEventType.KeyUp:
                        var ku_e = new KeyEventArgs (KeyInterop.AddModifiers ((Keys)KeyInterop.VirtualKeyFromKey (ke.Key), ke.Modifiers));
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
            var skia_framebuffer = window.Surfaces.OfType<IFramebufferPlatformSurface> ().First ();

            using var framebuffer = skia_framebuffer.Lock ();

            var framebufferImageInfo = new SKImageInfo (framebuffer.Size.Width, framebuffer.Size.Height,
                framebuffer.Format.ToSkColorType (), framebuffer.Format == PixelFormat.Rgb565 ? SKAlphaType.Opaque : SKAlphaType.Premul);

            var scaled_client_size = window.ScaledClientSize;
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
            e.Canvas.DrawBackground (CurrentStyle);
        }

        private void OnResize (Size size)
        {
            adapter.SetBounds (DisplayRectangle.Left, DisplayRectangle.Top, ScaledSize.Width, ScaledSize.Height);
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
        /// Converts a point from window coordinates to monitor coordinates.
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

        /// <summary>
        /// Gets the scaled bounds of the form not including borders.
        /// </summary>
        public System.Drawing.Rectangle ScaledDisplayRectangle => new System.Drawing.Rectangle (CurrentStyle.Border.Left.GetWidth (), CurrentStyle.Border.Top.GetWidth (), (int)window.ScaledClientSize.Width - CurrentStyle.Border.Right.GetWidth () - CurrentStyle.Border.Left.GetWidth (), (int)window.ScaledClientSize.Height - CurrentStyle.Border.Top.GetWidth () - CurrentStyle.Border.Bottom.GetWidth ());

        /// <summary>
        /// Gets or sets the scaled size of the window.
        /// </summary>
        public System.Drawing.Size ScaledSize => new System.Drawing.Size ((int)window.ScaledClientSize.Width, (int)window.ScaledClientSize.Height);

        /// <summary>
        /// Gets the current scale factor of the window.
        /// </summary>
        public double Scaling => window.Scaling;

        internal Screens Screens => new Screens (window!.Screen);

        internal void SetCursor (Cursor cursor)
        {
            current_cursor = cursor;
        }

        private void SetWindowStartupLocation (IWindowBaseImpl? owner = null)
        {
            var scaling = Scaling;

            // TODO: We really need non-client size here.
            var rect = new PixelRect (
                PixelPoint.Origin,
                PixelSize.FromSize (window.ClientSize, scaling));

            if (StartPosition == FormStartPosition.CenterScreen) {
                var screen = Screens.ScreenFromPoint (owner?.Position ?? Location.ToPixelPoint ());

                if (screen != null) {
                    var position = screen.WorkingArea.CenterRect (rect).Position.ToDrawingPoint ();

                    // Ensure we don't position the titlebar offscreen
                    position.X = Math.Max (position.X, screen.WorkingArea.X);
                    position.Y = Math.Max (position.Y, screen.WorkingArea.Y);

                    Location = position;
                }
            } else if (StartPosition == FormStartPosition.CenterParent) {
                if (owner != null) {
                    // TODO: We really need non-client size here.
                    var ownerRect = new PixelRect (
                        owner.Position,
                        PixelSize.FromSize (owner.ClientSize, scaling));
                    Location = ownerRect.CenterRect (rect).Position.ToDrawingPoint ();
                }
            }
        }

        /// <summary>
        /// Displays the window to the user.
        /// </summary>
        public void Show ()
        {
            Visible = true;
            OnVisibleChanged (EventArgs.Empty);

            SetWindowStartupLocation ();
            window.Show ();

            if (!shown) {
                shown = true;
                OnShown (EventArgs.Empty);
            }
        }

        /// <summary>
        /// Displays the window to the user modally, preventing interaction with other windows until closed.
        /// </summary>
        internal void ShowDialog (IWindowImpl parent)
        {
            if (window is IWindowImpl win) {
                dialog_parent = parent;
                SetWindowStartupLocation (parent);
                win.ShowDialog (parent);
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
            set => window.Resize (new Avalonia.Size (value.Width, value.Height));
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

        public System.Drawing.Size MinimumFormSize { get; set; } = new System.Drawing.Size (50, 30);

        private enum WindowElement
        {
            Client,
            TopBorder,
            RightBorder,
            BottomBorder,
            LeftBorder,
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner
        }
    }
}
