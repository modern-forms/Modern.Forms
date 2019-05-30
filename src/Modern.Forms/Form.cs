using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;

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

        private string text;

        public Form () : base (AvaloniaGlobals.WindowingInterface.CreateWindow ())
        {
            Window.SetSystemDecorations (false);

            SetWindowStartupLocation ();
        }

        protected override System.Drawing.Size DefaultSize => new System.Drawing.Size (1080, 720);

        public void ShowDialog (Form parent) => Window.ShowDialog (parent.Window);

        /// <summary>
        /// Gets or sets the startup location of the window.
        /// </summary>
        public FormStartPosition StartPosition { get; set; } = FormStartPosition.CenterScreen;

        public string Text {
            get => text;
            set {
                if (text != value) {
                    text = value;
                    Window.SetTitle (text);
                }
            }
        }

        public FormWindowState WindowState {
            get => (FormWindowState)Window.WindowState;
            set => Window.WindowState = (WindowState)value;
        }

        private void SetWindowStartupLocation (IWindowBaseImpl owner = null)
        {
            var scaling = owner?.Scaling ?? 1; // PlatformImpl?.Scaling ?? 1;

            // TODO: We really need non-client size here.
            var rect = new PixelRect (
                PixelPoint.Origin,
                PixelSize.FromSize (window.ClientSize, scaling));

            if (StartPosition == FormStartPosition.CenterScreen) {
                var screen = Screens.ScreenFromPoint (owner?.Position ?? Location);

                if (screen != null) {
                    Location = screen.WorkingArea.CenterRect (rect).Position;
                }
            } else if (StartPosition == FormStartPosition.CenterParent) {
                if (owner != null) {
                    // TODO: We really need non-client size here.
                    var ownerRect = new PixelRect (
                        owner.Position,
                        PixelSize.FromSize (owner.ClientSize, scaling));
                    Location = ownerRect.CenterRect (rect).Position;
                }
            }
        }

        private IWindowImpl Window => (IWindowImpl)window;
    }
}
