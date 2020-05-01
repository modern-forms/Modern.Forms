using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Defines the border style of a control or form.
    /// </summary>
    public class BorderStyle
    {
        private readonly BorderStyle? _parent;

        private SKColor color;
        private int width;

        /// <summary>
        /// Initializes a new instance of the BorderStyle class.
        /// </summary>
        public BorderStyle (BorderStyle? parent)
        {
            _parent = parent;

            Left = new BorderSideStyle (_parent?.Left);
            Top = new BorderSideStyle (_parent?.Top);
            Right = new BorderSideStyle (_parent?.Right);
            Bottom = new BorderSideStyle (_parent?.Bottom);
        }

        /// <summary>
        /// Gets the styles for the bottom border.
        /// </summary>
        public BorderSideStyle Bottom { get; }

        /// <summary>
        /// Gets or sets the color of all sides of the border.
        /// </summary>
        public SKColor Color {
            get => color;
            set {
                color = value;
                Left.Color = value;
                Right.Color = value;
                Top.Color = value;
                Bottom.Color = value;
            }
        }

        //public int GetRadius () => Radius ?? _parent?.GetRadius () ?? 0;

        /// <summary>
        /// Gets the styles for the left border.
        /// </summary>
        public BorderSideStyle Left { get; }

        //public int? Radius { get; set; }

        /// <summary>
        /// Gets the styles for the right border.
        /// </summary>
        public BorderSideStyle Right { get; }

        /// <summary>
        /// Gets the styles for the top border.
        /// </summary>
        public BorderSideStyle Top { get; }

        /// <summary>
        /// Gets or sets the width of all sides of the border.
        /// </summary>
        public int Width {
            get => width;
            set {
                width = value;
                Left.Width = value;
                Right.Width = value;
                Top.Width = value;
                Bottom.Width = value;
            }
        }
    }

    /// <summary>
    /// Defines the border style for a single side of a control or form.
    /// </summary>
    public class BorderSideStyle
    {
        private readonly BorderSideStyle? _parent;

        /// <summary>
        /// Initializes a new instance of the BorderSideStyle class.
        /// </summary>
        public BorderSideStyle (BorderSideStyle? parent) => _parent = parent;

        /// <summary>
        /// Gets or sets the color of this side of the border.
        /// </summary>
        public SKColor? Color { get; set; }

        /// <summary>
        /// Gets the computed color of this side of the border.
        /// </summary>
        public SKColor GetColor () => Color ?? _parent?.GetColor () ?? Theme.BorderGray;

        /// <summary>
        /// Gets or sets the width of this side of the border.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets the computed width of this side of the border.
        /// </summary>
        public int GetWidth () => Width ?? _parent?.GetWidth () ?? 0;
    }
}
