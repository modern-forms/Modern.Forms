using System.Drawing;

namespace Modern.Forms
{
    // TODO: Need to unify this with ScrollableControl
    /// <summary>
    /// Represents a control that can scroll.
    /// </summary>
    public class ScrollControl : Control
    {
        private ScrollBars scroll_bars;
        private readonly HorizontalScrollBar hscrollbar;
        private readonly VerticalScrollBar vscrollbar;
        private readonly SizeGrip sizegrip;

        /// <summary>
        /// Initializes a new instance of the ScrollControl class.
        /// </summary>
        public ScrollControl ()
        {
            sizegrip = Controls.AddImplicitControl (
                new SizeGrip {
                    Visible = false,
                    Cursor = Cursors.Arrow
                });

            hscrollbar = Controls.AddImplicitControl (
                new HorizontalScrollBar {
                    Visible = false,
                    Dock = DockStyle.Bottom,
                    Cursor = Cursors.Arrow
                });

            vscrollbar = Controls.AddImplicitControl (
                new VerticalScrollBar {
                    Visible = false,
                    Dock = DockStyle.Right,
                    Cursor = Cursors.Arrow
                });
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            vscrollbar.RaiseMouseWheel (e);
        }

        /// <inheritdoc/>
        public override Rectangle PaddedClientRectangle {
            get {
                var client_rect = ClientRectangle;

                var x = client_rect.Left + Padding.Left;
                var y = client_rect.Top + Padding.Top;
                var w = client_rect.Width - Padding.Horizontal;
                var h = client_rect.Height - Padding.Vertical;

                if (hscrollbar.Visible)
                    h -= hscrollbar.Height;

                if (vscrollbar.Visible)
                    w -= vscrollbar.Width;

                return new Rectangle (x, y, w, h);
            }
        }

        /// <summary>
        /// Gets or sets which scrollbars should be visible.
        /// </summary>
        public ScrollBars ScrollBars {
            get => scroll_bars;
            set {
                if (scroll_bars != value) {
                    scroll_bars = value;

                    hscrollbar.Visible = value.In (ScrollBars.Horizontal, ScrollBars.Both);
                    vscrollbar.Visible = value.In (ScrollBars.Vertical, ScrollBars.Both);
                }
            }
        }

        /// <summary>
        /// Gets the control's vertical scrollbar.
        /// </summary>
        public VerticalScrollBar VerticalScrollBar => vscrollbar;
    }
}
