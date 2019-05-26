using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ScrollableControlPanel : Panel
    {
        public ScrollableControlPanel ()
        {
            var sc = new ScrollableControl {
                Left = 100,
                Top = 100,
                Width = 200,
                Height = 200,
                AutoScroll = true
            };

            sc.Controls.AddRange (
                CreateButton ("1", 0, 0),
                CreateButton ("2", 100, 0),
                CreateButton ("3", 0, 100),
                CreateButton ("4", 100, 100),
                CreateButton ("5", 0, 200),
                CreateButton ("6", 100, 200),
                CreateButton ("7", 0, 300),
                CreateButton ("8", 100, 300));

            Controls.Add (sc);

            var sc2 = new ScrollableControl {
                Left = 350,
                Top = 100,
                Width = 225,
                Height = 200,
                AutoScroll = true
            };

            sc2.Controls.AddRange (
                CreateButton ("1", 0, 0),
                CreateButton ("2", 100, 0),
                CreateButton ("3", 0, 100),
                CreateButton ("4", 100, 100),
                CreateButton ("5", 0, 200),
                CreateButton ("6", 100, 200),
                CreateButton ("7", 0, 300),
                CreateButton ("8", 100, 300));

            Controls.Add (sc2);

            var sc3 = new ScrollableControl {
                Left = 100,
                Top = 350,
                Width = 225,
                Height = 200,
                AutoScroll = true
            };

            sc3.Controls.AddRange (
                CreateButton ("1", 0, 0),
                CreateButton ("2", 100, 0),
                CreateButton ("3", 200, 0),
                CreateButton ("4", 300, 0));

            Controls.Add (sc3);

            var sc4 = new ScrollableControl {
                Left = 350,
                Top = 350,
                Width = 225,
                Height = 200,
                AutoScroll = true
            };

            sc4.Controls.Add (CreateButton ("1", 0, 0));

            Controls.Add (sc4);
        }

        private Button CreateButton (string text, int x, int y)
        {
            return new Button {
                Left = x,
                Top = y,
                Width = 110,
                Height = 110,
                Text = text
            };
        }
    }
}
