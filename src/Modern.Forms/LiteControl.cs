using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;

namespace Modern.Forms
{
    public class LiteControl : ILayoutable, IDisposable
    {
        public static ControlStyle DefaultStyle = new ControlStyle (null,
            (style) => {
                style.ForegroundColor = ModernTheme.DarkTextColor;
                style.BackgroundColor = ModernTheme.NeutralGray;
                style.Font = ModernTheme.UIFont;
                style.FontSize = ModernTheme.FontSize;
                style.Border.Radius = 0;
                style.Border.Color = ModernTheme.BorderGray;
                style.Border.Width = 0;
            });

        public static ControlStyle DefaultStyleHover = new ControlStyle (DefaultStyle);

        private SKBitmap back_buffer;
        private ControlBehaviors behaviors;
        private LiteControl current_mouse_in;
        private bool is_captured;
        private bool is_selected;
        private Rectangle bounds;
        private string text;

        public virtual ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public virtual ControlStyle StyleHover { get; } = new ControlStyle (DefaultStyleHover);

        public bool IsHovering { get; private set; }

        public virtual ControlStyle CurrentStyle => IsHovering ? StyleHover : Style;

        public LiteControl ()
        {
            Controls = new LiteControlCollection (this);

            Width = DefaultSize.Width;
            Height = DefaultSize.Height;

            behaviors = ControlBehaviors.Selectable;
        }

        public event EventHandler<MouseEventArgs> Click;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;

        public virtual AnchorStyles Anchor { get; set; } = AnchorStyles.Left | AnchorStyles.Top;

        public Rectangle Bounds {
            get => bounds;
            set => SetBounds (value.Left, value.Top, value.Width, value.Height);
        }

        public bool CanSelect {
            get {
                if (!behaviors.HasFlag (ControlBehaviors.Selectable))
                    return false;

                var parent = this;

                while (parent != null) {
                    if (!parent.Visible || !parent.Enabled)
                        return false;

                    parent = parent.Parent;
                }

                return true;
            }
        }

        public Rectangle ClientBounds {
            get {
                var x = CurrentStyle.Border.Left.GetWidth ();
                var y = CurrentStyle.Border.Top.GetWidth ();
                var w = Width - CurrentStyle.Border.Right.GetWidth () - x;
                var h = Height - CurrentStyle.Border.Bottom.GetWidth () - y;
                return new Rectangle (x, y, w, h);
            }
        }

        public DockStyle Dock { get; set; }

        public void DoLayout ()
        {
            new System.Windows.Forms.Layout.ModernDefaultLayout ().Layout (this, null);

            foreach (var child in Controls)
                child.DoLayout ();
        }

        public bool Enabled { get; set; } = true;

        public int Height {
            get => bounds.Height;
            set => SetBounds (bounds.X, bounds.Y, bounds.Width, value, BoundsSpecified.Height);
        }

        public void Invalidate ()
        {
            FindForm ()?.Invalidate ();
        }

        public void Invalidate (Rectangle rectangle)
        {
            Invalidate ();
        }

        public bool Contains (LiteControl control)
        {
            // Is control one of our children or grandchildren
            while (control != null) {
                control = control.Parent;

                if (control == this)
                    return true;
            }

            return false;
        }

        public LiteControlCollection Controls { get; }

        public Cursor Cursor { get; set; }

        public LiteControlAdapter FindAdapter ()
        {
            if (this is LiteControlAdapter adapter)
                return adapter;

            return Parent?.FindAdapter ();
        }

        public ModernForm FindForm ()
        {
            if (this is LiteControlAdapter adapter)
                return adapter.ParentForm;

            return Parent?.FindForm ();
        }

        public IContainerControl GetContainerControl ()
        {
            var current = this;

            while (current != null) {
                if (current is IContainerControl container)
                    return container;

                current = current.Parent;
            }

            return null;
        }

        public int Left {
            get => bounds.Left;
            set => SetBounds (value, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.X);
        }
        public Point Location {
            get => bounds.Location;
            set => SetBounds (value.X, value.Y, bounds.Width, bounds.Height, BoundsSpecified.Location);
        }

        public LiteControl Parent { get; set; }

        public Size Size {
            get => bounds.Size;
            set => SetBounds (bounds.X, bounds.Y, value.Width, value.Height, BoundsSpecified.Size);
        }

        public int TabIndex { get; set; } = -1;

        public bool TabStop { get; set; } = true;

        public int Top {
            get => bounds.Top;
            set => SetBounds (bounds.X, value, bounds.Width, bounds.Height, BoundsSpecified.Y);
        }

