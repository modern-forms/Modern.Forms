using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class Ribbon : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => {
                style.BackgroundColor = ModernTheme.RibbonColor;
                style.Border.Bottom.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public List<RibbonTabPage> TabPages { get; } = new List<RibbonTabPage> ();

        protected override Size DefaultSize => new Size (600, 111);

        public Ribbon ()
        {
            Dock = DockStyle.Top;

            TabStrip = new TabStrip {
                Dock = DockStyle.Top
            };

            TabStrip.SelectedTabChanged += (o, e) => SetSelectedTab ();
            Controls.Add (TabStrip);
        }

        public TabStrip TabStrip { get; }

        public RibbonTabPage AddTabPage (string name)
        {
            var page = new RibbonTabPage { Text = name };
            TabPages.Add (page);

            TabStrip.Tabs.Add (new TabStripItem { Text = name });

            if (TabStrip.Tabs.Count == 1)
                TabStrip.SelectedTab = TabStrip.Tabs[0];

            return page;
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            // TabPages
            var selected_tab = TabPages.FirstOrDefault (tp => tp.Selected);
            selected_tab?.SetBounds (0, 28, Width, Height - 29);
            selected_tab?.DrawTabPage (e.Canvas);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (SelectedPage != null)
                SetHighlight (SelectedPage.Groups.SelectMany (g => g.Items).FirstOrDefault (item => item.Bounds.Contains (e.Location)));
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            SetHighlight ((RibbonItem)null);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            var clicked_item = GetItemAtPosition (e.Location);

            if (clicked_item != null)
                clicked_item.PerformClick ();
        }

        public RibbonTabPage SelectedPage {
            get {
                var selected_tab = TabStrip.SelectedTab;

                if (selected_tab == null)
                    return null;

                return TabPages.FirstOrDefault (tp => tp.Text == selected_tab.Text);
            }
        }

        private void SetHighlight (RibbonItem item)
        {
            var page = SelectedPage;
            var old = SelectedPage?.Groups.SelectMany (g => g.Items).FirstOrDefault (tp => tp.Highlighted);

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

        public void SetSelectedTab ()
        {
            var item = TabStrip.SelectedTab;
            var page = TabPages.FirstOrDefault (tp => tp.Text == item.Text);
            var old = TabPages.FirstOrDefault (tp => tp.Selected);

            if (old == page)
                return;

            if (old != null)
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
