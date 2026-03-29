using System.Drawing;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a SplitContainer control.
    /// </summary>
    public class SplitContainer : Control
    {
        private readonly Splitter splitter;
        private Orientation orientation;
        private int panel1_min_size = 25;
        private int panel2_min_size = 25;

        /// <summary>
        /// Initializes a new instance of the SplitContainer class.
        /// </summary>
        public SplitContainer ()
        {
            Dock = DockStyle.Fill;

            Panel2 = Controls.Add (new Panel { Dock = DockStyle.Fill });
            splitter = Controls.Add (new Splitter { SplitterWidth = 5 });
            Panel1 = Controls.Add (new Panel { Dock = DockStyle.Left });

            splitter.Drag += Splitter_Drag;
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (150, 100);

        // Calculates the size of Panel1.
        private int GetMaximumPanel1Size ()
        {
            // This is the maximum Panel1 size taking the Panel2MinimumSize into account
            if (orientation == Orientation.Horizontal)
                return PaddedClientRectangle.Width - SplitterWidth - panel2_min_size;
            else
                return PaddedClientRectangle.Height - SplitterWidth - panel2_min_size;
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Gets or sets a value indicating the orientation of the SplitContainer.
        /// </summary>
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

        /// <summary>
        /// Gets the left or top panel, depending on orientation.
        /// </summary>
        public Panel Panel1 { get; }

        /// <summary>
        /// Gets or sets the minimum size Panel1 can be set to.
        /// </summary>
        public int Panel1MinimumSize {
            get => panel1_min_size;
            set {
                panel1_min_size = value;

                ResizePanels (orientation == Orientation.Horizontal ? Panel1.Width : Panel1.Height);
            }
        }

        /// <summary>
        /// Gets the right or bottom panel, depending on orientation.
        /// </summary>
        public Panel Panel2 { get; }

        /// <summary>
        /// Gets or sets the minimum size Panel2 can be set to.
        /// </summary>
        public int Panel2MinimumSize {
            get => panel2_min_size;
            set {
                panel2_min_size = value;

                ResizePanels (orientation == Orientation.Horizontal ? Panel1.Width : Panel1.Height);
            }
        }

        // Updates the size of Panel1 to resize and move all controls.
        private void ResizePanels (int value)
        {
            if (orientation == Orientation.Horizontal)
                Panel1.Width = value.Clamp (panel1_min_size, GetMaximumPanel1Size ());
            else
                Panel1.Height = value.Clamp (panel1_min_size, GetMaximumPanel1Size ());
        }

        /// <summary>
        /// Gets or sets the color of the splitter.
        /// </summary>
        public SKColor SplitterColor {
            get => splitter.Style.GetBackgroundColor ();
            set => splitter.Style.BackgroundColor = value;
        }

        /// <summary>
        /// Gets or sets the width of the splitter.
        /// </summary>
        public int SplitterWidth {
            get => splitter.SplitterWidth;
            set => splitter.SplitterWidth = value;
        }

        // Handles the splitter's Drag event.
        private void Splitter_Drag (object? sender, EventArgs<Point> e)
        {
            if (orientation == Orientation.Horizontal)
                ResizePanels (Panel1.Width - (int)(e.Value.X / ScaleFactor.Width));
            else
                ResizePanels (Panel1.Height - (int)(e.Value.Y / ScaleFactor.Height));

            Invalidate ();
        }
    }
}
