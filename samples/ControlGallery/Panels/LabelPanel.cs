using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class LabelPanel : Panel
    {
        public LabelPanel ()
        {
            // 9 alignment labels
            var image1 = ImageLoader.Get ("button.png");
            var alignment_labels = CreateAlignmentLabels ();

            Controls.AddRange (alignment_labels.ToArray ());

            // Alignment label options
            var show_text_cb = Controls.Add (new CheckBox { Text = "Show Text", Top = 10, Left = 470, Checked = true });

            show_text_cb.CheckedChanged += (o, e) => {
                alignment_labels.ForEach (label => label.Text = show_text_cb.Checked ? label.TextAlign.ToString () : string.Empty);
            };

            var show_image_cb = Controls.Add (new CheckBox { Text = "Show Image", Top = 40, Left = 470 });

            show_image_cb.CheckedChanged += (o, e) => {
                alignment_labels.ForEach (label => label.Image = show_image_cb.Checked ? image1 : null);
            };

            var text_image_relation = Controls.Add (new ComboBox { Top = 70, Left = 470, Width = 150 });

            text_image_relation.Items.AddRange (Enum.GetNames<TextImageRelation> ());
            text_image_relation.SelectedIndex = 0;

            text_image_relation.SelectedIndexChanged += (o, e) => {
                var relation = Enum.Parse<TextImageRelation> (text_image_relation.SelectedItem?.ToString ()!);
                alignment_labels.ForEach (label => label.TextImageRelation = relation);
            };

            // Other examples
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

            Controls.Add (new Label { Text = "Image", Image = image1, Left = 10, Top = 310, Height = 35, ImageAlign = ContentAlignment.MiddleLeft, TextAlign = ContentAlignment.MiddleCenter });
            Controls.Add (new Label { Text = "Image", Image = image1, Left = 10, Top = 350, Height = 35, Enabled = false });
        }

        private List<Label> CreateAlignmentLabels ()
        {
            var labels = new List<Label> {
                CreateAlignmentLabel (ContentAlignment.TopLeft),
                CreateAlignmentLabel (ContentAlignment.TopCenter),
                CreateAlignmentLabel (ContentAlignment.TopRight),
                CreateAlignmentLabel (ContentAlignment.MiddleLeft),
                CreateAlignmentLabel (ContentAlignment.MiddleCenter),
                CreateAlignmentLabel (ContentAlignment.MiddleRight),
                CreateAlignmentLabel (ContentAlignment.BottomLeft),
                CreateAlignmentLabel (ContentAlignment.BottomCenter),
                CreateAlignmentLabel (ContentAlignment.BottomRight)
            };

            return labels;
        }

        private Label CreateAlignmentLabel (ContentAlignment alignment)
        {
            var label = new Label {
                Text = alignment.ToString (),
                Left = GetAlignmentLeft (alignment),
                Top = GetAlignmentTop (alignment),
                Height = 35,
                Width = 150,
                TextAlign = alignment,
                ImageAlign = alignment,
                Padding = new Padding (3)
            };

            label.Style.Border.Width = 1;

            return label;
        }

        private static int GetAlignmentLeft (ContentAlignment alignment)
        {
            return alignment switch {
                ContentAlignment.TopLeft => 10,
                ContentAlignment.MiddleLeft => 10,
                ContentAlignment.BottomLeft => 10,
                ContentAlignment.TopCenter => 160,
                ContentAlignment.MiddleCenter => 160,
                ContentAlignment.BottomCenter => 160,
                ContentAlignment.TopRight => 310,
                ContentAlignment.MiddleRight => 310,
                ContentAlignment.BottomRight => 310,
                _ => throw new NotImplementedException ()
            };
        }

        private static int GetAlignmentTop (ContentAlignment alignment)
        {
            return alignment switch {
                ContentAlignment.TopLeft => 10,
                ContentAlignment.TopCenter => 10,
                ContentAlignment.TopRight => 10,
                ContentAlignment.MiddleLeft => 45,
                ContentAlignment.MiddleCenter => 45,
                ContentAlignment.MiddleRight => 45,
                ContentAlignment.BottomLeft => 80,
                ContentAlignment.BottomCenter => 80,
                ContentAlignment.BottomRight => 80,
                _ => throw new NotImplementedException ()
            };
        }
    }
}
