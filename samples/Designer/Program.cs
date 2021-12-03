using System;
using Modern.Forms;

namespace Designer
{
    public class Program
    {
     [STAThread]
       static void Main (string[] args)
        {
            //Application.Run (new MainForm ());
            Application.Run (new ModernFormDesigner ());
            //System.Windows.Forms.Application.Run (new WinForm ());
            //System.Windows.Forms.Application.Run (new WinFormDesigner ());
        }
    }
}
