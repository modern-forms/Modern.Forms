using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Renders a custom TrackBar control.
    /// </summary>
    public class TrackBarRenderer : Renderer<TrackBar>
    {
        /// <inheritdoc/>
        protected override void Render (TrackBar control, PaintEventArgs e)
        {
            var track_bounds = GetTrackBounds (control, e);
            var thumb_bounds = GetThumbBounds (control, e);

            DrawTicks (control, e, track_bounds);
            DrawTrack (control, e, track_bounds);
            DrawThumb (control, e, thumb_bounds);

            if (control.Selected && control.ShowFocusCues) {
                var focus_bounds = control.ClientRectangle;
                focus_bounds.Width -= 1;
                focus_bounds.Height -= 1;
                e.Canvas.DrawRectangle (focus_bounds, Theme.AccentColor2);
            }
        }

        /// <summary>
        /// Gets the bounds of the drawn track line.
        /// </summary>
        public Rectangle GetTrackBounds (TrackBar control)
            => GetTrackBounds (control, null);

        /// <summary>
        /// Gets the bounds of the thumb.
        /// </summary>
        public Rectangle GetThumbBounds (TrackBar control)
            => GetThumbBounds (control, null);

        /// <summary>
        /// Converts a point on the control into a value.
        /// </summary>
        public int PositionToValue (TrackBar control, Point location)
        {
            var thumb_bounds = GetThumbBounds (control);
            return PositionToValueFromThumb (control, location, control.Orientation == Orientation.Horizontal ? thumb_bounds.Width / 2 : thumb_bounds.Height / 2);
        }

        /// <summary>
        /// Converts a point on the control into a value using the thumb drag offset.
        /// </summary>
        public int PositionToValueFromThumb (TrackBar control, Point location, int dragOffsetFromThumbOrigin)
        {
            var track_bounds = GetTrackBounds (control);
            var thumb_size = GetThumbSize (control, null);

            if (control.Maximum <= control.Minimum)
                return control.Minimum;

            if (control.Orientation == Orientation.Horizontal) {
                var usable = Math.Max (1, track_bounds.Width - thumb_size.Width);
                var thumb_left = location.X - dragOffsetFromThumbOrigin;
                var percent = (double)(thumb_left - track_bounds.Left) / usable;
                percent = Math.Max (0d, Math.Min (1d, percent));

                return control.Minimum + (int)Math.Round (percent * (control.Maximum - control.Minimum));
            } else {
                var usable = Math.Max (1, track_bounds.Height - thumb_size.Height);
                var thumb_top = location.Y - dragOffsetFromThumbOrigin;
                var percent = 1d - ((double)(thumb_top - track_bounds.Top) / usable);
                percent = Math.Max (0d, Math.Min (1d, percent));

                return control.Minimum + (int)Math.Round (percent * (control.Maximum - control.Minimum));
            }
        }

        private void DrawThumb (TrackBar control, PaintEventArgs e, Rectangle thumb_bounds)
        {
            var fill_color = !control.Enabled ? Theme.ControlMidHighColor
                : control.ThumbPressed ? Theme.AccentColor2
                : control.ThumbHovered ? Theme.AccentColor
                : Theme.ControlMidHighColor;

            var border_color = !control.Enabled ? Theme.ForegroundDisabledColor
                : control.ThumbPressed ? Theme.AccentColor2
                : Theme.BorderLowColor;

            var draw_bounds = thumb_bounds;
            draw_bounds.Width -= 1;
            draw_bounds.Height -= 1;

            e.Canvas.FillRectangle (draw_bounds, fill_color);
            e.Canvas.DrawRectangle (draw_bounds, border_color);
        }

        private void DrawTicks (TrackBar control, PaintEventArgs e, Rectangle track_bounds)
        {
            if (control.TickStyle == TickStyle.None || control.Maximum < control.Minimum)
                return;

            var tick_length = e.LogicalToDeviceUnits (4);
            var tick_color = control.Enabled ? Theme.ForegroundColor : Theme.ForegroundDisabledColor;

            foreach (var position in GetTickPositions (control, e, track_bounds)) {
                if (control.Orientation == Orientation.Horizontal) {
                    if (control.TickStyle == TickStyle.TopLeft || control.TickStyle == TickStyle.Both)
                        e.Canvas.DrawLine (position, track_bounds.Top - tick_length - 1, position, track_bounds.Top - 1, tick_color);

                    if (control.TickStyle == TickStyle.BottomRight || control.TickStyle == TickStyle.Both)
                        e.Canvas.DrawLine (position, track_bounds.Bottom + 1, position, track_bounds.Bottom + tick_length + 1, tick_color);
                } else {
                    if (control.TickStyle == TickStyle.TopLeft || control.TickStyle == TickStyle.Both)
                        e.Canvas.DrawLine (track_bounds.Left - tick_length - 1, position, track_bounds.Left - 1, position, tick_color);

                    if (control.TickStyle == TickStyle.BottomRight || control.TickStyle == TickStyle.Both)
                        e.Canvas.DrawLine (track_bounds.Right + 1, position, track_bounds.Right + tick_length + 1, position, tick_color);
                }
            }
        }

        private void DrawTrack (TrackBar control, PaintEventArgs e, Rectangle track_bounds)
        {
            var track_color = control.Enabled ? Theme.ControlMidHighColor : Theme.ControlMidColor;
            var border_color = control.Enabled ? Theme.BorderLowColor : Theme.ForegroundDisabledColor;

            e.Canvas.FillRectangle (track_bounds, track_color);
            e.Canvas.DrawRectangle (track_bounds, border_color);
        }

        private Rectangle GetTrackBounds (TrackBar control, PaintEventArgs? e)
        {
            var client = control.ClientRectangle;
            var thumb_size = GetThumbSize (control, e);
            var track_thickness = e?.LogicalToDeviceUnits (4) ?? control.LogicalToDeviceUnits (4);

            if (control.Orientation == Orientation.Horizontal) {
                var x = thumb_size.Width / 2;
                var width = Math.Max (1, client.Width - thumb_size.Width);

                var y = (client.Height - track_thickness) / 2;
                return new Rectangle (x, y, width, track_thickness);
            } else {
                var y = thumb_size.Height / 2;
                var height = Math.Max (1, client.Height - thumb_size.Height);

                var x = (client.Width - track_thickness) / 2;
                return new Rectangle (x, y, track_thickness, height);
            }
        }

        private Rectangle GetThumbBounds (TrackBar control, PaintEventArgs? e)
        {
            var track_bounds = GetTrackBounds (control, e);
            var thumb_size = GetThumbSize (control, e);

            if (control.Maximum <= control.Minimum) {
                if (control.Orientation == Orientation.Horizontal)
                    return new Rectangle (track_bounds.Left, (control.ClientRectangle.Height - thumb_size.Height) / 2, thumb_size.Width, thumb_size.Height);

                return new Rectangle ((control.ClientRectangle.Width - thumb_size.Width) / 2, track_bounds.Bottom - thumb_size.Height, thumb_size.Width, thumb_size.Height);
            }

            var percent = (double)(control.Value - control.Minimum) / (control.Maximum - control.Minimum);
            percent = Math.Max (0d, Math.Min (1d, percent));

            if (control.Orientation == Orientation.Horizontal) {
                var usable = Math.Max (1, track_bounds.Width - thumb_size.Width);
                var x = track_bounds.Left + (int)Math.Round (percent * usable);
                var y = (control.ClientRectangle.Height - thumb_size.Height) / 2;

                return new Rectangle (x, y, thumb_size.Width, thumb_size.Height);
            } else {
                var usable = Math.Max (1, track_bounds.Height - thumb_size.Height);
                var y = track_bounds.Top + (int)Math.Round ((1d - percent) * usable);
                var x = (control.ClientRectangle.Width - thumb_size.Width) / 2;

                return new Rectangle (x, y, thumb_size.Width, thumb_size.Height);
            }
        }

        private Size GetThumbSize (TrackBar control, PaintEventArgs? e)
        {
            var width = e?.LogicalToDeviceUnits (14) ?? control.LogicalToDeviceUnits (14);
            var height = e?.LogicalToDeviceUnits (20) ?? control.LogicalToDeviceUnits (20);

            return control.Orientation == Orientation.Horizontal
                ? new Size (width, height)
                : new Size (height, width);
        }

        private IEnumerable<int> GetTickPositions (TrackBar control, PaintEventArgs e, Rectangle track_bounds)
        {
            if (control.Maximum < control.Minimum)
                yield break;

            var thumb_size = GetThumbSize (control, e);

            if (control.Maximum == control.Minimum) {
                if (control.Orientation == Orientation.Horizontal)
                    yield return track_bounds.Left + (thumb_size.Width / 2);
                else
                    yield return track_bounds.Top + (thumb_size.Height / 2);

                yield break;
            }

            var producedMaximum = false;

            for (var value = control.Minimum; value <= control.Maximum; value += control.TickFrequency) {
                if (value == control.Maximum)
                    producedMaximum = true;

                yield return ValueToPixel (control, track_bounds, thumb_size, value);
            }

            if (!producedMaximum)
                yield return ValueToPixel (control, track_bounds, thumb_size, control.Maximum);
        }

        private int ValueToPixel (TrackBar control, Rectangle track_bounds, Size thumb_size, int value)
        {
            if (control.Maximum <= control.Minimum)
                return control.Orientation == Orientation.Horizontal
                    ? track_bounds.Left + (thumb_size.Width / 2)
                    : track_bounds.Top + (thumb_size.Height / 2);

            var percent = (double)(value - control.Minimum) / (control.Maximum - control.Minimum);
            percent = Math.Max (0d, Math.Min (1d, percent));

            if (control.Orientation == Orientation.Horizontal) {
                var usable = Math.Max (1, track_bounds.Width - thumb_size.Width);
                return track_bounds.Left + (thumb_size.Width / 2) + (int)Math.Round (percent * usable);
            } else {
                var usable = Math.Max (1, track_bounds.Height - thumb_size.Height);
                return track_bounds.Top + (thumb_size.Height / 2) + (int)Math.Round ((1d - percent) * usable);
            }
        }
    }
}
