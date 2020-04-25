using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms.Renderers
{
    public class PictureBoxRenderer : Renderer<PictureBox>
    {
        protected override void Render (PictureBox control, PaintEventArgs e)
        {
            if (control.Image != null) {
                var client = control.PaddedClientRectangle;

                switch (control.SizeMode) {
                    //case PictureBoxSizeMode.AutoSize:
                    case PictureBoxSizeMode.Normal:
                        e.Canvas.DrawBitmap (control.Image, new Rectangle (0, 0, control.Image.Width, control.Image.Height), !control.Enabled);
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        e.Canvas.DrawBitmap (control.Image, client, !control.Enabled);
                        break;
                    case PictureBoxSizeMode.CenterImage:
                        e.Canvas.DrawBitmap (control.Image, (client.Width / 2) - (control.Image.Width / 2), (client.Height / 2) - (control.Image.Height / 2), !control.Enabled);
                        break;
                    case PictureBoxSizeMode.Zoom:
                        Size image_size;

                        if (((float)control.Image.Width / control.Image.Height) >= ((float)client.Width / client.Height))
                            image_size = new Size (client.Width, (control.Image.Height * client.Width) / control.Image.Width);
                        else
                            image_size = new Size ((control.Image.Width * client.Height) / control.Image.Height, client.Height);

                        e.Canvas.DrawBitmap (control.Image, new Rectangle ((client.Width / 2) - (image_size.Width / 2), (client.Height / 2) - (image_size.Height / 2), image_size.Width, image_size.Height), !control.Enabled);
                        break;
                }
            } else if (control.IsErrored) {
                e.Canvas.DrawLine (0, 0, control.Width, control.Height, SKColors.Red, 2);
                e.Canvas.DrawLine (0, control.Height, control.Width, 0, SKColors.Red, 2);
            }
        }
    }
}
