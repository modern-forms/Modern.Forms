using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a TreeViewItem.
    /// </summary>
    public class TreeViewItem : ILayoutable
    {
        private readonly TreeView? tree_view;

        private bool expanded;
        internal TreeViewItemCollection? items;

        /// <summary>
        /// Initializes a new instance of the TreeViewItem class.
        /// </summary>
        public TreeViewItem ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewItem class with the specified text.
        /// </summary>
        public TreeViewItem (string text) : this () => Text = text;

        /// <summary>
        /// Initializes a new instance of the TreeViewItem class with the specified text and child nodes.
        /// </summary>
        public TreeViewItem (string text, params TreeViewItem[] children) : this (text) => Items.AddRange (children);

        // This constructor is used by the TreeView to create the root node
        internal TreeViewItem (TreeView treeView)
        {
            tree_view = treeView;
            Expanded = true;
        }

        /// <summary>
        /// Gets the current bounding box of the item.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Hides this item's children.
        /// </summary>
        public void Collapse ()
        {
            // Don't let the root_item be collapsed
            if (expanded && tree_view is null) {
                expanded = false;
                Invalidate ();
            }
        }

        /// <summary>
        /// Gets or sets a context menu to display when the item is right-clicked.
        /// </summary>
        public ContextMenu? ContextMenu { get; set; }

        /// <summary>
        /// Ensure this item is visible, expanding items and scrolling the view as needed.
        /// </summary>
        public void EnsureVisible ()
        {
            tree_view?.EnsureItemVisible (this);
        }

        /// <summary>
        /// Shows this item's children.
        /// </summary>
        public void Expand ()
        {
            TreeView?.OnBeforeExpand (new EventArgs<TreeViewItem> (this));

            // If no nodes were added, don't actually expand
            // Note this also calls Items, which creates the collection, denoting that an
            // Expand has been called and we don't need to draw the dropdown glyph anymore
            if (tree_view == null && Items.Count == 0) {
                Invalidate ();
                return;
            }

            expanded = true;
            Invalidate ();
        }

        /// <summary>
        /// Gets or sets a value indicating this node is showing its child nodes.
        /// </summary>
        public bool Expanded {
            get => expanded;
            set {
                if (value)
                    Expand ();
                else
                    Collapse ();
            }
        }

        // Get an IEnumerable of this item and all of its children, recursive.
        internal IEnumerable<TreeViewItem> GetAllItems ()
        {
            yield return this;

            if (HasChildren)
                foreach (var item in Items)
                    foreach (var child in item.GetAllItems ())
                        yield return child;
        }

        // Gets the element of the item at the specified location.
        internal TreeViewItemElement GetElementAtLocation (Point location)
        {
            var tv = TreeView;

            if (tv is null)
                return TreeViewItemElement.None;

            var renderer = RenderManager.GetRenderer<TreeViewRenderer> ();

            var glyph_bounds = renderer!.GetGlyphBounds (tv, this);

            // Give the user a slightly more generous click target
            if (!glyph_bounds.IsEmpty)
                glyph_bounds.Inflate (4, 4);

            if (glyph_bounds.Contains (location))
                return TreeViewItemElement.Glyph;

            return TreeViewItemElement.None;
        }

        /// <summary>
        /// Gets the preferred size of the item.
        /// </summary>
        public virtual Size GetPreferredSize (Size proposedSize)
        {
            var font_size = LogicalToDeviceUnits (Theme.FontSize);
            var padding = LogicalToDeviceUnits (10);

            return new Size (0, font_size + padding);
        }

        // Gets the number of currently visible children nodes, recursively.
        // Note this is nodes whose state is visible (parent is expanded).
        // Not necessarily nodes currently scrolled into view.
        internal int GetVisibleChildrenCount () => GetVisibleItems ().Count () - 1;

        // Gets an enumerator of this node and currently visible children nodes, recursively.
        // Note this is nodes whose state is visible (parent is expanded).
        // Not necessarily nodes currently scrolled into view.
        internal IEnumerable<TreeViewItem> GetVisibleItems ()
        {
            yield return this;

            if (Expanded && HasChildren)
                foreach (var item in Items)
                    foreach (var child in item.GetVisibleItems ())
                        yield return child;
        }

        /// <summary>
        /// Gets a value indicating whether this item contains child items.
        /// </summary>
        public bool HasChildren => (items?.Count ?? 0) > 0;

        /// <summary>
        /// Gets or sets the image of the item.
        /// </summary>
        public SKBitmap? Image { get; set; }

        /// <summary>
        /// Gets a value indicating how many levels this item is nested from the root.
        /// </summary>
        public int IndentLevel {
            get {
                // Root node is -1
                if (tree_view != null)
                    return -1;

                // If this is called without a Parent, return 0 ?
                if (Parent == null)
                    return 0;

                return Parent.IndentLevel + 1;
            }
        }

        // Invalidates the node.
        internal void Invalidate ()
        {
            TreeView?.Invalidate ();
        }

        /// <summary>
        /// Gets the collection of child nodes.
        /// </summary>
        public TreeViewItemCollection Items => items ??= new TreeViewItemCollection (this);

        /// <summary>
        /// Gets the amount of margin to leave around this item. This is internal API and should not be called.
        /// </summary>
        public Padding Margin => Padding.Empty;

        /// <summary>
        /// Retrives the next sibling of this item.
        /// </summary>
        public TreeViewItem? NextItem ()
        {
            if (Parent is TreeViewItem parent) {
                var index = Parent.Items.IndexOf (this);

                if (parent.Items.Count > index + 1)
                    return parent.Items[index + 1];
            }

            return null;
        }

        /// <summary>
        /// The parent item that contains this item.
        /// </summary>
        public TreeViewItem? Parent { get; internal set; }

        /// <summary>
        /// Retrives the previous sibling of this item.
        /// </summary>
        public TreeViewItem? PreviousItem ()
        {
            if (Parent is TreeViewItem parent) {
                var index = Parent.Items.IndexOf (this);

                if (index > 0)
                    return parent.Items[index - 1];
            }

            return null;
        }

        /// <summary>
        /// Sets the bounding box of the item. This is internal API and should not be called.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        /// <summary>
        /// Gets or sets an object with additional user data about this item.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets the TreeView that contains this item.
        /// </summary>
        public TreeView? TreeView {
            get {
                if (tree_view != null)
                    return tree_view;

                return Parent?.TreeView;
            }
        }

        private int LogicalToDeviceUnits (int value) => TreeView?.LogicalToDeviceUnits (value) ?? value;

        /// <summary>
        /// Elements of a TreeViewItem.
        /// </summary>
        protected internal enum TreeViewItemElement
        {
            /// <summary>
            /// No element.
            /// </summary>
            None,

            /// <summary>
            /// The glyph (dropdown arrow) of the TreeViewItem.
            /// </summary>
            Glyph,

            /// <summary>
            /// The image of the TreeViewItem.
            /// </summary>
            Image,

            /// <summary>
            /// The text of the TreeViewItem.
            /// </summary>
            Text
        }
    }
}
