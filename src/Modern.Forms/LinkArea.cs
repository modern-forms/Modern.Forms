namespace Modern.Forms
{
    /// <summary>
    /// Represents a range of text treated as a link.
    /// </summary>
    public struct LinkArea : IEquatable<LinkArea>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkArea"/> struct.
        /// </summary>
        /// <param name="start">The zero-based starting character index.</param>
        /// <param name="length">The length of the link range.</param>
        public LinkArea (int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Gets or sets the zero-based starting character index of the link range.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the length of the link range.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets a value indicating whether this link area is empty.
        /// </summary>
        public bool IsEmpty => Start == 0 && Length == 0;

        /// <inheritdoc/>
        public override bool Equals (object? obj) => obj is LinkArea other && Equals (other);

        /// <inheritdoc/>
        public bool Equals (LinkArea other) => Start == other.Start && Length == other.Length;

        /// <inheritdoc/>
        public override int GetHashCode () => HashCode.Combine (Start, Length);

        /// <summary>
        /// Determines whether two <see cref="LinkArea"/> values are equal.
        /// </summary>
        public static bool operator == (LinkArea left, LinkArea right) => left.Equals (right);

        /// <summary>
        /// Determines whether two <see cref="LinkArea"/> values are not equal.
        /// </summary>
        public static bool operator != (LinkArea left, LinkArea right) => !left.Equals (right);

        /// <inheritdoc/>
        public override string ToString () => $"{{Start={Start}, Length={Length}}}";
    }
}
