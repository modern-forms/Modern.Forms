using System;
using System.Drawing;
using Modern.Forms.Layout;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ScrollableControl control.
    /// </summary>
    public class ScrollableControl : Control
    {
        private readonly HorizontalScrollBar hscrollbar;
        private readonly VerticalScrollBar vscrollbar;
        private readonly SizeGrip sizegrip;

        private Point scroll_position = Point.Empty;
        private Size canvas_size = Size.Empty;
        private Size auto_scroll_min_size = Size.Empty;
        private Size auto_scroll_margin = Size.Empty;
        private bool auto_scroll = false;
        private bool force_hscroll_visible = false;
        private bool force_vscroll_visible = false;

        /// <summary>
        /// Initializes a new instance of the ScrollableControl class.
        /// </summary>
        public ScrollableControl ()
        {
            hscrollbar = Controls.AddImplicitControl (new HorizontalScrollBar {
                Visible = false
            });

            hscrollbar.ValueChanged += HandleScroll;
            hscrollbar.Scroll += (o, e) => OnScroll (e);

            vscrollbar = Controls.AddImplicitControl (new VerticalScrollBar {
                Visible = false
            });

            vscrollbar.ValueChanged += HandleScroll;
            vscrollbar.Scroll += (o, e) => OnScroll (e);

            sizegrip = Controls.AddImplicitControl (new SizeGrip {
                Visible = false
            });

            SizeChanged += (o, e) => Recalculate (true);
            VisibleChanged += (o, e) => Recalculate (true);
        }

        /// <summary>
        /// Adjusts the scrollbars based on the currently contained controls.
        /// </summary>
        protected virtual void AdjustFormScrollbars (bool displayScrollbars) => Recalculate (false);

        /// <summary>
        /// Gets or sets a value indicating the user can scroll to controls beyond the ScrollableControl's bounds.
        /// </summary>
        public bool AutoScroll {
            get => auto_scroll;
            set {
                if (auto_scroll != value) {
                    auto_scroll = value;
                    PerformLayout (this, nameof (AutoScroll));
                }
            }
        }

        // Calculates and sets the current canvas size.
        private void CalculateCanvasSize ()
        {
            var width = 0;
            var height = 0;
            var extra_width = hscrollbar.Value + Padding.Right;
            var extra_height = vscrollbar.Value + Padding.Bottom;

            foreach (var c in Controls) {
                if (c.Dock == DockStyle.Right)
                    extra_width += c.Width;
                else if (c.Dock == DockStyle.Bottom)
                    extra_height += c.Height;
            }

            if (!auto_scroll_min_size.IsEmpty) {
                width = auto_scroll_min_size.Width;
                height = auto_scroll_min_size.Height;
            }

            foreach (var c in Controls) {
                switch (c.Dock) {
                    case DockStyle.Left:
                        width = Math.Max (width, c.Right + extra_width);
                        continue;
                    case DockStyle.Top:
                        height = Math.Max (height, c.Bottom + extra_height);
                        continue;
                    case DockStyle.Bottom:
                    case DockStyle.Right:
                    case DockStyle.Fill:
                        continue;
                    default:
                        var anchor = c.Anchor;

                        if (anchor.HasFlag (AnchorStyles.Left) && !anchor.HasFlag (AnchorStyles.Right))
                            width = Math.Max (width, c.Right + extra_width);

                        if (anchor.HasFlag (AnchorStyles.Top) && !anchor.HasFlag (AnchorStyles.Bottom))
                            height = Math.Max (height, c.Bottom + extra_height);

                        continue;
                }
            }

            canvas_size.Width = width;
            canvas_size.Height = height;
        }

        /// <inheritdoc/>
        public override Rectangle DisplayRectangle {
            get {
                // A ScrollableControl DisplayRectangle includes Padding, while a normal Control does not.
                var rect = ClientRectangle;

                if (hscrollbar.Visible)
                    rect.Height -= hscrollbar.Height;

                if (vscrollbar.Visible)
                    rect.Width -= vscrollbar.Width;

                // TODO: Scale padding?
                return LayoutUtils.DeflateRect (rect, Padding);
            }
        }

        // Handles events from the scrollbars to update the window position.
        private void HandleScroll (object? sender, EventArgs e)
        {
            if (sender == vscrollbar && vscrollbar.Visible)
                ScrollWindow (0, vscrollbar.Value - scroll_position.Y);
            else if (sender == hscrollbar && hscrollbar.Visible)
                ScrollWindow (hscrollbar.Value - scroll_position.X, 0);
        }

        /// <summary>
        /// Provides access to the properties of the horizontal scrollbar.
        /// </summary>
        public ScrollProperties HorizontalScrollProperties => new ScrollProperties (hscrollbar);

        /// <inheritdoc/>
        protected override void OnLayout (LayoutEventArgs e)
        {
            CalculateCanvasSize ();
            AdjustFormScrollbars (AutoScroll);

            base.OnLayout (e);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the Scroll event.
        /// </summary>
        protected virtual void OnScroll (ScrollEventArgs e) => Scroll?.Invoke (this, e);

        // Recalculates all components of the ScrollableControl.
        private void Recalculate (bool doLayout)
        {
            var canvas = canvas_size;
            var client = Bounds;

            canvas.Width += auto_scroll_margin.Width;
            canvas.Height += auto_scroll_margin.Height;

            var right_edge = client.Width;
            var bottom_edge = client.Height;
            var prev_right_edge = 0;
            var prev_bottom_edge = 0;

            var hscroll_visible = false;
            var vscroll_visible = false;

            var bar_size = 15;

            do {
                prev_right_edge = right_edge;
                prev_bottom_edge = bottom_edge;

                if ((force_hscroll_visible || (canvas.Width > right_edge && auto_scroll)) && client.Width > 0) {
                    hscroll_visible = true;
                    bottom_edge = client.Height - bar_size;// SystemInformation.HorizontalScrollBarHeight;
                } else {
                    hscroll_visible = false;
                    bottom_edge = client.Height;
                }

                if ((force_vscroll_visible || (canvas.Height > bottom_edge && auto_scroll)) && client.Height > 0) {
                    vscroll_visible = true;
                    right_edge = client.Width - bar_size;// SystemInformation.VerticalScrollBarWidth;
                } else {
                    vscroll_visible = false;
                    right_edge = client.Width;
                }
            } while (right_edge != prev_right_edge || bottom_edge != prev_bottom_edge);

            right_edge = Math.Max (right_edge, 0);
            bottom_edge = Math.Max (bottom_edge, 0);

            if (!vscroll_visible)
                vscrollbar.Value = 0;
            if (!hscroll_visible)
                hscrollbar.Value = 0;

            if (hscroll_visible) {
                hscrollbar.LargeChange = right_edge;
                hscrollbar.SmallChange = 5;
                hscrollbar.Maximum = canvas.Width - client.Width + bar_size;

            } else {
                if (hscrollbar.Visible)
                    ScrollWindow (-scroll_position.X, 0);

                scroll_position.X = 0;
            }

            if (vscroll_visible) {
                vscrollbar.LargeChange = bottom_edge;
                vscrollbar.SmallChange = 5;
                vscrollbar.Maximum = canvas.Height - client.Height + bar_size;

            } else {
                if (vscrollbar.Visible)
                    ScrollWindow (0, -scroll_position.Y);

                scroll_position.X = 0;
            }

            SuspendLayout ();

            var sizegrip_visible = hscroll_visible && vscroll_visible;

            hscrollbar.SetBounds (0, client.Height - bar_size, sizegrip_visible ? Bounds.Width - bar_size : Bounds.Height, bar_size);
            hscrollbar.Visible = hscroll_visible;

            vscrollbar.SetBounds (client.Width - bar_size, 0, bar_size, sizegrip_visible ? Bounds.Height - bar_size : Bounds.Height);
            vscrollbar.Visible = vscroll_visible;

            sizegrip.SetBounds (client.Width - bar_size, client.Height - bar_size, bar_size, bar_size);
            sizegrip.Visible = sizegrip_visible;

            ResumeLayout (doLayout);
        }

        /// <summary>
        /// Raised when the ScrollableControl is scrolled.
        /// </summary>
        public event EventHandler<ScrollEventArgs>? Scroll;

        // Scrolls the control by the requested offsets.
        private void ScrollWindow (int xOffset, int yOffset)
        {
            if (xOffset == 0 && yOffset == 0)
                return;

            SuspendLayout ();

            foreach (var c in Controls)
                c.Location = new Point (c.Left - xOffset, c.Top - yOffset);

            scroll_position.Offset (xOffset, yOffset);

            ResumeLayout (false);
        }

        /// <summary>
        /// Provides access to the properties of the vertical scrollbar.
        /// </summary>
        public ScrollProperties VerticalScrollProperties => new ScrollProperties (vscrollbar);
    }
}
