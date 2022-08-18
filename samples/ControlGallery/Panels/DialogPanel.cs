using Modern.Forms;

namespace ControlGallery.Panels
{
    public class DialogPanel : Panel
    {
        public DialogPanel ()
        {
            var label = Controls.Add (new Label {
                Left = 160,
                Top = 10,
                Width = 200
            });

            var button1 = Controls.Add (new Button {
                Text = "Show Dialog",
                Left = 10,
                Top = 10,
                Width = 130
            });

            button1.Click += async (o, e) => {
                label.Text = (await new DialogForm ().ShowDialog (FindForm ()!)).ToString ();
            };

            // This dialog shouldn't show because the DialogResult has already been set
            var button2 = Controls.Add (new Button {
                Text = "Already Set Dialog",
                Left = 10,
                Top = 50,
                Width = 130
            });

            button2.Click += async (o, e) => {
                var dialog = new DialogForm { DialogResult = DialogResult.OK };
                label.Text = (await dialog.ShowDialog (FindForm ()!)).ToString ();
            };

            var button3 = Controls.Add (new Button {
                Text = "Two Dialogs",
                Left = 10,
                Top = 90,
                Width = 130
            });

            button3.Click += async (o, e) => {
                label.Text = (await new DialogForm ().ShowDialog (FindForm ()!)).ToString ();
                label.Text = (await new DialogForm ().ShowDialog (FindForm ()!)).ToString ();
            };
        }
    }
}
