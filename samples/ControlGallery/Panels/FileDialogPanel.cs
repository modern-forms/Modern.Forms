using System;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class FileDialogPanel : Panel
    {
        private readonly ListBox list_box;

        public FileDialogPanel ()
        {
            var button1 = Controls.Add (new Button { Left = 10, Top = 10, Width = 150, Text = "Open File Dialog" });
            button1.Click += Button1_Click;

            var button2 = Controls.Add (new Button { Left = 10, Top = 45, Width = 150, Text = "Save File Dialog" });
            button2.Click += Button2_Click;

            var button3 = Controls.Add (new Button { Left = 10, Top = 80, Width = 150, Text = "Folder Browser Dialog" });
            button3.Click += Button3_Click;

            list_box = Controls.Add (new ListBox { Left = 200, Top = 10, Width = 550 });
        }

        private async void Button1_Click (object? sender, MouseEventArgs e)
        {
            list_box.Items.Clear ();

            var ofd = new OpenFileDialog {
                AllowMultiple = true,
                InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
                Title = "Open some files up"
            };

            ofd.AddFilter ("All Files", "*");
            ofd.AddFilter ("Image Files", "png", "gif", "jpg", "jpeg");
            ofd.AddFilter ("Text Files", "txt", "log");

            if ((await ofd.ShowDialog (FindForm ()!)) == DialogResult.OK)
                foreach (var file in ofd.FileNames)
                    list_box.Items.Add (file);
        }

        private async void Button2_Click (object? sender, MouseEventArgs e)
        {
            list_box.Items.Clear ();

            var sfd = new SaveFileDialog {
                DefaultExtension = "txt",
                InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
                Title = "Save this file"
            };

            sfd.AddFilter ("All Files", "*");
            sfd.AddFilter ("Image Files", "png", "gif", "jpg", "jpeg");
            sfd.AddFilter ("Text Files", "txt", "log");

            if ((await sfd.ShowDialog (FindForm ()!)) == DialogResult.OK)
                foreach (var file in sfd.FileNames)
                    list_box.Items.Add (file);
        }

        private async void Button3_Click (object? sender, MouseEventArgs e)
        {
            list_box.Items.Clear ();

            var fbd = new FolderBrowserDialog {
                InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
                Title = "Choose a directory"
            };

            if ((await fbd.ShowDialog (FindForm ()!)) == DialogResult.OK && fbd.SelectedPath is not null)
                list_box.Items.Add (fbd.SelectedPath);
        }
    }
}
