using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TabStripPanel : Panel
    {
        public TabStripPanel ()
        {
            var tabstrip = new TabStrip {
                Left = 10,
                Top = 10,
                Width = 600
            };

            tabstrip.Tabs.Add (new TabStripItem { Text = "File", Selected = true });
            tabstrip.Tabs.Add (new TabStripItem { Text = "Share" });
            tabstrip.Tabs.Add (new TabStripItem { Text = "View" });

            Controls.Add (tabstrip);
        }
    }
}
