using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;
using SkiaSharp;

namespace Modern.Forms.Design
{
    public class GrabHandleGlyph : Glyph
    {
        private readonly Point center;
        private readonly Cursor? cursor;
        private readonly bool _isPrimary;

        public GrabHandleGlyphType Direction { get; }
        public override Rectangle Bounds => new Rectangle (center.X - 2, center.Y - 2, 6, 6);

        public GrabHandleGlyph (Rectangle controlBounds, GrabHandleGlyphType direction, Behavior? behavior, bool primarySelection) : base (behavior)
        {
            Direction = direction;
            _isPrimary = primarySelection;

            switch (direction) {
                case GrabHandleGlyphType.UpperLeft:
                    center = new Point (controlBounds.X - 3, controlBounds.Y - 3);
                    cursor = Cursors.TopLeftCorner;
                    break;
                case GrabHandleGlyphType.MiddleTop:
                    center = new Point (controlBounds.X + (controlBounds.Width / 2), controlBounds.Y - 3);
                    cursor = Cursors.TopSide;
                    break;
                case GrabHandleGlyphType.UpperRight:
                    center = new Point (controlBounds.Right + 2, controlBounds.Y - 3);
                    cursor = Cursors.TopRightCorner;
                    break;
                case GrabHandleGlyphType.MiddleLeft:
                    center = new Point (controlBounds.X - 3, controlBounds.Y + (controlBounds.Height / 2));
                    cursor = Cursors.LeftSide;
                    break;
                case GrabHandleGlyphType.MiddleRight:
                    center = new Point (controlBounds.Right + 2, controlBounds.Y + (controlBounds.Height / 2));
                    cursor = Cursors.RightSide;
                    break;
                case GrabHandleGlyphType.LowerLeft:
                    center = new Point (controlBounds.X - 3, controlBounds.Bottom + 2);
                    cursor = Cursors.BottomLeftCorner;
                    break;
                case GrabHandleGlyphType.MiddleBottom:
                    center = new Point (controlBounds.X + (controlBounds.Width / 2), controlBounds.Bottom + 2);
                    cursor = Cursors.BottomSide;
                    break;
                case GrabHandleGlyphType.LowerRight:
                    center = new Point (controlBounds.Right + 2, controlBounds.Bottom + 2);
                    cursor = Cursors.BottomRightCorner;
                    break;
                default:
                    throw new NotImplementedException (); ;
            }
        }

        public override Cursor? GetHitTest (Point p)
        {
            if (Bounds.Contains (p))
                return cursor;

            return null;
        }

        public override void Paint (PaintEventArgs pe)
        {
            var canvas = pe.Canvas;

            canvas.FillRectangle (center.X - 2, center.Y - 2, 5, 5, SKColors.White);

            canvas.DrawLine (center.X - 2, center.Y - 3, center.X + 3, center.Y - 3, SKColors.Black, 1);
            canvas.DrawLine (center.X - 2, center.Y + 3, center.X + 3, center.Y + 3, SKColors.Black, 1);

            canvas.DrawLine (center.X - 3, center.Y - 2, center.X - 3, center.Y + 3, SKColors.Black, 1);
            canvas.DrawLine (center.X + 3, center.Y - 2, center.X + 3, center.Y + 3, SKColors.Black, 1);
        }

        public Size GetNewSize (Rectangle currentBounds, Size minimumSize, Point point)
        {
            return Direction switch {
                GrabHandleGlyphType.MiddleRight => new Size (Math.Max (point.X - currentBounds.Left, minimumSize.Width), currentBounds.Height - 1),
                GrabHandleGlyphType.MiddleBottom => new Size (currentBounds.Width - 1, Math.Max (point.Y - currentBounds.Top, minimumSize.Height)),//new Size (bounds.Width, bounds.Height + (point.Y - resizeAnchor.Y)),
                GrabHandleGlyphType.LowerRight => new Size (Math.Max (point.X - currentBounds.Left, minimumSize.Width), Math.Max (point.Y - currentBounds.Top, minimumSize.Height)),
                _ => throw new NotImplementedException ()
            };
        }

        public Rectangle GetNewBounds (Rectangle currentBounds, Size minimumSize, Point point)
        {
            var left = Math.Min (currentBounds.Right - minimumSize.Width, point.X);
            var top = Math.Min (point.Y, currentBounds.Bottom - minimumSize.Height);
            var right = Math.Max (currentBounds.Left + minimumSize.Width, point.X);
            var bottom = Math.Max (point.Y, currentBounds.Top + minimumSize.Height);

            return Direction switch {
                GrabHandleGlyphType.UpperLeft => Rectangle.FromLTRB (left, top, currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.MiddleTop => Rectangle.FromLTRB (currentBounds.Left, top, currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.UpperRight => Rectangle.FromLTRB (currentBounds.Left, top, right, currentBounds.Bottom),
                GrabHandleGlyphType.MiddleLeft => Rectangle.FromLTRB (left, currentBounds.Top, currentBounds.Right, currentBounds.Bottom),
                GrabHandleGlyphType.MiddleRight => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top, right, currentBounds.Bottom),
                GrabHandleGlyphType.LowerLeft => Rectangle.FromLTRB (left, currentBounds.Top, currentBounds.Right, bottom),
                GrabHandleGlyphType.MiddleBottom => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top, currentBounds.Right, bottom),
                GrabHandleGlyphType.LowerRight => Rectangle.FromLTRB (currentBounds.Left, currentBounds.Top, right, bottom),
                _ => throw new NotImplementedException ()
            };
        }
    }
}
