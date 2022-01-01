using System;
using Modern.WindowKit;
using Modern.WindowKit.Controls;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class for windows, like Form
    /// </summary>
    public abstract class Window : WindowBase
    {
        // If the border is only 1 pixel it's too hard to resize, so we may steal some pixels from the client area
        private const int MINIMUM_RESIZE_PIXELS = 4;

        private IWindowImpl? dialog_parent;
        private System.Drawing.Size minimum_size;
        private System.Drawing.Size maximum_size;

        private IWindowImpl WindowImpl => (IWindowImpl)window;

        internal Window (IWindowImpl window) : base (window)
        {
            WindowImpl.Resize (new Size (DefaultSize.Width, DefaultSize.Height));
        }

        /// <summary>
        /// Begins dragging the window to move it.
        /// </summary>
        public void BeginMoveDrag () => WindowImpl.BeginMoveDrag (new PointerPressedEventArgs ());

        /// <summary>
        /// Gets or sets the bounds of the Window.
        /// </summary>
        public new System.Drawing.Rectangle Bounds {
            get => new System.Drawing.Rectangle (Location, Size);
            set {
                Location = value.Location;
                Size = value.Size;
            }
        }

        /// <inheritdoc/>
        public override void Close ()
        {
            base.Close ();

            // If this was a dialog box we need to reactivate the parent
            if (dialog_parent is not null) {
                dialog_parent.Activate ();
                dialog_parent = null;
            }
        }

        /// <summary>
        /// Gets or sets the unscaled location of the control.
        /// </summary>
        public new System.Drawing.Point Location {
            get => window.Position.ToDrawingPoint ();
            set {
                if (window.Position.ToDrawingPoint () != value) {
                    WindowImpl.Move (value.ToPixelPoint ());
                }
            }
        }

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

        internal override bool HandleMouseDown (int x, int y)
        {
            var element = GetElementAtLocation (x, y);

            switch (element) {
                case WindowElement.TopBorder:
                    WindowImpl.BeginResizeDrag (WindowEdge.North, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.RightBorder:
                    WindowImpl.BeginResizeDrag (WindowEdge.East, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomBorder:
                    WindowImpl.BeginResizeDrag (WindowEdge.South, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.LeftBorder:
                    WindowImpl.BeginResizeDrag (WindowEdge.West, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopLeftCorner:
                    WindowImpl.BeginResizeDrag (WindowEdge.NorthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopRightCorner:
                    WindowImpl.BeginResizeDrag (WindowEdge.NorthEast, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomLeftCorner:
                    WindowImpl.BeginResizeDrag (WindowEdge.SouthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomRightCorner:
                    WindowImpl.BeginResizeDrag (WindowEdge.SouthEast, new PointerPressedEventArgs ());
                    return true;
            }

            return false;
        }

        internal override bool HandleMouseMove (int x, int y)
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

            return base.HandleMouseMove (x, y);
        }

        /// <summary>
        /// Gets or sets the maximum size of the Window
        /// </summary>
        public System.Drawing.Size MaximumSize {
            get => maximum_size;
            set {
                if (maximum_size != value) {
                    maximum_size = value;

                    // Don't let MinimumSize be larger than MaximumSize
                    if (!minimum_size.IsEmpty && !maximum_size.IsEmpty)
                        minimum_size = new System.Drawing.Size (Math.Min (minimum_size.Width, maximum_size.Width), Math.Min (minimum_size.Height, maximum_size.Height));

                    WindowImpl.SetMinMaxSize (minimum_size.ToAvaloniaSize (), maximum_size.ToAvaloniaSize ());

                    // Keep form size within new limits
                    var size = Size;
                    if (!value.IsEmpty && (size.Width > value.Width || size.Height > value.Height))
                        Size = new System.Drawing.Size (Math.Min (size.Width, value.Width), Math.Min (size.Height, value.Height));

                    OnMaximumSizeChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum size of the Window
        /// </summary>
        public System.Drawing.Size MinimumSize {
            get => minimum_size;
            set {
                if (minimum_size != value) {
                    minimum_size = value;
                    WindowImpl.SetMinMaxSize (minimum_size.ToAvaloniaSize (), maximum_size.ToAvaloniaSize ());

                    // Don't let MaximumSize be smaller than MinimumSize
                    if (!minimum_size.IsEmpty && !maximum_size.IsEmpty)
                        maximum_size = new System.Drawing.Size (Math.Max (minimum_size.Width, maximum_size.Width), Math.Max (minimum_size.Height, maximum_size.Height));

                    // Keep form size within new limits
                    var size = Size;
                    if (size.Width < value.Width || size.Height < value.Height)
                        Size = new System.Drawing.Size (Math.Max (size.Width, value.Width), Math.Max (size.Height, value.Height));

                    OnMinimumSizeChanged (EventArgs.Empty);
                }
            }
        }

        internal override void SetWindowStartupLocation (IWindowBaseImpl? owner = null)
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
        /// Displays the window to the user modally, preventing interaction with other windows until closed.
        /// </summary>
        internal void ShowDialog (IWindowImpl parent)
        {
            dialog_parent = parent;
            SetWindowStartupLocation (parent);
            //win.ShowDialog (parent);
        }

        /// <summary>
        /// Gets or sets the unscaled size of the window.
        /// </summary>
        public new System.Drawing.Size Size {
            get => new System.Drawing.Size ((int)window.ClientSize.Width, (int)window.ClientSize.Height);
            set => WindowImpl.Resize (new Modern.WindowKit.Size (value.Width, value.Height));
        }

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
