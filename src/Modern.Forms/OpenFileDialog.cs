using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Modern.WindowKit.Controls.Platform;
using Modern.WindowKit.Platform.Storage.FileIO;
using Modern.WindowKit.Platform.Storage;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a class for a file open dialog.
    /// </summary>
    public class OpenFileDialog : FileDialog
    {
        /// <summary>
        /// Gets or sets whether multiple files can be selected.
        /// </summary>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// Shows the dialog to the user.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        public async Task<DialogResult> ShowDialog (Form owner)
        {
            if (owner.window is ITopLevelImplWithStorageProvider parent) {
                var options = new FilePickerOpenOptions {
                    AllowMultiple = AllowMultiple,
                    SuggestedStartLocation = InitialDirectory is not null ? new BclStorageFolder (InitialDirectory) : null,
                    Title = Title,
                    FileTypeFilter = filters
                };

                var result = await parent.StorageProvider.OpenFilePickerAsync (options);

                FileNames.Clear ();

                var files = result.Select (f => f.GetFullPath ()).WhereNotNull ();

                if (files.Any ())
                    FileNames.AddRange (files);

                return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
            }
            
            throw new ArgumentException ("Owner does not support system dialogs.");
        }
    }
}
