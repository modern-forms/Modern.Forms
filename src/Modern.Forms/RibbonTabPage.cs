using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonTabPage
    {
        public string Text { get; set; }
        public bool Selected { get; set; }
        public bool Highlighted { get; set; }
        public Ribbon Owner { get; set; }

        public RibbonItemGroupCollection Groups { get; }

        public RibbonTabPage ()
        {
            Groups = new RibbonItemGroupCollection (this);
        }

        // This is the bounds for the tab page, where the buttons are
        public Rectangle Bounds { get; private set; }

        public void SetBounds (int x, int y, int width, int height)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public void DrawTabPage (SKCanvas canvas)
        {
            canvas.FillRectangle (Bounds, Theme.NeutralGray);

            LayoutItems ();

            foreach (var group in Groups)
                group.DrawGroup (canvas);
        }

        private void LayoutItems ()
        {
            // Lay out each group
            StackLayoutEngine.HorizontalExpand.Layout (Bounds, Groups.Cast<ILayoutable> ());
        }
    }
}
