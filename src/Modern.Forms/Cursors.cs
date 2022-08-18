namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of system provided mouse cursors.
    /// </summary>
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

        /// <summary>
        /// The default app starting cursor provided by the operating system.
        /// </summary>
        public static Cursor AppStarting => app_starting ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.AppStarting);

        /// <summary>
        /// The default arrow cursor provided by the operating system.
        /// </summary>
        public static Cursor Arrow => arrow ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Arrow);

        /// <summary>
        /// The default bottom left corner cursor provided by the operating system.
        /// </summary>
        public static Cursor BottomLeftCorner => bottom_left_corner ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.BottomLeftCorner);

        /// <summary>
        /// The default bottom right corner cursor provided by the operating system.
        /// </summary>
        public static Cursor BottomRightCorner => bottom_right_corner ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.BottomRightCorner);

        /// <summary>
        /// The default bottom cursor provided by the operating system.
        /// </summary>
        public static Cursor BottomSide => bottom_side ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.BottomSide);

        /// <summary>
        /// The default cross cursor provided by the operating system.
        /// </summary>
        public static Cursor Cross => cross ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Cross);

        /// <summary>
        /// The default drag copy cursor provided by the operating system.
        /// </summary>
        public static Cursor DragCopy => drag_copy ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.DragCopy);

        /// <summary>
        /// The default drag link cursor provided by the operating system.
        /// </summary>
        public static Cursor DragLink => drag_link ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.DragLink);

        /// <summary>
        /// The default drag move cursor provided by the operating system.
        /// </summary>
        public static Cursor DragMove => drag_move ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.DragMove);

        /// <summary>
        /// The default hand cursor provided by the operating system.
        /// </summary>
        public static Cursor Hand => hand ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Hand);

        /// <summary>
        /// The default help cursor provided by the operating system.
        /// </summary>
        public static Cursor Help => help ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Help);

        /// <summary>
        /// The default ibeam cursor provided by the operating system.
        /// </summary>
        public static Cursor IBeam => ibeam ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Ibeam);

        /// <summary>
        /// The default left side cursor provided by the operating system.
        /// </summary>
        public static Cursor LeftSide => left_side ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.LeftSide);

        /// <summary>
        /// The default no cursor provided by the operating system.
        /// </summary>
        public static Cursor No => no ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.No);

        /// <summary>
        /// The default right side cursor provided by the operating system.
        /// </summary>
        public static Cursor RightSide => right_side ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.RightSide);

        /// <summary>
        /// The default size all cursor provided by the operating system.
        /// </summary>
        public static Cursor SizeAll => size_all ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.SizeAll);

        /// <summary>
        /// The default size north-south cursor provided by the operating system.
        /// </summary>
        public static Cursor SizeNorthSouth => size_north_south ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.SizeNorthSouth);

        /// <summary>
        /// The default size west-east cursor provided by the operating system.
        /// </summary>
        public static Cursor SizeWestEast => size_west_east ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.SizeWestEast);

        /// <summary>
        /// The default top left corner cursor provided by the operating system.
        /// </summary>
        public static Cursor TopLeftCorner => top_left_corner ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.TopLeftCorner);

        /// <summary>
        /// The default top right corner cursor provided by the operating system.
        /// </summary>
        public static Cursor TopRightCorner => top_right_corner ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.TopRightCorner);

        /// <summary>
        /// The default top side cursor provided by the operating system.
        /// </summary>
        public static Cursor TopSide => top_side ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.TopSide);

        /// <summary>
        /// The default up arrow cursor provided by the operating system.
        /// </summary>
        public static Cursor UpArrow => up_arrow ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.UpArrow);

        /// <summary>
        /// The default wait cursor provided by the operating system.
        /// </summary>
        public static Cursor Wait => wait ??= new Cursor (Modern.WindowKit.Input.StandardCursorType.Wait);
    }
}
