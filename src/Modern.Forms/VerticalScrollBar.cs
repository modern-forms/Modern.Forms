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

        protected override int ArrowButtonSize => Height < 15 * 2 ? Height / 2 : 15;

        protected override Rectangle DecrementArrowBounds => new Rectangle (0, 0, Width, ArrowButtonSize);

        protected override Rectangle IncrementArrowBounds => new Rectangle (0, ClientRectangle.Height - ArrowButtonSize, Width, ArrowButtonSize);

        protected override Rectangle DecrementTrackBounds => new Rectangle (0, ArrowButtonSize + 1, Width, ThumbDragBounds.Top - 1);

        protected override Rectangle IncrementTrackBounds => new Rectangle (0, ThumbDragBounds.Bottom + 1, Width, ClientRectangle.Height - ArrowButtonSize - ThumbDragBounds.Bottom);

        protected override ArrowDirection DecrementArrowDirection => ArrowDirection.Up;

        protected override ArrowDirection IncrementArrowDirection => ArrowDirection.Down;

        protected override Rectangle TotalTrackBrounds => new Rectangle (0, ArrowButtonSize, Width, Height - (2 * ArrowButtonSize));

        protected override Rectangle EffectiveTrackBounds => new Rectangle (0, ArrowButtonSize + (int)(ThumbDragSize / 2f), Width, Height - (2 * ArrowButtonSize) - ThumbDragSize);

        protected override Rectangle ThumbDragBounds {
            get {
                var thumb_size = ThumbDragSize;
                var half_thumb = thumb_size / 2;

                return new Rectangle (0, thumb_drag_position - half_thumb, Width - 1, thumb_size - 1);
            }
        }

        protected override int ThumbDragSize {
            get {
                if (Height < thumb_notshown_size)
                    return 0;

                // Give the user something to drag if LargeChange is zero
                if (LargeChange == 0)
                    return 17;

                var size = 1 + (int)(((double)LargeChange / (PossibleValues + LargeChange)) * TotalTrackBrounds.Height);

                return Math.Max (size, thumb_min_size);
            }
        }
    }
}
