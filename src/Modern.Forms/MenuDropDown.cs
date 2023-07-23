using System;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a MenuDropDown control.
    /// </summary>
    public class MenuDropDown : MenuBase
    {
        private Form? parent_form;
        private PopupWindow? popup;
        private int width = 400;
        private int height = 400;

        /// <summary>
        /// Initializes a new instance of the MenuDropDown class.
        /// </summary>
        public MenuDropDown () : base ()
        {
            Dock = DockStyle.Fill;

            foreach (var item in Items)
                item.ParentControl = this;
        }

        /// <summary>
        /// Initializes a new instance of the MenuDropDown class with the provided root MenuItem.
        /// </summary>
        public MenuDropDown (MenuItem root) : base (root)
        {
            Dock = DockStyle.Fill;

            foreach (var item in Items)
                item.ParentControl = this;
        }

        /// <inheritdoc/>
        internal override void Deactivate ()
        {
            base.Deactivate ();

            Hide ();
        }

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.ControlMidColor;
                style.Border.Width = 1;
            });

        /// <inheritdoc/>
        public override Form? FindForm ()
        {
            if (base.FindForm () is Form f)
                return f;

            return parent_form;
        }

        /// <summary>
        /// Hides the drop down.
        /// </summary>
        public new void Hide ()
        {
            popup?.Hide ();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override void OnHoverChanged (MenuItem? oldItem, MenuItem? newItem)
        {
            if (newItem != null) {
                oldItem?.HideDropDown ();

                Items.FirstOrDefault (i => i.IsDropDownOpened)?.HideDropDown ();
            }

            newItem?.ShowDropDown ();
        }

        /// <summary>
        /// Scales size by specified factor.
        /// </summary>
        internal static Size ScaleSize (Size startSize, float x, float y)
        {
            var size = startSize;

            size.Width = (int)Math.Round ((float)size.Width * x);
            size.Height = (int)Math.Round ((float)size.Height * y);

            return size;
        }

        /// <summary>
        /// Shows the drop down at the specified location.
        /// </summary>
        public virtual void Show (Control parent, Point location)
        {
            if (popup == null) {
                if (parent.FindForm () is not Form parent_form)
                    throw new InvalidOperationException ("Control 'parent' must belong to a Form.");

                this.parent_form = parent_form;
                popup = new PopupWindow (parent_form);
                popup.Controls.Add (this);
            }

            LayoutItems ();
            popup.Size = ScaleSize (new Size (width, height), 1 / (float)Scaling, 1 / (float)Scaling);

            Invalidate ();
            popup.Show (location);
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <inheritdoc/>
        public override bool Visible {
            get => popup?.Visible ?? false;
            set => popup?.Show ();
        }
    }
}
