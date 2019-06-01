using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class SplitContainerPanel : Panel
    {
        public SplitContainerPanel ()
        {
            var sc1 = Controls.Add (new SplitContainer { Dock = DockStyle.Fill, SplitterColor = SKColors.Black, Panel1MinimumSize = 125, Panel2MinimumSize = 125 });
            var sc2 = sc1.Panel2.Controls.Add (new SplitContainer { Dock = DockStyle.Fill, SplitterWidth = 8, SplitterColor = SKColors.Green, Orientation = Orientation.Vertical });

            sc1.Panel1.Style.BackgroundColor = SKColors.CornflowerBlue;
            sc2.Panel1.Style.BackgroundColor = SKColors.LightPink;

            sc1.Panel1.Controls.Add (new Button { Text = "Panel1", Left = 10, Top = 10 });
            sc2.Panel1.Controls.Add (new Button { Text = "Top Panel", Left = 10, Top = 10 });
            sc2.Panel2.Controls.Add (new Button { Text = "Bottom Panel", Left = 10, Top = 10 });
        }
    }
}
