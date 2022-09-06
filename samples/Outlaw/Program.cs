using System;
using Modern.Forms;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Outlaw
{
    class Program
    {
        [STAThread]
        public static void Main (string[] args)
        {
            Theme.BorderGray = new SKColor (237, 235, 233);

            var form = new MainForm ();

            if (args.Length > 0)
                form.Shown += (o, e) => {
                    var launch = DateTime.Parse (args[0]);
                    var now = DateTime.Now;
                    Console.WriteLine (args[0]);
                    Console.WriteLine (now.ToLongTimeString ());
                    Console.WriteLine (now.Subtract (launch).TotalMilliseconds);
                };

            Application.Run (form);
        }
    }
}
