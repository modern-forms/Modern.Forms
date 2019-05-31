using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TabControlPanel : Panel
    {
        public TabControlPanel ()
        {
            var tc = new TabControl {
                Dock = DockStyle.Fill
            };

            Controls.Add (tc);

            var tp1 = tc.Pages.Add ("Tab X");
            var tp2 = tc.Pages.Add ("Tab Y");
            var tp3 = tc.Pages.Add ("Tab 3");

            tp1.Controls.Add (new Label { Text = "Tab X Contents!", Left = 3, Top = 3 });
            tp2.Controls.Add (new Label { Text = "Tab Y Contents!", Left = 3, Top = 3 });
            tp3.Controls.Add (new Label { Text = "Tab 3 Contents!", Left = 3, Top = 3 });

            var tp4 = new TabPage { Text = "Tab 1" };
            tp4.Controls.Add (new Label { Text = "Tab 1 Contents!", Left = 3, Top = 3 });
            tc.Pages.Insert (1, tp4);

            var tp5 = new TabPage { Text = "Tab 2" };
            tp5.Controls.Add (new Label { Text = "Tab 2 Contents!", Left = 3, Top = 3 });
            tc.Pages[2] = tp5;

            tc.Pages.RemoveAt (0);
        }
    }
}
