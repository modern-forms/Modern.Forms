using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Modern.Forms.Design
{
    /// <summary>
    ///  The SelectionBorderGlyph draws one side (depending on type) of a SelectionBorder.
    /// </summary>
    internal class SelectionBorderGlyph : SelectionGlyphBase
    {
        // selection border size
        public static int SELECTIONBORDERSIZE = 1;
        // Although the selection border is only 1, we actually want a 3 pixel hittestarea
        public static int SELECTIONBORDERHITAREA = 3;
        // We want to make sure that the 1 pixel selectionborder is centered on the handles. The fact that the border is actually 3 pixels wide works like magic. If you draw a picture, then you will see why.
        //grabhandle size (diameter)
        public static int HANDLESIZE = 7;
        //how much should the grabhandle overlap the control
        public static int HANDLEOVERLAP = 2;
        //we want the selection border to be centered on a grabhandle, so how much do. we need to offset the border from the control to make that happen
        public static int SELECTIONBORDEROFFSET = ((HANDLESIZE - SELECTIONBORDERSIZE) / 2) - HANDLEOVERLAP;

        /// <summary>
        ///  This constructor extends from the standard SelectionGlyphBase constructor.
        /// </summary>
        internal SelectionBorderGlyph (Rectangle controlBounds, SelectionRules rules, SelectionBorderGlyphType type, Behavior? behavior)
            : base (behavior)
        {
            InitializeGlyph (controlBounds, rules, type);
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        ///  Offset - how many pixels between the border glyph and the control
        /// </summary>
        private static Rectangle GetBoundsForSelectionType (Rectangle originalBounds, SelectionBorderGlyphType type, int bordersize, int offset)
        {
            Rectangle bounds = GetBoundsForSelectionType (originalBounds, type, bordersize);
            if (offset != 0) {
                switch (type) {
                    case SelectionBorderGlyphType.Top:
                        bounds.Offset (-offset, -offset);
                        bounds.Width += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Bottom:
                        bounds.Offset (-offset, offset + 2);
                        bounds.Width += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Left:
                        bounds.Offset (-offset, -offset);
                        bounds.Height += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Right:
                        bounds.Offset (offset + 2, -offset);
                        bounds.Height += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Body:
                        bounds = originalBounds;
                        break;
                }
            }

            return bounds;
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        /// </summary>
        public static Rectangle GetBoundsForSelectionType (Rectangle originalBounds, SelectionBorderGlyphType type, int borderSize)
        {
            Rectangle bounds = Rectangle.Empty;
            switch (type) {
                case SelectionBorderGlyphType.Top:
                    bounds = new Rectangle (originalBounds.Left - borderSize, originalBounds.Top - borderSize, originalBounds.Width + 2 * borderSize, borderSize);
                    break;
                case SelectionBorderGlyphType.Bottom:
                    bounds = new Rectangle (originalBounds.Left - borderSize, originalBounds.Bottom, originalBounds.Width + 2 * borderSize, borderSize);
                    break;
                case SelectionBorderGlyphType.Left:
                    bounds = new Rectangle (originalBounds.Left - borderSize, originalBounds.Top - borderSize, borderSize, originalBounds.Height + 2 * borderSize);
                    break;
                case SelectionBorderGlyphType.Right:
                    bounds = new Rectangle (originalBounds.Right, originalBounds.Top - borderSize, borderSize, originalBounds.Height + 2 * borderSize);
                    break;
                case SelectionBorderGlyphType.Body:
                    bounds = originalBounds;
                    break;
            }

            return bounds;
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        /// </summary>
        public static Rectangle GetBoundsForSelectionType (Rectangle originalBounds, SelectionBorderGlyphType type)
        {
            return GetBoundsForSelectionType (originalBounds, type, SELECTIONBORDERSIZE, SELECTIONBORDEROFFSET);
        }

        //public static Rectangle GetBoundsForNoResizeSelectionType (Rectangle originalBounds, SelectionBorderGlyphType type)
        //{
        //    return GetBoundsForSelectionType (originalBounds, type, SELECTIONBORDERSIZE, NORESIZEBORDEROFFSET);
        //}

        /// <summary>
        ///  Helper function that initializes the Glyph based on bounds, type, and bordersize.
        /// </summary>
        private void InitializeGlyph (Rectangle controlBounds, SelectionRules selRules, SelectionBorderGlyphType type)
        {
            rules = SelectionRules.None;
            hitTestCursor = Cursor.Default;

            //this will return the rect representing the bounds of the glyph
            bounds = GetBoundsForSelectionType (controlBounds, type);
            hitBounds = bounds;
            
            // The hitbounds for the border is actually a bit bigger than the glyph bounds
            switch (type) {
                case SelectionBorderGlyphType.Top:
                    if ((selRules & SelectionRules.TopSizeable) != 0) {
                        hitTestCursor = Cursors.SizeNorthSouth;
                        rules = SelectionRules.TopSizeable;
                    }

                    // We want to apply the SELECTIONBORDERHITAREA to the top and the bottom of the selection border glyph
                    hitBounds.Y -= (SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE) / 2;
                    hitBounds.Height += SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE;
                    break;
                case SelectionBorderGlyphType.Bottom:
                    if ((selRules & SelectionRules.BottomSizeable) != 0) {
                        hitTestCursor = Cursors.SizeNorthSouth;
                        rules = SelectionRules.BottomSizeable;
                    }

                    // We want to apply the SELECTIONBORDERHITAREA to the top and the bottom of the selection border glyph
                    hitBounds.Y -= (SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE) / 2;
                    hitBounds.Height += SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE;
                    break;
                case SelectionBorderGlyphType.Left:
                    if ((selRules & SelectionRules.LeftSizeable) != 0) {
                        hitTestCursor = Cursors.SizeWestEast;
                        rules = SelectionRules.LeftSizeable;
                    }

                    // We want to apply the SELECTIONBORDERHITAREA to the left and the right of the selection border glyph
                    hitBounds.X -= (SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE) / 2;
                    hitBounds.Width += SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE;
                    break;
                case SelectionBorderGlyphType.Right:
                    if ((selRules & SelectionRules.RightSizeable) != 0) {
                        hitTestCursor = Cursors.SizeWestEast;
                        rules = SelectionRules.RightSizeable;
                    }

                    // We want to apply the SELECTIONBORDERHITAREA to the left and the right of the selection border glyph
                    hitBounds.X -= (SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE) / 2;
                    hitBounds.Width += SELECTIONBORDERHITAREA - SELECTIONBORDERSIZE;
                    break;
            }
        }

        /// <summary>
        ///  Simple painting logic for selection Glyphs.
        /// </summary>
        public override void Paint (PaintEventArgs pe)
        {
            pe.Canvas.FillRectangle (bounds, SKColors.Gray);
        }
    }
}
