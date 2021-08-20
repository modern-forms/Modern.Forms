using System;
using System.Threading.Tasks;

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
        public async Task<DialogResult> ShowDialog (Window owner)
        {
            var dialog = new Modern.WindowKit.Controls.SaveFileDialog {
                DefaultExtension = DefaultExtension,
                Directory = InitialDirectory,
                InitialFileName = FileName,
                Title = Title,
                Filters = filters
            };

            var file = await dialog.ShowAsync (owner.window);

            FileNames.Clear ();

            if (file is not null)
                FileNames.Add (file);

            return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}
