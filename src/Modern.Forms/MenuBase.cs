using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Modern.Forms
{
    public abstract class MenuBase : Control
    {
        protected MenuItem root_item;

        protected MenuBase ()
        {
            root_item = new MenuRootItem (this);
        }

        protected MenuBase (MenuItem root)
        {
            root_item = root;
        }

        internal virtual void Activate ()
        {
            IsActivated = true;

            if (IsTopLevelMenu)
                Application.ActiveMenu = this;
        }

        internal virtual void Deactivate ()
        {
            IsActivated = false;
            SelectedItem = null;

            root_item.HideDropDown ();

            if (IsTopLevelMenu)
                Application.ActiveMenu = null;
        }

        public MenuItem GetItemAtLocation (Point location) => Items.FirstOrDefault (item => item.Bounds.Contains (location));

        public MenuBase? GetTopLevelMenu ()
        {
            if (IsTopLevelMenu)
                return this;

            return root_item.GetTopMenu ();
        }

        public bool IsActivated { get; private set; }

        protected virtual bool IsReleaseOnClick => true;

        protected virtual bool IsTopLevelMenu => false;

        public virtual MenuItemCollection Items => root_item.Items;

        protected abstract void LayoutItems ();

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_item = GetItemAtLocation (e.Location);

            // Clicking the currently dropped down item releases the menu
            if (IsActivated && IsReleaseOnClick && clicked_item == SelectedItem) {
                Deactivate ();
                return;
            }

            // If we clicked an item, raise the Click events
            if (clicked_item != null) {
                SelectedItem = clicked_item;
                clicked_item.OnClick (e);
                OnItemClicked (e, clicked_item);
                Activate ();
            } else {
                Deactivate ();
            }
        }

        protected virtual void OnHoverChanged (MenuItem? oldItem, MenuItem? newItem) { }

        protected virtual void OnItemClicked (MouseEventArgs e, MenuItem item) { }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetHover (null);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetHover (GetItemAtLocation (e.Location));
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutItems ();

            foreach (var item in Items)
                item.OnPaint (e.Canvas);
        }

        public MenuItem? SelectedItem {
            get => Items.FirstOrDefault (tp => tp.Selected);
            internal set {
                var old = SelectedItem;

                // Nothing is changing
                if (old == value)
                    return;

                if (old != null)
                    old.Selected = false;

                if (value != null) {
                    value.Selected = true;
                    Activate ();
                }

                Invalidate ();
            }
        }

        private void SetHover (MenuItem? item)
        {
            var old = Items.FirstOrDefault (tp => tp.Hovered);

            if (item == null || item != old) {
                // Clear any existing hovers
                if (old != null) {
                    old.Hovered = false;
                    Invalidate (old.Bounds);
                }

                if (item == null) {
                    OnHoverChanged (old, item);
                    return;
                }
            }

            if (item.Hovered)
                return;

            item.Hovered = true;

            Invalidate (item.Bounds);
            OnHoverChanged (old, item);
        }
    }
}
