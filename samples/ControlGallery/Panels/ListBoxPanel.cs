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
            var fruits = new[] { "Apple", "Banana", "Carrot", "Donut", "Eggs", "Figs", "Grapes", "Hamburger and Cheeseburgers", "Ice Cream", "Jelly" };

            Controls.Add (new Label { Text = "None", Left = 10, Top = 10 });

            var listbox1 = new ListBox {
                Left = 10,
                Top = 30
            };

            listbox1.Items.Add (new TestBlah ("A Option"));
            listbox1.Items.Add (new TestBlah ("B Option"));
            listbox1.Items.Add (new TestBlah ("D Option"));
            listbox1.Items.Add (new TestBlah ("C Option"));
            listbox1.Items.Add (new TestBlah ("E Option"));
            listbox1.Items.Add (new TestBlah ("F Option"));

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
            listbox3.Items.Add ("E Option");
            listbox3.Items.Add ("F Option");

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
            listbox2.Items.Add ("E Option");
            listbox2.Items.Add ("F Option");

            listbox2.SelectionMode = SelectionMode.MultiSimple;

            Controls.Add (listbox2);

            Controls.Add (new Label { Text = "MultiExtended", Left = 10, Top = 400 });

            var listbox4 = new ListBox {
                Left = 10,
                Top = 420
            };

            listbox4.Items.Add (new TestBlah ("Apple"));
            listbox4.Items.Add (new TestBlah ("Banana"));
            listbox4.Items.Add (new TestBlah ("Cactus"));
            listbox4.Items.Add (new TestBlah ("Cherry"));
            listbox4.Items.Add (new TestBlah ("Cranberry"));
            listbox4.Items.Add (new TestBlah ("Donut"));

            listbox4.SelectedIndex = 2;
            listbox4.SelectionMode = SelectionMode.MultiExtended;

            Controls.Add (listbox4);

            var lb5 = new ListBox {
                Left = 200,
                Top = 30,
                SelectionMode = SelectionMode.MultiExtended
            };

            lb5.Items.AddRange (fruits);

            Controls.Add (lb5);

            var b1 = new Button { Left = 350, Top = 30, Width = 120, Text = "Set Donut Top" };
            b1.Click += (o, e) => lb5.FirstVisibleIndex = 3;
            Controls.Add (b1);

            var b2 = new Button { Left = 350, Top = 70, Width = 120, Text = "Set Ice Cream Top" };
            b2.Click += (o, e) => lb5.FirstVisibleIndex = 8;
            Controls.Add (b2);

            Controls.Add (new Label { Text = "ScrollbarAlwaysVisible", Left = 200, Top = 135, Width = 200 });
            Controls.Add (new ListBox { Left = 200, Top = 160, ScrollbarAlwaysVisible = true }).Items.AddRange ("Apple", "Banana");

            Controls.Add (new Label { Text = "ShowHover", Left = 200, Top = 265, Width = 200 });
            Controls.Add (new ListBox { Left = 200, Top = 290, ShowHover = true }).Items.AddRange (fruits);

            Controls.Add (new Label { Text = "Disabled", Left = 200, Top = 395, Width = 200 });
            Controls.Add (new ListBox { Left = 200, Top = 420, Enabled = false }).Items.AddRange (fruits);

            Controls.Add (new Label { Text = "No Items", Left = 200, Top = 525, Width = 200 });
            Controls.Add (new ListBox { Left = 200, Top = 550 });
        }

        public class TestBlah
        {
            public string Text { get; set; }

            public TestBlah (string text)
            {
                Text = text;
            }

            public override string ToString () => Text;
        }
    }
}
