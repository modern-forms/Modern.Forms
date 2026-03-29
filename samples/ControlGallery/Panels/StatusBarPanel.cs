using Modern.Forms;

namespace ControlGallery.Panels
{
    public class StatusBarPanel : Panel
    {
        public StatusBarPanel ()
        {
            var statusbar = new StatusBar {
                Text = "Test Text!"
            };

            Controls.Add (statusbar);
        }
    }
}
