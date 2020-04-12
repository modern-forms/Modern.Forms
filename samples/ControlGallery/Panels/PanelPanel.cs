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
            var p1 = Controls.Add (new Panel { Left = 10, Top = 10, Height = 100, Width = 100 });
            p1.Style.Border.Width = 1;

            var p2 = Controls.Add (new Panel { Left = 10, Top = 120, Height = 100, Width = 100 });
            p2.Style.BackgroundColor = SKColors.CornflowerBlue;

            Controls.Add (new Label { Text = "Disabled", Left = 10, Top = 230 });
            var p3 = Controls.Add (new Panel { Left = 10, Top = 255, Height = 100, Width = 100, Enabled = false });
            p3.Style.Border.Width = 1;

            p3.Controls.Add (new CheckBox { Text = "Option 1", Left = 5, Top = 10, Width = 90, Checked = true });
            p3.Controls.Add (new RadioButton { Text = "Option 2", Left = 5, Top = 40, Width = 90, Checked = true });
        }
    }
}
