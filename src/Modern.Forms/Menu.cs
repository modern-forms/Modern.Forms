using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class Menu : MenuBase
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
          (style) => {
              style.BackgroundColor = Theme.NeutralGray;
          });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public Menu ()
        {
            Dock = DockStyle.Top;
        }

        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            Deactivate ();
        }

        protected override Size DefaultSize => new Size (600, 28);

        protected override void LayoutItems ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ClientRectangle, Items.Cast<ILayoutable> ());
        }

        protected override void OnClick (MouseEventArgs e)
        {
            var clicked_item = GetItemAtLocation (e.Location);

            // Clicking the currently dropped down item releases the menu
            if (clicked_item == SelectedItem) {
                Deactivate ();
                return;
            }

            SelectedItem = clicked_item;

            // If we clicked an item, raise the Click events
            if (clicked_item != null) {
                clicked_item.OnClick (e);
                OnItemClicked (e, clicked_item);
            }

            if (clicked_item != null)
                Activate ();
            else
                Deactivate ();
        }

        protected override void OnHoverChanged (MenuItem? oldItem, MenuItem? newItem)
        {
            if (IsActivated && newItem != null)
                SelectedItem = newItem;
        }
    }
}
