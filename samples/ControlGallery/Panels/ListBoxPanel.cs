using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ListBoxPanel : Panel
    {
        public ListBoxPanel ()
        {
            Controls.Add (new Label { Text = "None", Left = 10, Top = 10 });

            var listbox1 = new ListBox {
                Left = 10,
                Top = 30
            };

            listbox1.Items.Add (new TestBlah { Text = "A Option" });
            listbox1.Items.Add (new TestBlah { Text = "B Option" });
            listbox1.Items.Add (new TestBlah { Text = "D Option" });
            listbox1.Items.Add (new TestBlah { Text = "C Option" });

            listbox1.SelectionMode = SelectionMode.None;

            Controls.Add (listbox1);

            Controls.Add (new Label { Text = "One", Left = 10, Top = 140 });

            var listbox3 = new ListBox {
                Left = 10,
                Top = 160
            };

            listbox3.Items.Add ("A Option");
            listbox3.Items.Add ("B Option");
            listbox3.Items.Add ("D Option");
            listbox3.Items.Add ("C Option");

            listbox3.SelectedIndex = 1;
            listbox3.SelectionMode = SelectionMode.One;

            Controls.Add (listbox3);

            Controls.Add (new Label { Text = "MultiSimple", Left = 10, Top = 270 });

            var listbox2 = new ListBox {
                Left = 10,
                Top = 290
            };

            listbox2.Items.Add ("A Option");
            listbox2.Items.Add ("B Option");
            listbox2.Items.Add ("D Option");
            listbox2.Items.Add ("C Option");

            listbox2.SelectionMode = SelectionMode.MultiSimple;

            Controls.Add (listbox2);

            Controls.Add (new Label { Text = "MultiExtended", Left = 10, Top = 400 });

            var listbox4 = new ListBox {
                Left = 10,
                Top = 420
            };

            listbox4.Items.Add (new TestBlah { Text = "A Option" });
            listbox4.Items.Add (new TestBlah { Text = "B Option" });
            listbox4.Items.Add (new TestBlah { Text = "D Option" });
            listbox4.Items.Add (new TestBlah { Text = "C Option" });

            listbox4.SelectedIndex = 2;
            listbox4.SelectionMode = SelectionMode.MultiExtended;

            Controls.Add (listbox4);
        }

        public class TestBlah
        {
            public string Text { get; set; }

            public override string ToString () => Text;
        }
    }
}
