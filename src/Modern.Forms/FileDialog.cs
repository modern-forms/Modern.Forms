using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;

namespace Modern.Forms
{
    public abstract class FileDialog : FileSystemDialog
    {
        internal List<FileDialogFilter> filters = new List<FileDialogFilter> ();

        public void AddFilter (string name, params string[] extensions)
        {
            var filter = new FileDialogFilter {
                Name = name,
                Extensions = new List<string> (extensions)
            };

            filters.Add (filter);
        }

        public string? FileName {
            get => FileNames.Count > 0 ? Path.GetFullPath (FileNames[0]) : null;
            set {
                FileNames.Clear ();

                if (value != null)
                    FileNames.Add (Path.GetFullPath (value));
            }
        }

        public List<string> FileNames { get; } = new List<string> ();
    }
}
