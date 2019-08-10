using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class MenuPanel : Panel
    {
        public MenuPanel ()
        {
            var menu = new Menu ();

            var file = menu.Items.Add ("File");
            menu.Items.Add (new MenuSeparatorItem ());

            var noo = file.Items.Add ("New");
            var open = file.Items.Add ("Open", ImageLoader.Get ("folder.png"));
            var recent = file.Items.Add ("Open Recent", ImageLoader.Get ("folder.png"));

            var file1 = recent.Items.Add ("file1.txt");
            var file2 = recent.Items.Add ("file2.txt");

            var file2_1 = file2.Items.Add ("Open");
            var file2_2 = file2.Items.Add ("Delete");

            file.Items.Add (new MenuSeparatorItem ());

            var exit = file.Items.Add ("Exit");

            var edit = menu.Items.Add ("Edit");

            var copy = edit.Items.Add ("Copy", ImageLoader.Get ("copy.png"));
            var cut = edit.Items.Add ("Cut", ImageLoader.Get ("cut.png"));
            var paste = edit.Items.Add ("Paste", ImageLoader.Get ("paste.png"));

            Controls.Add (menu);

            var label = Controls.Add (new Label { Left = 250, Top = 10, Width = 200, Height = 50 });

            foreach (var item in menu.Items)
                HookUpEvent (item, label);
        }

        private void HookUpEvent (MenuItem item, Label label)
        {
            item.Click += (o, e) => label.Text = item.Text + " clicked";

            foreach (var child in item.Items)
                HookUpEvent (child, label);
        }
    }
}
