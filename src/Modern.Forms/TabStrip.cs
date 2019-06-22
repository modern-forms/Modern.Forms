using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Modern.Forms
{
    // TODO:
    // Disabled styles
    // Pressed styles
    // Image
    // Closeable?
    // Overflow?
    // Measuring
    // OnSelectedTabChanging?
    public class TabStrip : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.RibbonColor;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public TabStrip ()
        {
            Tabs = new TabStripItemCollection (this);
        }

        public event EventHandler SelectedTabChanged;

        public TabStripItem SelectedTab {
            get => Tabs.FirstOrDefault (tp => tp.Selected);
            set {
                var old = SelectedTab;

                // Nothing is changing
                if (old == value)
                    return;

                if (old != null)
                    old.Selected = false;

                value.Selected = true;

                Invalidate ();

                OnSelectedTabChanged (EventArgs.Empty);
            }
        }

        public TabStripItemCollection Tabs { get; }

        protected override Size DefaultSize => new Size (600, 28);

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_tab = GetTabAtLocation (e.Location);

            if (clicked_tab != null)
                SelectedTab = clicked_tab;
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetHover (null);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetHover (GetTabAtLocation (e.Location));
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            LayoutTabs ();

            foreach (var item in Tabs)
                item.OnPaint (e.Canvas);
        }

        protected virtual void OnSelectedTabChanged (EventArgs e) => SelectedTabChanged?.Invoke (this, e);

        private TabStripItem GetTabAtLocation (Point location) => Tabs.FirstOrDefault (tp => tp.Bounds.Contains (location));

        private void LayoutTabs ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ScaledClientRectangle, Tabs.Cast<ILayoutable> ());
        }

        private void SetHover (TabStripItem item)
        {
            var old = Tabs.FirstOrDefault (tp => tp.Hovered);

            if (item == null || item != old) {
                // Clear any existing hovers
                if (old != null) {
                    old.Hovered = false;
                    Invalidate (old.Bounds);
                }

                if (item == null)
                    return;
            }

            if (item.Hovered)
                return;

            item.Hovered = true;

            Invalidate (item.Bounds);
        }
    }
}
