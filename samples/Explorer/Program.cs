using System;
using System.Drawing;
using System.Windows.Forms;
using Modern.Forms;

namespace Explorer
{
    class Program
    {
        [STAThread]
        public static void Main (string[] args)
        {
            //Application.Run (new LiteFormTest ());
            //Application.Run (new ButtonForm ());
            Application.Run (new MainForm ());
            //Application.Run (new MessageBoxForm ("hey", "There"));
        }
    }
}
