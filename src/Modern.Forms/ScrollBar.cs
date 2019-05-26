using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO:
    // Disabled styles
    // Timer based repeat
    public abstract class ScrollBar : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = Theme.DarkNeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private int large_change = 10;
        private int maximum = 100;
        private int minimum = 0;
        private int current_value = 0;
        private int small_change = 1;

        protected bool vertical;
        protected int thumb_drag_position;      // Current pixel of the midpoint of the thumb drag 
        protected bool thumb_pressed;
        protected int thumbclick_offset;		// Position of the last button-down event relative to the thumb edge

        protected const int thumb_min_size = 8;
        protected const int thumb_notshown_size = 40;

        public ScrollBar ()
        {
            TabStop = false;
        }

        public event EventHandler<ScrollEventArgs> Scroll;
        public event EventHandler ValueChanged;

        public int LargeChange {
            get => Math.Min (large_change, maximum - minimum + 1);
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (LargeChange), $"Value '{value}' must be greater than or equal to 0.");

                if (large_change != value) {
                    large_change = value;
                    UpdateFromValue (Value);
                }
            }
        }

        public int Maximum {
            get => maximum;
            set {
                if (maximum != value) {
                    maximum = value;

                    if (maximum < minimum)
                        minimum = maximum;
                    if (Value > maximum)
                        Value = maximum;

                    UpdateFromValue (Value);
                }
            }
        }

        public int Minimum {
            get => minimum;
            set {
                if (minimum != value) {
                    minimum = value;

                    if (minimum > maximum)
                        maximum = minimum;
                    if (Value < minimum)
                        Value = minimum;

                    UpdateFromValue (Value);
                }
            }
        }

        public int SmallChange {
            get => small_change;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (SmallChange), $"Value '{value}' must be greater than or equal to 0.");

                if (small_change != value) {
                    small_change = value;
                    Invalidate ();
                }
            }
        }

        public int Value {
            get => current_value;
            set {
                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException (nameof (Value), $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");

                UpdateFromValue (value);
            }
        }

        protected abstract int ArrowButtonSize { get; }
        protected abstract int ThumbDragSize { get; }
        protected abstract Rectangle DecrementArrowBounds { get; }
        protected abstract ArrowDirection DecrementArrowDirection { get; }
        protected abstract Rectangle IncrementArrowBounds { get; }
        protected abstract ArrowDirection IncrementArrowDirection { get; }
        protected abstract Rectangle ThumbDragBounds { get; }
        protected abstract Rectangle DecrementTrackBounds { get; }
        protected abstract Rectangle IncrementTrackBounds { get; }
        protected abstract Rectangle TotalTrackBrounds { get; }
        protected abstract Rectangle EffectiveTrackBounds { get; }

        protected int PossibleValues => maximum - minimum + 1;

        protected ScrollBarElement GetElementAtLocation (Point location)
        {
            if (DecrementArrowBounds.Contains (location))
                return ScrollBarElement.DecrementArrow;

            if (IncrementArrowBounds.Contains (location))
                return ScrollBarElement.IncrementArrow;

            if (ThumbDragBounds.Contains (location))
                return ScrollBarElement.Thumb;

            if (DecrementTrackBounds.Contains (location))
                return ScrollBarElement.DecrementTrack;

            if (IncrementTrackBounds.Contains (location))
                return ScrollBarElement.IncrementTrack;

            // In theory this shouldn't be possible...
            return ScrollBarElement.None;
        }

        protected override void OnSizeChanged (EventArgs e)
        {
            base.OnSizeChanged (e);

            UpdateFromValue (current_value);
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            switch (GetElementAtLocation (e.Location)) {
                case ScrollBarElement.DecrementArrow:
                    Value = Math.Max (Value - SmallChange, Minimum);
                    break;
                case ScrollBarElement.DecrementTrack:
                    Value = Math.Max (Value - LargeChange, Minimum);
                    break;
                case ScrollBarElement.Thumb:
                    thumb_pressed = true;
                    thumbclick_offset = (vertical ? e.Y : e.X) - thumb_drag_position;
                    break;
                case ScrollBarElement.IncrementTrack:
                    Value = Math.Min (Value + LargeChange, Maximum);
                    break;
                case ScrollBarElement.IncrementArrow:
                    Value = Math.Min (Value + SmallChange, Maximum);
                    break;
            }
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (thumb_pressed == true) {
                UpdateFromPoint ((vertical ? e.Y : e.X) - thumbclick_offset);
                OnScroll (new ScrollEventArgs (ScrollEventType.ThumbTrack, Value));
            }
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            thumb_pressed = false;
        }

        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (e.Delta.Y != 0)
                UpdateFromValue (Value - (e.Delta.Y * SmallChange));
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            var top_arrow_area = DecrementArrowBounds;
            top_arrow_area.Width -= 1;
            top_arrow_area.Height -= 1;

            var bottom_arrow_area = IncrementArrowBounds;
            bottom_arrow_area.Width -= 1;
            bottom_arrow_area.Height -= 1;

            // Top Arrow
            e.Canvas.FillRectangle (top_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (top_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (top_arrow_area, Theme.BorderGray, DecrementArrowDirection);

            // Bottom Arrow
            e.Canvas.FillRectangle (bottom_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (bottom_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (bottom_arrow_area, Theme.BorderGray, IncrementArrowDirection);

            // Grip
            var thumb_bounds = ThumbDragBounds;

            if (thumb_bounds.Width > 0 && thumb_bounds.Height > 0) {
                e.Canvas.FillRectangle (thumb_bounds, SKColors.White);
                e.Canvas.DrawRectangle (thumb_bounds, Theme.BorderGray);
            }
        }

        protected virtual void OnScroll (ScrollEventArgs e)
        {
            e.NewValue = Math.Max (e.NewValue, Minimum);
            e.NewValue = Math.Min (e.NewValue, Maximum);

            Scroll?.Invoke (this, e);
        }

        protected virtual void OnValueChanged (EventArgs e) => ValueChanged?.Invoke (this, e);

        protected override void OnVisibleChanged (EventArgs e)
        {
            base.OnVisibleChanged (e);

            if (Visible)
                UpdateFromValue (Value);
        }

        protected void UpdateFromValue (int value)
        {
            value = Math.Max (value, minimum);
            value = Math.Min (value, maximum);

            var value_percent = (double)(value - minimum) / (PossibleValues - 1);

            var new_pos =
                vertical ? EffectiveTrackBounds.Y + (value_percent * EffectiveTrackBounds.Height)
                         : EffectiveTrackBounds.X + (value_percent * EffectiveTrackBounds.Width);

            thumb_drag_position = (int)new_pos;

            if (current_value == value)
                return;

            current_value = value;

            OnValueChanged (EventArgs.Empty);

            Invalidate ();
        }

        protected void UpdateFromPoint (int pixel)
        {
            if (thumb_drag_position == pixel)
                return;

            pixel = Math.Max (pixel, vertical ? EffectiveTrackBounds.Top : EffectiveTrackBounds.Left);
            pixel = Math.Min (pixel, vertical ? EffectiveTrackBounds.Bottom : EffectiveTrackBounds.Right);

            var position_percent = 
                vertical ? (double)(pixel - EffectiveTrackBounds.Top) / EffectiveTrackBounds.Height
                         : (double)(pixel - EffectiveTrackBounds.Left) / EffectiveTrackBounds.Width;

            var value_position = (int)(position_percent * (PossibleValues - 1));

            var new_value = minimum + value_position;

            thumb_drag_position = pixel;

            if (current_value != new_value) {
                current_value = new_value;
                OnValueChanged (EventArgs.Empty);
            }

            // We need to invalidate even if the value didn't change, because the position
            // changed, but each value may span multiple pixels
            Invalidate ();
        }

        protected enum ScrollBarElement
        {
            None,
            DecrementArrow,
            DecrementTrack,
            Thumb,
            IncrementTrack,
            IncrementArrow
        }
    }
}
