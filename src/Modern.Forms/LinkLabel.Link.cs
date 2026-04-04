using System.Drawing;

namespace Modern.Forms
{
    public partial class LinkLabel
    {
        /// <summary>
        /// Represents a clickable link range within a <see cref="LinkLabel"/>.
        /// </summary>
        public class Link
        {
            private readonly List<Rectangle> visual_bounds = [];
            private int start;
            private int length;
            private bool enabled = true;
            private string? name;

            /// <summary>
            /// Initializes a new instance of the <see cref="Link"/> class.
            /// </summary>
            public Link ()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Link"/> class.
            /// </summary>
            /// <param name="start">The zero-based starting character index.</param>
            /// <param name="length">
            /// The length of the link.
            /// A value of -1 means the link extends to the end of the text.
            /// </param>
            public Link (int start, int length)
            {
                this.start = start;
                this.length = length;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Link"/> class.
            /// </summary>
            /// <param name="start">The zero-based starting character index.</param>
            /// <param name="length">
            /// The length of the link.
            /// A value of -1 means the link extends to the end of the text.
            /// </param>
            /// <param name="linkData">An arbitrary object associated with the link.</param>
            public Link (int start, int length, object? linkData)
                : this (start, length)
            {
                LinkData = linkData;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the link can be activated.
            /// </summary>
            public bool Enabled {
                get => enabled;
                set {
                    if (enabled != value) {
                        enabled = value;

                        if ((State & (LinkState.Hover | LinkState.Active)) != 0)
                            State &= ~(LinkState.Hover | LinkState.Active);

                        Owner?.Invalidate ();
                    }
                }
            }

            /// <summary>
            /// Gets or sets the effective length of the link.
            /// </summary>
            public int Length {
                get {
                    if (length == -1) {
                        var text_length = Owner?.Text?.Length ?? 0;
                        return Math.Max (0, text_length - Start);
                    }

                    return length;
                }
                set {
                    if (length != value) {
                        length = value;
                        Owner?.InvalidateLayout ();
                        Owner?.Invalidate ();
                    }
                }
            }

            /// <summary>
            /// Gets or sets an optional object associated with the link.
            /// </summary>
            public object? LinkData { get; set; }

            /// <summary>
            /// Gets or sets the name of the link.
            /// </summary>
            public string Name {
                get => name ?? string.Empty;
                set => name = value;
            }

            /// <summary>
            /// Gets or sets the link owner.
            /// </summary>
            internal LinkLabel? Owner { get; set; }

            /// <summary>
            /// Gets or sets the raw stored length value.
            /// </summary>
            internal int RawLength {
                get => length;
                set => length = value;
            }

            /// <summary>
            /// Gets or sets the zero-based starting character index.
            /// </summary>
            public int Start {
                get => start;
                set {
                    if (start != value) {
                        start = value;

                        if (Owner is not null) {
                            Owner.Links.SortByStart ();
                            Owner.InvalidateLayout ();
                            Owner.Invalidate ();
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets the current visual state of the link.
            /// </summary>
            internal LinkState State { get; set; }

            /// <summary>
            /// Gets or sets an arbitrary user-defined object associated with the link.
            /// </summary>
            public object? Tag { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the link has been visited.
            /// </summary>
            public bool Visited {
                get => (State & LinkState.Visited) == LinkState.Visited;
                set {
                    if (value != Visited) {
                        if (value)
                            State |= LinkState.Visited;
                        else
                            State &= ~LinkState.Visited;

                        Owner?.Invalidate ();
                    }
                }
            }

            /// <summary>
            /// Gets the rectangles occupied by the rendered link.
            /// </summary>
            public IReadOnlyList<Rectangle> VisualBounds => visual_bounds;

            /// <summary>
            /// Clears cached visual bounds.
            /// </summary>
            internal void ClearVisualBounds () => visual_bounds.Clear ();

            /// <summary>
            /// Adds a visual bounds rectangle for the link.
            /// </summary>
            /// <param name="bounds">The bounds to add.</param>
            internal void AddVisualBounds (Rectangle bounds)
            {
                if (bounds.Width > 0 && bounds.Height > 0)
                    visual_bounds.Add (bounds);
            }

            /// <summary>
            /// Determines whether the given point lies within the rendered link area.
            /// </summary>
            /// <param name="location">The point to test.</param>
            /// <returns><see langword="true"/> if the point is within the link; otherwise, <see langword="false"/>.</returns>
            internal bool Contains (Point location) => visual_bounds.Any (bounds => bounds.Contains (location));
        }
    }
}
