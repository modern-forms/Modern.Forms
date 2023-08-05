using System;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ComboBoxPanel : Panel
    {
        public ComboBoxPanel ()
        {
            var fruits = new[] { "Apple", "Banana", "Carrot", "Donut", "Eggs", "Figs", "Grapes", "Hamburger and Cheeseburgers", "Ice Cream", "Jelly" };

            Controls.Add (new Label { Text = "DropDown", Left = 10, Top = 10, Width = 200 });
            var cb1 = Controls.Add (new ComboBox { Left = 10, Top = 35 });
            cb1.Items.AddRange (fruits);

            var button = Controls.Add (new Button { Text = "Open", Left = 140, Top = 35 });
            button.Click += (o, e) => cb1.DroppedDown = !cb1.DroppedDown;

            Controls.Add (new Label { Text = "Disabled", Left = 10, Top = 70, Width = 200 });
            var disabled = Controls.Add (new ComboBox { Left = 10, Top = 95, Enabled = false });
            disabled.Items.AddRange (fruits);
            disabled.SelectedIndex = 3;
        }
    }
}
