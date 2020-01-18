using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class MessageBoxForm : Form
    {
        private readonly Button ok_button;
        private readonly TextBox label;

        public MessageBoxForm ()
        {
            StartPosition = FormStartPosition.CenterParent;
            AllowMinimize = false;
            AllowMaximize = false;

            label = Controls.Add (new TextBox {
                Dock = DockStyle.Fill,
                Height = 105,
                MultiLine = true,
                ReadOnly = true,
                Padding = new Padding (10)
            });

            var label_panel = Controls.Add (new Panel {
                Dock = DockStyle.Bottom,
                Height = 45
            });

            label.Style.BackgroundColor = Theme.FormBackgroundColor;
            label.Style.Border.Width = 0;

            ok_button = label_panel.Controls.Add (new Button {
                Text = "OK",
                Left = 150,
                Top = 0
            });

            ok_button.Click += (o, e) => Close ();
        }

        public MessageBoxForm (string title, string message) : this ()
        {
            Text = title;
            label.Text = message;

            CalculateDialogSize ();
        }

        private void CalculateDialogSize ()
        {
            var num_lines = label?.Text?.Count (c => c == '\n') ?? 0;

            if (num_lines > 10)
                Size = new Size (800, 400);
            else if (num_lines > 4)
                Size = new Size (600, 300);
            else
                Size = new Size (400, 200);

            ok_button.Left = (int)((Size.Width - ok_button.Width) / 2);
        }

        protected override Size DefaultSize => new Size (400, 200);

        public string Message {
            get => label.Text;
            set {
                if (label.Text != value) {
                    label.Text = value;
                    CalculateDialogSize ();
                }
            }
        }
    }
}
