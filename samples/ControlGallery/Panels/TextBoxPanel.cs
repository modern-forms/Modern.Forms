using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TextBoxPanel : Panel
    {
        public TextBoxPanel ()
        {
            var tb1 = new TextBox {
                Text = "Option 1",
                Left = 10,
                Top = 10,
                Width = 150,
                Placeholder = "Type Here"
            };

            Controls.Add (tb1);

            var tb2 = new TextBox {
                Left = 10,
                Top = 50,
                Width = 150,
                Placeholder = "Type Here"
            };

            Controls.Add (tb2);

            var tb3 = new TextBox {
                Left = 10,
                Top = 90,
                Width = 150,
                Text = "Read Only",
                ReadOnly = true
            };

            Controls.Add (tb3);
        }
    }
}
