using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    public class ButtonPanel : Panel
    {
        public ButtonPanel ()
        {
            var b = new Button {
                Text = "OK",
                Left = 100,
                Top = 100,
                Width = 100,
                Height = 30
            };

            Controls.Add (b);

            var b2 = new Button {
                Text = "OK",
                Left = 225,
                Top = 100
            };

            b2.Style.BackgroundColor = SKColors.White;
            b2.Style.ForegroundColor = Theme.RibbonColor;
            b2.Style.Border.Width = 3;
            b2.Style.Border.Color = SKColors.Red;

            Controls.Add (b2);

            var b3 = new Button {
                Text = "OK",
                Left = 350,
                Top = 100
            };

            b3.Style.BackgroundColor = new SKColor (243, 246, 248);
            b3.Style.Border.Color = new SKColor (204, 206, 208);
            b3.Style.ForegroundColor = new SKColor (36, 41, 46);
            b3.Style.Border.Radius = 3;
            b3.Style.Border.Width = 1;

            b3.StyleHover.BackgroundColor = new SKColor (230, 235, 241);
            b3.StyleHover.Border.Color = new SKColor (165, 168, 172);
            b3.StyleHover.ForegroundColor = new SKColor (36, 41, 46);
            b3.StyleHover.Border.Radius = 3;

            Controls.Add (b3);

            var b4 = new Button {
                Text = "OK",
                Left = 475,
                Top = 100
            };

            b4.Style.BackgroundColor = new SKColor (40, 167, 69);
            b4.Style.Border.Color = new SKColor (46, 172, 77);
            b4.Style.ForegroundColor = new SKColor (255, 255, 255);
            b4.Style.Border.Radius = 3;
            b4.Style.Border.Width = 1;

            b4.StyleHover.BackgroundColor = new SKColor (38, 159, 66);
            b4.StyleHover.Border.Color = new SKColor (36, 115, 58);
            b4.StyleHover.ForegroundColor = new SKColor (255, 255, 255);
            b4.StyleHover.Border.Radius = 3;

            Controls.Add (b4);

            var b5 = new Button {
                Text = "OK",
                Left = 100,
                Top = 200
            };

            b5.Style.Border.Width = 1;
            b5.Style.BackgroundColor = new SKColor (243, 246, 248);
            b5.Style.ForegroundColor = new SKColor (36, 41, 46);
            b5.TextAlign = ContentAlignment.MiddleLeft;
            b5.Padding = new Padding (15, 0, 0, 0);

            Controls.Add (b5);

            var b6 = new Button {
                Text = "OK",
                Left = 225,
                Top = 200
            };

            b6.Style.Border.Width = 2;
            b6.Style.BackgroundColor = new SKColor (243, 246, 248);
            b6.Style.ForegroundColor = new SKColor (36, 41, 46);
            b6.TextAlign = ContentAlignment.TopLeft;

            Controls.Add (b6);

            var b7 = new Button {
                Text = "OK",
                Left = 350,
                Top = 200
            };

            b7.Style.Border.Width = 3;
            b7.Style.BackgroundColor = new SKColor (243, 246, 248);
            b7.Style.ForegroundColor = new SKColor (36, 41, 46);
            b7.TextAlign = ContentAlignment.TopRight;

            Controls.Add (b7);

            var b8 = new Button {
                Text = "OK",
                Left = 475,
                Top = 200
            };

            b8.Style.Border.Width = 4;
            b8.Style.BackgroundColor = new SKColor (243, 246, 248);
            b8.Style.ForegroundColor = new SKColor (36, 41, 46);
            b8.TextAlign = ContentAlignment.BottomLeft;

            Controls.Add (b8);

            var b9 = new Button {
                Text = "OK",
                Left = 600,
                Top = 200
            };

            b9.Style.Border.Width = 5;
            b9.Style.BackgroundColor = new SKColor (243, 246, 248);
            b9.Style.ForegroundColor = new SKColor (36, 41, 46);
            b9.TextAlign = ContentAlignment.BottomRight;

            Controls.Add (b9);

            var b10 = new Button {
                Text = "OK",
                Left = 725,
                Top = 200
            };

            b10.Style.Border.Width = 6;
            b10.Style.BackgroundColor = new SKColor (243, 246, 248);
            b10.Style.ForegroundColor = new SKColor (36, 41, 46);
            b10.TextAlign = ContentAlignment.BottomCenter;

            Controls.Add (b10);

            var b11 = new Button {
                Text = "OK",
                Left = 100,
                Top = 300
            };

            b11.Style.Border.Top.Width = 0;

            b11.Style.Border.Left.Width = 2;
            b11.Style.Border.Left.Color = SKColors.Green;
            b11.Style.Border.Right.Width = 3;
            b11.Style.Border.Right.Color = SKColors.Orange;
            b11.Style.Border.Bottom.Width = 3;
            b11.Style.Border.Bottom.Color = SKColors.Black;

            b11.Style.BackgroundColor = new SKColor (243, 246, 248);
            b11.Style.ForegroundColor = new SKColor (36, 41, 46);

            Controls.Add (b11);

            Controls.Add (new Button { Text = "Disabled", Left = 225, Top = 300, Enabled = false });
        }
    }
}
