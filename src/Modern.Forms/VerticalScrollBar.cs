using System;
using System.Drawing;

namespace Modern.Forms
{
    public class VerticalScrollBar : ScrollBar
    {
        public VerticalScrollBar ()
        {
            vertical = true;
        }

        protected override Size DefaultSize => new Size (15, 80);

        protected override int ArrowButtonSize => ClientRectangle.Height < LogicalToDeviceUnits (15) * 2 ? ClientRectangle.Height / 2 : LogicalToDeviceUnits (15);

        protected override Rectangle DecrementArrowBounds => new Rectangle (0, 0, ClientRectangle.Width, ArrowButtonSize);

        protected override Rectangle IncrementArrowBounds => new Rectangle (0, ClientRectangle.Height - ArrowButtonSize, ClientRectangle.Width, ArrowButtonSize);

        protected override Rectangle DecrementTrackBounds => new Rectangle (0, ArrowButtonSize + 1, ClientRectangle.Width, ThumbDragBounds.Top - 1);

        protected override Rectangle IncrementTrackBounds => new Rectangle (0, ThumbDragBounds.Bottom + 1, ClientRectangle.Width, ClientRectangle.Height - ArrowButtonSize - ThumbDragBounds.Bottom);

        protected override ArrowDirection DecrementArrowDirection => ArrowDirection.Up;

        protected override ArrowDirection IncrementArrowDirection => ArrowDirection.Down;

        protected override Rectangle TotalTrackBrounds => new Rectangle (0, ArrowButtonSize, ClientRectangle.Width, ClientRectangle.Height - (2 * ArrowButtonSize));

        protected override Rectangle EffectiveTrackBounds => new Rectangle (0, ArrowButtonSize + (int)(ThumbDragSize / 2f), ClientRectangle.Width, ClientRectangle.Height - (2 * ArrowButtonSize) - ThumbDragSize);

        protected override Rectangle ThumbDragBounds {
            get {
                var thumb_size = ThumbDragSize;
                var half_thumb = thumb_size / 2;

                return new Rectangle (0, thumb_drag_position - half_thumb, ClientRectangle.Width - 1, thumb_size - 1);
            }
        }

        protected override int ThumbDragSize {
            get {
                if (Height < thumb_notshown_size)
                    return 0;

                // Give the user something to drag if LargeChange is zero
                if (LargeChange == 0)
                    return LogicalToDeviceUnits (17);

                var size = LogicalToDeviceUnits (1) + (int)(((double)LargeChange / (PossibleValues + LargeChange)) * TotalTrackBrounds.Height);

                return Math.Max (size, thumb_min_size);
            }
        }
    }
}
