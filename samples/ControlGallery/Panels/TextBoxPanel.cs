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

            var tb4 = new TextBox {
                Left = 10,
                Top = 130,
                Width = 300,
                Height = 56,
                Text = "Not\nMultiline",
            };

            Controls.Add (tb4);

            var tb5 = new TextBox {
                Left = 10,
                Top = 200,
                Width = 300,
                Height = 100,
                Text = "The quick brown fox\njumped over the lazy\ndogs.",
                MultiLine = true
            };

            Controls.Add (tb5);

            var tb6 = new TextBox {
                Text = "With Padding",
                Left = 200,
                Top = 10,
                Width = 150,
                Padding = new Padding (5)
            };

            Controls.Add (tb6);
        }
    }
}
