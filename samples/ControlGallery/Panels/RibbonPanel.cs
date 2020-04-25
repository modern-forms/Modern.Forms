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
            var ribbon = Controls.Add (new Ribbon ());

            var home_tab = ribbon.TabPages.Add ("Home");

            var group1 = home_tab.Groups.Add ("Home");
            group1.Items.Add ("Parent Folder", ImageLoader.Get ("folder-up.png"));

            var group2 = home_tab.Groups.Add ("Actions");
            group2.Items.Add ("New Folder", ImageLoader.Get ("folder-add.png"));
            group2.Items.Add ("Search", ImageLoader.Get ("search.png"));
            group2.Items.Add ("Delete", ImageLoader.Get ("delete-red.png"));

            var share_tab = ribbon.TabPages.Add ("Share");

            var share_group = share_tab.Groups.Add ("Send");
            share_group.Items.Add ("Email", ImageLoader.Get ("mail.png"));
            share_group.Items.Add ("Zip", ImageLoader.Get ("compress.png"));
            share_group.Items.Add ("Burn DVD", ImageLoader.Get ("cd-burn.png"));
            share_group.Items.Add ("Print", ImageLoader.Get ("print.png"));

            var view_tab = ribbon.TabPages.Add ("View");

            var group3 = view_tab.Groups.Add ("Themes");
            group3.Items.Add ("Default", ImageLoader.Get ("swatches.png"));
            group3.Items.Add ("Green", ImageLoader.Get ("swatches.png"));
            group3.Items.Add ("Orange", ImageLoader.Get ("swatches.png"));
            group3.Items.Add ("Purple", ImageLoader.Get ("swatches.png"));
            group3.Items.Add ("Hotdog Stand", ImageLoader.Get ("swatches.png"));
        }
    }
}
