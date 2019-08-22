#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Platform;
using Avalonia.Platform;

namespace Avalonia.Controls
{
    abstract class FileDialog : FileSystemDialog
    {
        public List<FileDialogFilter> Filters { get; set; } = new List<FileDialogFilter>();
        public string InitialFileName { get; set; }        
    }

    abstract class FileSystemDialog : SystemDialog
    {
        public string InitialDirectory { get; set; }
    }

    class SaveFileDialog : FileDialog
    {
        public string DefaultExtension { get; set; }

        public async Task<string> ShowAsync(IWindowBaseImpl parent)
        {
            if(parent == null)
                throw new ArgumentNullException(nameof(parent));
            return ((await AvaloniaGlobals.SystemDialogImplementation
                 .ShowFileDialogAsync(this, parent)) ??
             Array.Empty<string>()).FirstOrDefault();
        }
    }

    class OpenFileDialog : FileDialog
    {
        public bool AllowMultiple { get; set; }

        public Task<string[]> ShowAsync(IWindowBaseImpl parent)
        {
            if(parent == null)
                throw new ArgumentNullException(nameof(parent));
            return AvaloniaGlobals.SystemDialogImplementation.ShowFileDialogAsync(this, parent);
        }
    }

    class OpenFolderDialog : FileSystemDialog
    {
        public string DefaultDirectory { get; set; }

        public Task<string> ShowAsync(IWindowBaseImpl parent)
        {
            if(parent == null)
                throw new ArgumentNullException(nameof(parent));
            return AvaloniaGlobals.SystemDialogImplementation.ShowFolderDialogAsync(this, parent);
        }
    }

    abstract class SystemDialog
    {
        public string Title { get; set; }
    }

    class FileDialogFilter
    {
        public string Name { get; set; }
        public List<string> Extensions { get; set; } = new List<string>();
    }
}
