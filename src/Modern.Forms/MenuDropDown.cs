using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    public class MenuDropDown : MenuBase
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.RibbonItemHighlightColor;
                style.Border.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private PopupWindow? popup;
        private int width = 400;
        private int height = 400;

        public MenuDropDown () : base ()
        {
            Dock = DockStyle.Fill;

            foreach (var item in Items)
                item.ParentControl = this;
        }

        public MenuDropDown (MenuItem root) : base (root)
        {
            Dock = DockStyle.Fill;

            foreach (var item in Items)
                item.ParentControl = this;
        }

        internal override void Deactivate ()
        {
            base.Deactivate ();

            Hide ();
        }

        public void Hide ()
        {
            popup?.Hide ();
        }

        protected override void LayoutItems ()
        {
            if (Items.Count == 0)
                return;

            var sizes = Items.Select (i => i.GetPreferredSize (Size.Empty));

            width = sizes.Select (s => s.Width).Max ();
            height = sizes.Select (s => s.Height).Sum () + 2;

            var client_rect = new Rectangle (1, 1, width - 2, height - 2);

            StackLayoutEngine.VerticalExpand.Layout (client_rect, Items.Cast<ILayoutable> ());
        }

        protected override void OnClick (MouseEventArgs e)
        {
            var clicked_item = GetItemAtLocation (e.Location);

            if (clicked_item != null && !clicked_item.HasItems) {

                if (!clicked_item.HasItems) {
                    Application.ActiveMenu?.Deactivate ();

                    clicked_item.OnClick (e);
                    OnItemClicked (e, clicked_item);
                }
            }
        }

        protected override void OnHoverChanged (MenuItem? oldItem, MenuItem? newItem)
        {
            if (newItem != null) {
                oldItem?.HideDropDown ();

                Items.FirstOrDefault (i => i.IsDropDownOpened)?.HideDropDown ();
            }

            newItem?.ShowDropDown ();
        }

        public virtual void Show (Point location)
        {
            if (popup == null) {
                popup = new PopupWindow (root_item.GetTopMenu ()?.FindForm ());
                popup.Controls.Add (this);
            }

            popup.Location = new Avalonia.PixelPoint (location.X, location.Y);

            LayoutItems ();
            popup.Size = ScaleSize (new Size (width, height), 1 / (float)Scaling, 1 / (float)Scaling);

            Invalidate ();
            popup.Show ();
        }

        public override bool Visible {
            get => popup?.Visible ?? false;
            set => popup?.Show ();
        }
    }
}
