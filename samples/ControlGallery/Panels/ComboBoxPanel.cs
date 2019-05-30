using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ComboBoxPanel : Panel
    {
        public ComboBoxPanel ()
        {
            var cb1 = new ComboBox {
                Left = 10,
                Top = 10
            };

            cb1.Items.AddRange ("Apple", "Banana", "Carrot", "Donut", "Eggs", "Figs", "Grapes", "Hamburger and Cheeseburgers", "Ice Cream", "Jelly");

            Controls.Add (cb1);
        }
    }
}
