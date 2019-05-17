using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class RibbonPanel : Panel
    {
        public RibbonPanel ()
        {
            var ribbon = new Ribbon ();

            var home_tab = ribbon.TabPages.Add ("Home");

            var group1 = new RibbonItemGroup { Text = "Home" };

            group1.Items.Add (new RibbonItem ("Parent Folder", ImageLoader.Get ("folder-up.png")));

            var group2 = new RibbonItemGroup { Text = "Actions" };

            group2.Items.Add (new RibbonItem ("New Folder", ImageLoader.Get ("folder-add.png")));
            group2.Items.Add (new RibbonItem ("Search", ImageLoader.Get ("search.png")));
            group2.Items.Add (new RibbonItem ("Delete", ImageLoader.Get ("delete-red.png")));

            home_tab.Groups.Add (group1);
            home_tab.Groups.Add (group2);

            var share_tab = ribbon.TabPages.Add ("Share");
            var share_group = new RibbonItemGroup { Text = "Send" };

            share_group.Items.Add (new RibbonItem ("Email", ImageLoader.Get ("mail.png")));
            share_group.Items.Add (new RibbonItem ("Zip", ImageLoader.Get ("compress.png")));
            share_group.Items.Add (new RibbonItem ("Burn DVD", ImageLoader.Get ("cd-burn.png")));
            share_group.Items.Add (new RibbonItem ("Print", ImageLoader.Get ("print.png")));

            share_tab.Groups.Add (share_group);

            var view_tab = ribbon.TabPages.Add ("View");
            var group3 = new RibbonItemGroup { Text = "Themes" };
            group3.Items.Add (new RibbonItem ("Default", ImageLoader.Get ("swatches.png")));
            group3.Items.Add (new RibbonItem ("Green", ImageLoader.Get ("swatches.png")));
            group3.Items.Add (new RibbonItem ("Orange", ImageLoader.Get ("swatches.png")));
            group3.Items.Add (new RibbonItem ("Purple", ImageLoader.Get ("swatches.png")));
            group3.Items.Add (new RibbonItem ("Hotdog Stand", ImageLoader.Get ("swatches.png")));

            view_tab.Groups.Add (group3);

            Controls.Add (ribbon);
        }
    }
}
