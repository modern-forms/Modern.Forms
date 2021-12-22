using System;
using System.ComponentModel;
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
    public class Form : Window, ICloseable
    {
        private bool show_focus_cues;
        private string text = string.Empty;
        private bool use_system_decorations;

        /// <summary>
        /// Initializes a new instance of the Form class.
        /// </summary>
        public Form () : base (AvaloniaGlobals.WindowingInterface.CreateWindow ())
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
        /// Raises the Closing event.
        /// </summary>
        public virtual void OnClosing (CancelEventArgs e)
        {
            Closing?.Invoke (this, e);
        }

        /// <summary>
        /// Displays the window to the user modally, preventing interaction with other windows until closed.
        /// </summary>
        public void ShowDialog (Form parent) => ShowDialog (parent.Window);

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
    }
}
