using System;
using System.Drawing;
using System.Windows.Forms;
using Modern.Forms;

namespace Explore
{
    class Program
    {
        [STAThread]
        public static void Main (string[] args)
        {
            Application.Run (new MainAvaloniaForm ());
        }
    }
}
