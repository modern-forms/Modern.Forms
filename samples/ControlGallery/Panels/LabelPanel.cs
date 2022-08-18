using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class LabelPanel : Panel
    {
        public LabelPanel ()
        {
            var lbl1 = new Label {
                Text = "TopLeft",
                Left = 10,
                Top = 10,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.TopLeft
            };

            lbl1.Style.Border.Width = 1;
            lbl1.Style.ForegroundColor = SKColors.Red;

            Controls.Add (lbl1);

            var lbl2 = new Label {
                Text = "MiddleLeft",
                Left = 10,
                Top = 45,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lbl2.Style.Border.Width = 1;

            Controls.Add (lbl2);

            var lbl3 = new Label {
                Text = "BottomLeft",
                Left = 10,
                Top = 80,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.BottomLeft
            };

            lbl3.Style.Border.Width = 1;

            Controls.Add (lbl3);

            var lbl4 = new Label {
                Text = "TopCenter",
                Left = 160,
                Top = 10,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.TopCenter
            };

            lbl4.Style.Border.Width = 1;

            Controls.Add (lbl4);

            var lbl5 = new Label {
                Text = "MiddleCenter",
                Left = 160,
                Top = 45,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lbl5.Style.Border.Width = 1;

            Controls.Add (lbl5);

            var lbl6 = new Label {
                Text = "BottomCenter",
                Left = 160,
                Top = 80,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.BottomCenter
            };

            lbl6.Style.Border.Width = 1;

            Controls.Add (lbl6);

            var lbl7 = new Label {
                Text = "TopRight",
                Left = 310,
                Top = 10,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.TopRight
            };

            lbl7.Style.Border.Width = 1;

            Controls.Add (lbl7);

            var lbl8 = new Label {
                Text = "MiddleRight",
                Left = 310,
                Top = 45,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.MiddleRight
            };

            lbl8.Style.Border.Width = 1;

            Controls.Add (lbl8);

            var lbl9 = new Label {
                Text = "BottomRight",
                Left = 310,
                Top = 80,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.BottomRight
            };

            lbl9.Style.Border.Width = 1;

            Controls.Add (lbl9);

            var lbl10 = new Label {
                Text = "Border",
                Left = 10,
                Top = 150,
                Height = 35,
                Width = 150,
                TextAlign = ContentAlignment.TopLeft
            };

            lbl10.Style.Border.Width = 1;
            lbl10.Style.Border.Top.Width = 5;
            lbl10.Style.Border.Left.Width = 5;

            Controls.Add (lbl10);

            Controls.Add (new Label { Text = "This text is too long to fit", Left = 10, Top = 190, Height = 35 });
            Controls.Add (new Label { Text = "This text is too long to fit", Left = 10, Top = 230, Height = 35, AutoEllipsis = true });
            Controls.Add (new Label { Text = "Disabled", Left = 10, Top = 270, Height = 35, Enabled = false });

            Controls.Add (new Label { Text = "This text is too long to fit on two lines", Left = 160, Top = 190, Height = 45, Multiline = true });
            Controls.Add (new Label { Text = "This text is too long to fit on two lines", Left = 160, Top = 250, Height = 45, Multiline = true, AutoEllipsis = true });
        }
    }
}
