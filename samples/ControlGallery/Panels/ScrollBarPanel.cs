using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class ScrollBarPanel : Panel
    {
        public ScrollBarPanel ()
        {
            var v_scroll = new VerticalScrollBar {
                Left = 10,
                Top = 10,
                Height = 500
            };

            Controls.Add (v_scroll);

            var v_scroll_label = new Label {
                Left = 45,
                Top = 10
            };

            v_scroll.ValueChanged += (o, e) => v_scroll_label.Text = v_scroll.Value.ToString ();

            Controls.Add (v_scroll_label);

            var h_scroll = new HorizontalScrollBar {
                Left = 10,
                Top = 550,
                Width = 500
            };

            Controls.Add (h_scroll);

            var h_scroll_label = new Label {
                Left = 10,
                Top = 585
            };

            h_scroll.ValueChanged += (o, e) => h_scroll_label.Text = h_scroll.Value.ToString ();

            Controls.Add (h_scroll_label);
        }
    }
}
