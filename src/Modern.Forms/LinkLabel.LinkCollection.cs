using System.Collections;

namespace Modern.Forms
{
    public partial class LinkLabel
    {
        /// <summary>
        /// Represents a strongly typed collection of <see cref="Link"/> objects.
        /// </summary>
        public class LinkCollection : IList<Link>
        {
            private readonly LinkLabel owner;
            private readonly List<Link> items = [];

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkCollection"/> class.
            /// </summary>
            /// <param name="owner">The owning <see cref="LinkLabel"/>.</param>
            public LinkCollection (LinkLabel owner)
            {
                this.owner = owner ?? throw new ArgumentNullException (nameof (owner));
            }

            /// <summary>
            /// Gets the number of links in the collection.
            /// </summary>
            public int Count => items.Count;

            /// <summary>
            /// Gets a value indicating whether the collection is read-only.
            /// </summary>
            public bool IsReadOnly => false;

            /// <summary>
            /// Gets or sets the link at the specified index.
            /// </summary>
            public Link this[int index] {
                get => items[index];
                set {
                    ArgumentNullException.ThrowIfNull (value);

                    value.Owner = owner;
                    items[index] = value;

                    SortByStart ();
                    owner.ValidateNoOverlappingLinks ();
                    owner.UpdateSelectability ();
                    owner.InvalidateLayout ();
                    owner.Invalidate ();
                }
            }

            /// <summary>
            /// Adds a link to the collection.
            /// </summary>
            /// <param name="item">The link to add.</param>
            public void Add (Link item)
            {
                ArgumentNullException.ThrowIfNull (item);

                item.Owner = owner;
                items.Add (item);

                SortByStart ();
                owner.ValidateNoOverlappingLinks ();
                owner.UpdateSelectability ();
                owner.InvalidateLayout ();
                owner.Invalidate ();
            }

            /// <summary>
            /// Adds a link to the collection.
            /// </summary>
            /// <param name="start">The zero-based starting character index.</param>
            /// <param name="length">The length of the link.</param>
            /// <returns>The created <see cref="Link"/>.</returns>
            public Link Add (int start, int length)
            {
                var link = new Link (start, length);
                Add (link);
                return link;
            }

            /// <summary>
            /// Adds a link to the collection.
            /// </summary>
            /// <param name="start">The zero-based starting character index.</param>
            /// <param name="length">The length of the link.</param>
            /// <param name="linkData">An arbitrary object associated with the link.</param>
            /// <returns>The created <see cref="Link"/>.</returns>
            public Link Add (int start, int length, object? linkData)
            {
                var link = new Link (start, length, linkData);
                Add (link);
                return link;
            }

            /// <summary>
            /// Removes all links from the collection.
            /// </summary>
            public void Clear ()
            {
                items.Clear ();
                owner.UpdateSelectability ();
                owner.InvalidateLayout ();
                owner.Invalidate ();
            }

            /// <summary>
            /// Determines whether the specified link exists in the collection.
            /// </summary>
            /// <param name="item">The link to locate.</param>
            /// <returns><see langword="true"/> if the link exists; otherwise, <see langword="false"/>.</returns>
            public bool Contains (Link item) => items.Contains (item);

            /// <summary>
            /// Copies the collection to an array.
            /// </summary>
            /// <param name="array">The destination array.</param>
            /// <param name="arrayIndex">The zero-based starting index in the destination array.</param>
            public void CopyTo (Link[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator over the collection.</returns>
            public IEnumerator<Link> GetEnumerator () => items.GetEnumerator ();

            /// <summary>
            /// Returns the index of the specified link.
            /// </summary>
            /// <param name="item">The link to locate.</param>
            /// <returns>The zero-based index of the link, or -1 if not found.</returns>
            public int IndexOf (Link item) => items.IndexOf (item);

            /// <summary>
            /// Inserts a link at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index.</param>
            /// <param name="item">The link to insert.</param>
            public void Insert (int index, Link item)
            {
                ArgumentNullException.ThrowIfNull (item);

                item.Owner = owner;
                items.Insert (index, item);

                SortByStart ();
                owner.ValidateNoOverlappingLinks ();
                owner.UpdateSelectability ();
                owner.InvalidateLayout ();
                owner.Invalidate ();
            }

            /// <summary>
            /// Removes the first occurrence of the specified link.
            /// </summary>
            /// <param name="item">The link to remove.</param>
            /// <returns><see langword="true"/> if the link was removed; otherwise, <see langword="false"/>.</returns>
            public bool Remove (Link item)
            {
                var removed = items.Remove (item);

                if (removed) {
                    owner.UpdateSelectability ();
                    owner.InvalidateLayout ();
                    owner.Invalidate ();

                    if (ReferenceEquals (owner.FocusLink, item))
                        owner.FocusLink = items.Count > 0 ? items[0] : null;
                }

                return removed;
            }

            /// <summary>
            /// Removes the link at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the link to remove.</param>
            public void RemoveAt (int index)
            {
                var removed = items[index];
                items.RemoveAt (index);

                owner.UpdateSelectability ();
                owner.InvalidateLayout ();
                owner.Invalidate ();

                if (ReferenceEquals (owner.FocusLink, removed))
                    owner.FocusLink = items.Count > 0 ? items[0] : null;
            }

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

            /// <summary>
            /// Returns the first link with the specified name.
            /// </summary>
            /// <param name="key">The link name to locate.</param>
            /// <returns>The matching link, or <see langword="null"/> if not found.</returns>
            public Link? this[string key] => string.IsNullOrEmpty (key)
                ? null
                : items.FirstOrDefault (item => string.Equals (item.Name, key, StringComparison.OrdinalIgnoreCase));

            /// <summary>
            /// Removes the first link with the specified name.
            /// </summary>
            /// <param name="key">The link name to remove.</param>
            public void RemoveByKey (string key)
            {
                var item = this[key];
                if (item is not null)
                    Remove (item);
            }

            /// <summary>
            /// Sorts the collection by link start position.
            /// </summary>
            internal void SortByStart ()
            {
                items.Sort ((left, right) => left.Start.CompareTo (right.Start));
            }

            /// <summary>
            /// Clears cached visual bounds on all links.
            /// </summary>
            internal void ClearVisualBounds ()
            {
                foreach (var item in items)
                    item.ClearVisualBounds ();
            }
        }
    }
}
