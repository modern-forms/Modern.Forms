using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TitleBarPanel : Panel
    {
        public TitleBarPanel ()
        {
            var titlebar = new FormTitleBar {
                Text = "Test Text!"
            };

            Controls.Add (titlebar);
        }
    }
}
