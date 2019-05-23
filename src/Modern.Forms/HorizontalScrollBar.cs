using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class HorizontalScrollBar : ScrollBarControl
    {
        public HorizontalScrollBar ()
        {
        }

        protected override Size DefaultSize => new Size (80, 15);

        protected override Rectangle DecrementArrowBounds => new Rectangle (0, 0, scrollbutton_width, Height);

        protected override Rectangle IncrementArrowBounds => new Rectangle (ClientRectangle.Width - scrollbutton_width, 0, scrollbutton_width, Height);

        protected override Rectangle DecrementTrackBounds => new Rectangle (scrollbutton_width + 1, 0, ThumbBounds.Left - 1, Height);

        protected override Rectangle IncrementTrackBounds => new Rectangle (ThumbBounds.Right + 1, 0, ClientRectangle.Width - scrollbutton_width - ThumbBounds.Right, Height);

        protected override ArrowDirection DecrementArrowDirection => ArrowDirection.Left;

        protected override ArrowDirection IncrementArrowDirection => ArrowDirection.Right;

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (thumb_pressed == true) {
                var thumb_edge = e.X - thumbclick_offset;

                if (thumb_edge < thumb_area.X)
                    thumb_edge = thumb_area.X;
                else if (thumb_edge > thumb_area.Right - thumb_size)
                    thumb_edge = thumb_area.Right - thumb_size;

                if (thumb_edge != thumb_pos.X) {
                    var thumb_rect = thumb_pos;

                    UpdateThumbPos (thumb_edge, false, true);
                    MoveThumb (thumb_rect, thumb_pos.X);

                    OnScroll (new ScrollEventArgs (ScrollEventType.ThumbTrack, position));
                }
            }
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

            thumb_pos.Height = Height;

            // Top Arrow
            e.Canvas.FillRectangle (top_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (top_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (top_arrow_area, Theme.BorderGray, DecrementArrowDirection);

            // Bottom Arrow
            e.Canvas.FillRectangle (bottom_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (bottom_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (bottom_arrow_area, Theme.BorderGray, IncrementArrowDirection);

            // Grip
            thumb_pos.Height -= 1;
            e.Canvas.FillRectangle (thumb_pos, SKColors.White);
            e.Canvas.DrawRectangle (thumb_pos, Theme.BorderGray);
        }
    }
}
