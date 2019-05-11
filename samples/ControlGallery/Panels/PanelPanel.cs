using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class PanelPanel : Panel
    {
        public PanelPanel ()
        {
            var p1 = new Panel {
                Left = 10,
                Top = 10,
                Height = 100,
                Width = 100
            };

            p1.Style.Border.Width = 1;

            Controls.Add (p1);

            var p2 = new Panel {
                Left = 10,
                Top = 120,
                Height = 100,
                Width = 100
            };

            p2.Style.BackgroundColor = SKColors.CornflowerBlue;

            Controls.Add (p2);
        }
    }
}
