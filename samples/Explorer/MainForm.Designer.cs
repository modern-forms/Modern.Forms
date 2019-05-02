using System;
using System.Windows.Forms;
using Modern.Forms;

namespace Explorer
{
    public partial class MainForm : ModernForm
    {
        private Ribbon ribbon;
        private TreeView tree;
        private ListView view;
        private StatusBar statusbar;
        private ModernFormTitleBar titlebar;

        private void InitializeComponent ()
        {
            SuspendLayout ();

            // ListView
            view = new ListView {
                Dock = DockStyle.Fill
            };

            view.ItemDoubleClicked += View_ItemDoubleClicked;
            Controls.Add (view);

            // TreeControl
            tree = new TreeView {
                Dock = DockStyle.Left
            };

            tree.ItemSelected += Tree_ItemSelected;
            Controls.Add (tree);

            // RibbonControl
            ribbon = new Ribbon ();

            var home_tab = new RibbonTabPage { Text = "Home", Selected = true };

            var group1 = new RibbonItemGroup { Text = "Home" };

            group1.Items.Add (new RibbonItem ("Parent Folder", ImageLoader.Get ("folder-up.png"), new EventHandler (ParentFolder_Clicked)));

            var group2 = new RibbonItemGroup { Text = "Actions" };

            group2.Items.Add (new RibbonItem ("New Folder", ImageLoader.Get ("folder-add.png"), new EventHandler (NotImplemented_Clicked)));
            group2.Items.Add (new RibbonItem ("Search", ImageLoader.Get ("search.png"), new EventHandler (NotImplemented_Clicked)));
            group2.Items.Add (new RibbonItem ("Delete", ImageLoader.Get ("delete-red.png"), new EventHandler (NotImplemented_Clicked)));

            home_tab.Groups.Add (group1);
            home_tab.Groups.Add (group2);

            ribbon.TabPages.Add (home_tab);

            var share_tab = new RibbonTabPage { Text = "Share" };
            var share_group = new RibbonItemGroup { Text = "Send" };

            share_group.Items.Add (new RibbonItem ("Email", ImageLoader.Get ("mail.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new RibbonItem ("Zip", ImageLoader.Get ("compress.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new RibbonItem ("Burn DVD", ImageLoader.Get ("cd-burn.png"), new EventHandler (NotImplemented_Clicked)));
            share_group.Items.Add (new RibbonItem ("Print", ImageLoader.Get ("print.png"), new EventHandler (NotImplemented_Clicked)));

            share_tab.Groups.Add (share_group);
            ribbon.TabPages.Add (share_tab);

            var view_tab = new RibbonTabPage { Text = "View" };
            var group3 = new RibbonItemGroup { Text = "Themes" };
            group3.Items.Add (new RibbonItem ("Default", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new RibbonItem ("Green", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new RibbonItem ("Orange", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new RibbonItem ("Purple", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));
            group3.Items.Add (new RibbonItem ("Hotdog Stand", ImageLoader.Get ("swatches.png"), new EventHandler (ThemeButton_Clicked)));

            view_tab.Groups.Add (group3);
            ribbon.TabPages.Add (view_tab);

            Controls.Add (ribbon);

            // StatusBar
            statusbar = new StatusBar ();

            Controls.Add (statusbar);

            // TitleBar
            titlebar = new ModernFormTitleBar {
                Text = "Explorer Sample",
                Image = ImageLoader.Get ("layout-folder-pane.png")
            };

            Controls.Add (titlebar);

            ResumeLayout ();
        }
    }
}
