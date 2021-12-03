using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Explore;

namespace Designer
{
    public partial class WinForm : Form
    {
        public WinForm ()
        {
            InitializeComponent ();

            var ribbon = new Modern.Forms.Ribbon { ShowTabs = true };

            var home_tab = ribbon.TabPages.Add ("Home");

            var group1 = home_tab.Groups.Add ("Home");
            group1.Items.Add ("Parent Folder", ImageLoader.Get ("folder-up.png"), null);

            var group2 = home_tab.Groups.Add ("Actions");
            group2.Items.Add ("New Folder", ImageLoader.Get ("folder-add.png"), null);
            group2.Items.Add ("Search", ImageLoader.Get ("search.png"), null);
            group2.Items.Add ("Delete", ImageLoader.Get ("delete-red.png"), null);

            var share_tab = ribbon.TabPages.Add ("Share");

            var share_group = share_tab.Groups.Add ("Send");
            share_group.Items.Add ("Email", ImageLoader.Get ("mail.png"), null);
            share_group.Items.Add ("Zip", ImageLoader.Get ("compress.png"), null);
            share_group.Items.Add ("Burn DVD", ImageLoader.Get ("cd-burn.png"), null);
            share_group.Items.Add ("Print", ImageLoader.Get ("print.png"), null);

            var view_tab = ribbon.TabPages.Add ("View");

            var group3 = view_tab.Groups.Add ("Themes");
            group3.Items.Add ("Default", ImageLoader.Get ("swatches.png"), null);
            group3.Items.Add ("Green", ImageLoader.Get ("swatches.png"), null);
            group3.Items.Add ("Orange", ImageLoader.Get ("swatches.png"), null);
            group3.Items.Add ("Purple", ImageLoader.Get ("swatches.png"), null);
            group3.Items.Add ("Hotdog Stand", ImageLoader.Get ("swatches.png"), null);

            var button = new Modern.Forms.Button { Text = "Hello!" };

            var control = new ModernControlHost (ribbon) {
                Width = 100,
                Height = 111,
                Dock = DockStyle.Top
            };

            Controls.Add (control);
        }
    }
}
