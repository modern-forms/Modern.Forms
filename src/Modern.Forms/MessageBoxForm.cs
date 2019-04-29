using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class MessageBoxForm : ModernForm
    {
        private ModernFormTitleBar titlebar;
        private string text;

        public MessageBoxForm ()
        {
            Size = new System.Drawing.Size (400, 200);
            Text = "Demo";
            StartPosition = FormStartPosition.CenterParent;

            titlebar = new ModernFormTitleBar {
                Text = "Demo",
                AllowMinimize = false
            };

            Controls.Add (titlebar);

            var button = new ButtonControl {
                Text = "OK",
                Location = new System.Drawing.Point (150, 150)
            };

            button.Click += (o, e) => Close ();

            Controls.Add (button);
        }

        public MessageBoxForm (string title, string text) : this ()
        {
            Text = title;
            titlebar.Text = title;
            this.text = text;
        }

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            e.Surface.Canvas.FillRectangle (1, 1, Width - 2, Height - 2, Theme.FormBackColor);

            e.Surface.Canvas.DrawCenteredText (text, Theme.UIFont, 16, 200, 75, Theme.DarkText);
        }

        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;

                // Can't decide if I like the drop shadow or not
                //cp.ClassStyle |= 0x00020000;

                return cp;
            }
        }
    }
}
