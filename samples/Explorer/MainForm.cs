using System;
using System.IO;
using System.Linq;
using Modern.Forms;
using SkiaSharp;

namespace Explore
{
    public partial class MainForm : Form
    {
        private string current_directory;

        public MainForm ()
        {
            InitializeComponent ();

            // Populate the drive list
            foreach (var drive in DriveInfo.GetDrives ().Where (d => d.IsReady))
                tree.Items.Add ($"{drive.Name.Trim ('\\')} - {drive.VolumeLabel}", ImageLoader.Get ("drive.png")).Tag = drive.Name;

            tree.Items.First ().Selected = true;

            // Select the first available drive
            SetSelectedDirectory ((string)tree.Items.First ().Tag);
        }

        private void View_ItemDoubleClicked (object sender, EventArgs<ListViewItem> e)
        {
            var item = e.Value;

            if (item.Tag is string s && s == "Directory")
                SetSelectedDirectory (Path.Combine (current_directory, item.Text));
        }

        private void Tree_ItemSelected (object sender, EventArgs<TreeViewItem> e)
        {
            var drive = (string)e.Value.Tag;

            if (drive != null)
                SetSelectedDirectory (drive);
        }

        private void SetSelectedDirectory (string directory)
        {
            current_directory = directory;

            view.Items.Clear ();

            var directories = 0;
            var files = 0;
            var tree_item = tree.SelectedItem;

            try {
                foreach (var d in Directory.EnumerateDirectories (directory).Take (30))
                    view.Items.Add (new ListViewItem { Text = Path.GetFileName (d), Image = ImageLoader.Get ("folder-closed.png"), Tag = "Directory" });

                directories = view.Items.Count;

                if (!tree_item.HasChildren)
                    tree_item.Items.AddRange (view.Items.Select (l => new TreeViewItem (l.Text) { Image = ImageLoader.Get ("folder.png"), Tag = Path.Combine (current_directory, l.Text) }));

                foreach (var f in Directory.EnumerateFiles (directory).Take (50))
                    view.Items.Add (new ListViewItem { Text = Path.GetFileName (f), Image = ImageLoader.Get ("new.png") });

                files = view.Items.Count - directories;

                statusbar.Text = $"{directories} directories, {files} files";

            } catch (UnauthorizedAccessException) {
                // Ignore
                statusbar.Text = "Unable to open directory due to permissions";
            }

            Text = "Explore Sample - " + directory;
        }

        private void ThemeButton_Clicked (object sender, EventArgs args)
        {
            var item = sender as MenuItem;

            Theme.FormBackgroundColor = SKColors.White;
            Theme.NeutralGray = new SKColor (240, 240, 240);
            Theme.LightNeutralGray = new SKColor (251, 251, 251);
            Theme.RibbonItemHighlightColor = new SKColor (198, 198, 198);

            switch (item.Text) {
                case "Default":
                    Theme.BeginUpdate ();
                    Theme.RibbonTabHighlightColor = new SKColor (42, 138, 208);
                    Theme.RibbonColor = new SKColor (16, 110, 190);
                    Theme.EndUpdate ();
                    break;
                case "Green":
                    Theme.BeginUpdate ();
                    Theme.RibbonTabHighlightColor = new SKColor (67, 148, 103);
                    Theme.RibbonColor = new SKColor (33, 115, 70);
                    Theme.EndUpdate ();
                    break;
                case "Orange":
                    Theme.BeginUpdate ();
                    Theme.RibbonTabHighlightColor = new SKColor (220, 89, 57);
                    Theme.RibbonColor = new SKColor (183, 71, 42);
                    Theme.EndUpdate ();
                    break;
                case "Purple":
                    Theme.BeginUpdate ();
                    Theme.RibbonTabHighlightColor = new SKColor (163, 86, 158);
                    Theme.RibbonColor = new SKColor (128, 57, 123);
                    Theme.EndUpdate ();
                    break;
                case "Hotdog Stand":
                    Theme.BeginUpdate ();
                    Theme.RibbonTabHighlightColor = new SKColor (255, 128, 128);
                    Theme.FormBackgroundColor = SKColors.Yellow;
                    Theme.NeutralGray = SKColors.White;
                    Theme.LightNeutralGray = new SKColor (255, 0, 0);
                    Theme.RibbonItemHighlightColor = new SKColor (255, 255, 255);
                    Theme.RibbonColor = new SKColor (255, 0, 0);
                    Theme.EndUpdate ();
                    break;
            }

            Invalidate ();
        }

        private void ParentFolder_Clicked (object sender, EventArgs args)
        {
            var parent_folder = Path.GetDirectoryName (current_directory);

            if (parent_folder != null)
                SetSelectedDirectory (parent_folder);
        }

        private void NotImplemented_Clicked (object sender, EventArgs args)
        {
            new MessageBoxForm ("Demo", "Functionality not available in demo").ShowDialog (this);
        }
    }
}
