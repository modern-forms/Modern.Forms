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
    }
}
