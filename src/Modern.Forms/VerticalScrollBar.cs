using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class VerticalScrollBar : ScrollBarControl
    {
        public VerticalScrollBar ()
        {
            vertical = true;
        }

        protected override Size DefaultSize => new Size (15, 80);

        protected override Rectangle DecrementArrowBounds => new Rectangle (0, 0, Width, scrollbutton_height);

        protected override Rectangle IncrementArrowBounds => new Rectangle (0, ClientRectangle.Height - scrollbutton_height, Width, scrollbutton_height);

        protected override Rectangle DecrementTrackBounds => new Rectangle (0, scrollbutton_height + 1, Width, ThumbBounds.Top - 1);

        protected override Rectangle IncrementTrackBounds => new Rectangle (0, ThumbBounds.Bottom + 1, Width, ClientRectangle.Height - scrollbutton_height - ThumbBounds.Bottom);

        protected override ArrowDirection DecrementArrowDirection => ArrowDirection.Up;

        protected override ArrowDirection IncrementArrowDirection => ArrowDirection.Down;

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (thumb_pressed == true) {
                var thumb_edge = e.Y - thumbclick_offset;

                if (thumb_edge < thumb_area.Y)
                    thumb_edge = thumb_area.Y;
                else if (thumb_edge > thumb_area.Bottom - thumb_size)
                    thumb_edge = thumb_area.Bottom - thumb_size;

                if (thumb_edge != thumb_pos.Y) {
                    var thumb_rect = thumb_pos;

                    UpdateThumbPos (thumb_edge, false, true);
                    MoveThumb (thumb_rect, thumb_pos.Y);

                    OnScroll (new ScrollEventArgs (ScrollEventType.ThumbTrack, position));
                }
            }
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            var top_arrow_area = DecrementArrowBounds;
            top_arrow_area.Width -= 1;
            top_arrow_area.Height -= 1;

            var bottom_arrow_area = IncrementArrowBounds;
            bottom_arrow_area.Width -= 1;
            bottom_arrow_area.Height -= 1;

            thumb_pos.Width = Width;

            // Top Arrow
            e.Canvas.FillRectangle (top_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (top_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (top_arrow_area, Theme.BorderGray, DecrementArrowDirection);

            // Bottom Arrow
            e.Canvas.FillRectangle (bottom_arrow_area, SKColors.White);
            e.Canvas.DrawRectangle (bottom_arrow_area, Theme.BorderGray);
            e.Canvas.DrawArrow (bottom_arrow_area, Theme.BorderGray, IncrementArrowDirection);

            // Grip
            thumb_pos.Width -= 1;
            e.Canvas.FillRectangle (thumb_pos, SKColors.White);
            e.Canvas.DrawRectangle (thumb_pos, Theme.BorderGray);
        }
    }
}
