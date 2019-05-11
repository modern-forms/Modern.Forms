using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class RadioButtonPanel : Panel
    {
        public RadioButtonPanel ()
        {
            var rb1 = new RadioButton {
                Text = "Option 1",
                Left = 10,
                Top = 10,
                Checked = true
            };

            Controls.Add (rb1);

            var rb2 = new RadioButton {
                Text = "Option 2",
                Left = 10,
                Top = 45
            };

            Controls.Add (rb2);

            var panel = new Panel {
                Left = 10,
                Top = 100,
                Width = 200,
                Height = 100
            };

            panel.Style.Border.Width = 1;

            Controls.Add (panel);

            var rb3 = new RadioButton {
                Text = "Hot",
                Left = 10,
                Top = 10,
                Checked = true
            };

            panel.Controls.Add (rb3);

            var rb4 = new RadioButton {
                Text = "Cold",
                Left = 10,
                Top = 45
            };

            panel.Controls.Add (rb4);
        }
    }
}
