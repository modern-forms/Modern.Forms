using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Designer
{
    public partial class WinFormDesigner : Form
    {
        public WinFormDesigner ()
        {
            InitializeComponent ();
        }

        private void WinFormDesigner_Load (object sender, EventArgs e)
        {
            var designSurface = new DesignSurface (typeof (Form));
            var host = (IDesignerHost)designSurface.GetService (typeof (IDesignerHost));
            var root = (Form)host.RootComponent;
            TypeDescriptor.GetProperties (root)["Name"].SetValue (root, "Form1");
            root.Text = "Form1";

            var component = host.CreateComponent (typeof (Button), "button1");
            var button1 = (Button)component;
            button1.Text = "button1";
            button1.Location = new Point (8, 8);
            root.Controls.Add (button1);

            var timer1 = (Timer)host.CreateComponent (typeof (Timer), "timer1");
            timer1.Interval = 2000;
            var view = (Control)designSurface.View;
            view.Dock = DockStyle.Fill;
            view.BackColor = Color.White;
            this.Controls.Add (view);
        }
    }
}
