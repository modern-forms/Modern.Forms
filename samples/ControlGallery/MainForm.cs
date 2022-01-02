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
        private TreeView tree;

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
            tree.Items.Add ("FileDialogs", ImageLoader.Get ("button.png"));
            tree.Items.Add ("FormPaint", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Label", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListView", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Menu", ImageLoader.Get ("button.png"));
            tree.Items.Add ("MessageBox", ImageLoader.Get ("button.png"));
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
                case "FileDialogs":
                    return new FileDialogPanel ();
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

            if (tree.SelectedItem.Text == "FormPaint")
                e.Canvas.FillRectangle (300, 50, 100, 100, SKColors.Red);
        }

        protected override void OnPaintBackground (PaintEventArgs e)
        {
            base.OnPaintBackground (e);

            if (tree.SelectedItem.Text == "FormPaint")
                e.Canvas.Clear (SKColors.Green);
        }
    }
}
