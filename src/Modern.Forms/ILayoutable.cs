using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public interface ILayoutable
    {
        Size GetPreferredSize (Size proposedSize);
        void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All);
        Padding Margin { get; }
    }
}
