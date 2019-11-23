using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    public static class Cursors
    {
        private static Cursor? app_starting;
        private static Cursor? arrow;
        private static Cursor? bottom_left_corner;
        private static Cursor? bottom_right_corner;
        private static Cursor? bottom_side;
        private static Cursor? cross;
        private static Cursor? drag_copy;
        private static Cursor? drag_link;
        private static Cursor? drag_move;
        private static Cursor? hand;
        private static Cursor? help;
        private static Cursor? ibeam;
        private static Cursor? left_side;
        private static Cursor? no;
        private static Cursor? right_side;
        private static Cursor? size_all;
        private static Cursor? size_north_south;
        private static Cursor? size_west_east;
        private static Cursor? top_left_corner;
        private static Cursor? top_right_corner;
        private static Cursor? top_side;
        private static Cursor? up_arrow;
        private static Cursor? wait;

        public static Cursor AppStarting => app_starting ??= new Cursor (Avalonia.Input.StandardCursorType.AppStarting);
        public static Cursor Arrow => arrow ??= new Cursor (Avalonia.Input.StandardCursorType.Arrow);
        public static Cursor BottomLeftCorner => bottom_left_corner ??= new Cursor (Avalonia.Input.StandardCursorType.BottomLeftCorner);
        public static Cursor BottomRightCorner => bottom_right_corner ??= new Cursor (Avalonia.Input.StandardCursorType.BottomRightCorner);
        public static Cursor BottomSide => bottom_side ??= new Cursor (Avalonia.Input.StandardCursorType.BottomSide);
        public static Cursor Cross => cross ??= new Cursor (Avalonia.Input.StandardCursorType.Cross);
        public static Cursor DragCopy => drag_copy ??= new Cursor (Avalonia.Input.StandardCursorType.DragCopy);
        public static Cursor DragLink => drag_link ??= new Cursor (Avalonia.Input.StandardCursorType.DragLink);
        public static Cursor DragMove => drag_move ??= new Cursor (Avalonia.Input.StandardCursorType.DragMove);
        public static Cursor Hand => hand ??= new Cursor (Avalonia.Input.StandardCursorType.Hand);
        public static Cursor Help => help ??= new Cursor (Avalonia.Input.StandardCursorType.Help);
        public static Cursor IBeam => ibeam ??= new Cursor (Avalonia.Input.StandardCursorType.Ibeam);
        public static Cursor LeftSide => left_side ??= new Cursor (Avalonia.Input.StandardCursorType.LeftSide);
        public static Cursor No => no ??= new Cursor (Avalonia.Input.StandardCursorType.No);
        public static Cursor RightSide => right_side ??= new Cursor (Avalonia.Input.StandardCursorType.RightSide);
        public static Cursor SizeAll => size_all ??= new Cursor (Avalonia.Input.StandardCursorType.SizeAll);
        public static Cursor SizeNorthSouth => size_north_south ??= new Cursor (Avalonia.Input.StandardCursorType.SizeNorthSouth);
        public static Cursor SizeWestEast => size_west_east ??= new Cursor (Avalonia.Input.StandardCursorType.SizeWestEast);
        public static Cursor TopLeftCorner => top_left_corner ??= new Cursor (Avalonia.Input.StandardCursorType.TopLeftCorner);
        public static Cursor TopRightCorner => top_right_corner ??= new Cursor (Avalonia.Input.StandardCursorType.TopRightCorner);
        public static Cursor TopSide => top_side ??= new Cursor (Avalonia.Input.StandardCursorType.TopSide);
        public static Cursor UpArrow => up_arrow ??= new Cursor (Avalonia.Input.StandardCursorType.UpArrow);
        public static Cursor Wait => wait ??= new Cursor (Avalonia.Input.StandardCursorType.Wait);
    }
}
