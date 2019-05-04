using System;
using System.Drawing;
using System.Windows.Forms;

namespace Modern.Forms
{
    // TODO:
    // Disabled styles
    // Fix bug where you can't scroll to maximum
    // Timer based repeat
    // Mouse wheel
    public abstract class ScrollBarControl : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => style.BackgroundColor = ModernTheme.NeutralGray);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private int large_change = 10;
        private int maximum = 100;
        private int minimum = 0;
        private float pixel_per_pos = 0;
        protected int position = 0;
        private int small_change = 1;
        protected bool vertical;

        private bool use_manual_thumb_size;
        private int manual_thumb_size;
        protected int scrollbutton_height;
        protected int scrollbutton_width;
        protected int thumb_size = 40;
        protected bool thumb_pressed;
        private int lastclick_pos;              // Position of the last button-down event
        protected int thumbclick_offset;		// Position of the last button-down event relative to the thumb edge

        protected Rectangle thumb_area = new Rectangle ();
        protected Rectangle thumb_pos = new Rectangle ();
        private Rectangle dirty;

        private const int thumb_min_size = 8;
        private const int thumb_notshown_size = 40;

        public ScrollBarControl ()
        {
        }

        public event ScrollEventHandler Scroll;
        public event EventHandler ValueChanged;

        public int LargeChange {
            get => Math.Min (large_change, maximum - minimum + 1);
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException (nameof (LargeChange), $"Value '{value}' must be greater than or equal to 0.");

                if (large_change != value) {
                    large_change = value;

                    // thumb area depends on large change value,
                    // so we need to recalculate it.
                    CalcThumbArea ();
                    UpdatePos (Value, true);
                    InvalidateDirty ();
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

                    // thumb area depends on maximum value,
                    // so we need to recalculate it.
                    CalcThumbArea ();
                    UpdatePos (Value, true);
                    InvalidateDirty ();
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

                    // thumb area depends on minimum value,
                    // so we need to recalculate it.
                    CalcThumbArea ();
                    UpdatePos (Value, true);
                    InvalidateDirty ();
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
                    UpdatePos (Value, true);
                    InvalidateDirty ();
                }
            }
        }

        public int Value {
            get => position;
            set {
                if (value < minimum || value > maximum)
                    throw new ArgumentOutOfRangeException (nameof (Value), $"'{value}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'");

                if (position != value) {
                    position = value;

                    OnValueChanged (EventArgs.Empty);

                    if (IsHandleCreated) {
                        var thumb_rect = thumb_pos;

                        UpdateThumbPos ((vertical ? thumb_area.Y : thumb_area.X) + (int)(((float)(position - minimum)) * pixel_per_pos), false, false);

                        MoveThumb (thumb_rect, vertical ? thumb_pos.Y : thumb_pos.X);
                    }
                }
            }
        }

        protected abstract Rectangle DecrementArrowBounds { get; }
        protected abstract ArrowDirection DecrementArrowDirection { get; }
        protected abstract Rectangle IncrementArrowBounds { get; }
        protected abstract ArrowDirection IncrementArrowDirection { get; }
        protected Rectangle ThumbBounds => thumb_pos;
        protected abstract Rectangle DecrementTrackBounds { get; }
        protected abstract Rectangle IncrementTrackBounds { get; }

        protected ScrollBarElement GetElementAtLocation (Point location)
        {
            if (DecrementArrowBounds.Contains (location))
                return ScrollBarElement.DecrementArrow;

            if (IncrementArrowBounds.Contains (location))
                return ScrollBarElement.IncrementArrow;

            if (ThumbBounds.Contains (location))
                return ScrollBarElement.Thumb;

            if (DecrementTrackBounds.Contains (location))
                return ScrollBarElement.DecrementTrack;

            if (IncrementTrackBounds.Contains (location))
                return ScrollBarElement.IncrementTrack;

            // In theory this shouldn't be possible...
            return ScrollBarElement.None;
        }

        protected override void OnHandleCreated (System.EventArgs e)
        {
            base.OnHandleCreated (e);

            CalcButtonSizes ();
            CalcThumbArea ();
            UpdateThumbPos (thumb_area.Y + (int)(((float)(position - minimum)) * pixel_per_pos), true, false);
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (Enabled == false || (e.Button & MouseButtons.Left) == 0)
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
                    thumbclick_offset = e.Y - thumb_pos.Y;
                    lastclick_pos = e.Y;
                    break;
                case ScrollBarElement.IncrementTrack:
                    Value = Math.Min (Value + LargeChange, Maximum);
                    break;
                case ScrollBarElement.IncrementArrow:
                    Value = Math.Min (Value + SmallChange, Maximum);
                    break;
            }
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (thumb_pressed)
                thumb_pressed = false;
        }

        protected virtual void OnScroll (ScrollEventArgs e)
        {
            e.NewValue = Math.Max (e.NewValue, Minimum);
            e.NewValue = Math.Min (e.NewValue, Maximum);

            Scroll?.Invoke (this, e);
        }

        protected virtual void OnValueChanged (EventArgs e) => ValueChanged?.Invoke (this, e);

        private void CalcButtonSizes ()
        {
            if (vertical) {
                if (Height < 15 * 2)
                    scrollbutton_height = Height / 2;
                else
                    scrollbutton_height = 15;

            } else {
                if (Width < 15 * 2)
                    scrollbutton_width = Width / 2;
                else
                    scrollbutton_width = 15;
            }
        }

        private void CalcThumbArea ()
        {
            var lchange = use_manual_thumb_size ? manual_thumb_size : LargeChange;

            // Thumb area
            if (vertical) {

                thumb_area.Height = Height - scrollbutton_height - scrollbutton_height;
                thumb_area.X = 0;
                thumb_area.Y = scrollbutton_height;
                thumb_area.Width = Width;

                if (Height < thumb_notshown_size)
                    thumb_size = 0;
                else {
                    var per = ((double)lchange / (double)((1 + maximum - minimum)));
                    thumb_size = 1 + (int)(thumb_area.Height * per);

                    if (thumb_size < thumb_min_size)
                        thumb_size = thumb_min_size;

                    // Give the user something to drag if LargeChange is zero
                    if (LargeChange == 0)
                        thumb_size = 17;
                }

                pixel_per_pos = ((float)(thumb_area.Height - thumb_size) / (float)((maximum - minimum - lchange) + 1));

            } else {

                thumb_area.Y = 0;
                thumb_area.X = scrollbutton_width;
                thumb_area.Height = Height;
                thumb_area.Width = Width - scrollbutton_width - scrollbutton_width;

                if (Width < thumb_notshown_size)
                    thumb_size = 0;
                else {
                    var per = ((double)lchange / (double)((1 + maximum - minimum)));
                    thumb_size = 1 + (int)(thumb_area.Width * per);

                    if (thumb_size < thumb_min_size)
                        thumb_size = thumb_min_size;

                    // Give the user something to drag if LargeChange is zero
                    if (LargeChange == 0)
                        thumb_size = 17;
                }

                pixel_per_pos = ((float)(thumb_area.Width - thumb_size) / (float)((maximum - minimum - lchange) + 1));
            }
        }

        private void Dirty (Rectangle r)
        {
            if (dirty == Rectangle.Empty) {
                dirty = r;
                return;
            }
            dirty = Rectangle.Union (dirty, r);
        }

        private void InvalidateDirty ()
        {
            Invalidate ();
            //Invalidate (dirty);
            Update ();
            dirty = Rectangle.Empty;
        }

        private int MaximumAllowed {
            get {
                return use_manual_thumb_size ? maximum - manual_thumb_size + 1 :
                    maximum - LargeChange + 1;
            }
        }

        protected void MoveThumb (Rectangle original_thumbpos, int value)
        {
            /* so, the reason this works can best be
     * described by the following 1 dimensional
     * pictures
     *
     * say you have a scrollbar thumb positioned
     * thusly:
     *
     * <---------------------|          |------------------------------>
     *
     * and you want it to end up looking like this:
     *
     * <-----------------------------|          |---------------------->
     *
     * that can be done with the scrolling api by
     * extending the rectangle to encompass both
     * positions:
     *
     *               start of range          end of range
     *                       \	    	    /
     * <---------------------|	    |-------|---------------------->
     *
     * so, we end up scrolling just this little region:
     *
     *                       |          |-------|
     *
     * and end up with       ********|          |
     *
     * where ****** is space that is automatically
     * redrawn.
     *
     * It's clear that in both cases (left to
     * right, right to left) we need to extend the
     * size of the scroll rectangle to encompass
     * both.  In the right to left case, we also
     * need to decrement the X coordinate.
     *
     * We call Update after scrolling to make sure
     * there's no garbage left in the window to be
     * copied again if we're called before the
     * paint events have been handled.
     *
     */
            int delta;

            if (vertical) {
                delta = value - original_thumbpos.Y;

                if (delta < 0) {
                    original_thumbpos.Y += delta;
                    original_thumbpos.Height -= delta;
                } else {
                    original_thumbpos.Height += delta;
                }

                InvalidateDirty ();
                // XplatUI.ScrollWindow (Handle, original_thumbpos, 0, delta, false);
            } else {
                delta = value - original_thumbpos.X;

                if (delta < 0) {
                    original_thumbpos.X += delta;
                    original_thumbpos.Width -= delta;
                } else {
                    original_thumbpos.Width += delta;
                }

                InvalidateDirty ();
                //XplatUI.ScrollWindow (Handle, original_thumbpos, delta, 0, false);
            }

            Update ();
        }

        private void UpdatePos (int newPos, bool update_thumbpos)
        {
            int pos;

            if (newPos < minimum)
                pos = minimum;
            else
                if (newPos > MaximumAllowed)
                pos = MaximumAllowed;
            else
                pos = newPos;

            // pos can't be less than minimum or greater than maximum
            if (pos < minimum)
                pos = minimum;
            if (pos > maximum)
                pos = maximum;

            if (update_thumbpos) {
                if (vertical)
                    UpdateThumbPos (thumb_area.Y + (int)(((float)(pos - minimum)) * pixel_per_pos), true, false);
                else
                    UpdateThumbPos (thumb_area.X + (int)(((float)(pos - minimum)) * pixel_per_pos), true, false);
                SetValue (pos);
            } else {
                position = pos; // Updates directly the value to avoid thumb pos update

                OnValueChanged (EventArgs.Empty);
                // XXX some reason we don't call OnValueChanged?
                //EventHandler eh = (EventHandler)(Events[ValueChangedEvent]);
                //if (eh != null)
                //    eh (this, EventArgs.Empty);
            }
        }
        protected void UpdateThumbPos (int pixel, bool dirty, bool update_value)
        {
            float new_pos = 0;

            if (vertical) {
                if (dirty)
                    Dirty (thumb_pos);
                if (pixel < thumb_area.Y)
                    thumb_pos.Y = thumb_area.Y;
                else if (pixel > thumb_area.Bottom - thumb_size)
                    thumb_pos.Y = thumb_area.Bottom - thumb_size;
                else
                    thumb_pos.Y = pixel;

                thumb_pos.X = 0;
                thumb_pos.Width = 16 - 1;
                thumb_pos.Height = thumb_size;
                new_pos = (float)(thumb_pos.Y - thumb_area.Y);
                new_pos = new_pos / pixel_per_pos;
                if (dirty)
                    Dirty (thumb_pos);
            } else {
                if (dirty)
                    Dirty (thumb_pos);
                if (pixel < thumb_area.X)
                    thumb_pos.X = thumb_area.X;
                else if (pixel > thumb_area.Right - thumb_size)
                    thumb_pos.X = thumb_area.Right - thumb_size;
                else
                    thumb_pos.X = pixel;

                thumb_pos.Y = 0;
                thumb_pos.Width = thumb_size;
                thumb_pos.Height = 16;
                new_pos = (float)(thumb_pos.X - thumb_area.X);
                new_pos = new_pos / pixel_per_pos;

                if (dirty)
                    Dirty (thumb_pos);
            }

            if (update_value)
                UpdatePos ((int)new_pos + minimum, false);
        }

        private void SetValue (int value)
        {
            if (value < minimum || value > maximum)
                throw new ArgumentException (
                    String.Format ("'{0}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'", value));

            if (position != value) {
                position = value;

                OnValueChanged (EventArgs.Empty);
                UpdatePos (value, true);
            }
        }

        public enum ScrollBarElement
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
