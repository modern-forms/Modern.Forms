using System;
using System.Drawing;

namespace Modern.Forms
{
    public class ContextMenu : MenuDropDown
    {
        public ContextMenu () : base ()
        {
        }

        protected override bool IsTopLevelMenu => true;

        public override void Show (Point location)
        {
            Application.ActiveMenu ??= this;

            base.Show (location);
        }
    }
}
