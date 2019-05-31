using System;
using System.Collections.ObjectModel;

namespace Modern.Forms
{
    public class TabPageCollection : Collection<TabPage>
    {
        private readonly TabControl tab_control;
        private readonly TabStrip tab_strip;

        internal TabPageCollection (TabControl tabControl, TabStrip tabStrip)
        {
            tab_control = tabControl;
            tab_strip = tabStrip;
        }

        public TabPage Add (string text)
        {
            var page = new TabPage { Text = text };
            Add (page);

            return page;
        }

        protected override void InsertItem (int index, TabPage item)
        {
            base.InsertItem (index, item);

            item.Visible = false;
            tab_control.Controls.Insert (index, item);
            tab_strip.Tabs.Insert (index, CreateTabStripItem (item));
        }

        protected override void RemoveItem (int index)
        {
            base.RemoveItem (index);
            
            tab_control.Controls.RemoveAt (index);
            tab_strip.Tabs.RemoveAt (index);
        }

        protected override void SetItem (int index, TabPage item)
        {
            base.SetItem (index, item);

            item.Visible = false;
            tab_control.Controls[index] = item;
            tab_strip.Tabs[index] = CreateTabStripItem (item);
        }

        private TabStripItem CreateTabStripItem (TabPage item) => new TabStripItem { Text = item.Text, Tag = item };
    }
}
