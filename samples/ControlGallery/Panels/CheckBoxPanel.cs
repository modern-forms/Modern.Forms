using Modern.Forms;

namespace ControlGallery.Panels
{
    public class CheckBoxPanel : Panel
    {
        public CheckBoxPanel ()
        {
            Controls.Add (new CheckBox {
                Text = "Option 1",
                Left = 10,
                Top = 10,
                Width = 200,
                Checked = true
            });

            Controls.Add (new CheckBox {
                Text = "Option 2",
                Left = 10,
                Top = 45,
                Width = 200
            });

            Controls.Add (new CheckBox {
                Text = "ThreeState",
                Left = 10,
                Top = 80,
                Width = 200,
                ThreeState = true
            });

            Controls.Add (new CheckBox {
                Text = "Indeterminate by Code Only",
                Left = 10,
                Top = 115,
                Width = 200,
                CheckState = CheckState.Indeterminate
            });

            Controls.Add (new CheckBox {
                Text = "Disabled",
                Left = 10,
                Top = 150,
                Width = 200,
                Checked = true,
                Enabled = false
            });

            Controls.Add (new CheckBox {
                Text = "AutoCheck False",
                Left = 10,
                Top = 185,
                Width = 200,
                AutoCheck = false
            });
        }
    }
}
