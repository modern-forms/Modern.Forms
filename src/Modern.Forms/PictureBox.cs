using System;
using System.Drawing;
using System.Net.Http;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO: 
    // AutoSize
    // Async Loading      
    public class PictureBox : Control
    {
        private static HttpClient? client;

        private SKBitmap? image;
        private string? image_location;
        private bool is_error;
        private PictureBoxSizeMode size_mode;

        public PictureBox ()
        {
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        public event EventHandler? SizeModeChanged;

        protected override Size DefaultSize => new Size (100, 50);

        public SKBitmap? Image {
            get => image;
            set {
                if (image == value)
                    return;

                image = value;
                is_error = false;

                UpdateSize ();
            }
        }

        public string? ImageLocation {
            get => image_location;
            set => Load (value);
        }

        public void Load (string? url)
        {
            if (string.IsNullOrWhiteSpace (url))
                throw new InvalidOperationException ("ImageLocation not specified.");

            if (image_location == url)
                return;

            is_error = false;
            image_location = url;

            try {
                if (url.Contains ("://"))
                    Image = SKBitmap.Decode (Client.GetStreamAsync (url).Result);
                else
                    Image = SKBitmap.Decode (url);
            } catch (Exception) {
                is_error = true;
            }
        }

        public PictureBoxSizeMode SizeMode {
            get => size_mode;
            set {
                if (size_mode == value)
                    return;

                size_mode = value;

                //AutoSize = size_mode == PictureBoxSizeMode.AutoSize;
                SetAutoSizeMode (size_mode == PictureBoxSizeMode.AutoSize ? AutoSizeMode.GrowAndShrink : AutoSizeMode.GrowOnly);

                UpdateSize ();
            }
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            if (image != null) {
                var client = PaddedClientRectangle;

                switch (size_mode) {
                    case PictureBoxSizeMode.AutoSize:
                    case PictureBoxSizeMode.Normal:
                        e.Canvas.DrawBitmap (image, new SKRect (0, 0, image.Width, image.Height));
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        e.Canvas.DrawBitmap (image, client);
                        break;
                    case PictureBoxSizeMode.CenterImage:
                        e.Canvas.DrawBitmap (image, (client.Width / 2) - (image.Width / 2), (client.Height / 2) - (image.Height / 2));
                        break;
                    case PictureBoxSizeMode.Zoom:
                        Size image_size;

                        if (((float)image.Width / image.Height) >= ((float)client.Width / client.Height))
                            image_size = new Size (client.Width, (image.Height * client.Width) / image.Width);
                        else
                            image_size = new Size ((image.Width * client.Height) / image.Height, client.Height);

                        e.Canvas.DrawBitmap (image, SKRect.Create ((client.Width / 2) - (image_size.Width / 2), (client.Height / 2) - (image_size.Height / 2), image_size.Width, image_size.Height));
                        break;
                }
            } else if (is_error) {
                e.Canvas.DrawLine (0, 0, Width, Height, SKColors.Red, 2);
                e.Canvas.DrawLine (0, Height, Width, 0, SKColors.Red, 2);
            }
        }

        protected void OnSizeModeChanged (EventArgs e) => SizeModeChanged?.Invoke (this, e);

        private static HttpClient Client => client ??= new HttpClient ();

        private void UpdateSize ()
        {
            if (image == null)
                return;

            Parent?.PerformLayout (this, nameof (AutoSize));
        }
    }
}
