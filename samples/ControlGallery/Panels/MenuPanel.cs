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

            menu.Items.Add ("Disabled").Enabled = false;

            Controls.Add (menu);

            var label = Controls.Add (new Label { Left = 250, Top = 10, Width = 200, Height = 50 });

            foreach (var item in menu.Items)
                HookUpEvent (item, label);

            // Context menu
            var context_menu = new ContextMenu ();

            context_menu.Items.Add ("Copy", ImageLoader.Get ("copy.png"));
            context_menu.Items.Add ("Cut", ImageLoader.Get ("cut.png"));
            context_menu.Items.Add ("Paste", ImageLoader.Get ("paste.png"));
            context_menu.Items.Add (new MenuSeparatorItem ());
            var delete = context_menu.Items.Add ("Delete");
            delete.Items.Add ("Recycle");
            delete.Items.Add ("Delete");

            var context_button = Controls.Add (new Button { Left = 10, Top = 250, Width = 250, Text = "Right click for context menu", ContextMenu = context_menu });

            foreach (var item in context_menu.Items)
                HookUpEvent (item, context_button);
        }

        private void HookUpEvent (MenuItem item, Control control)
        {
            item.Click += (o, e) => control.Text = item.Text + " clicked";

            foreach (var child in item.Items)
                HookUpEvent (child, control);
        }
    }
}
