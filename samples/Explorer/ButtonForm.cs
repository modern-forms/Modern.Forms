using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Modern.Forms;
using SkiaSharp;

namespace Explore
{
    public class ButtonForm : ModernForm
    {
        public ButtonForm ()
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
            b2.Style.ForegroundColor = ModernTheme.RibbonColor;
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

            var cb1 = new CheckBox {
                Text = "Option 1",
                Left = 100,
                Top = 400,
                Checked = true
            };

            Controls.Add (cb1);

            var cb2 = new CheckBox {
                Text = "Option 2",
                Left = 225,
                Top = 400
            };

            Controls.Add (cb2);

            var rb1 = new RadioButton {
                Text = "Option 1",
                Left = 100,
                Top = 500,
                Checked = true
            };

            Controls.Add (rb1);

            var rb2 = new RadioButton {
                Text = "Option 2",
                Left = 225,
                Top = 500
            };

            Controls.Add (rb2);

            var tabstrip = new TabStrip {
                Left = 100,
                Top = 600,
                Width = 600
            };

            tabstrip.Tabs.Add (new TabStripItem { Text = "File", Selected = true });
            tabstrip.Tabs.Add (new TabStripItem { Text = "Share" });
            tabstrip.Tabs.Add (new TabStripItem { Text = "View" });

            Controls.Add (tabstrip);

            var v_scroll = new VerticalScrollBar {
                Left = 900,
                Top = 100,
                Height = 500
            };

            Controls.Add (v_scroll);

            var v_scroll_label = new Label {
                Left = 875,
                Top = 625
            };

            v_scroll.ValueChanged += (o, e) => v_scroll_label.Text = v_scroll.Value.ToString ();

            Controls.Add (v_scroll_label);

            var h_scroll = new HorizontalScrollBar {
                Left = 100,
                Top = 650,
                Width = 500
            };

            Controls.Add (h_scroll);

            var h_scroll_label = new Label {
                Left = 100,
                Top = 685
            };

            h_scroll.ValueChanged += (o, e) => h_scroll_label.Text = h_scroll.Value.ToString ();

            Controls.Add (h_scroll_label);

            var listbox = new ListBox {
                Left = 300,
                Top = 300
            };

            listbox.Items.Add (new TestBlah { Text = "A Option" });
            listbox.Items.Add (new TestBlah { Text = "B Option" });
            listbox.Items.Add (new TestBlah { Text = "D Option" });
            listbox.Items.Add (new TestBlah { Text = "C Option" });

            listbox.SelectedIndex = 2;
            listbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;

            Controls.Add (listbox);

            var titlebar = new ModernFormTitleBar {
                Text = "Button Style Sample"
            };

            Controls.Add (titlebar);
        }

        public class TestBlah
        {
            public string Text { get; set; }

            public override string ToString () => Text;
        }
    }
}
