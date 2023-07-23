using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a Menu control.
    /// </summary>
    public class Menu : MenuBase
    {
        /// <summary>
        /// Initializes a new instance of the Menu class.
        /// </summary>
        public Menu ()
        {
            Dock = DockStyle.Top;
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 28);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
          (style) => {
              style.BackgroundColor = Theme.ControlMidColor;
          });

        /// <inheritdoc/>
        protected override bool IsTopLevelMenu => true;

        /// <inheritdoc/>
        protected override void LayoutItems ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ClientRectangle, Items.Cast<ILayoutable> ());
        }

        /// <inheritdoc/>
        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            Deactivate ();
        }

        /// <inheritdoc/>
        protected override void OnHoverChanged (MenuItem? oldItem, MenuItem? newItem)
        {
            if (IsActivated && newItem != null)
                SelectedItem = newItem;
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