        public string Text {
            get => text;
            set {
                if (text == value)
                    return;

                text = value;

                if (behaviors.HasFlag (ControlBehaviors.InvalidateOnTextChanged))
                    Invalidate ();
            }
        }

        public int Width {
            get => bounds.Width;
            set => SetBounds (bounds.X, bounds.Y, value, bounds.Height, BoundsSpecified.Width);
        }

        protected virtual Size DefaultSize => Size.Empty;

        public virtual Padding Margin => new Padding (3);

        public MouseEventArgs MouseEventsForControl (MouseEventArgs e, LiteControl control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.Left, e.Location.Y - control.Top, e.Delta);
        }

        public bool Selected => is_selected;

        public void Select ()
        {
            if (is_selected)
                return;

            is_selected = true;

            var adapter = FindAdapter ();

            if (adapter != null)
                adapter.SelectedControl = this;

            Invalidate ();
        }

        public bool SelectNextControl (LiteControl start, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            LiteControl c;

            if (!Contains (start) || (!nested && (start.Parent != this)))
                start = null;

            c = start;

            do {
                c = GetNextControl (c, forward);

                if (c is null) {
                    if (wrap) {
                        wrap = false;
                        continue;
                    }

                    break;
                }

                if (c.CanSelect && ((c.Parent == this) || nested) && (c.TabStop || !tabStopOnly)) {
                    c.Select ();
                    return true;
                }

            } while (c != start);

            return false;
        }

        public LiteControl GetNextControl (LiteControl start, bool forward = true)
        {
            if (Controls.Count == 0)
                return null;

            // Ignore start control if it isn't our child/grandchild
            if (!Contains (start))
                start = null;

            // If the start control is the only control, return null
            if (start != null && Controls.Count == 1)
                return null;

            // See if we need recurse into the start control
            var child_control = start?.GetNextControl (start, forward);

            if (child_control != null)
                return child_control;

            // If this is a grandchild, we need to give the parent a chance to move next
            while (start?.Parent != null && start?.Parent != this) {
                var old_start = start;
                start = start.Parent;

                var child_control2 = start?.GetNextControl (old_start, forward);

                if (child_control2 != null)
                    return child_control2;
            }

            // Build an array sorted by TabIndex then element order (OrderBy is stable)
            var array = Controls.OrderBy (c => c.TabIndex).ToList ();

            // If we don't have a start control, just return the first or last control
            if (start == null)
                return forward ? array[0] : array[array.Count - 1];

            var start_index = array.IndexOf (start);

            // Find the "next" control in the array
            if (forward && start_index + 1 < array.Count)
                return array[start_index + 1];

            if (!forward && start_index > 0)
                return array[start_index - 1];

            return null;
        }

        internal void Deselect ()
        {
            is_selected = false;
            Invalidate ();
        }

        public bool Visible { get; set; } = true;

