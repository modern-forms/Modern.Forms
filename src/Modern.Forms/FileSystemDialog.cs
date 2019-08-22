using System;

namespace Modern.Forms
{
    public abstract class FileSystemDialog
    {
        public string? InitialDirectory { get; set; }

        public string Title { get; set; } = string.Empty;
    }
}
