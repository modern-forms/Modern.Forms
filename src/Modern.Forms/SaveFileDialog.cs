using System;
using System.Threading.Tasks;

namespace Modern.Forms
{
    public class SaveFileDialog : FileDialog
    {
        public string? DefaultExtension { get; set; }

        public async Task<DialogResult> ShowDialog (Window window)
        {
            var dialog = new Avalonia.Controls.SaveFileDialog {
                DefaultExtension = DefaultExtension,
                InitialDirectory = InitialDirectory,
                InitialFileName = FileName,
                Title = Title,
                Filters = filters
            };

            var file = await dialog.ShowAsync (window.window);

            FileNames.Clear ();

            if (file != null)
                FileNames.Add (file);

            return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}
