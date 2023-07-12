using System;
using System.IO;
using Modern.WindowKit.Platform.Storage.FileIO;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a base class for file system dialogs.
    /// </summary>
    public abstract class FileSystemDialog
    {
        /// <summary>
        /// Gets or sets the initial directory for the dialog.
        /// </summary>
        public string? InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title for the dialog.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        internal BclStorageFolder? GetInitialDirectory ()
        {
            if (InitialDirectory is not null) {
                var dir_info = new DirectoryInfo (InitialDirectory);

                if (dir_info.Exists)
                    return new BclStorageFolder (dir_info);
            }

            return null;
        }
    }
}
