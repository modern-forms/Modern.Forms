using System;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class RadioButtonPanel : Panel
    {
        public RadioButtonPanel ()
        {
            Controls.Add (new RadioButton { Text = "Option 1", Left = 10, Top = 10, Width = 150 });
            Controls.Add (new RadioButton { Text = "Option 2", Left = 10, Top = 45, Width = 150 });
            Controls.Add (new RadioButton { Text = "Disabled", Left = 10, Top = 80, Width = 150, Enabled = false, Checked = true });
            Controls.Add (new RadioButton { Text = "AutoCheck Off", Left = 10, Top = 115, Width = 150, AutoCheck = false });

            var panel = Controls.Add (new Panel { Left = 10, Top = 150, Width = 200, Height = 100 });
            panel.Style.Border.Width = 1;

            var light = panel.Controls.Add (new RadioButton { Text = "Light", Left = 10, Top = 10, Checked = true });
            var dark = panel.Controls.Add (new RadioButton { Text = "Dark", Left = 10, Top = 45 });

            light.CheckedChanged += (s, e) => {
                if (light.Checked)
                    Theme.SetBuiltInTheme (BuiltInTheme.Light);
            };

            dark.CheckedChanged += (s, e) => {
                if (dark.Checked)
                    Theme.SetBuiltInTheme (BuiltInTheme.Dark);
            };
        }
    }
}
