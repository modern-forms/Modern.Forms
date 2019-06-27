using System;
using System.Drawing;

namespace Modern.Forms
{
    public class HorizontalScrollBar : ScrollBar
    {
        public HorizontalScrollBar ()
        {
        }

        protected override int ArrowButtonSize => ClientRectangle.Width < LogicalToDeviceUnits (15) * 2 ? ClientRectangle.Width / 2 : LogicalToDeviceUnits (15);

        protected override Size DefaultSize => new Size (80, 15);

        protected override Rectangle DecrementArrowBounds => new Rectangle (0, 0, ArrowButtonSize, ClientRectangle.Height);

        protected override Rectangle IncrementArrowBounds => new Rectangle (ClientRectangle.Width - ArrowButtonSize, 0, ArrowButtonSize, ClientRectangle.Height);

        protected override Rectangle DecrementTrackBounds => new Rectangle (ArrowButtonSize + 1, 0, ThumbDragBounds.Left - 1, ClientRectangle.Height);

        protected override Rectangle IncrementTrackBounds => new Rectangle (ThumbDragBounds.Right + 1, 0, ClientRectangle.Width - ArrowButtonSize - ThumbDragBounds.Right, ClientRectangle.Height);

        protected override ArrowDirection DecrementArrowDirection => ArrowDirection.Left;

        protected override ArrowDirection IncrementArrowDirection => ArrowDirection.Right;

        protected override Rectangle TotalTrackBrounds => new Rectangle (ArrowButtonSize, 0, ClientRectangle.Width - (2 * ArrowButtonSize), ClientRectangle.Height);

        protected override Rectangle EffectiveTrackBounds => new Rectangle (ArrowButtonSize + (int)(ThumbDragSize / 2f), 0, ClientRectangle.Width - (2 * ArrowButtonSize) - ThumbDragSize, ClientRectangle.Height);

        protected override Rectangle ThumbDragBounds {
            get {
                var thumb_size = ThumbDragSize;
                var half_thumb = thumb_size / 2;

                return new Rectangle (thumb_drag_position - half_thumb, 0, thumb_size - 1, ClientRectangle.Height - 1);
            }
        }

        protected override int ThumbDragSize {
            get {
                if (Width < thumb_notshown_size)
                    return 0;

                // Give the user something to drag if LargeChange is zero
                if (LargeChange == 0)
                    return LogicalToDeviceUnits (17);

                var size = LogicalToDeviceUnits (1) + (int)(((double)LargeChange / (PossibleValues + LargeChange)) * TotalTrackBrounds.Width);

                return Math.Max (size, thumb_min_size);
            }
        }
    }
}
