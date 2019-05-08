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
        }
    }
}
