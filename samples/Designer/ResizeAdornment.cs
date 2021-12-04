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

        public GrabHandleGlyphType Direction { get; }
        public Rectangle Bounds => new Rectangle (center.X - 2, center.Y - 2, 6, 6);

        public ResizeAdornment (GrabHandleGlyphType direction, Point center)
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
                GrabHandleGlyphType.MiddleRight => new Size (currentSize.Width + (point.X - resizeAnchor.X), currentSize.Height),
                GrabHandleGlyphType.MiddleBottom => new Size (currentSize.Width, currentSize.Height + (point.Y - resizeAnchor.Y)),
                GrabHandleGlyphType.LowerRight => new Size (currentSize.Width + (point.X - resizeAnchor.X), currentSize.Height + (point.Y - resizeAnchor.Y)),
                _ => throw new NotImplementedException ()
            };
        }

        public Rectangle GetNewBounds (Rectangle currentBounds, Point resizeAnchor, Point point)
        {
            return Direction switch {
                GrabHandleGlyphType.UpperLeft => Rectangle.FromLTRB (currentBounds.Left + (point.X - resizeAnchor.X), currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.MiddleTop => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.UpperRight => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top + (point.Y - resizeAnchor.Y), currentBounds.Right + (point.X - resizeAnchor.X), currentBounds.Bottom),
                GrabHandleGlyphType.MiddleLeft => Rectangle.FromLTRB (currentBounds.Left + (point.X - resizeAnchor.X), currentBounds.Top, currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.MiddleRight => new Rectangle (currentBounds.Location, new Size (currentBounds.Width + (point.X - resizeAnchor.X), currentBounds.Height)),
                GrabHandleGlyphType.MiddleBottom => new Rectangle (currentBounds.Location, new Size (currentBounds.Width, currentBounds.Height + (point.Y - resizeAnchor.Y))),
                GrabHandleGlyphType.LowerRight => new Rectangle (currentBounds.Location, new Size (currentBounds.Width + (point.X - resizeAnchor.X), currentBounds.Height + (point.Y - resizeAnchor.Y))),
                _ => throw new NotImplementedException ()
            };
        }
    }
}
