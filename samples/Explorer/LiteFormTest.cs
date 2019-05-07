using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Modern.Forms;
using SkiaSharp;

namespace Explorer
{
    public class LiteFormTest : LiteForm
    {
        private string current_directory;

        private LiteRibbon ribbon;
        private LiteTreeView tree;
        private LiteListView view;
        private LiteStatusBar statusbar;
        private LiteModernFormTitleBar titlebar;

        public LiteFormTest ()
        {
            Width = 1080;
            Height = 720;

            // ListView
            view = new LiteListView {
                Dock = DockStyle.Fill
            };

            view.ItemDoubleClicked += View_ItemDoubleClicked;
            LiteControls.Add (view);

            // TreeControl
            tree = new LiteTreeView {
                Dock = DockStyle.Left
            };

            tree.ItemSelected += Tree_ItemSelected;
            LiteControls.Add (tree);

            // RibbonControl
            ribbon = new LiteRibbon ();

            var home_tab = ribbon.AddTabPage ("Home");

            var group1 = new LiteRibbonItemGroup { Text = "Home" };

            group1.Items.Add (new LiteRibbonItem ("Parent Folder", ImageLoader.Get ("folder-up.png"), new EventHandler (ParentFolder_Clicked)));

            var group2 = new LiteRibbonItemGroup { Text = "Actions" };

            group2.Items.Add (new LiteRibbonItem ("New Folder", ImageLoader.Get ("folder-add.png"), new EventHandler (NotImplemented_Clicked)));
            group2.Items.Add (new LiteRibbonItem ("Search", ImageLoader.Get ("search.png"), new EventHandler (NotImplemented_Clicked)));
            group2.Items.Add (new LiteRibbonItem ("Delete", ImageLoader.Get ("delete-red.png"), new EventHandler (ShowButtonForm_Clicked)));

            home_tab.Groups.Add (group1);
            home_tab.Groups.Add (group2);

            var share_tab = ribbon.AddTabPage ("Share");
            var share_group = new LiteRibbonItemGroup { Text = "Send" };

            share_group.Items.Add (new LiteRibbonItem ("Email", ImageLoader.Get ("mail.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new LiteRibbonItem ("Zip", ImageLoader.Get ("compress.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new LiteRibbonItem ("Burn DVD", ImageLoader.Get ("cd-burn.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new LiteRibbonItem ("Print", ImageLoader.Get ("print.png"), new EventHandler (NotImplemented_Clicked)));

            share_tab.Groups.Add (share_group);

            var view_tab = ribbon.AddTabPage ("View");
            var group3 = new LiteRibbonItemGroup { Text = "Themes" };
            group3.Items.Add (new LiteRibbonItem ("Default", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new LiteRibbonItem ("Green", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new LiteRibbonItem ("Orange", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new LiteRibbonItem ("Purple", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new LiteRibbonItem ("Hotdog Stand", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));

            view_tab.Groups.Add (group3);

            LiteControls.Add (ribbon);

            // StatusBar
            statusbar = new LiteStatusBar ();

            LiteControls.Add (statusbar);

            titlebar = new LiteModernFormTitleBar {
                Text = "Button Style Sample"
            };

            LiteControls.Add (titlebar);

            // Populate the drive list
            foreach (var drive in DriveInfo.GetDrives ().Where (d => d.IsReady))
                tree.Items.Add ($"{drive.Name.Trim ('\\')} - {drive.VolumeLabel}", ImageLoader.Get ("drive.png")).Tag = drive;

            tree.Items.First ().Selected = true;

            // Select the first available drive
            SetSelectedDirectory (((DriveInfo)tree.Items.First ().Tag).Name);

            DoLayout ();
        }

        private void Tree_ItemSelected (object sender, EventArgs<LiteTreeViewItem> e)
        {
            var drive = (DriveInfo)e.Value.Tag;

            if (drive != null)
                SetSelectedDirectory (drive.Name);
        }

        private void View_ItemDoubleClicked (object sender, EventArgs<LiteListViewItem> e)
        {
            var item = e.Value;

            if (item.Tag is string s && s == "Directory")
                SetSelectedDirectory (Path.Combine (current_directory, item.Text));
        }

        private void SetSelectedDirectory (string directory)
        {
            current_directory = directory;

            view.Items.Clear ();

            var directories = 0;
            var files = 0;

            try {
                foreach (var d in Directory.EnumerateDirectories (directory).Take (30))
                    view.Items.Add (new LiteListViewItem { Text = Path.GetFileName (d), Image = ImageLoader.Get ("folder-closed.png"), Tag = "Directory" });

                directories = view.Items.Count;

                foreach (var f in Directory.EnumerateFiles (directory).Take (50))
                    view.Items.Add (new LiteListViewItem { Text = Path.GetFileName (f), Image = ImageLoader.Get ("new.png") });

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
    }
}
