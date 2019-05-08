using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class CheckBoxPanel : Panel
    {
        public CheckBoxPanel ()
        {
            var cb1 = new CheckBox {
                Text = "Option 1",
                Left = 10,
                Top = 10,
                Checked = true
            };

            Controls.Add (cb1);

            var cb2 = new CheckBox {
                Text = "Option 2",
                Left = 10,
                Top = 45
            };

            Controls.Add (cb2);
        }
    }
}
