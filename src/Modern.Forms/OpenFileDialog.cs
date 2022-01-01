using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var dialog = new Modern.WindowKit.Controls.OpenFileDialog {
                AllowMultiple = AllowMultiple,
                Directory = InitialDirectory,
                InitialFileName = FileName,
                Title = Title,
                Filters = filters
            };

            var files = await dialog.ShowAsync (owner.window);

            FileNames.Clear ();

            if (files?.Any () == true)
                FileNames.AddRange (files.Select (f => Path.GetFullPath (f)));

            return FileNames.Count > 0 ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}
