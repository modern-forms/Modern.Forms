using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modern.Forms
{
    public class OpenFileDialog : FileDialog
    {
        public bool AllowMultiple { get; set; }

        public async Task<DialogResult> ShowDialog (Window window)
        {
            var dialog = new Avalonia.Controls.OpenFileDialog {
                AllowMultiple = AllowMultiple,
                InitialDirectory = InitialDirectory,
                InitialFileName = FileName,
                Title = Title,
                Filters = filters
            };

            var files = await dialog.ShowAsync (window.window);

            FileNames.Clear ();
            FileNames.AddRange (files.Select (f => Path.GetFullPath (f)));

            return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}
