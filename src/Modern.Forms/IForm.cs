using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public interface IForm
    {
        void Invalidate ();
        void Close ();
        FormWindowState WindowState { get; set; }
        void BeginMoveDrag ();
    }
}
