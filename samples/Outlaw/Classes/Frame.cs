using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;

namespace Outlaw
{
    public class Frame : Panel
    {
        public Frame ()
        {
            SetControlBehavior (ControlBehaviors.Transparent, true);
        }

        public override Rectangle ClientRectangle {
            get {
                // Adds Padding to ClientRectangle, so child controls will be inset
                var client_rect = base.ClientRectangle;

                var x = client_rect.Left + Padding.Left;
                var y = client_rect.Top + Padding.Top;
                var w = client_rect.Width - Padding.Horizontal;
                var h = client_rect.Height - Padding.Vertical;

                return new Rectangle (x, y, w, h);
            }
        }
    }
}
