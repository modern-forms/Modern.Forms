using System;
using System.Windows.Forms;

namespace Explorer
{
    class Program
    {
        [STAThread]
        public static void Main (string[] args)
        {
            Application.Run (new MainForm ());
        }
    }
}
