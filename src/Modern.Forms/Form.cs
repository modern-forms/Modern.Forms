using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Modern.WindowKit;
using Modern.WindowKit.Controls;
using Modern.WindowKit.Input;
using Modern.WindowKit.Platform;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a top-level window to display to the user.
    /// </summary>
    public class Form : WindowBase, ICloseable
    {
        // If the border is only 1 pixel it's too hard to resize, so we may steal some pixels from the client area
        private const int MINIMUM_RESIZE_PIXELS = 4;

        private IWindowImpl? dialog_parent;
        private DialogResult dialog_result = DialogResult.None;
        private TaskCompletionSource<DialogResult>? dialog_task;
        private System.Drawing.Size minimum_size;
        private System.Drawing.Size maximum_size;

        private bool show_focus_cues;
        private string text = string.Empty;
        private bool use_system_decorations;

        /// <summary>
        /// Initializes a new instance of the Form class.
        /// </summary>
        public Form () : base (AvaloniaGlobals.GetRequiredService<IWindowingPlatform> ().CreateWindow ())
        {
            TitleBar = Controls.AddImplicitControl (new FormTitleBar ());

            Resizeable = true;
            Window.SetSystemDecorations (SystemDecorations.None);
            Window.SetExtendClientAreaToDecorationsHint (true);

            Window.Closing = () => {
                var args = new CancelEventArgs ();

                OnClosing (args);

                return args.Cancel;
            };

            Window.Resize (new Size (DefaultSize.Width, DefaultSize.Height));
        }

        /// <summary>
        /// Gets or sets whether the form can be maximized.
        /// </summary>
        public bool AllowMaximize {
            get => TitleBar.AllowMaximize;
            set => TitleBar.AllowMaximize = value;
        }

        /// <summary>
        /// Gets or sets whether the form can be minimized.
        /// </summary>
        public bool AllowMinimize {
            get => TitleBar.AllowMinimize;
            set => TitleBar.AllowMinimize = value;
        }

        /// <summary>
        /// Begins dragging the window to move it.
        /// </summary>
        public void BeginMoveDrag () => Window.BeginMoveDrag (new PointerPressedEventArgs ());

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
                dialog_parent.SetEnabled (true);
                dialog_parent.Activate ();
                dialog_parent = null;
            }

            // If this was a dialog box we need to resume the execution task
            if (dialog_task is not null) {
                var task = dialog_task;
                dialog_task = null;
                task.SetResult (dialog_result);
            }
        }

        /// <summary>
        /// Raised before the form is closed, allowing close to be programatically canceled.
        /// </summary>
        public event EventHandler<CancelEventArgs>? Closing;

        /// <inheritdoc/>
        protected override System.Drawing.Size DefaultSize => new System.Drawing.Size (1080, 720);

        /// <summary>
        /// Gets the default style for all forms.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = Theme.FormBackgroundColor;
             style.Border.Color = Theme.PrimaryColor;
             style.Border.Width = 1;
         });

        /// <summary>
        ///  Gets or sets the dialog result for the form.
        /// </summary>
        public DialogResult DialogResult {
            get => dialog_result;
            set {
                dialog_result = value;

                // If we're showing a dialog, setting this closes the dialog
                if (dialog_result != DialogResult.None && dialog_parent is not null)
                    Close ();
            }
        }
        /// <summary>
        /// Gets the next control in tab order.
        /// </summary>
        /// <param name="start">The control to start from.</param>
        /// <param name="forward">True to get the next control, false to get the previous control.</param>
        public Control? GetNextControl (Control? start, bool forward = true) => adapter.GetNextControl (start, forward);

        /// <summary>
        /// Gets or sets the icon for the form.
        /// </summary>
        public SKBitmap? Image {
            get => TitleBar.Image;
            set {
                TitleBar.Image = value;
                Window.SetIcon (value);
            }
        }

        /// <summary>
        /// Gets or sets the unscaled location of the control.
        /// </summary>
        public new System.Drawing.Point Location {
            get => window.Position.ToDrawingPoint ();
            set {
                if (window.Position.ToDrawingPoint () != value)
                    Window.Move (value.ToPixelPoint ());
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
                    Window.BeginResizeDrag (WindowEdge.North, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.RightBorder:
                    Window.BeginResizeDrag (WindowEdge.East, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomBorder:
                    Window.BeginResizeDrag (WindowEdge.South, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.LeftBorder:
                    Window.BeginResizeDrag (WindowEdge.West, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopLeftCorner:
                    Window.BeginResizeDrag (WindowEdge.NorthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.TopRightCorner:
                    Window.BeginResizeDrag (WindowEdge.NorthEast, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomLeftCorner:
                    Window.BeginResizeDrag (WindowEdge.SouthWest, new PointerPressedEventArgs ());
                    return true;
                case WindowElement.BottomRightCorner:
                    Window.BeginResizeDrag (WindowEdge.SouthEast, new PointerPressedEventArgs ());
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

                    Window.SetMinMaxSize (minimum_size.ToAvaloniaSize (), maximum_size.ToAvaloniaSize ());

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
                    Window.SetMinMaxSize (minimum_size.ToAvaloniaSize (), maximum_size.ToAvaloniaSize ());

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

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        public virtual void OnClosing (CancelEventArgs e)
        {
            Closing?.Invoke (this, e);
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
        public Task<DialogResult> ShowDialog (Form parent)
        {
            dialog_task = new TaskCompletionSource<DialogResult> ();

            // If the DialogResult has already been set we don't show the dialog
            if (dialog_result != DialogResult.None) {
                dialog_task.SetResult (dialog_result);
                return dialog_task.Task;
            }

            dialog_parent = parent.Window;
            SetWindowStartupLocation (parent.Window);
            parent.Window.SetEnabled (false);
            Window.SetParent (parent.Window);
            window.Show (true, true);

            return dialog_task.Task;
        }

        /// <summary>
        /// Gets a value indicating a focus rectangle should be drawn on the selected control.
        /// </summary>
        public bool ShowFocusCues {
            get => show_focus_cues;
            internal set {
                if (show_focus_cues != value) {
                    show_focus_cues = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the unscaled size of the window.
        /// </summary>
        public new System.Drawing.Size Size {
            get => new System.Drawing.Size ((int)window.ClientSize.Width, (int)window.ClientSize.Height);
            set => Window.Resize (new Modern.WindowKit.Size (value.Width, value.Height));
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets or sets the text for the form title bar.
        /// </summary>
        public string Text {
            get => text;
            set {
                if (text != value) {
                    text = value;
                    Window.SetTitle (text);
                    TitleBar.Text = text;
                }
            }
        }

        /// <summary>
        /// Gets the title bar for the form.
        /// </summary>
        public FormTitleBar TitleBar { get; }

        /// <summary>
        /// Gets or sets whether the form should use the operating system's title bar and decorations,
        /// or use managed decorations.  The default is false.  This must be changed before the form
        /// is shown for the first time.
        /// </summary>
        public bool UseSystemDecorations {
            get => use_system_decorations;
            set {
                if (shown)
                    throw new InvalidOperationException ($"Cannot change {nameof (UseSystemDecorations)} once a Form has been shown.");
                
                if (use_system_decorations != value) {
                    use_system_decorations = value;
                    TitleBar.Visible = !use_system_decorations;
                    Style.Border.Width = use_system_decorations ? 0 : 1;
                    Window.SetSystemDecorations (value ? SystemDecorations.Full : SystemDecorations.None);
                    Window.SetExtendClientAreaToDecorationsHint (!value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the state of the form (normal/minimized/maximized).
        /// </summary>
        public FormWindowState WindowState {
            get => (FormWindowState)Window.WindowState;
            set => Window.WindowState = (WindowState)value;
        }

        private IWindowImpl Window => (IWindowImpl)window;

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
