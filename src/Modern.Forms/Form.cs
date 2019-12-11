using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using SkiaSharp;

namespace Modern.Forms
{
    public class Form : Window, ICloseable
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = Theme.FormBackgroundColor;
             style.Border.Color = Theme.RibbonColor;
             style.Border.Width = 1;
         });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private string text = string.Empty;
        private bool use_system_decorations;

        public Form () : base (AvaloniaGlobals.WindowingInterface.CreateWindow ())
        {
            TitleBar = Controls.AddImplicitControl (new FormTitleBar ());

            Resizeable = true;
            Window.SetSystemDecorations (false);

            SetWindowStartupLocation ();
        }

        public bool AllowMaximize {
            get => TitleBar.AllowMaximize;
            set => TitleBar.AllowMaximize = value;
        }

        public bool AllowMinimize {
            get => TitleBar.AllowMinimize;
            set => TitleBar.AllowMinimize = value;
        }

        protected override System.Drawing.Size DefaultSize => new System.Drawing.Size (1080, 720);

        public SKBitmap? Image {
            get => TitleBar.Image;
            set {
                TitleBar.Image = value;
                Window.SetIcon (value);
            }
        }

        public void ShowDialog (Form parent) => Window.ShowDialog (parent.Window);

        /// <summary>
        /// Gets or sets the startup location of the window.
        /// </summary>
        public FormStartPosition StartPosition { get; set; } = FormStartPosition.CenterScreen;

        public FormTitleBar TitleBar { get; }

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

        public bool UseSystemDecorations {
            get => use_system_decorations;
            set {
                if (shown)
                    throw new InvalidOperationException ($"Cannot change {nameof (UseSystemDecorations)} once a Form has been shown.");
                
                if (use_system_decorations != value) {
                    use_system_decorations = value;
                    TitleBar.Visible = !use_system_decorations;
                    Style.Border.Width = use_system_decorations ? 0 : 1;
                    Window.SetSystemDecorations (value);
                }
            }
        }

        public FormWindowState WindowState {
            get => (FormWindowState)Window.WindowState;
            set => Window.WindowState = (WindowState)value;
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
                    Location = screen.WorkingArea.CenterRect (rect).Position.ToDrawingPoint ();
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

        private IWindowImpl Window => (IWindowImpl)window;
    }
}
