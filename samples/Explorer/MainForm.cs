using System;
using System.IO;
using System.Linq;
using Modern.Forms;
using SkiaSharp;

namespace Explorer
{
    public partial class MainForm : ModernForm
    {
        private string current_directory;

        public MainForm ()
        {
            InitializeComponent ();

            // Populate the drive list
            foreach (var drive in DriveInfo.GetDrives ().Where (d => d.IsReady))
                tree.Items.Add ($"{drive.Name.Trim ('\\')} - {drive.VolumeLabel}", ImageLoader.Get ("drive.png")).Tag = drive;

            tree.Items.First ().Selected = true;

            // Select the first available drive
            SetSelectedDirectory (((DriveInfo)tree.Items.First().Tag).Name);
        }

        private void View_ItemDoubleClicked (object sender, EventArgs<ListViewItem> e)
        {
            var item = e.Value;

            if (item.Tag is string s && s == "Directory")
                SetSelectedDirectory (Path.Combine (current_directory, item.Text));
        }

        private void Tree_ItemSelected (object sender, EventArgs<TreeViewItem> e)
        {
            var drive = (DriveInfo)e.Value.Tag;

            if (drive != null)
                SetSelectedDirectory (drive.Name);
        }

        private void SetSelectedDirectory (string directory)
        {
            current_directory = directory;

            view.Items.Clear ();

            var directories = 0;
            var files = 0;

            try {
                foreach (var d in Directory.EnumerateDirectories (directory).Take (30))
                    view.Items.Add (new ListViewItem { Text = Path.GetFileName (d), Image = ImageLoader.Get ("folder-closed.png"), Tag = "Directory" });

                directories = view.Items.Count;

                foreach (var f in Directory.EnumerateFiles (directory).Take (50))
                    view.Items.Add (new ListViewItem { Text = Path.GetFileName (f), Image = ImageLoader.Get ("new.png") });

                files = view.Items.Count - directories;

                statusbar.Text = $"{directories} directories, {files} files";
                statusbar.Invalidate ();

            } catch (UnauthorizedAccessException) {
                // Ignore
                statusbar.Text = "Unable to open directory due to permissions";
                statusbar.Invalidate ();
            }

            view.Invalidate ();

            Text = "Explorer Sample - " + directory;
            titlebar.Text = "Explorer Sample - " + directory;
            titlebar.Invalidate ();
        }

        private void ThemeButton_Clicked (object sender, EventArgs args)
        {
            var item = sender as RibbonItem;

            ModernTheme.FormBackgroundColor = SKColors.White;
            ModernTheme.NeutralGray = new SKColor (240, 240, 240);
            ModernTheme.LightNeutralGray = new SKColor (251, 251, 251);
            ModernTheme.RibbonItemHighlightColor = new SKColor (198, 198, 198);

            switch (item.Text) {
                case "Default":
                    ModernTheme.BeginUpdate ();
                    ModernTheme.RibbonTabHighlightColor = new SKColor (42, 138, 208);
                    ModernTheme.RibbonColor = new SKColor (16, 110, 190);
                    ModernTheme.EndUpdate ();
                    break;
                case "Green":
                    ModernTheme.BeginUpdate ();
                    ModernTheme.RibbonTabHighlightColor = new SKColor (67, 148, 103);
                    ModernTheme.RibbonColor = new SKColor (33, 115, 70);
                    ModernTheme.EndUpdate ();
                    break;
                case "Orange":
                    ModernTheme.BeginUpdate ();
                    ModernTheme.RibbonTabHighlightColor = new SKColor (220, 89, 57);
                    ModernTheme.RibbonColor = new SKColor (183, 71, 42);
                    ModernTheme.EndUpdate ();
                    break;
                case "Purple":
                    ModernTheme.BeginUpdate ();
                    ModernTheme.RibbonTabHighlightColor = new SKColor (163, 86, 158);
                    ModernTheme.RibbonColor = new SKColor (128, 57, 123);
                    ModernTheme.EndUpdate ();
                    break;
                case "Hotdog Stand":
                    ModernTheme.BeginUpdate ();
                    ModernTheme.RibbonTabHighlightColor = new SKColor (255, 128, 128);
                    ModernTheme.FormBackgroundColor = SKColors.Yellow;
                    ModernTheme.NeutralGray = SKColors.White;
                    ModernTheme.LightNeutralGray = new SKColor (255, 0, 0);
                    ModernTheme.RibbonItemHighlightColor = new SKColor (255, 255, 255);
                    ModernTheme.RibbonColor = new SKColor (255, 0, 0);
                    ModernTheme.EndUpdate ();
                    break;
            }

            Refresh ();
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

        private void ShowButtonForm_Clicked (object sender, EventArgs e)
        {
            //tree.Items.Add (DateTime.Now.ToString ());
            new ButtonForm ().Show ();
        }
    }
}
