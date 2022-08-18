﻿using System;
using System.IO;
using System.Linq;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TreeViewPanel : Panel
    {
        private int max_dirs = 5;

        public TreeViewPanel ()
        {
            var tree = new TreeView {
                Left = 10,
                Top = 10
            };

            foreach (var drive in DriveInfo.GetDrives ().Where (d => d.IsReady)) {
                var tvi = CreateDirectoryNode (drive.Name, 1);

                tvi.Text = $"{drive.Name.Trim ('\\')} - {drive.VolumeLabel}";
                tvi.Image = ImageLoader.Get ("drive.png");
                tvi.Tag = drive;
                tvi.Expanded = true;

                tree.Items.Add (tvi);
            }

            Controls.Add (tree);

            var show_dropdowns = Controls.Add (new CheckBox {
                Text = "Show Dropdown Glyph",
                Checked = true,
                Left = 550,
                Top = 10,
                Width = 200
            });

            show_dropdowns.CheckedChanged += (o, e) => tree.ShowDropdownGlyph = show_dropdowns.Checked;

            var show_images = Controls.Add (new CheckBox {
                Text = "Show Images",
                Checked = true,
                Left = 550,
                Top = 40,
                Width = 200
            });

            show_images.CheckedChanged += (o, e) => tree.ShowItemImages = show_images.Checked;

            var enabled = Controls.Add (new CheckBox { Text = "Enabled", Left = 550, Top = 70, Width = 200, Checked = true });
            enabled.CheckedChanged += (o, e) => tree.Enabled = enabled.Checked;

            var virtual_mode = Controls.Add (new CheckBox {
                Text = "Virtual Mode",
                Left = 550,
                Top = 100,
                Width = 200
            });

            virtual_mode.CheckedChanged += (o, e) => tree.VirtualMode = virtual_mode.Checked;
        }

        private TreeViewItem CreateDirectoryNode (string path, int level)
        {
            var tvi = new TreeViewItem (Path.GetFileName (path)) { Image = ImageLoader.Get ("folder.png") };

            if (level > 3)
                return tvi;

            try {
                foreach (var dir in Directory.EnumerateDirectories (path).Take (max_dirs))
                    tvi.Items.Add (CreateDirectoryNode (dir, level + 1));
            } catch (Exception) {

            }

            return tvi;
        }
    }
}
