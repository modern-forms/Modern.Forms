using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class SplitContainer : Control
    {
        private readonly Splitter splitter;
        private Orientation orientation;
        private int panel1_min_size = 25;
        private int panel2_min_size = 25;

        public SplitContainer ()
        {
            Panel2 = Controls.Add (new Panel { Dock = DockStyle.Fill });
            splitter = Controls.Add (new Splitter { SplitterWidth = 5 });
            Panel1 = Controls.Add (new Panel { Dock = DockStyle.Left });

            splitter.Drag += Splitter_Drag;
        }

        public Orientation Orientation {
            get => orientation;
            set {
                if (orientation != value) {
                    orientation = value;

                    SuspendLayout ();

                    splitter.Orientation = orientation;
                    Panel1.Dock = orientation == Orientation.Horizontal ? DockStyle.Left : DockStyle.Top;
                    Panel1.Size = new Size (Panel1.Height, Panel1.Width);

                    ResumeLayout (true);
                }
            }
        }

        public Panel Panel1 { get; }

        public int Panel1MinimumSize {
            get => panel1_min_size;
            set {
                panel1_min_size = value;

                ResizePanels (orientation == Orientation.Horizontal ? Panel1.Width : Panel1.Height);
            }
        }

        public Panel Panel2 { get; }

        public int Panel2MinimumSize {
            get => panel2_min_size;
            set {
                panel2_min_size = value;

                ResizePanels (orientation == Orientation.Horizontal ? Panel1.Width : Panel1.Height);
            }
        }

        public SKColor SplitterColor {
            get => splitter.Style.GetBackgroundColor ();
            set => splitter.Style.BackgroundColor = value;
        }

        public int SplitterWidth {
            get => splitter.SplitterWidth;
            set => splitter.SplitterWidth = value;
        }

        protected override Size DefaultSize => new Size (150, 100);

        private void Splitter_Drag (object sender, EventArgs<Point> e)
        {
            if (orientation == Orientation.Horizontal)
                ResizePanels (Panel1.Width - (int)(e.Value.X / ScaleFactor.Width));
            else
                ResizePanels (Panel1.Height - (int)(e.Value.Y / ScaleFactor.Height));

            Invalidate ();
        }

        private int GetMaximumPanelSize ()
        {
            // This is the maximum Panel1 size taking the Panel2MinimumSize into account
            if (orientation == Orientation.Horizontal)
                return PaddedClientRectangle.Width - SplitterWidth - panel2_min_size;
            else
                return PaddedClientRectangle.Height - SplitterWidth - panel2_min_size;
        }

        private void ResizePanels (int value)
        {
            if (orientation == Orientation.Horizontal)
                Panel1.Width = value.Clamp (panel1_min_size, GetMaximumPanelSize ());
            else
                Panel1.Height = value.Clamp (panel1_min_size, GetMaximumPanelSize ());
        }
    }
}
