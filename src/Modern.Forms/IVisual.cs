using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms
{
    public interface IVisual
    {
        Rectangle Bounds { get; set; }
        bool Enabled { get; set; }
        bool Capture { get; set; }
    }
}
