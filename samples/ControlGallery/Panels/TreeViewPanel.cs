using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Modern.Forms;

namespace ControlGallery.Panels
{
    public class TreeViewPanel : Panel
    {
        public TreeViewPanel ()
        {
            var tree = new TreeView {
                Left = 10,
                Top = 10
            };

            foreach (var drive in DriveInfo.GetDrives ().Where (d => d.IsReady))
                tree.Items.Add ($"{drive.Name.Trim ('\\')} - {drive.VolumeLabel}", ImageLoader.Get ("drive.png")).Tag = drive;

            Controls.Add (tree);
        }
    }
}
