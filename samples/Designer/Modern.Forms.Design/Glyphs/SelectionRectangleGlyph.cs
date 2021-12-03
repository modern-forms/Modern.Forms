using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;

namespace Modern.Forms.Design
{
    public class SelectionRectangleGlyph : Glyph
    {
        public override Rectangle Bounds { get; }

        public SelectionRectangleGlyph (Rectangle controlBounds)
        {
            Bounds = controlBounds;
        }

        public override Cursor? GetHitTest (Point p)
        {
            return null;
        }

        public override void Paint (PaintEventArgs pe)
        {
            pe.Canvas.DrawSelectionAdornment (Bounds);
        }
    }
}
