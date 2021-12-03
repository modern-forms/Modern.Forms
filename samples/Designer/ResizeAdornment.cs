using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Designer
{
    public class ResizeAdornment
    {
        private Point center;

        public SelectionDirection Direction { get; }
        public Rectangle Bounds => new Rectangle (center.X - 2, center.Y - 2, 6, 6);

        public ResizeAdornment (SelectionDirection direction, Point center)
        {
            Direction = direction;
            this.center = center;
        }

        public void Draw (SKCanvas canvas)
        {
            canvas.FillRectangle (center.X - 2, center.Y - 2, 5, 5, SKColors.White);

            canvas.DrawLine (center.X - 2, center.Y - 3, center.X + 3, center.Y - 3, SKColors.Black, 1);
            canvas.DrawLine (center.X - 2, center.Y + 3, center.X + 3, center.Y + 3, SKColors.Black, 1);

            canvas.DrawLine (center.X - 3, center.Y - 2, center.X - 3, center.Y + 3, SKColors.Black, 1);
            canvas.DrawLine (center.X + 3, center.Y - 2, center.X + 3, center.Y + 3, SKColors.Black, 1);
        }

        public Size GetNewSize (Size currentSize, Point resizeAnchor, Point point)
        {
            return Direction switch {
                SelectionDirection.CenterRight => new Size (currentSize.Width + (point.X - resizeAnchor.X), currentSize.Height),
                SelectionDirection.BottomMiddle => new Size (currentSize.Width, currentSize.Height + (point.Y - resizeAnchor.Y)),
                SelectionDirection.BottomRight => new Size (currentSize.Width + (point.X - resizeAnchor.X), currentSize.Height + (point.Y - resizeAnchor.Y)),
                _ => throw new NotImplementedException ()
            };
        }

        public Rectangle GetNewBounds (Rectangle currentBounds, Point resizeAnchor, Point point)
        {
            return Direction switch {
                SelectionDirection.TopLeft => Rectangle.FromLTRB (currentBounds.Left + (point.X - resizeAnchor.X), currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right, currentBounds.Bottom),
                SelectionDirection.TopMiddle => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right, currentBounds.Bottom),
                SelectionDirection.TopRight => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right + (point.X - resizeAnchor.X), currentBounds.Bottom),
                SelectionDirection.CenterLeft => Rectangle.FromLTRB (currentBounds.Left + (point.X - resizeAnchor.X), currentBounds.Top, currentBounds.Right, currentBounds.Bottom),
                SelectionDirection.CenterRight => new Rectangle (currentBounds.Location, new Size (currentBounds.Width + (point.X - resizeAnchor.X), currentBounds.Height)),
                SelectionDirection.BottomMiddle => new Rectangle (currentBounds.Location, new Size (currentBounds.Width, currentBounds.Height + (point.Y - resizeAnchor.Y))),
                SelectionDirection.BottomRight => new Rectangle (currentBounds.Location, new Size (currentBounds.Width + (point.X - resizeAnchor.X), currentBounds.Height + (point.Y - resizeAnchor.Y))),
                _ => throw new NotImplementedException ()
            };
        }
    }
}
