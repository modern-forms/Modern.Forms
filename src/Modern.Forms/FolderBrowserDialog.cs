using System;
using System.Threading.Tasks;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a dialog for choosing a file system directory.
    /// </summary>
    public class FolderBrowserDialog : FileSystemDialog
    {
        /// <summary>
        /// Gets or sets the selected folder path.
        /// </summary>
        public string? SelectedPath { get; set; }

        /// <summary>
        /// Shows the dialog to the user.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        public async Task<DialogResult> ShowDialog (Window owner)
        {
            var dialog = new Modern.WindowKit.Controls.OpenFolderDialog {
                Directory = InitialDirectory,
                Title = Title,
            };

            SelectedPath = await dialog.ShowAsync (owner.window);

            return SelectedPath is null ? DialogResult.Cancel : DialogResult.OK;
        }
    }
}
