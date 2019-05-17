using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class Ribbon : LiteControl
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => {
                style.BackgroundColor = ModernTheme.RibbonColor;
                style.Border.Bottom.Width = 1;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private RibbonItem mouse_in_item;

        public RibbonTagPageCollection TabPages { get; }

        protected override Size DefaultSize => new Size (600, 111);

        public Ribbon ()
        {
            TabPages = new RibbonTagPageCollection (this);

            Dock = DockStyle.Top;

            TabStrip = new TabStrip {
                Dock = DockStyle.Top
            };

            TabStrip.SelectedTabChanged += (o, e) => SetSelectedTab ();
            Controls.Add (TabStrip);
        }

        public TabStrip TabStrip { get; }

        public bool ShowTabs {
            get => TabStrip.Visible;
            set {
                if (ShowTabs == value)
                    return;

                TabStrip.Visible = value;
                Height += value ? 28 : -28;
            }
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            // TabPages
            var top = ShowTabs ? 28 : 0;
            var selected_tab = TabPages.FirstOrDefault (tp => tp.Selected);
            selected_tab?.SetBounds (0, top, Width, Height - top - 1);
            selected_tab?.DrawTabPage (e.Canvas);
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (SelectedPage != null) {
                var item = GetItemAtPosition (e.Location);
                item.FireEvent (e, ToolStripItemEventType.MouseDown);
            }
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (SelectedPage != null) {
                var new_mouse_in = GetItemAtPosition (e.Location);

                if (new_mouse_in == mouse_in_item)
                    return;

                // Clear out the old item
                mouse_in_item?.FireEvent (e, ToolStripItemEventType.MouseLeave);

                mouse_in_item = new_mouse_in;

                // Fire events on new item
                mouse_in_item?.FireEvent (e, ToolStripItemEventType.MouseEnter);
                mouse_in_item?.FireEvent (e, ToolStripItemEventType.MouseMove);
            }
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (SelectedPage != null) {
                // Clear out the old item
                mouse_in_item?.FireEvent (e, ToolStripItemEventType.MouseLeave);
                mouse_in_item = null;
            }
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (SelectedPage != null) {
                var item = GetItemAtPosition (e.Location);
                item.FireEvent (e, ToolStripItemEventType.MouseUp);
            }
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_item = GetItemAtPosition (e.Location);

            if (clicked_item != null)
                clicked_item.FireEvent (e, ToolStripItemEventType.Click);
        }

        public RibbonTabPage SelectedPage {
            get {
                var selected_tab = TabStrip.SelectedTab;

                if (selected_tab == null)
                    return null;

                return TabPages.FirstOrDefault (tp => tp.Text == selected_tab.Text);
            }
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
