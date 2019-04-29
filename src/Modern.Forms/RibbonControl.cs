using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class RibbonControl : ModernControl
    {
        public List<RibbonTabPage> TabPages { get; } = new List<RibbonTabPage> ();

        public RibbonControl ()
        {
            Height = 111;
        }

        protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface (e);

            e.Surface.Canvas.Clear (Theme.RibbonColor);

            // Tabs
            LayoutTabs ();

            foreach (var item in TabPages)
                item.DrawTab (e.Surface.Canvas);

            // TabPage
            var selected_tab = TabPages.First (tp => tp.Selected);
            selected_tab.SetBounds (0, 28, Width, Height - 28);
            selected_tab.DrawTabPage (e.Surface.Canvas);

            // Bottom Border
            e.Surface.Canvas.DrawLine (0, Height - 1, Width, Height - 1, Theme.BorderGray);
        }

        private void LayoutTabs ()
        {
            var left = 0;

            foreach (var item in TabPages) {
                item.SetTabBounds (left, 0, 56, 28);
                left += 56;
            }
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            SetHighlight (TabPages.FirstOrDefault (tp => tp.TabBounds.Contains (e.Location)));

            if (SelectedPage != null)
                SetHighlight (SelectedPage.Groups.SelectMany (g => g.Items).FirstOrDefault (item => item.Bounds.Contains (e.Location)));
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetHighlight ((RibbonTabPage)null);
            SetHighlight ((RibbonItem)null);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            var clicked_tab = TabPages.FirstOrDefault (tp => tp.TabBounds.Contains (e.Location));

            if (clicked_tab != null)
                SetSelectedTab (clicked_tab);

            var clicked_item = GetItemAtPosition (e.Location);

            if (clicked_item != null)
                clicked_item.PerformClick ();
        }

        private void SetHighlight (RibbonTabPage page)
        {
            var old = TabPages.FirstOrDefault (tp => tp.Highlighted);

            if (page == null || page != old) {
                // Clear any existing highlights
                if (old != null) {
                    old.Highlighted = false;
                    Invalidate (old.TabBounds);
                }

                if (page == null)
                    return;
            }

            if (page.Highlighted)
                return;

            page.Highlighted = true;

            Invalidate (page.TabBounds);
        }

        public RibbonTabPage SelectedPage => TabPages?.First (tp => tp.Selected);

        private void SetHighlight (RibbonItem item)
        {
            var page = SelectedPage;
            var old = SelectedPage.Groups.SelectMany (g => g.Items).FirstOrDefault (tp => tp.Highlighted);

            if (item == null || item != old) {
                // Clear any existing highlights
                if (old != null) {
                    old.Highlighted = false;
                    Invalidate (old.Bounds);
                }

                if (item == null)
                    return;
            }

            if (item.Highlighted)
                return;

            item.Highlighted = true;

            Invalidate (item.Bounds);
        }

        public void SetSelectedTab (RibbonTabPage page)
        {
            if (page == null)
                throw new ArgumentNullException (nameof (page));

            var old = TabPages.FirstOrDefault (tp => tp.Selected);

            if (old == page)
                return;

            old.Selected = false;
            page.Selected = true;

            Invalidate ();
        }

        private RibbonItem GetItemAtPosition (Point position)
        {
            return SelectedPage.Groups.SelectMany (g => g.Items).FirstOrDefault (item => item.Bounds.Contains (position));
        }
    }
}
