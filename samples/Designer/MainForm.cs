using System;
using System.Collections.Generic;
using System.Text;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Designer
{
    public class MainForm : Form
    {
        public MainForm ()
        {
            Size = new System.Drawing.Size (1200, 850);

            //var form = CreateDesignedForm ();
            var form = new Explore.MainForm ();

            //var designer = new FormDocumentDesigner (form);

            //var surface = Controls.Add (new DesignerFrame (designer) { Dock = DockStyle.Fill });

        }

        private Form CreateDesignedForm ()
        {
            var form = new Form { Size = new System.Drawing.Size (300, 300) };
            var button = new Button { Text = "Button1", Height = 80, Anchor = AnchorStyles.Bottom | AnchorStyles.Right  };
            button.Style.BackgroundColor = SKColors.Red;

            var checkbox = new CheckBox { Top = 130 };

            form.Controls.Add (button);
            form.Controls.Add (checkbox);

            return form;
        }
    }
}
