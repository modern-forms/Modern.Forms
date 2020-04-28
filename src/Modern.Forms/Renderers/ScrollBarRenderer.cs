using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a ScrollBar.
    /// </summary>
    public class ScrollBarRenderer : Renderer<ScrollBar>
    {
        /// <summary>
        /// Minimum size of the scroll bar thumb element.
        /// </summary>
        protected const int THUMB_MIN_SIZE = 8;
        /// <summary>
        /// Minimum size of the scroll bar which will show the thumb element.
        /// </summary>
        protected const int THUMB_NOTSHOWN_SIZE = 40;

        /// <inheritdoc/>
        protected override void Render (ScrollBar control, PaintEventArgs e)
        {
            var top_arrow_area = GetDecrementArrowBounds (control);
            top_arrow_area.Width -= 1;
            top_arrow_area.Height -= 1;

            var bottom_arrow_area = GetIncrementArrowBounds (control);
            bottom_arrow_area.Width -= 1;
            bottom_arrow_area.Height -= 1;

            // Top Arrow
            e.Canvas.FillRectangle (top_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (top_arrow_area, Theme.BorderGray);
            ControlPaint.DrawArrowGlyph (e, top_arrow_area, Theme.BorderGray, GetDecrementArrowDirection (control));

            // Bottom Arrow
            e.Canvas.FillRectangle (bottom_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (bottom_arrow_area, Theme.BorderGray);
            ControlPaint.DrawArrowGlyph (e, bottom_arrow_area, Theme.BorderGray, GetIncrementArrowDirection (control));

            if (!control.Enabled)
                return;

            // Grip
            var thumb_bounds = GetThumbDragBounds (control);

            if (thumb_bounds.Width > 0 && thumb_bounds.Height > 0) {
                e.Canvas.FillRectangle (thumb_bounds, SKColors.White);
                e.Canvas.DrawRectangle (thumb_bounds, Theme.BorderGray);
            }
        }

        /// <summary>
        /// Gets the size of an arrow button.
        /// </summary>
        public virtual int GetArrowButtonSize (ScrollBar control)
        {
            var unit_15 = control.LogicalToDeviceUnits (15);

            if (control is VerticalScrollBar)
                return control.ClientRectangle.Height < unit_15 * 2 ? control.ClientRectangle.Height / 2 : unit_15;
            else
                return control.ClientRectangle.Width < unit_15 * 2 ? control.ClientRectangle.Width / 2 : unit_15;
        }

        /// <summary>
        /// Gets the bounds of the decrement arrow button.
        /// </summary>
        public virtual Rectangle GetDecrementArrowBounds (ScrollBar control)
        {
            if (control is VerticalScrollBar)
                return new Rectangle (0, 0, control.ClientRectangle.Width, GetArrowButtonSize (control));
            else
                return new Rectangle (0, 0, GetArrowButtonSize (control), control.ClientRectangle.Height); ;
        }

        /// <summary>
        /// Gets the direction of the decrement arrow.
        /// </summary>
        public virtual ArrowDirection GetDecrementArrowDirection (ScrollBar control) => control is VerticalScrollBar ? ArrowDirection.Up : ArrowDirection.Left;

        /// <summary>
        /// Gets the bounds of the decrement track area.
        /// </summary>
        public virtual Rectangle GetDecrementTrackBounds (ScrollBar control)
        {
            var arrow_button_size = GetArrowButtonSize (control);
            var thumb_drag_bounds = GetThumbDragBounds (control);

            if (control is VerticalScrollBar)
                return new Rectangle (0, arrow_button_size + 1, control.ClientRectangle.Width, thumb_drag_bounds.Top - 1);
            else
                return new Rectangle (arrow_button_size + 1, 0, thumb_drag_bounds.Left - 1, control.ClientRectangle.Height);
        }

        /// <summary>
        /// Gets the bounds of the total track area.
        /// </summary>
        public virtual Rectangle GetEffectiveTrackBounds (ScrollBar control)
        {
            var arrow_button_size = GetArrowButtonSize (control);
            var thumb_drag_size = GetThumbDragSize (control);

            if (control is VerticalScrollBar)
                return new Rectangle (0, arrow_button_size + (int)(thumb_drag_size / 2f), control.ClientRectangle.Width, control.ClientRectangle.Height - (2 * arrow_button_size) - thumb_drag_size);
            else
                return new Rectangle (arrow_button_size + (int)(thumb_drag_size / 2f), 0, control.ClientRectangle.Width - (2 * arrow_button_size) - thumb_drag_size, control.ClientRectangle.Height);
        }

        /// <summary>
        /// Gets the bounds of the increment arrow area.
        /// </summary>
        public virtual Rectangle GetIncrementArrowBounds (ScrollBar control)
        {
            if (control is VerticalScrollBar)
                return new Rectangle (0, control.ClientRectangle.Height - GetArrowButtonSize (control), control.ClientRectangle.Width, GetArrowButtonSize (control));
            else
                return new Rectangle (control.ClientRectangle.Width - GetArrowButtonSize (control), 0, GetArrowButtonSize (control), control.ClientRectangle.Height);
        }

        /// <summary>
        /// Gets the direction of the increment arrow.
        /// </summary>
        public virtual ArrowDirection GetIncrementArrowDirection (ScrollBar control) => control is VerticalScrollBar ? ArrowDirection.Down : ArrowDirection.Right;

        /// <summary>
        /// Gets the bounds of the increment track area.
        /// </summary>
        public virtual Rectangle GetIncrementTrackBounds (ScrollBar control)
        {
            var arrow_button_size = GetArrowButtonSize (control);
            var thumb_drag_bounds = GetThumbDragBounds (control);

            if (control is VerticalScrollBar)
                return new Rectangle (0, thumb_drag_bounds.Bottom + 1, control.ClientRectangle.Width, control.ClientRectangle.Height - arrow_button_size - thumb_drag_bounds.Bottom);
            else
                return new Rectangle (thumb_drag_bounds.Right + 1, 0, control.ClientRectangle.Width - arrow_button_size - thumb_drag_bounds.Right, control.ClientRectangle.Height);
        }

        private int GetPossibleValues (ScrollBar control) => control.Maximum - control.Minimum + 1;

        /// <summary>
        /// Gets the bounds of the thumb drag element.
        /// </summary>
        public virtual Rectangle GetThumbDragBounds (ScrollBar control)
        {
            var thumb_size = GetThumbDragSize (control);
            var half_thumb = thumb_size / 2;

            if (control is VerticalScrollBar)
                return new Rectangle (0, GetThumbDragPosition (control) - half_thumb, control.ClientRectangle.Width - 1, thumb_size - 1);
            else
                return new Rectangle (GetThumbDragPosition (control) - half_thumb, 0, thumb_size - 1, control.ClientRectangle.Height - 1);
        }

        /// <summary>
        /// Gets the size of the thumb drag element.
        /// </summary>
        public virtual int GetThumbDragSize (ScrollBar control)
        {
            if (control is VerticalScrollBar) {
                if (control.Height < THUMB_NOTSHOWN_SIZE)
                    return 0;

                // Give the user something to drag if LargeChange is zero
                if (control.LargeChange == 0)
                    return control.LogicalToDeviceUnits (17);

                var size = control.LogicalToDeviceUnits (1) + (int)(((double)control.LargeChange / (GetPossibleValues (control) + control.LargeChange)) * GetTotalTrackBounds (control).Height);

                return Math.Max (size, THUMB_MIN_SIZE);
            } else {
                if (control.Width < THUMB_NOTSHOWN_SIZE)
                    return 0;

                // Give the user something to drag if LargeChange is zero
                if (control.LargeChange == 0)
                    return control.LogicalToDeviceUnits (17);

                var size = control.LogicalToDeviceUnits (1) + (int)(((double)control.LargeChange / (GetPossibleValues (control) + control.LargeChange)) * GetTotalTrackBounds (control).Width);

                return Math.Max (size, THUMB_MIN_SIZE);
            }
        }

        /// <summary>
        /// Gets the position of the thumb drag element.
        /// </summary>
        public virtual int GetThumbDragPosition (ScrollBar control) => control.thumb_drag_position;

        /// <summary>
        /// Gets the bounds of the track area.
        /// </summary>
        public virtual Rectangle GetTotalTrackBounds (ScrollBar control)
        {
            if (control is VerticalScrollBar)
                return new Rectangle (0, GetArrowButtonSize (control), control.ClientRectangle.Width, control.ClientRectangle.Height - (2 * GetArrowButtonSize (control)));
            else
                return new Rectangle (GetArrowButtonSize (control), 0, control.ClientRectangle.Width - (2 * GetArrowButtonSize (control)), control.ClientRectangle.Height);
        }
    }
}
