using Modern.Forms;

namespace ControlGallery.Panels;

public class NavigationPanePanel : Panel
{
    public NavigationPanePanel ()
    {
        var tb1 = Controls.Add (new NavigationPane ());

        tb1.Items.Add (ImageLoader.Get ("folder.png"));
        tb1.Items.Add (ImageLoader.Get ("folder-add.png"));
        tb1.Items.Add (ImageLoader.Get ("folder-closed.png"));
        tb1.Items.Add (ImageLoader.Get ("folder-up.png")).Enabled = false;
    }
}
