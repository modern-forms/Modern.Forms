using System;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a Splitter control.
    /// </summary>
    public class Splitter : Control
    {
        private Orientation orientation;
        private bool is_dragging;
        private Point drag_start_point;
        private Point? last_drag_point;

        /// <summary>
        /// Initializes a new instance of the Splitter class.
        /// </summary>
        public Splitter ()
        {
            Dock = DockStyle.Left;
            Cursor = Cursors.SizeWestEast;
        }

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Raised when the user drags the Splitter.
        /// </summary>
        public event EventHandler<EventArgs<Point>>? Drag;

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Raises the Drag event.
        /// </summary>
        protected void OnDrag (EventArgs<Point> e) => Drag?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            is_dragging = true;
            drag_start_point = e.ScreenLocation;
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (is_dragging) {
                last_drag_point ??= drag_start_point;

                var current = e.ScreenLocation;
                OnDrag (new EventArgs<Point> (new Point (last_drag_point.Value.X - current.X, last_drag_point.Value.Y - current.Y)));

                last_drag_point = current;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            is_dragging = false;
            last_drag_point = null;
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Gets or sets a value indicating the orientation of the Splitter.
        /// </summary>
        public Orientation Orientation {
            get => orientation;
            set {
                if (orientation != value) {
                    orientation = value;

                    Size = new Size (Height, Width);
                    Dock = orientation == Orientation.Horizontal ? DockStyle.Left : DockStyle.Top;
                    Cursor = orientation == Orientation.Horizontal ? Cursors.SizeWestEast : Cursors.SizeNorthSouth;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the splitter.
        /// </summary>
        public int SplitterWidth {
            get => orientation == Orientation.Horizontal ? Width : Height;
            set {
                if (orientation == Orientation.Horizontal)
                    Width = value;
                else
                    Height = value;
            }
        }
    }
}
