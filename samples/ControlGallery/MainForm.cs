using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ControlGallery.Panels;
using Modern.Forms;
using Panel = Modern.Forms.Panel;

namespace ControlGallery
{
    public class MainForm : AvaloniaForm
    {
        private Panel current_panel;

        public MainForm ()
        {
            var tree = new TreeView {
                Dock = DockStyle.Left
            };

            tree.Items.Add ("Button", ImageLoader.Get ("button.png"));
            tree.Items.Add ("CheckBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Label", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ListView", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Panel", ImageLoader.Get ("button.png"));
            tree.Items.Add ("RadioButton", ImageLoader.Get ("button.png"));
            tree.Items.Add ("Ribbon", ImageLoader.Get ("button.png"));
            tree.Items.Add ("ScrollBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("StatusBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TabStrip", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TextBox", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TitleBar", ImageLoader.Get ("button.png"));
            tree.Items.Add ("TreeView", ImageLoader.Get ("button.png"));

            tree.ItemSelected += Tree_ItemSelected;
            Controls.Add (tree);


            // TitleBar
            var titlebar = new ModernFormTitleBar {
                Text = "Control Gallery",
                Image = ImageLoader.Get ("button.png")
            };

            Controls.Add (titlebar);
        }

        private void Tree_ItemSelected (object sender, EventArgs<TreeViewItem> e)
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

        private Panel CreatePanel (string text)
        {
            switch (text) {
                case "Button":
                    return new ButtonPanel ();
                case "CheckBox":
                    return new CheckBoxPanel ();
                case "Label":
                    return new LabelPanel ();
                case "ListBox":
                    return new ListBoxPanel ();
                case "ListView":
                    return new ListViewPanel ();
                case "Panel":
                    return new PanelPanel ();
                case "RadioButton":
                    return new RadioButtonPanel ();
                case "Ribbon":
                    return new RibbonPanel ();
                case "ScrollBar":
                    return new ScrollBarPanel ();
                case "StatusBar":
                    return new StatusBarPanel ();
                case "TabStrip":
                    return new TabStripPanel ();
                case "TextBox":
                    return new TextBoxPanel ();
                case "TitleBar":
                    return new TitleBarPanel ();
                case "TreeView":
                    return new TreeViewPanel ();
            }

            return null;
        }
    }
}
