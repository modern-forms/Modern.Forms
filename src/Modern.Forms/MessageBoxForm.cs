using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class MessageBoxForm : Form
    {
        private readonly TextBox label;

        public MessageBoxForm ()
        {
            StartPosition = FormStartPosition.CenterParent;
            AllowMinimize = false;
            AllowMaximize = false;
            Resizeable = false;
            
            label = new TextBox {
                Dock = DockStyle.Top,
                Height = 105,
                MultiLine = true,
                ReadOnly = true,
                Padding = new Padding (10)
            };

            label.Style.BackgroundColor = Theme.FormBackgroundColor;
            label.Style.FontSize = 16;
            label.Style.Border.Width = 0;

            Controls.Add (label);

            var button = new Button {
                Text = "OK",
                Left = 150,
                Top = 150
            };

            button.Click += (o, e) => Close ();

            Controls.Add (button);
        }

        protected override Size DefaultSize => new Size (400, 200);

        public MessageBoxForm (string title, string text) : this ()
        {
            Text = title;

            label.Text = text;
        }
    }
}
