using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class MessageBoxForm : ModernForm
    {
        private ModernFormTitleBar titlebar;
        private string text;
        private Label label;

        public MessageBoxForm ()
        {
            Text = "Demo";
            StartPosition = FormStartPosition.CenterParent;

            titlebar = new ModernFormTitleBar {
                Text = "Demo",
                AllowMinimize = false
            };

            Controls.Add (titlebar);

            label = new Label {
                Width = 398,
                Location = new Point (1, 50)
            };

            label.Style.BackgroundColor = SKColors.White;
            label.Style.FontSize = 16;

            Controls.Add (label);

            var button = new Button {
                Text = "OK",
                Location = new Point (150, 150)
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

            label.Text = text;
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