        internal void RaiseClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseClick (MouseEventsForControl (e, child));
            else
                OnClick (e);
        }

        protected virtual void OnClick (MouseEventArgs e) => Click?.Invoke (this, e);

        internal void RaiseDoubleClick (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseDoubleClick (MouseEventsForControl (e, child));
            else
                OnDoubleClick (e);
        }

        protected virtual void OnDoubleClick (MouseEventArgs e)
        {
        }

        protected virtual void OnLocationChanged (EventArgs e) => LocationChanged?.Invoke (this, e);

        internal void RaiseKeyDown (KeyEventArgs e)
        {
            if (this is LiteControlAdapter adapter) {
                adapter.SelectedControl?.RaiseKeyDown (e);
                return;
            }

            OnKeyDown (e);
        }

        protected virtual void OnKeyDown (KeyEventArgs e) => KeyDown?.Invoke (this, e);

        internal void RaiseKeyPress (KeyPressEventArgs e)
        {
            if (this is LiteControlAdapter adapter) {
                // Tab
                if (e.KeyChar == 9) {
                    SelectNextControl (adapter.SelectedControl, !XplatUI.State.ModifierKeys.HasFlag (Keys.Shift), true, true, true);
                    e.Handled = true;
                    return;
                }

                adapter.SelectedControl?.RaiseKeyPress (e);
                return;
            }

            OnKeyPress (e);
        }

        protected virtual void OnKeyPress (KeyPressEventArgs e) => KeyPress?.Invoke (this, e);

        internal void RaiseMouseDown (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseDown (MouseEventsForControl (e, child));
            else {
                Select ();
                Capture = true;
                OnMouseDown (e);
            }
        }

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
        }

        internal void RaiseMouseEnter (MouseEventArgs e)
        {
            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseEnter (MouseEventsForControl (e, child));
            else
                OnMouseEnter (e);
        }

        protected virtual void OnMouseEnter (MouseEventArgs e)
        {
            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = true;
                Invalidate ();
            }
        }

        internal void RaiseMouseLeave (EventArgs e)
        {
            if (current_mouse_in != null)
                current_mouse_in.RaiseMouseLeave (e);

            current_mouse_in = null;

            OnMouseLeave (e);
        }

        protected virtual void OnMouseLeave (EventArgs e)
        {
            if (behaviors.HasFlag (ControlBehaviors.Hoverable)) {
                IsHovering = false;
                Invalidate ();
            }
        }

        internal void RaiseMouseMove (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.FirstOrDefault (c => c.is_captured);

            if (captured != null) {
                captured.RaiseMouseMove (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (current_mouse_in != null && current_mouse_in != child) {
                current_mouse_in.RaiseMouseLeave (e);
                current_mouse_in = null;
            }

            if (current_mouse_in == null && child != null)
                child.RaiseMouseEnter (MouseEventsForControl (e, child));

            current_mouse_in = child;

            if (child != null)
                child?.RaiseMouseMove (MouseEventsForControl (e, child));
            else
                OnMouseMove (e);
        }

        protected virtual void OnMouseMove (MouseEventArgs e)
        {
        }

        internal void RaiseMouseUp (MouseEventArgs e)
        {
            // If something has the mouse captured, they get all the events
            var captured = Controls.FirstOrDefault (c => c.is_captured);

            if (captured != null) {
                captured.RaiseMouseUp (MouseEventsForControl (e, captured));
                return;
            }

            var child = Controls.FirstOrDefault (c => c.Bounds.Contains (e.Location));

            if (child != null)
                child.RaiseMouseUp (MouseEventsForControl (e, child));
            else {
                Capture = false;
                OnMouseUp (e);
            }
        }

        protected virtual void OnMouseUp (MouseEventArgs e)
        {
        }

        internal void RaisePaint (SKPaintEventArgs e) => OnPaint (e);

        protected virtual void OnPaint (SKPaintEventArgs e)
        {
            foreach (var control in Controls.Where (c => c.Visible)) {
                var info = new SKImageInfo (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var buffer = control.GetBackBuffer ();

                using (var canvas = new SKCanvas (buffer)) {
                    // start drawing
                    var args = new SKPaintEventArgs (null, info, canvas);

                    control.RaisePaintBackground (args);
                    control.RaisePaint (args);

                    canvas.Flush ();
                }

                e.Canvas.DrawBitmap (buffer, control.Left, control.Top);
            }
        }

        protected virtual void OnSizeChanged (EventArgs e) => SizeChanged?.Invoke (this, e);

        protected void SetControlBehavior (ControlBehaviors behavior, bool value = true)
        {
            if (value)
                behaviors |= behavior;
            else
                behaviors &= behavior;
        }

        internal void RaisePaintBackground (SKPaintEventArgs e) => OnPaintBackground (e);

        protected virtual void OnPaintBackground (SKPaintEventArgs e)
        {
            e.Canvas.DrawBackground (Bounds, CurrentStyle);
            e.Canvas.DrawBorder (Bounds, CurrentStyle);
        }

        internal SKBitmap GetBackBuffer ()
        {
            if (back_buffer == null || back_buffer.Width != Width || back_buffer.Height != Height) {
                FreeBitmap ();
                back_buffer = new SKBitmap (Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            }

            return back_buffer;
        }

        private void FreeBitmap ()
        {
            if (back_buffer != null) {
                back_buffer.Dispose ();
                back_buffer = null;
            }
        }

        public virtual Size GetPreferredSize (Size proposedSize) => new Size (Width, Height);

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            var moved = bounds.X != x || bounds.Y != y;
            var resized = bounds.Width != width || bounds.Height != height;

            bounds.X = x;
            bounds.Y = y;
            bounds.Width = width;
            bounds.Height = height;

            if (moved)
                OnLocationChanged (EventArgs.Empty);

            if (resized)
                OnSizeChanged (EventArgs.Empty);
        }

        public bool Capture {
            get => is_captured;
            set {
                is_captured = value;

                if (Parent != null)
                    Parent.Capture = value;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue) {
                FreeBitmap ();

                foreach (var c in Controls)
                    c.Dispose (disposing);

                disposedValue = true;
            }
        }

        ~LiteControl ()
        {
            Dispose (false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose ()
        {
            Dispose (true);
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
