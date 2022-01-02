using System.Drawing;
using Modern.Forms;

namespace ControlGallery
{
    public class DialogForm : Form
    {
        public DialogForm ()
        {
            Text = "Dialog Form";
            Size = new Size (500, 250);

            var button1 = Controls.Add (new Button {
                Text = "Set Form's DialogResult to Retry",
                Left = 10,
                Top = 44,
                Width = 250
            });

            button1.Click += (o, e) => DialogResult = DialogResult.Retry;

            var button2 = Controls.Add (new Button {
                Text = "Set Form's DialogResult to Ignore",
                Left = 10,
                Top = 84,
                Width = 250
            });

            button2.Click += (o, e) => DialogResult = DialogResult.Ignore;

            var button3 = Controls.Add (new Button {
                Text = "DialogResult.Abort button",
                Left = 10,
                Top = 124,
                Width = 250,
                DialogResult = DialogResult.Abort
            });

            var button4 = Controls.Add (new Button {
                Text = "DialogResult.None button",
                Left = 10,
                Top = 164,
                Width = 250,
                DialogResult = DialogResult.None
            });
        }
    }
}
