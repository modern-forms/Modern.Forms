using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class PictureBoxPanel : Panel
    {
        public PictureBoxPanel ()
        {
            AddPictureBox (10, 10, PictureBoxSizeMode.Normal);
            AddPictureBox (10, 120, PictureBoxSizeMode.Normal, null, ImageLoader.Get ("swatches.png"));
            AddPictureBox (10, 230, PictureBoxSizeMode.Normal, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
            AddPictureBox (10, 340, PictureBoxSizeMode.Normal, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png2");
            AddPictureBox (10, 450, PictureBoxSizeMode.Normal, null, ImageLoader.Get ("swatches.png"), false);

            AddPictureBox (120, 10, PictureBoxSizeMode.CenterImage);
            AddPictureBox (120, 120, PictureBoxSizeMode.CenterImage, null, ImageLoader.Get ("swatches.png"));
            AddPictureBox (120, 230, PictureBoxSizeMode.CenterImage, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
            AddPictureBox (120, 340, PictureBoxSizeMode.CenterImage, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png2");
            AddPictureBox (120, 450, PictureBoxSizeMode.CenterImage, null, ImageLoader.Get ("swatches.png"), false);

            AddPictureBox (230, 10, PictureBoxSizeMode.StretchImage);
            AddPictureBox (230, 120, PictureBoxSizeMode.StretchImage, null, ImageLoader.Get ("swatches.png"));
            AddPictureBox (230, 230, PictureBoxSizeMode.StretchImage, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
            AddPictureBox (230, 340, PictureBoxSizeMode.StretchImage, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png2");
            AddPictureBox (230, 450, PictureBoxSizeMode.StretchImage, null, ImageLoader.Get ("swatches.png"), false);

            AddPictureBox (340, 10, PictureBoxSizeMode.Zoom);
            AddPictureBox (340, 120, PictureBoxSizeMode.Zoom, null, ImageLoader.Get ("swatches.png"));
            AddPictureBox (340, 230, PictureBoxSizeMode.Zoom, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
            AddPictureBox (340, 340, PictureBoxSizeMode.Zoom, "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png2");
            AddPictureBox (340, 450, PictureBoxSizeMode.Zoom, null, ImageLoader.Get ("swatches.png"), false);

            Controls.Add (new Label { Left = 10, Top = 5, Text = "Normal" });
            Controls.Add (new Label { Left = 120, Top = 5, Text = "CenterImage" });
            Controls.Add (new Label { Left = 230, Top = 5, Text = "StretchImage" });
            Controls.Add (new Label { Left = 340, Top = 5, Text = "Zoom" });
        }

        private void AddPictureBox (int left, int top, PictureBoxSizeMode mode, string? url = null, SKBitmap? image = null, bool enabled = true)
        {
            var pb = new PictureBox {
                Left = left,
                Top = top + 20,
                Height = 100,
                Width = 100,
                SizeMode = mode,
                Image = image,
                Enabled = enabled
            };

            if (!string.IsNullOrWhiteSpace (url))
                pb.ImageLocation = url;

            pb.Style.Border.Width = 1;

            Controls.Add (pb);
        }
    }
}
