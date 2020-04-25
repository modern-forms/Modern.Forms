using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a Ribbon control.
    /// Note the Ribbon control has not been fully developed, and probably does not contain enough functionality to be useful yet.
    /// </summary>
    public class Ribbon : Control
    {
        private readonly TabStrip tab_strip;
        private RibbonItem? mouse_in_item;

        /// <summary>
        /// Initializes a new instance of the Ribbon class.
        /// </summary>
        public Ribbon ()
        {
            tab_strip = Controls.AddImplicitControl (new TabStrip {
                Dock = DockStyle.Top
            });

            tab_strip.SelectedTabChanged += TabStrip_SelectedTabChanged;

            TabPages = new RibbonTabPageCollection (this, tab_strip);

            Dock = DockStyle.Top;
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 111);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle, 
            (style) => {
                style.BackgroundColor = Theme.RibbonColor;
                style.Border.Bottom.Width = 1;
            });

        private RibbonItem? GetItemAtLocation (Point location)
        {
            return SelectedTabPage?.Groups.SelectMany (g => g.Items).FirstOrDefault (item => item.Bounds.Contains (location));
        }

        private RibbonTabPage? GetPageFromTab (TabStripItem? item) => TabPages.FirstOrDefault (p => p.TabStripItem == item);

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_item = GetItemAtLocation (e.Location);
            clicked_item?.FireEvent (e, ItemEventType.Click);
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (SelectedTabPage != null) {
                var item = GetItemAtLocation (e.Location);
                item?.FireEvent (e, ItemEventType.MouseDown);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (SelectedTabPage != null) {
                // Clear out the old item
                mouse_in_item?.FireEvent (e, ItemEventType.MouseLeave);
                mouse_in_item = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (SelectedTabPage != null) {
                var new_mouse_in = GetItemAtLocation (e.Location);

                if (new_mouse_in == mouse_in_item)
                    return;

                // Clear out the old item
                mouse_in_item?.FireEvent (e, ItemEventType.MouseLeave);

                mouse_in_item = new_mouse_in;

                // Fire events on new item
                mouse_in_item?.FireEvent (e, ItemEventType.MouseEnter);
                mouse_in_item?.FireEvent (e, ItemEventType.MouseMove);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (SelectedTabPage != null) {
                var item = GetItemAtLocation (e.Location);
                item?.FireEvent (e, ItemEventType.MouseUp);
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            // Set the bounds of the selected tab, this should be moved out of the Paint loop
            var top = ShowTabs ? tab_strip.ScaledSize.Height : 0;
            var selected_tab = TabPages.FirstOrDefault (tp => tp.Selected);
            selected_tab?.SetBounds (0, top, ScaledSize.Width, ScaledSize.Height - top - 1);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the SelectedTabPageIndexChanged event.
        /// </summary>
        protected virtual void OnSelectedTabPageIndexChanged (EventArgs e) => SelectedTabPageIndexChanged?.Invoke (this, e);

        /// <summary>
        /// Gets or sets the currently selected tab page.
        /// </summary>
        public RibbonTabPage? SelectedTabPage {
            get => GetPageFromTab (tab_strip.SelectedTab);
            set {
                // We don't support deselecting all tabs
                if (value is null)
                    return;

                var index = TabPages.IndexOf (value);

                if (index == -1)
                    throw new ArgumentException ("RibbonTabPage is not part of this Ribbon");

                tab_strip.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected tab page. This value will be -1 if there is not a selected tab page;
        /// </summary>
        public int SelectedTabPageIndex {
            get => tab_strip.SelectedIndex;
            set => tab_strip.SelectedIndex = value;
        }

        /// <summary>
        /// Raised when the value of the SelectedTabPageIndex property changes.
        /// </summary>
        public event EventHandler? SelectedTabPageIndexChanged;

        /// <summary>
        /// Gets or sets a value indicating if the ribbon tabs are shown.
        /// </summary>
        public bool ShowTabs {
            get => tab_strip.Visible;
            set {
                if (ShowTabs == value)
                    return;

                tab_strip.Visible = value;
                Height += value ? tab_strip.ScaledSize.Height : -tab_strip.ScaledSize.Height;
            }
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets the collection of tab pages contained by this Ribbon.
        /// </summary>
        public RibbonTabPageCollection TabPages { get; }

        // Handles changes of the TabStrip's selected tab.
        private void TabStrip_SelectedTabChanged (object? sender, EventArgs e)
        {
            var old_selected = TabPages.FirstOrDefault (tp => tp.Selected);
            var new_selected = GetPageFromTab (tab_strip.SelectedTab);

            if (old_selected == new_selected)
                return;

            if (old_selected != null)
                old_selected.Selected = false;

            if (new_selected != null)
                new_selected.Selected = true;

            Invalidate ();

            OnSelectedTabPageIndexChanged (EventArgs.Empty);
        }
    }
}
