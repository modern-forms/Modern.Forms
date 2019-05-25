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
            AddVerticalScrollbar (40, 0, 100, 10, 1);
            AddVerticalScrollbar (140, 30, 40, 2, 1);
            AddVerticalScrollbar (240, -30, 0, 10, 5);
            AddVerticalScrollbar (340, 25, 50, 25, 5);
            AddVerticalScrollbar (440, 0, 100, 10, 1, 40);
            AddVerticalScrollbar (540, 0, 100, 10, 1, 39);
            AddVerticalScrollbar (640, 0, 100, 0, 0);

            AddHorizontalScrollbar (400, 0, 100, 10, 1);
            AddHorizontalScrollbar (440, 30, 40, 2, 1);
            AddHorizontalScrollbar (480, -30, 0, 10, 5);
            AddHorizontalScrollbar (520, 25, 50, 25, 5);
            AddHorizontalScrollbar (560, 0, 100, 10, 1, 40);
            AddHorizontalScrollbar (600, 0, 100, 10, 1, 39);
            AddHorizontalScrollbar (640, 0, 100, 0, 0);
        }

        private void AddVerticalScrollbar (int left, int minimum, int maximum, int largeChange, int smallChange, int size = 300)
        {
            var desc_label = new Label {
                Left = left - 30,
                Top = 10,
                Text = $"{minimum} - {maximum}, {smallChange}, {largeChange}"
            };

            Controls.Add (desc_label);

            var sb = new VerticalScrollBar {
                Left = left,
                Top = 30,
                Height = size,
                Minimum = minimum,
                Maximum = maximum,
                LargeChange = largeChange,
                SmallChange = smallChange
            };

            Controls.Add (sb);

            var sb_label = new Label {
                Left = left,
                Top = 340,
                Text = sb.Value.ToString ()
            };

            sb.ValueChanged += (o, e) => sb_label.Text = sb.Value.ToString ();

            Controls.Add (sb_label);
        }

        private void AddHorizontalScrollbar (int top, int minimum, int maximum, int largeChange, int smallChange, int size = 300)
        {
            var desc_label = new Label {
                Left = 10,
                Top = top,
                Text = $"{minimum} - {maximum}, {smallChange}, {largeChange}"
            };

            Controls.Add (desc_label);

            var sb = new HorizontalScrollBar {
                Left = 120,
                Top = top,
                Width = size,
                Minimum = minimum,
                Maximum = maximum,
                LargeChange = largeChange,
                SmallChange = smallChange
            };

            Controls.Add (sb);

            var sb_label = new Label {
                Left = 440,
                Top = top,
                Text = sb.Value.ToString ()
            };

            sb.ValueChanged += (o, e) => sb_label.Text = sb.Value.ToString ();

            Controls.Add (sb_label);
        }
    }
}
