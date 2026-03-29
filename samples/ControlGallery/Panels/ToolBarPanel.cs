using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ToolBarPanel : Panel
    {
        public ToolBarPanel ()
        {
            var tb2 = Controls.Add (new ToolBar { Enabled = false });
            tb2.Items.Add (new MenuItem ("Parent Folder", ImageLoader.Get ("folder-up.png")));
            tb2.Items.Add (new MenuSeparatorItem ());

            var search2 = tb2.Items.Add (new MenuItem ("Search", ImageLoader.Get ("search.png")));

            search2.Items.Add ("Images");
            search2.Items.Add ("Music");
            search2.Items.Add ("Movies");
            search2.Items.Add (new MenuSeparatorItem ());

            var more2 = search2.Items.Add ("More");
            more2.Items.Add ("Zips");
            more2.Items.Add ("Apps");
            more2.Items.Add ("Games");

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
            more.Items.Add ("Apps").Enabled = false;
            more.Items.Add ("Games");

            var delete = tb.Items.Add (new MenuItem ("Delete", ImageLoader.Get ("delete-red.png")));

            delete.Items.Add ("Send to recycle bin");
            delete.Items.Add ("Permanently delete");

            tb.Items.Add (new MenuItem ("Email"));
            tb.Items.Add (new MenuItem ("Zip", ImageLoader.Get ("compress.png")) { Enabled = false });

            Controls.Add (tb);

        }
    }
}
