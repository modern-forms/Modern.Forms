using Modern.Forms;
using SkiaSharp;

namespace Outlaw
{
    partial class MainForm
    {
        private StatusBar statusbar;
        private NavigationPane navigation_pane;
        private TreeView nav_tree_view;
        private TreeView email_list;
        private Panel message_pane;
        private TabControl menus;

        private void InitializeComponent ()
        {
            Size = new System.Drawing.Size (1600, 1024);
            Text = "Outlaw";
            Image = ImageLoader.Get ("email-icon.png");

            // Message Pane
            message_pane = Controls.Add (new MessagePanePlaceholder { Dock = DockStyle.Fill });
            message_pane.Style.BackgroundColor = SKColors.White;

            // Email list
            email_list = Controls.Add (new TreeView { Dock = DockStyle.Left, Width = 325, DrawMode = TreeViewDrawMode.OwnerDrawContent });
            email_list.Style.Border.Left.Width = 0;
            email_list.Style.Border.Top.Width = 0;
            email_list.Style.Border.Bottom.Width = 0;
            email_list.Style.BackgroundColor = SKColors.White;

            // Email folder list
            nav_tree_view = Controls.Add (new TreeView { Dock = DockStyle.Left, Width = 225 });
            nav_tree_view.Style.Border.Left.Width = 0;
            nav_tree_view.Style.Border.Top.Width = 0;
            nav_tree_view.Style.Border.Bottom.Width = 0;

            var account = nav_tree_view.Items.Add ("joe@example.com");

            account.Items.Add ("Inbox", ImageLoader.Get ("email.png"));
            account.Items.Add ("Drafts", ImageLoader.Get ("email.png"));
            account.Items.Add ("Sent Items", ImageLoader.Get ("email.png"));
            account.Items.Add ("Deleted Items", ImageLoader.Get ("email.png"));
            account.Items.Add ("Junk Email", ImageLoader.Get ("email.png"));

            account.Expand ();

            // Menu TabControl
            menus = Controls.Add (new TabControl { Dock = DockStyle.Top, Height = 71 });

            var home_tab = menus.TabPages.Add ("Home");
            var send_tab = menus.TabPages.Add ("Send / Receive");
            var view_tab = menus.TabPages.Add ("View");
            var help_tab = menus.TabPages.Add ("Help");

            // Home Toolbar
            var home_toolbar = home_tab.Controls.Add (new ToolBar { Height = 40 });

            home_toolbar.Items.Add ("New Email", ImageLoader.Get ("mail-40.png"));
            home_toolbar.Items.Add (new MenuSeparatorItem ());
            home_toolbar.Items.Add ("Delete", ImageLoader.Get ("delete.png"));
            home_toolbar.Items.Add ("Archive", ImageLoader.Get ("archive.png"));
            home_toolbar.Items.Add ("Move", ImageLoader.Get ("move.png"));
            home_toolbar.Items.Add (new MenuSeparatorItem ());
            home_toolbar.Items.Add ("Reply", ImageLoader.Get ("reply.png"));
            home_toolbar.Items.Add ("Reply All", ImageLoader.Get ("reply-all.png"));
            home_toolbar.Items.Add ("Forward", ImageLoader.Get ("forward.png"));
            home_toolbar.Items.Add (new MenuSeparatorItem ());
            home_toolbar.Items.Add ("Send / Receive All Folders", ImageLoader.Get ("send-receive.png"));

            // Send / Receive Toolbar
            var send_toolbar = send_tab.Controls.Add (new ToolBar { Height = 40 });

            send_toolbar.Items.Add ("Send/Receive All Folders", ImageLoader.Get ("send-receive.png"));
            send_toolbar.Items.Add ("Send All", ImageLoader.Get ("send-receive.png"));
            send_toolbar.Items.Add ("Update Folder", ImageLoader.Get ("upload-folder.png"));
            send_toolbar.Items.Add ("Send/Receive Group", ImageLoader.Get ("upload-folder.png"));
            send_toolbar.Items.Add (new MenuSeparatorItem ());
            send_toolbar.Items.Add ("Show Progress", ImageLoader.Get ("progress.png"));
            send_toolbar.Items.Add ("Cancel All", ImageLoader.Get ("cancel-send.png"));
            send_toolbar.Items.Add (new MenuSeparatorItem ());
            send_toolbar.Items.Add ("Work Offline", ImageLoader.Get ("offline.png"));

            // View Toolbar
            var view_toolbar = view_tab.Controls.Add (new ToolBar { Height = 40 });

            view_toolbar.Items.Add ("Change View", ImageLoader.Get ("change-view.png"));
            view_toolbar.Items.Add ("Current View", ImageLoader.Get ("current-view.png"));
            view_toolbar.Items.Add (new MenuSeparatorItem ());
            view_toolbar.Items.Add ("Arrange By", ImageLoader.Get ("arrange.png"));
            view_toolbar.Items.Add ("Reverse Sort", ImageLoader.Get ("sort.png"));
            view_toolbar.Items.Add (new MenuSeparatorItem ());
            view_toolbar.Items.Add ("Use Tighter Spacing", ImageLoader.Get ("tighter.png"));
            view_toolbar.Items.Add ("Layout", ImageLoader.Get ("layout.png"));

            // Help Toolbar
            var help_toolbar = help_tab.Controls.Add (new ToolBar { Height = 40 });

            help_toolbar.Items.Add ("Help", ImageLoader.Get ("help.png"));
            help_toolbar.Items.Add ("Contact Support", ImageLoader.Get ("support.png"));
            help_toolbar.Items.Add ("Feedback", ImageLoader.Get ("feedback.png"));
            help_toolbar.Items.Add ("Suggest a Feature", ImageLoader.Get ("suggest.png"));

            // Navigation Pane
            navigation_pane = Controls.Add (new NavigationPane ());

            navigation_pane.Items.Add (ImageLoader.Get ("email.png"));
            navigation_pane.Items.Add (ImageLoader.Get ("calendar.png"));
            navigation_pane.Items.Add (ImageLoader.Get ("people.png"));
            navigation_pane.Items.Add (ImageLoader.Get ("task.png"));

            navigation_pane.SelectedIndex = 0;

            // StatusBar
            statusbar = Controls.Add (new StatusBar { Text = "Connected" });
        }
    }
}