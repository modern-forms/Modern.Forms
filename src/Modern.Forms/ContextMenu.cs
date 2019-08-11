using System;
using System.Drawing;

namespace Modern.Forms
{
    public class ContextMenu : MenuDropDown
    {
        public ContextMenu () : base ()
        {
        }

        public override void Show (Point location)
        {
            Application.ActiveMenu = this;

            base.Show (location);
        }
    }
}
