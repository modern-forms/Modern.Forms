using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;
using SkiaSharp;

namespace Outlaw
{
    internal class MessagePanePlaceholder : Panel
    {
        private readonly SKBitmap image;

        public MessagePanePlaceholder ()
        {
            image = ImageLoader.Get ("email.png");
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            var image_bounds = ClientRectangle.CenterSquare (e.LogicalToDeviceUnits (50));
            image_bounds.Y -= e.LogicalToDeviceUnits (40);

            e.Canvas.DrawBitmap (image, image_bounds);

            e.Canvas.DrawText ("Select an item to read", Theme.UIFont, e.LogicalToDeviceUnits (18), ClientRectangle, CustomTheme.GrayFont, ContentAlignment.MiddleCenter);
        }
    }
}
