using System;
using Modern.Forms;

namespace Explore
{
    public partial class MainForm : Form
    {
        private Ribbon ribbon;
        private TreeView tree;
        private ListView view;
        private StatusBar statusbar;

        private void InitializeComponent ()
        {
            //SuspendLayout ();

            // ListView
            view = Controls.Add (new ListView { Dock = DockStyle.Fill });
            view.ItemDoubleClicked += View_ItemDoubleClicked;

            // TreeControl
            tree = Controls.Add (new TreeView { Dock = DockStyle.Left });

            tree.Style.Border.Top.Width = 0;
            tree.Style.Border.Left.Width = 0;
            tree.Style.Border.Bottom.Width = 0;

            tree.ItemSelected += Tree_ItemSelected;

            // RibbonControl
            ribbon = Controls.Add (new Ribbon ());

            var home_tab = ribbon.TabPages.Add ("Home");

            var group1 = home_tab.Groups.Add ("Home");
            group1.Items.Add ("Parent Folder", ImageLoader.Get ("folder-up.png"), new EventHandler<MouseEventArgs> (ParentFolder_Clicked));

            var group2 = home_tab.Groups.Add ("Actions");
            group2.Items.Add ("New Folder", ImageLoader.Get ("folder-add.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));
            group2.Items.Add ("Search", ImageLoader.Get ("search.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));
            group2.Items.Add ("Delete", ImageLoader.Get ("delete-red.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));

            var share_tab = ribbon.TabPages.Add ("Share");

            var share_group = share_tab.Groups.Add ("Send");
            share_group.Items.Add ("Email", ImageLoader.Get ("mail.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));
            share_group.Items.Add ("Zip", ImageLoader.Get ("compress.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));
            share_group.Items.Add ("Burn DVD", ImageLoader.Get ("cd-burn.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));
            share_group.Items.Add ("Print", ImageLoader.Get ("print.png"), new EventHandler<MouseEventArgs> (NotImplemented_Clicked));

            var view_tab = ribbon.TabPages.Add ("View");

            var group3 = view_tab.Groups.Add ("Themes");
            group3.Items.Add ("Default", ImageLoader.Get ("swatches.png"), new EventHandler<MouseEventArgs> (ThemeButton_Clicked));
            group3.Items.Add ("Green", ImageLoader.Get ("swatches.png"), new EventHandler<MouseEventArgs> (ThemeButton_Clicked));
            group3.Items.Add ("Orange", ImageLoader.Get ("swatches.png"), new EventHandler<MouseEventArgs> (ThemeButton_Clicked));
            group3.Items.Add ("Purple", ImageLoader.Get ("swatches.png"), new EventHandler<MouseEventArgs> (ThemeButton_Clicked));
            group3.Items.Add ("Hotdog Stand", ImageLoader.Get ("swatches.png"), new EventHandler<MouseEventArgs> (ThemeButton_Clicked));

            // StatusBar
            statusbar = Controls.Add (new StatusBar ());

            Text = "Explore Sample";
            Image = ImageLoader.Get ("layout-folder-pane.png");

            //ResumeLayout ();
        }
    }
}
