using System;
using System.Collections.Generic;
using System.Drawing;
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
            Text = "Demo";
            StartPosition = FormStartPosition.CenterParent;

            titlebar = new ModernFormTitleBar {
                Text = "Demo",
                AllowMinimize = false
            };

            Controls.Add (titlebar);

            var button = new Button {
                Text = "OK",
                Location = new System.Drawing.Point (150, 150)
            };

            button.Click += (o, e) => Close ();

            Controls.Add (button);
        }

        protected override Size DefaultSize => new Size (400, 200);

        public MessageBoxForm (string title, string text) : this ()
        {
            Text = title;
            titlebar.Text = title;
            this.text = text;
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            e.Canvas.DrawCenteredText (text, ModernTheme.UIFont, 16, 200, 75, ModernTheme.DarkTextColor);
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
