using System;
using System.Threading.Tasks;

namespace Modern.Forms
{
    public class FolderBrowserDialog : FileSystemDialog
    {
        public string? SelectedPath { get; set; }

        public async Task<DialogResult> ShowDialog (Window window)
        {
            var dialog = new Avalonia.Controls.OpenFolderDialog {
                Directory = InitialDirectory,
                Title = Title,
            };

            SelectedPath = await dialog.ShowAsync (window.window);

            return SelectedPath is null ? DialogResult.Cancel : DialogResult.OK;
        }
    }
}
