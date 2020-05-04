using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a VerticalScrollBar control.
    /// </summary>
    public class TabStrip : Control
    {
        /// <summary>
        /// Initializes a new instance of the VerticalScrollBar class.
        /// </summary>
        public TabStrip ()
        {
            Tabs = new TabStripItemCollection (this);
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (600, 28);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.PrimaryColor;
            });

        // Returns the tab at the specified location.
        private TabStripItem? GetTabAtLocation (Point location) => Tabs.FirstOrDefault (tp => tp.Bounds.Contains (location));

        // Layout the tabs.
        private void LayoutTabs ()
        {
            StackLayoutEngine.HorizontalExpand.Layout (ClientRectangle, Tabs.Cast<ILayoutable> ());
        }

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            var clicked_tab = GetTabAtLocation (e.Location);

            // This does a null check
            if (clicked_tab?.Enabled == true)
                SelectedTab = clicked_tab;
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            Tabs.HoveredIndex = -1;
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            var hover_tab = GetTabAtLocation (e.Location);
            Tabs.HoveredIndex = hover_tab is null ? -1 : Tabs.IndexOf (hover_tab);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            // TODO: This should only be done when tabs are added or removed, or the TabStrip is resized.
            LayoutTabs ();

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the SelectedTabChanged event.
        /// </summary>
        protected virtual void OnSelectedTabChanged (EventArgs e) => SelectedTabChanged?.Invoke (this, e);

        /// <summary>
        /// Raised when the selected tab changes.
        /// </summary>
        public event EventHandler? SelectedTabChanged;

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets or sets the index of the currently selected tab.
        /// </summary>
        public int SelectedIndex {
            get => Tabs.SelectedIndex;
            set {
                if (Tabs.SelectedIndex != value) {
                    Tabs.SelectedIndex = value;
                    OnSelectedTabChanged (EventArgs.Empty);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected tab.
        /// </summary>
        public TabStripItem? SelectedTab {
            get => SelectedIndex >= 0 ? Tabs[SelectedIndex] : null;
            set {
                if (value is null) {
                    SelectedIndex = -1;
                    return;
                }

                var index = Tabs.IndexOf (value);

                if (index == -1)
                    throw new ArgumentException ("Item is not part of this list");

                SelectedIndex = index;
            }
        }

        /// <summary>
        /// Gets the collection of tabs contained by this TabStrip.
        /// </summary>
        public TabStripItemCollection Tabs { get; }
    }
}
