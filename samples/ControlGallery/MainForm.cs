using System;
using System.Collections.Generic;
using System.Text;
using ControlGallery.Panels;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery
{
    public class MainForm : Form
    {
        private Panel? current_panel;
        private readonly TreeView tree;

        public MainForm ()
        {
            tree = new TreeView {
                Dock = DockStyle.Left,
                ShowDropdownGlyph = false
            };

            tree.Style.Border.Width = 0;
            tree.Style.Border.Right.Width = 1;

            tree.Items.Add ("Button", ImageLoader.Get ("button.png"));
            tree.Items.Add ("CheckBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ComboBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Dialogs", ImageLoader.Get ("button.png"));
            tree.Items.Add ("FileDialogs", ImageLoader.Get ("button.png"));
            tree.Items.Add ("FlowLayoutPanel", ImageLoader.Get ("button.png"));
            tree.Items.Add ("FormPaint", ImageLoader.Get ("button.png"));
            tree.Items.Add ("FormShortcuts", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Label", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListView", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Menu", ImageLoader.Get ("button.png"));
            tree.Items.Add ("MessageBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("NavigationPane", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Panel", ImageLoader.Get ("button.png"));
            tree.Items.Add ("PictureBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ProgressBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("RadioButton", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Ribbon", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ScrollableControl", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ScrollBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("SplitContainer", ImageLoader.Get ("button.png"));
            tree.Items.Add ("StatusBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TabControl", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TableLayoutPanel", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TabStrip", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TextBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TitleBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ToolBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TreeView", ImageLoader.Get ("button.png"));

            tree.ItemSelected += Tree_ItemSelected;
            Controls.Add (tree);

            Text = "Control Gallery";
            Image = ImageLoader.Get ("button.png");
        }

        private void Tree_ItemSelected (object? sender, EventArgs<TreeViewItem> e)
        {
            if (current_panel != null) {
                Controls.Remove (current_panel);

                if (current_panel is BasePanel bp)
                    bp.UnloadPanel ();

                current_panel.Dispose ();
                current_panel = null;
            }

            var new_panel = CreatePanel (e.Value.Text);

            if (new_panel != null) {
                current_panel = new_panel;
                new_panel.Dock = DockStyle.Fill;
                Controls.Insert (0, new_panel);
            }
        }

        private Panel? CreatePanel (string text)
        {
            switch (text) {
                case "Button":
                    return new ButtonPanel ();
                case "CheckBox":
                    return new CheckBoxPanel ();
                case "ComboBox":
                    return new ComboBoxPanel ();
                case "Dialogs":
                    return new DialogPanel ();
                case "FileDialogs":
                    return new FileDialogPanel ();
                case "FlowLayoutPanel":
                    return new FlowLayoutPanelPanel ();
                case "FormShortcuts":
                    return new FormShortcutsPanel (this);
                case "Label":
                    return new LabelPanel ();
                case "ListBox":
                    return new ListBoxPanel ();
                case "ListView":
                    return new ListViewPanel ();
                case "Menu":
                    return new MenuPanel ();
                case "MessageBox":
                    return new MessageBoxPanel ();
                case "NavigationPane":
                    return new NavigationPanePanel ();
                case "Panel":
                    return new PanelPanel ();
                case "PictureBox":
                    return new PictureBoxPanel ();
                case "ProgressBar":
                    return new ProgressBarPanel ();
                case "RadioButton":
                    return new RadioButtonPanel ();
                case "Ribbon":
                    return new RibbonPanel ();
                case "ScrollableControl":
                    return new ScrollableControlPanel ();
                case "ScrollBar":
                    return new ScrollBarPanel ();
                case "SplitContainer":
                    return new SplitContainerPanel ();
                case "StatusBar":
                    return new StatusBarPanel ();
                case "TabControl":
                    return new TabControlPanel ();
                case "TableLayoutPanel":
                    return new TableLayoutPanelPanel ();
                case "TabStrip":
                    return new TabStripPanel ();
                case "TextBox":
                    return new TextBoxPanel ();
                case "TitleBar":
                    return new TitleBarPanel ();
                case "ToolBar":
                    return new ToolBarPanel ();
                case "TreeView":
                    return new TreeViewPanel ();
            }

            return null;
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            if (tree.SelectedItem.Text == "FormPaint") {
                e.Canvas.FillRectangle (Scale (300), Scale (50), Scale (100), Scale (100), SKColors.Red);

                DrawThemeColor (e.Canvas, Scale (450), Scale (50), Scale (150), Scale (40), Theme.BackgroundColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (90), Scale (150), Scale (40), Theme.ControlLowColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (130), Scale (150), Scale (40), Theme.ControlMidColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (170), Scale (150), Scale (40), Theme.ControlMidHighColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (210), Scale (150), Scale (40), Theme.ControlHighColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (250), Scale (150), Scale (40), Theme.ControlVeryHighColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (290), Scale (150), Scale (40), Theme.ControlHighlightLowColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (330), Scale (150), Scale (40), Theme.ControlHighlightMidColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (370), Scale (150), Scale (40), Theme.ControlHighlightHighColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (410), Scale (150), Scale (40), Theme.BorderLowColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (450), Scale (150), Scale (40), Theme.BorderMidColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (490), Scale (150), Scale (40), Theme.BorderHighColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (530), Scale (150), Scale (40), Theme.ForegroundColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (570), Scale (150), Scale (40), Theme.ForegroundDisabledColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (610), Scale (150), Scale (40), Theme.ForegroundColorOnAccent);
                DrawThemeColor (e.Canvas, Scale (450), Scale (650), Scale (150), Scale (40), Theme.AccentColor);
                DrawThemeColor (e.Canvas, Scale (450), Scale (690), Scale (150), Scale (40), Theme.AccentColor2);
                DrawThemeColor (e.Canvas, Scale (450), Scale (730), Scale (150), Scale (40), Theme.WarningHighlightColor);
            }
        }

        private static void DrawThemeColor (SKCanvas canvas, int x, int y, int width, int height, SKColor color)
        {
            canvas.FillRectangle (x, y, width, height, color);
            canvas.DrawText (color.ToString (), x + 10, y + 20, new SKPaint { Typeface = Theme.UIFont, Color = Theme.ForegroundColor });
        }

        protected override void OnPaintBackground (PaintEventArgs e)
        {
            base.OnPaintBackground (e);

            if (tree.SelectedItem.Text == "FormPaint")
                e.Canvas.Clear (SKColors.Green);
        }

        private int Scale (int value) => (int)(value * Scaling);
    }
}
