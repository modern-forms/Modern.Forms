using System;
using System.Drawing;

namespace Modern.Forms
{
    public class Splitter : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private Orientation orientation;
        private bool is_dragging;
        private Point drag_start_point;
        private Point? last_drag_point;

        public Splitter ()
        {
            Dock = DockStyle.Left;
            Cursor = Cursors.SizeWestEast;
        }
        
        public event EventHandler<EventArgs<Point>> Drag;

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

        public int SplitterWidth {
            get => orientation == Orientation.Horizontal ? Width : Height;
            set {
                if (orientation == Orientation.Horizontal)
                    Width = value;
                else
                    Height = value;
            }
        }

        protected void OnDrag (EventArgs<Point> e) => Drag?.Invoke (this, e);

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            is_dragging = true;
            drag_start_point = e.ScreenLocation;
        }

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

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            is_dragging = false;
            last_drag_point = null;
        }
    }
}
