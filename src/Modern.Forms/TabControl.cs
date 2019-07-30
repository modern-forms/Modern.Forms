using System;
using System.Linq;

namespace Modern.Forms
{
    public class TabControl : Control
    {
        private readonly TabStrip tab_strip;

        public TabControl ()
        {
            tab_strip = new TabStrip {
                Dock = DockStyle.Top
            };

            tab_strip.SelectedTabChanged += TabStrip_SelectedTabChanged;
            Controls.AddImplicitControl (tab_strip);

            Pages = new TabPageCollection (this, tab_strip);
        }

        public TabPageCollection Pages { get; }

        private TabPage? GetPageFromTab (TabStripItem item) => item.Tag as TabPage;

        private TabStripItem GetTabFromPage (TabPage page) => tab_strip.Tabs.FirstOrDefault (t => t.Tag == page);

        private void TabStrip_SelectedTabChanged (object sender, EventArgs e)
        {
            var old_selected = Controls.OfType<TabPage> ().FirstOrDefault (tp => tp.Visible);
            var new_selected = GetPageFromTab (tab_strip.SelectedTab);

            if (old_selected == new_selected)
                return;

            if (old_selected != null)
                old_selected.Visible = false;

            if (new_selected != null)
                new_selected.Visible = true;
        }
    }
}
