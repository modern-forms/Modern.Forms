using System;
using System.Threading.Tasks;
using Modern.WindowKit.Controls.Platform;
using Modern.WindowKit.Platform.Storage.FileIO;
using Modern.WindowKit.Platform.Storage;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a class for a file save dialog.
    /// </summary>
    public class SaveFileDialog : FileDialog
    {
        /// <summary>
        /// Gets or sets the default save extension. For example: "txt".
        /// </summary>
        public string? DefaultExtension { get; set; }

        /// <summary>
        /// Shows the dialog to the user.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        public async Task<DialogResult> ShowDialog (Form owner)
        {
            if (owner.window is ITopLevelImplWithStorageProvider parent) {
                var options = new FilePickerSaveOptions {
                    DefaultExtension= DefaultExtension,
                    SuggestedStartLocation = InitialDirectory is not null ? new BclStorageFolder (InitialDirectory) : null,
                    SuggestedFileName = FileName,
                    Title = Title,
                    FileTypeChoices = filters
                };

                var result = await parent.StorageProvider.SaveFilePickerAsync (options);

                FileNames.Clear ();

                var file = result?.GetFullPath ();

                if (file is not null)
                    FileNames.Add (file);

                return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
            }

            throw new ArgumentException ("Owner does not support system dialogs.");
        }
    }
}
