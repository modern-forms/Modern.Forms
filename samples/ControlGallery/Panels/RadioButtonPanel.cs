using System;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class RadioButtonPanel : Panel
    {
        private static SKBitmap _swatches = ImageLoader.Get ("swatches.png");

        public RadioButtonPanel ()
        {
            Controls.Add (new RadioButton { Text = "Option 1", Left = 10, Top = 10, Width = 150 });
            Controls.Add (new RadioButton { Text = "Option 2", Left = 10, Top = 45, Width = 150 });
            Controls.Add (new RadioButton { Text = "Disabled", Left = 10, Top = 80, Width = 150, Enabled = false, Checked = true });
            Controls.Add (new RadioButton { Text = "AutoCheck Off", Left = 10, Top = 115, Width = 150, AutoCheck = false });
            Controls.Add (new RadioButton { Text = "Text that is too long to fit", Left = 10, Top = 150, Width = 150, AutoEllipsis = true });

            var panel = Controls.Add (new Panel { Left = 10, Top = 185, Width = 200, Height = 100 });
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

            AddConfigurableRadioButton ();
        }

        private void AddConfigurableRadioButton ()
        {
            var test_cb = Controls.Add (new RadioButton {
                Text = "Test RadioButton",
                Image = _swatches,
                Left = 300,
                Top = 10,
                Width = 200,
                Height = 70
            });

            test_cb.Style.Border.Width = 1;

            var show_image_cb = Controls.Add (new CheckBox {
                Text = "Show Image",
                Top = 10,
                Left = 525,
                Checked = true
            });

            show_image_cb.CheckedChanged += (o, e) => {
                test_cb.Image = show_image_cb.Checked ? _swatches : null;
            };

            var show_text_cb = Controls.Add (new CheckBox {
                Text = "Show Text",
                Top = 40,
                Left = 525,
                Checked = true
            });

            show_text_cb.CheckedChanged += (o, e) => {
                test_cb.Text = show_text_cb.Checked ? "Test CheckBox" : string.Empty;
            };

            var text_image_relation = Controls.Add (new ComboBox {
                Top = 70,
                Left = 525,
                Width = 150
            });

            text_image_relation.Items.AddRange (Enum.GetNames<TextImageRelation> ());
            text_image_relation.SelectedIndex = 3;

            text_image_relation.SelectedIndexChanged += (o, e) => {
                var relation = Enum.Parse<TextImageRelation> (text_image_relation.SelectedItem?.ToString ()!);
                test_cb.TextImageRelation = relation;
            };

            var text_align = CreateAlignmentComboBox ("TextAlign", 105, 525);

            text_align.SelectedIndexChanged += (o, e) => {
                var align = Enum.Parse<ContentAlignment> (text_align.SelectedItem?.ToString ()!);
                test_cb.TextAlign = align;
            };

            var image_align = CreateAlignmentComboBox ("ImageAlign", 165, 525);

            image_align.SelectedIndexChanged += (o, e) => {
                var align = Enum.Parse<ContentAlignment> (image_align.SelectedItem?.ToString ()!);
                test_cb.ImageAlign = align;
            };

            var check_align = CreateAlignmentComboBox ("GlyphAlign", 225, 525);

            check_align.SelectedIndexChanged += (o, e) => {
                var align = Enum.Parse<ContentAlignment> (check_align.SelectedItem?.ToString ()!);
                test_cb.GlyphAlign = align;
            };
        }

        private ComboBox CreateAlignmentComboBox (string text, int top, int left)
        {
            var label = Controls.Add (new Label {
                Text = text,
                Top = top,
                Left = left
            });

            var combo = Controls.Add (new ComboBox {
                Top = label.Bottom,
                Left = left,
                Width = 150
            });

            combo.Items.AddRange (Enum.GetNames<ContentAlignment> ());
            combo.SelectedIndex = 3;

            return combo;
        }
    }
}
