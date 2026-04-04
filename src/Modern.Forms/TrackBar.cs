using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a slider control that allows the user to select a value
    /// from a bounded numeric range by dragging a thumb along a track.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="TrackBar"/> supports both horizontal and vertical layouts,
    /// keyboard and mouse interaction, tick mark rendering, and optional
    /// snapping to tick positions.
    /// </para>
    /// <para>
    /// The control uses <see cref="TrackBarRenderer"/> for layout calculations
    /// and painting.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var trackBar = new TrackBar
    /// {
    ///     Minimum = 0,
    ///     Maximum = 100,
    ///     Value = 25,
    ///     TickFrequency = 10,
    ///     TickStyle = TickStyle.BottomRight,
    ///     SnapToTicks = false,
    ///     Dock = DockStyle.Top
    /// };
    ///
    /// trackBar.ValueChanged += (sender, e) =>
    /// {
    ///     Console.WriteLine(trackBar.Value);
    /// };
    /// </code>
    /// </example>
    public class TrackBar : Control
    {
        private const int DEFAULT_MINIMUM = 0;
        private const int DEFAULT_MAXIMUM = 10;
        private const int DEFAULT_VALUE = 0;
        private const int DEFAULT_SMALL_CHANGE = 1;
        private const int DEFAULT_LARGE_CHANGE = 5;
        private const int DEFAULT_TICK_FREQUENCY = 1;
        private const int DEFAULT_PREFERRED_THICKNESS = 32;
        private const int DEFAULT_PREFERRED_LENGTH = 104;

        private bool thumb_pressed;
        private bool thumb_hovered;
        private int drag_offset_from_thumb_origin;

        private int minimum = DEFAULT_MINIMUM;
        private int maximum = DEFAULT_MAXIMUM;
        private int current_value = DEFAULT_VALUE;
        private int small_change = DEFAULT_SMALL_CHANGE;
        private int large_change = DEFAULT_LARGE_CHANGE;
        private int tick_frequency = DEFAULT_TICK_FREQUENCY;
        private Orientation orientation = Orientation.Horizontal;
        private TickStyle tick_style = TickStyle.BottomRight;
        private bool snap_to_ticks;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBar"/> class.
        /// </summary>
        public TrackBar ()
        {
            AutoSize = true;
            TabStop = true;

            SetAutoSizeMode (AutoSizeMode.GrowOnly);
            SetControlBehavior (ControlBehaviors.Hoverable | ControlBehaviors.Selectable);
        }

        /// <summary>
        /// Gets the default <see cref="ControlStyle"/> for all <see cref="TrackBar"/> instances.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.BackgroundColor;
            });

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <inheritdoc/>
        protected override Size DefaultSize
            => Orientation == Orientation.Horizontal
                ? new Size (DEFAULT_PREFERRED_LENGTH, DEFAULT_PREFERRED_THICKNESS)
                : new Size (DEFAULT_PREFERRED_THICKNESS, DEFAULT_PREFERRED_LENGTH);

        /// <summary>
        /// Gets or sets a value indicating whether the control automatically keeps
        /// its thickness appropriate for the current <see cref="Orientation"/>.
        /// </summary>
        public override bool AutoSize {
            get => base.AutoSize;
            set {
                if (base.AutoSize != value) {
                    base.AutoSize = value;
                    AdjustAutoSizeDimension ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount by which the value changes when Page Up or Page Down is used.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is less than zero.
        /// </exception>
        public int LargeChange {
            get => large_change;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (LargeChange), $"Value '{value}' must be greater than or equal to 0.");

                if (large_change != value) {
                    large_change = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the control range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the assigned value is less than <see cref="Minimum"/>.
        /// </exception>
        public int Maximum {
            get => maximum;
            set {
                if (maximum == value)
                    return;

                if (value < minimum)
                    throw new ArgumentOutOfRangeException (nameof (Maximum), $"Value '{value}' must be greater than or equal to Minimum.");

                maximum = value;

                if (current_value > maximum)
                    SetValueCore (maximum, raiseScroll: false);

                Invalidate ();
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the control range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the assigned value is greater than <see cref="Maximum"/>.
        /// </exception>
        public int Minimum {
            get => minimum;
            set {
                if (minimum == value)
                    return;

                if (value > maximum)
                    throw new ArgumentOutOfRangeException (nameof (Minimum), $"Value '{value}' must be less than or equal to Maximum.");

                minimum = value;

                if (current_value < minimum)
                    SetValueCore (minimum, raiseScroll: false);

                Invalidate ();
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
        /// </summary>
        public Orientation Orientation {
            get => orientation;
            set {
                if (orientation != value) {
                    orientation = value;
                    AdjustAutoSizeDimension ();
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount by which the value changes when arrow keys are used
        /// or when the mouse wheel is rotated.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is less than zero.
        /// </exception>
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

        /// <summary>
        /// Gets or sets a value indicating whether the current value should be rounded
        /// to the nearest tick position whenever it changes.
        /// </summary>
        public bool SnapToTicks {
            get => snap_to_ticks;
            set {
                if (snap_to_ticks != value) {
                    snap_to_ticks = value;

                    if (snap_to_ticks)
                        SetValueCore (current_value, raiseScroll: false);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between tick marks, expressed in value units.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is less than or equal to zero.
        /// </exception>
        public int TickFrequency {
            get => tick_frequency;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException (nameof (TickFrequency), $"Value '{value}' must be greater than 0.");

                if (tick_frequency != value) {
                    tick_frequency = value;

                    if (SnapToTicks)
                        SetValueCore (current_value, raiseScroll: false);

                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets where tick marks are drawn relative to the track.
        /// </summary>
        public TickStyle TickStyle {
            get => tick_style;
            set {
                if (tick_style != value) {
                    tick_style = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current value of the control.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is outside the <see cref="Minimum"/> and <see cref="Maximum"/> range.
        /// </exception>
        public int Value {
            get => current_value;
            set {
                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException (nameof (Value), $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'.");

                SetValueCore (value, raiseScroll: true);
            }
        }

        /// <summary>
        /// Occurs when the control is scrolled by the user or programmatically through value changes
        /// that should be treated as scroll interactions.
        /// </summary>
        public event EventHandler? Scroll;

        /// <summary>
        /// Occurs when the <see cref="Value"/> property changes.
        /// </summary>
        public event EventHandler? ValueChanged;

        /// <summary>
        /// Gets a value indicating whether the thumb is currently under the mouse pointer.
        /// </summary>
        internal bool ThumbHovered => thumb_hovered;

        /// <summary>
        /// Gets a value indicating whether the thumb is currently pressed.
        /// </summary>
        internal bool ThumbPressed => thumb_pressed;

        private TrackBarRenderer GetRenderer ()
        {
            return RenderManager.GetRenderer<TrackBarRenderer> ()
                ?? throw new InvalidOperationException ("No TrackBarRenderer has been registered.");
        }

        private void AdjustAutoSizeDimension ()
        {
            if (!AutoSize)
                return;

            if (Orientation == Orientation.Horizontal)
                Height = DEFAULT_PREFERRED_THICKNESS;
            else
                Width = DEFAULT_PREFERRED_THICKNESS;
        }

        private int Clamp (int value) => Math.Max (minimum, Math.Min (maximum, value));

        private void ChangeValueBy (int delta)
        {
            var new_value = Clamp (current_value + delta);
            SetValueCore (new_value, raiseScroll: true);
        }

        private Rectangle GetThumbBounds ()
            => GetRenderer ().GetThumbBounds (this);

        private int PositionToValue (Point location)
            => GetRenderer ().PositionToValue (this, location);

        private int SnapValueToTick (int value)
        {
            if (!SnapToTicks || tick_frequency <= 0 || maximum <= minimum)
                return Clamp (value);

            if (value <= minimum)
                return minimum;

            if (value >= maximum)
                return maximum;

            var relative = value - minimum;
            var snapped_relative = (int)Math.Round (relative / (double)tick_frequency) * tick_frequency;
            var snapped = minimum + snapped_relative;

            return Clamp (snapped);
        }

        /// <inheritdoc/>
        public override Size GetPreferredSize (Size proposedSize)
        {
            var current = Size;

            if (Orientation == Orientation.Horizontal) {
                var width = Math.Max (current.Width, DEFAULT_PREFERRED_LENGTH);
                return new Size (width, DEFAULT_PREFERRED_THICKNESS);
            }

            var height = Math.Max (current.Height, DEFAULT_PREFERRED_LENGTH);
            return new Size (DEFAULT_PREFERRED_THICKNESS, height);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown (KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Left:
                    if (Orientation == Orientation.Horizontal) {
                        ChangeValueBy (-SmallChange);
                        e.Handled = true;
                        return;
                    }

                    break;

                case Keys.Right:
                    if (Orientation == Orientation.Horizontal) {
                        ChangeValueBy (SmallChange);
                        e.Handled = true;
                        return;
                    }

                    break;

                case Keys.Up:
                    if (Orientation == Orientation.Vertical) {
                        ChangeValueBy (SmallChange);
                        e.Handled = true;
                        return;
                    }

                    break;

                case Keys.Down:
                    if (Orientation == Orientation.Vertical) {
                        ChangeValueBy (-SmallChange);
                        e.Handled = true;
                        return;
                    }

                    break;

                case Keys.PageUp:
                    ChangeValueBy (LargeChange);
                    e.Handled = true;
                    return;

                case Keys.PageDown:
                    ChangeValueBy (-LargeChange);
                    e.Handled = true;
                    return;

                case Keys.Home:
                    SetValueCore (Minimum, raiseScroll: true);
                    e.Handled = true;
                    return;

                case Keys.End:
                    SetValueCore (Maximum, raiseScroll: true);
                    e.Handled = true;
                    return;
            }

            base.OnKeyDown (e);
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            Select ();

            var thumb_bounds = GetThumbBounds ();

            if (thumb_bounds.Contains (e.Location)) {
                thumb_pressed = true;

                drag_offset_from_thumb_origin = Orientation == Orientation.Horizontal
                    ? Math.Max (0, Math.Min (thumb_bounds.Width, e.X - thumb_bounds.X))
                    : Math.Max (0, Math.Min (thumb_bounds.Height, e.Y - thumb_bounds.Y));

                Invalidate ();
                return;
            }

            SetValueCore (PositionToValue (e.Location), raiseScroll: true);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            if (thumb_hovered) {
                thumb_hovered = false;
                Invalidate ();
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            var renderer = GetRenderer ();
            var thumb_bounds = renderer.GetThumbBounds (this);

            var new_hover_state = thumb_bounds.Contains (e.Location);

            if (thumb_hovered != new_hover_state) {
                thumb_hovered = new_hover_state;
                Invalidate ();
            }

            if (!thumb_pressed)
                return;

            var new_value = renderer.PositionToValueFromThumb (this, e.Location, drag_offset_from_thumb_origin);
            SetValueCore (new_value, raiseScroll: true);
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (thumb_pressed) {
                thumb_pressed = false;
                Invalidate ();
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (!Enabled)
                return;

            if (e.Delta.Y > 0)
                ChangeValueBy (SmallChange);
            else if (e.Delta.Y < 0)
                ChangeValueBy (-SmallChange);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);
            RenderManager.Render (this, e);
        }

        /// <inheritdoc/>
        protected override void OnSizeChanged (EventArgs e)
        {
            base.OnSizeChanged (e);
            Invalidate ();
        }

        /// <inheritdoc/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (AutoSize) {
                if (Orientation == Orientation.Horizontal)
                    height = DEFAULT_PREFERRED_THICKNESS;
                else
                    width = DEFAULT_PREFERRED_THICKNESS;
            }

            base.SetBoundsCore (x, y, width, height, specified);
        }

        /// <summary>
        /// Raises the <see cref="Scroll"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnScroll (EventArgs e) => Scroll?.Invoke (this, e);

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnValueChanged (EventArgs e) => ValueChanged?.Invoke (this, e);

        private void SetValueCore (int value, bool raiseScroll)
        {
            value = Clamp (value);
            value = SnapValueToTick (value);

            if (current_value == value)
                return;

            current_value = value;

            if (raiseScroll)
                OnScroll (EventArgs.Empty);

            OnValueChanged (EventArgs.Empty);
            Invalidate ();
        }
    }
}
