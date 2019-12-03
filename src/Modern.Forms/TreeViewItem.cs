using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItem : ILayoutable
    {
        private const int INDENT_SIZE = 18;
        private const int IMAGE_SIZE = 16;
        private const int GLYPH_SIZE = 10;

        private readonly TreeView? tree_view;

        private bool expanded;
        private TreeViewItemCollection? items;

        public ContextMenu? ContextMenu { get; set; }

        public string Text { get; set; } = string.Empty;
        public SKBitmap? Image { get; set; }
        public TreeViewItemCollection Items => items ??= new TreeViewItemCollection (this);
        public bool Selected { get; set; }
        public object? Tag { get; set; }
        public TreeViewItem? Parent { get; internal set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin => new Padding (0);

        public TreeViewItem ()
        {
        }

        public TreeViewItem (string text) : this () => Text = text;

        public TreeViewItem (string text, params TreeViewItem[] children) : this (text) => Items.AddRange (children);

        // This constructor is used by the TreeView to create the root node
        internal TreeViewItem (TreeView treeView)
        {
            tree_view = treeView;
            Expanded = true;
        }

        public bool Expanded {
            get => expanded;
            set {
                if (value)
                    TreeView?.OnBeforeExpand (new EventArgs<TreeViewItem> (this));

                // If no nodes were added, don't actually expand
                // Note this also calls Items, which creates the collection, denoting that an
                // Expand has been called and we don't need to draw the dropdown glyph anymore
                if (tree_view == null && value && Items.Count == 0)
                    value = false;

                expanded = value;
                Invalidate ();
            }
        }

        public Size GetPreferredSize (Size proposedSize)
        {
            var font_size = LogicalToDeviceUnits (Theme.FontSize);
            var padding = LogicalToDeviceUnits (10);

            return new Size (0, font_size + padding);
        }

        public bool HasChildren => (items?.Count ?? 0) > 0;

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

        public void Invalidate ()
        {
            TreeView?.Invalidate ();
        }

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public TreeView? TreeView {
            get {
                if (tree_view != null)
                    return tree_view;

                return Parent?.TreeView;
            }
        }

        internal IEnumerable<TreeViewItem> GetAllItems ()
        {
            yield return this;

            if (HasChildren)
                foreach (var item in Items)
                    foreach (var child in item.GetAllItems ())
                        yield return child;
        }

        internal TreeViewItemElement GetElementAtLocation (Point location)
        {
            // Give the user a slightly more generous click target
            var glyph_bounds = GetGlyphBounds ();

            if (!glyph_bounds.IsEmpty)
                glyph_bounds.Inflate (4, 4);

            if (glyph_bounds.Contains (location))
                return TreeViewItemElement.Glyph;

            return TreeViewItemElement.None;
        }

        internal Rectangle GetGlyphBounds ()
        {
            if (!ShowDropdownGlyph)
                return Rectangle.Empty;

            var glyph_area = new Rectangle (GetIndentStart (), Bounds.Top, LogicalToDeviceUnits (GLYPH_SIZE), Bounds.Height);
            var glyph_bounds = DrawingExtensions.CenterSquare (glyph_area, LogicalToDeviceUnits (GLYPH_SIZE));

            glyph_bounds.Width = LogicalToDeviceUnits (GLYPH_SIZE);

            return glyph_bounds;
        }

        internal Rectangle GetImageBounds ()
        {
            if (!ShowItemImage || Image is null)
                return Rectangle.Empty;

            var left_index = ShowDropdownGlyph ? GetGlyphBounds ().Right : GetIndentStart ();
            var image_area = new Rectangle (left_index, Bounds.Top, Bounds.Height, Bounds.Height);

            return DrawingExtensions.CenterSquare (image_area, LogicalToDeviceUnits (IMAGE_SIZE));
        }

        internal int GetIndentStart () => Bounds.Left + IndentLevel * LogicalToDeviceUnits (INDENT_SIZE) + 2;

        internal Rectangle GetTextBounds ()
        {
            var show_glyph = ShowDropdownGlyph;
            var show_image = ShowItemImage;

            if (!show_glyph && !show_image)
                return new Rectangle (GetIndentStart (), Bounds.Top, Bounds.Width - GetIndentStart (), Bounds.Height);

            // One of these will be valid because we handled the other case above
            var padding = LogicalToDeviceUnits (6);
            var used_bounds = show_image ? GetImageBounds () : GetGlyphBounds ();
            return new Rectangle (used_bounds.Right + padding, Bounds.Top, Bounds.Right - used_bounds.Right - padding, Bounds.Height);
        }

        internal int GetVisibleChildrenCount ()
        {
            var value = 0;

            if (!Expanded || !HasChildren)
                return value;

            foreach (var item in Items)
                value += 1 + item.GetVisibleChildrenCount ();

            return value;
        }

        internal IEnumerable<TreeViewItem> GetVisibleItems ()
        {
            yield return this;

            if (Expanded && HasChildren)
                foreach (var item in Items)
                    foreach (var child in item.GetVisibleItems ())
                        yield return child;
        }

        internal void OnPaint (PaintEventArgs e)
        {
            var background_color = Selected ? Theme.RibbonItemHighlightColor : Theme.LightNeutralGray;

            e.Canvas.FillRectangle (Bounds, background_color);

            var tree_view = TreeView;

            if (tree_view?.ShowDropdownGlyph == true) {
                var glyph_bounds = GetGlyphBounds ();

                if (ShouldDrawDropdownGlyph)
                    ControlPaint.DrawArrowGlyph (e, glyph_bounds, Theme.DarkTextColor, Expanded ? ArrowDirection.Down : ArrowDirection.Right);
            }

            if (tree_view?.ShowItemImages == true && Image != null) {
                var image_bounds = GetImageBounds ();

                e.Canvas.DrawBitmap (Image!, image_bounds);
            }

            if (string.IsNullOrWhiteSpace (Text))
                return;

            var text_bounds = GetTextBounds ();

            e.Canvas.DrawText (Text.Trim (), Theme.UIFont, LogicalToDeviceUnits (Theme.FontSize), text_bounds, Theme.DarkTextColor, ContentAlignment.MiddleLeft);
        }

        private int LogicalToDeviceUnits (int value) => TreeView?.LogicalToDeviceUnits (value) ?? value;

        private bool ShouldDrawDropdownGlyph => ShowDropdownGlyph && (HasChildren || (TreeView?.VirtualMode == true && items == null));

        private bool ShowDropdownGlyph => TreeView?.ShowDropdownGlyph == true;

        private bool ShowItemImage => TreeView?.ShowItemImages == true;

        protected internal enum TreeViewItemElement
        {
            None,
            Glyph,
            Image,
            Text
        }
    }
}
