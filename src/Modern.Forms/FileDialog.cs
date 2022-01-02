using System;
using System.Collections.Generic;
using System.IO;
using Modern.WindowKit.Controls;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a base class for file dialogs.
    /// </summary>
    public abstract class FileDialog : FileSystemDialog
    {
        internal List<FileDialogFilter> filters = new List<FileDialogFilter> ();

        /// <summary>
        /// Adds a file filter choice to the dialog.
        /// </summary>
        /// <param name="name">Name of the filter, for example: "Text Files".</param>
        /// <param name="extensions">File extensions to filter for, for example: "txt", "log".</param>
        public void AddFilter (string name, params string[] extensions)
        {
            var filter = new FileDialogFilter {
                Name = name,
                Extensions = new List<string> (extensions)
            };

            filters.Add (filter);
        }

        /// <summary>
        /// Gets or sets the selected files. If there are multiple files selected, the first one is returned.
        /// </summary>
        public string? FileName {
            get => FileNames.Count > 0 ? Path.GetFullPath (FileNames[0]) : null;
            set {
                FileNames.Clear ();

                if (value != null)
                    FileNames.Add (Path.GetFullPath (value));
            }
        }

        /// <summary>
        /// Gets or sets the selected files.
        /// </summary>
        public List<string> FileNames { get; } = new List<string> ();
    }
}
