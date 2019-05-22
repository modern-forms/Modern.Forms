using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    public interface ILayoutable
    {
        Size GetPreferredSize (Size proposedSize);
        void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All);
        Padding Margin { get; }
    }
}
