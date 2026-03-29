using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ContextMenu control.
    /// </summary>
    public class ContextMenu : MenuDropDown
    {
        /// <summary>
        /// Initializes a new instance of the ContextMenu class.
        /// </summary>
        public ContextMenu () : base ()
        {
        }

        /// <inheritdoc/>
        protected override bool IsTopLevelMenu => true;

        /// <inheritdoc/>
        public override void Show (Control parent, Point location)
        {
            Application.ActiveMenu ??= this;

            base.Show (parent, location);
        }
    }
}
