using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class ListViewPanel : Panel
    {
        private static readonly Random random = new Random ();

        public ListViewPanel ()
        {
            // ListView
            var view = new ListView {
                Top = 10,
                Left = 10,
                Width = 700,
                Height = 200
            };

            for (var i = 0; i < 30; i++)
                view.Items.Add (new ListViewItem { Text = $"Item {i}", Image = GetImage () });

            Controls.Add (view);
        }

        private static SKBitmap GetImage ()
        {
            switch (random.Next () % 2) {
                case 0:
                    return ImageLoader.Get ("new.png");
                default:
                    return ImageLoader.Get ("button.png");
            }
        }
    }
}
