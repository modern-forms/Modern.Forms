using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TitleBarPanel : Panel
    {
        public TitleBarPanel ()
        {
            var titlebar = Controls.Add (new FormTitleBar {
                Text = "Test Text!",
                Image = ImageLoader.Get ("swatches.png")
            });

            titlebar.Controls.Add (
                new TextBox {
                    Placeholder = "Search",
                    Dock = DockStyle.Left
                }
            );
        }
    }
}
