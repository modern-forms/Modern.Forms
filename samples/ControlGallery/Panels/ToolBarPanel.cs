using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ToolBarPanel : Panel
    {
        public ToolBarPanel ()
        {
            var tb = new ToolBar ();

            tb.Items.Add (new MenuItem ("Parent Folder", ImageLoader.Get ("folder-up.png")));
            tb.Items.Add (new MenuItem ("New Folder", ImageLoader.Get ("folder-add.png")));
            tb.Items.Add (new MenuSeparatorItem ());

            var search = tb.Items.Add (new MenuItem ("Search", ImageLoader.Get ("search.png")));

            search.Items.Add ("Images");
            search.Items.Add ("Music");
            search.Items.Add ("Movies");
            search.Items.Add (new MenuSeparatorItem ());

            var more = search.Items.Add ("More");
            more.Items.Add ("Zips");
            more.Items.Add ("Apps");
            more.Items.Add ("Games");

            var delete = tb.Items.Add (new MenuItem ("Delete", ImageLoader.Get ("delete-red.png")));

            delete.Items.Add ("Send to recycle bin");
            delete.Items.Add ("Permanently delete");

            tb.Items.Add (new MenuItem ("Email"));
            tb.Items.Add (new MenuItem ("Zip", ImageLoader.Get ("compress.png")));

            Controls.Add (tb);
        }
    }
}
