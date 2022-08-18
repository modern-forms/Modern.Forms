using System.Diagnostics;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class MessageBoxPanel : Panel
    {
        public MessageBoxPanel ()
        {
            var button1 = Controls.Add (new Button {
                Text = "Short Message",
                Left = 10,
                Top = 10,
                Width = 130
            });

            button1.Click += (o, e) => new MessageBoxForm ("Short Title", "Short Message").ShowDialog (FindForm ()!);

            var button2 = Controls.Add (new Button {
                Text = "Medium Message",
                Left = 10,
                Top = 50,
                Width = 130
            });

            button2.Click += (o, e) => new MessageBoxForm ("This is a very very very very very very medium title", new StackTrace (6).ToString ()).ShowDialog (FindForm ()!);

            var button3 = Controls.Add (new Button {
                Text = "Long Message",
                Left = 10,
                Top = 90,
                Width = 130
            });

            button3.Click += (o, e) => new MessageBoxForm ("This is a very very very very very very very very very very very very very very long title", new StackTrace ().ToString ()).ShowDialog (FindForm ()!);

            var button4 = Controls.Add (new Button {
                Text = "Two Messages",
                Left = 10,
                Top = 130,
                Width = 130
            });

            button4.Click += async (o, e) => {
                await new MessageBoxForm ("This is a very very very very very very very very very very very very very very long title", new StackTrace ().ToString ()).ShowDialog (FindForm ()!);
                await new MessageBoxForm ("Short Title", "Short Message").ShowDialog (FindForm ()!);
            };
        }
    }
}
