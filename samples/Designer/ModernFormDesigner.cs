using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Designer
{
    public class ModernFormDesigner : Form
    {
        public ModernFormDesigner ()
        {
            Size = new System.Drawing.Size (1200, 850);

            Shown += ModernFormDesigner_Shown;
        }

        private void ModernFormDesigner_Shown (object sender, EventArgs e)
        {
            // Designed form
            var design_surface = new Modern.Forms.Design.DesignSurface (typeof (Form));
            var host = (IDesignerHost)design_surface.GetService (typeof (IDesignerHost));
            var designed_form = (Form)host.RootComponent;

            //TypeDescriptor.GetProperties (designed_form)["Size"].SetValue (designed_form, new System.Drawing.Size (800, 600));
            designed_form.Text = "Test Form";

            var button = new Button { Text = "Button1", Height = 80, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            button.Style.BackgroundColor = SKColors.Red;

            var checkbox = new CheckBox { Top = 130 };

            designed_form.Controls.Add (button);
            designed_form.Controls.Add (checkbox);

            // Design frame
            var design_frame = (Control)design_surface.View;
            design_frame.Dock = DockStyle.Fill;
            Controls.Add (design_frame);

        }
    }
}
