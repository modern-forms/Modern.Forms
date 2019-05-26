using System;
using System.Collections.Generic;
using System.Drawing;

namespace Modern.Forms
{
    public class ScrollableControl : Control
    {
        private HorizontalScrollBar hscrollbar;
        private VerticalScrollBar vscrollbar;
        private SizeGrip sizegrip;
        private Point scroll_position;
        private Size canvas_size;
        private Size auto_scroll_min_size = Size.Empty;
        private Size auto_scroll_margin = Size.Empty;
        private bool force_hscroll_visible = false;
        private bool force_vscroll_visible = false;
        private bool auto_scroll = false;

        public ScrollableControl ()
        {
            CreateScrollbars ();

            SizeChanged += (o, e) => Recalculate (true);
            VisibleChanged += (o, e) => Recalculate (true);
        }

        public event EventHandler<ScrollEventArgs> Scroll;

        public bool AutoScroll {
            get => auto_scroll;
            set {
                if (auto_scroll != value) {
                    auto_scroll = value;
                    PerformLayout (this, nameof (AutoScroll));
                }
            }
        }

        public ScrollProperties HorizontalScrollProperties => new ScrollProperties (hscrollbar);

        public ScrollProperties VerticalScrollProperties => new ScrollProperties (vscrollbar);

        protected virtual void AdjustFormScrollbars (bool displayScrollbars) => Recalculate (false);

        protected override void OnLayout (LayoutEventArgs e)
        {
            CalculateCanvasSize ();
            AdjustFormScrollbars (AutoScroll);

            base.OnLayout (e);
        }

        protected virtual void OnScroll (ScrollEventArgs e) => Scroll?.Invoke (this, e);

        protected override void OnVisibleChanged (EventArgs e)
        {
            if (Visible)
                PerformLayout (this, nameof (Visible));

            base.OnVisibleChanged (e);
        }

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

        private void CreateScrollbars ()
        {
            hscrollbar = new HorizontalScrollBar {
                Visible = false
            };

            hscrollbar.ValueChanged += HandleScroll;
            hscrollbar.Scroll += (o, e) => OnScroll (e);

            vscrollbar = new VerticalScrollBar {
                Visible = false
            };

            vscrollbar.ValueChanged += HandleScroll;
            vscrollbar.Scroll += (o, e) => OnScroll (e);

            sizegrip = new SizeGrip {
                Visible = false
            };

            Controls.AddImplicitControlRange (sizegrip, hscrollbar, vscrollbar);
        }

        private void HandleScroll (object sender, EventArgs e)
        {
            if (sender == vscrollbar && vscrollbar.Visible)
                ScrollWindow (0, vscrollbar.Value - scroll_position.Y);
            else if (sender == hscrollbar && hscrollbar.Visible)
                ScrollWindow (hscrollbar.Value - scroll_position.X, 0);
        }

        private void Recalculate (bool doLayout)
        {
            var canvas = canvas_size;
            var client = ClientSize;

            canvas.Width += auto_scroll_margin.Width;
            canvas.Height += auto_scroll_margin.Height;

            var right_edge = client.Width;
            var bottom_edge = client.Height;
            var prev_right_edge = 0;
            var prev_bottom_edge = 0;

            var hscroll_visible = false;
            var vscroll_visible = false;

            do {
                prev_right_edge = right_edge;
                prev_bottom_edge = bottom_edge;

                if ((force_hscroll_visible || (canvas.Width > right_edge && auto_scroll)) && client.Width > 0) {
                    hscroll_visible = true;
                    bottom_edge = client.Height - 15;// SystemInformation.HorizontalScrollBarHeight;
                } else {
                    hscroll_visible = false;
                    bottom_edge = client.Height;
                }

                if ((force_vscroll_visible || (canvas.Height > bottom_edge && auto_scroll)) && client.Height > 0) {
                    vscroll_visible = true;
                    right_edge = client.Width - 15;// SystemInformation.VerticalScrollBarWidth;
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
                hscrollbar.Maximum = canvas.Width - client.Width + 15;
                hscrollbar.LargeChange = right_edge;
                hscrollbar.SmallChange = 5;
            } else {
                if (hscrollbar.Visible)
                    ScrollWindow (-scroll_position.X, 0);

                scroll_position.X = 0;
            }

            if (vscroll_visible) {
                vscrollbar.Maximum = canvas.Height - client.Height + 15;
                vscrollbar.LargeChange = bottom_edge;
                vscrollbar.SmallChange = 5;
            } else {
                if (vscrollbar.Visible)
                    ScrollWindow (0, -scroll_position.Y);

                scroll_position.X = 0;
            }

            SuspendLayout ();

            hscrollbar.SetBounds (0, client.Height - 15, ClientRectangle.Width - (vscroll_visible ? 15 : 0), 15, BoundsSpecified.None);
            hscrollbar.Visible = hscroll_visible;
            vscrollbar.SetBounds (client.Width - 15, 0, 15, ClientRectangle.Height - (hscroll_visible ? 15 : 0), BoundsSpecified.None);
            vscrollbar.Visible = vscroll_visible;

            sizegrip.Visible = hscroll_visible && vscroll_visible;
            sizegrip.SetBounds (client.Width - 15, client.Height - 15, 15, 15);

            ResumeLayout (doLayout);
        }

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
    }
}
