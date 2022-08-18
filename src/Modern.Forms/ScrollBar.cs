using System;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents the base class of a ScrollBar control.
    /// </summary>
    public abstract class ScrollBar : Control
    {
        private int large_change = 10;
        private int maximum = 100;
        private int minimum = 0;
        private int current_value = 0;
        private int small_change = 1;
        private bool thumb_pressed;
        private int thumbclick_offset;              // Position of the last button-down event relative to the thumb edge

        private readonly bool vertical;

        internal int thumb_drag_position;     // Current pixel of the midpoint of the thumb drag 

        /// <summary>
        /// Initializes a new instance of the ScrollBar class.
        /// </summary>
        protected ScrollBar (bool vertical = false)
        {
            this.vertical = vertical;
            TabStop = false;
        }

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = Theme.DarkNeutralGray);

        /// <summary>
        /// Gets or sets the amount the ScrollBar will change when clicked in the track area.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum value the ScrollBar will allow.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minimum value the ScrollBar will allow.
        /// </summary>
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

        /// <summary>
        /// Raised when the ScrollBar is scrolled.
        /// </summary>
        public event EventHandler<ScrollEventArgs>? Scroll;

        /// <summary>
        /// Gets or sets the amount the ScrollBar will change when the increment or decrement arrows are clicked.
        /// </summary>
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

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets or sets the current value of the ScrollBar.
        /// </summary>
        public int Value {
            get => current_value;
            set {
                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException (nameof (Value), $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");

                UpdateFromValue (value);
            }
        }

        /// <summary>
        /// Raised when the value of the ScrollBar changes.
        /// </summary>
        public event EventHandler? ValueChanged;

        // The number of possible ScrollBar values.
        private int PossibleValuesCount => maximum - minimum + 1;

        // Retrieves the effective track bounds from the renderer.
        private Rectangle GetEffectiveTrackBounds () => RenderManager.GetRenderer<ScrollBarRenderer> ()!.GetEffectiveTrackBounds (this);

        private ScrollBarElement GetElementAtLocation (Point location)
        {
            var renderer = RenderManager.GetRenderer<ScrollBarRenderer> ()!;

            if (renderer.GetDecrementArrowBounds (this).Contains (location))
                return ScrollBarElement.DecrementArrow;

            if (renderer.GetIncrementArrowBounds (this).Contains (location))
                return ScrollBarElement.IncrementArrow;

            if (renderer.GetThumbDragBounds (this).Contains (location))
                return ScrollBarElement.Thumb;

            if (renderer.GetDecrementTrackBounds (this).Contains (location))
                return ScrollBarElement.DecrementTrack;

            if (renderer.GetIncrementTrackBounds (this).Contains (location))
                return ScrollBarElement.IncrementTrack;

            // In theory this shouldn't be possible...
            return ScrollBarElement.None;
        }

        /// <inheritdoc/>
        protected override void OnSizeChanged (EventArgs e)
        {
            base.OnSizeChanged (e);

            UpdateFromValue (current_value);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (thumb_pressed == true) {
                UpdateFromPoint ((vertical ? e.Y : e.X) - thumbclick_offset);
                OnScroll (new ScrollEventArgs (ScrollEventType.ThumbTrack, Value));
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            thumb_pressed = false;
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (!Enabled)
                return;

            if (e.Delta.Y != 0)
                UpdateFromValue (Value - (e.Delta.Y * SmallChange));
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
        protected virtual void OnScroll (ScrollEventArgs e)
        {
            e.NewValue = Math.Max (e.NewValue, Minimum);
            e.NewValue = Math.Min (e.NewValue, Maximum);

            Scroll?.Invoke (this, e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        protected virtual void OnValueChanged (EventArgs e) => ValueChanged?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnVisibleChanged (EventArgs e)
        {
            base.OnVisibleChanged (e);

            if (Visible)
                UpdateFromValue (Value);
        }

        /// <inheritdoc/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateFromValue (Value);
        }

        // Updates ScrollBar value from a thumb drag position.
        private void UpdateFromPoint (int pixel)
        {
            if (thumb_drag_position == pixel)
                return;

            var effective_track_bounds = GetEffectiveTrackBounds ();

            pixel = Math.Max (pixel, vertical ? effective_track_bounds.Top : effective_track_bounds.Left);
            pixel = Math.Min (pixel, vertical ? effective_track_bounds.Bottom : effective_track_bounds.Right);

            var position_percent =
                vertical ? (double)(pixel - effective_track_bounds.Top) / effective_track_bounds.Height
                         : (double)(pixel - effective_track_bounds.Left) / effective_track_bounds.Width;

            var value_position = (int)(position_percent * (PossibleValuesCount - 1));

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

        // Updates thumb drag position from a ScrollBar value.
        private void UpdateFromValue (int value)
        {
            value = Math.Max (value, minimum);
            value = Math.Min (value, maximum);

            var value_percent = (double)(value - minimum) / (PossibleValuesCount - 1);

            var effective_track_bounds = GetEffectiveTrackBounds ();

            var new_pos =
                vertical ? effective_track_bounds.Y + (value_percent * effective_track_bounds.Height)
                         : effective_track_bounds.X + (value_percent * effective_track_bounds.Width);

            thumb_drag_position = (int)new_pos;

            Invalidate ();

            if (current_value == value)
                return;

            current_value = value;

            OnValueChanged (EventArgs.Empty);
        }

        private enum ScrollBarElement
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
