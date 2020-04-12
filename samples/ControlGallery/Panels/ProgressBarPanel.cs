using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ProgressBarPanel : Panel
    {
        public ProgressBarPanel ()
        {
            var pb = Controls.Add (new ProgressBar { Left = 10, Top = 10, Value = 35 });

            var button = Controls.Add (new Button { Text = "Increment", Left = 140, Top = 10 });
            button.Click += (o, e) => pb.Increment ();

            var button2 = Controls.Add (new Button { Text = "Decrement", Left = 260, Top = 10 });
            button2.Click += (o, e) => pb.Increment (-23);

            Controls.Add (new Label { Text = "Disabled", Left = 10, Top = 50 });
            Controls.Add (new ProgressBar { Left = 10, Top = 75, Value = 70, Enabled = false });
        }
    }
}
