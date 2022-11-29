using System;
using System.Linq;
using System.Threading.Tasks;
using Modern.WindowKit.Controls.Platform;
using Modern.WindowKit.Platform;
using Modern.WindowKit.Platform.Storage;
using Modern.WindowKit.Platform.Storage.FileIO;

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
        public async Task<DialogResult> ShowDialog (Form owner)
        {
            if (owner.window is ITopLevelImplWithStorageProvider parent) {
                var options = new FolderPickerOpenOptions {
                    AllowMultiple = false,
                    SuggestedStartLocation = InitialDirectory is not null ? new BclStorageFolder (InitialDirectory) : null,
                    Title = Title
                };

                var result = await parent.StorageProvider.OpenFolderPickerAsync (options);

                var paths = result.Select (f =>  f.GetFullPath ()).WhereNotNull ();

                SelectedPath = paths?.FirstOrDefault ();

                return SelectedPath is null ? DialogResult.Cancel : DialogResult.OK;
            }

            throw new ArgumentException ("Owner does not support system dialogs.");
        }
    }
}
