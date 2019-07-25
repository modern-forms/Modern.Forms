using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    public class TreeViewItem : ILayoutable
    {
        private const int INDENT_SIZE = 20;
        private const int IMAGE_SIZE = 16;
        private const int GLYPH_SIZE = 8;

        private readonly TreeView tree_view;

        private bool expanded;
        private TreeViewItemCollection items;

        public string Text { get; set; }
        public SKBitmap Image { get; set; }
        public TreeViewItemCollection Items => items ??= new TreeViewItemCollection (this);
        public bool Selected { get; set; }
        public object Tag { get; set; }
        public TreeViewItem Parent { get; internal set; }

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
                expanded = value;
                Invalidate ();
            }
        }

        public Size GetPreferredSize (Size proposedSize)
        {
            var font_size = LogicalToDeviceUnits (Theme.FontSize);
            var padding = LogicalToDeviceUnits (16);

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

        public TreeView TreeView {
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
            if (GetGlyphBounds ().Contains (location))
                return TreeViewItemElement.Glyph;

            return TreeViewItemElement.None;
        }

        internal Rectangle GetGlyphBounds ()
        {
            if (!TreeView.ShowDropdownGlyph)
                return Rectangle.Empty;

            var glyph_area = new Rectangle (Bounds.Left + (IndentLevel * LogicalToDeviceUnits (INDENT_SIZE)), Bounds.Top, Bounds.Height, Bounds.Height);
            var glyph_bounds = DrawingExtensions.CenterSquare (glyph_area, LogicalToDeviceUnits (GLYPH_SIZE));

            glyph_bounds.Width = LogicalToDeviceUnits (GLYPH_SIZE);

            return glyph_bounds;
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

            var left_index = Bounds.Left + (IndentLevel * LogicalToDeviceUnits (INDENT_SIZE));
            var tree_view = TreeView;

            if (tree_view.ShowDropdownGlyph) {
                var glyph_bounds = GetGlyphBounds ();

                if (HasChildren)
                    ControlPaint.DrawArrowGlyph (e, glyph_bounds, Theme.DarkTextColor, Expanded ? ArrowDirection.Down : ArrowDirection.Right);

                left_index = glyph_bounds.Right + LogicalToDeviceUnits (1);
            }

            if (tree_view.ShowItemImages && Image != null) {
                var image_area = new Rectangle (left_index, Bounds.Top, Bounds.Height, Bounds.Height);
                var image_bounds = DrawingExtensions.CenterSquare (image_area, LogicalToDeviceUnits (IMAGE_SIZE));

                e.Canvas.DrawBitmap (Image, image_bounds);

                left_index = image_bounds.Right;
            }

            left_index += LogicalToDeviceUnits (7);

            if (string.IsNullOrWhiteSpace (Text))
                return;

            var text_bounds = new Rectangle (left_index, Bounds.Top, Bounds.Width - left_index, Bounds.Height);
            e.Canvas.DrawText (Text.Trim (), Theme.UIFont, LogicalToDeviceUnits (Theme.FontSize), text_bounds, Theme.DarkTextColor, ContentAlignment.MiddleLeft);
        }

        private int LogicalToDeviceUnits (int value) => TreeView?.LogicalToDeviceUnits (value) ?? value;

        protected internal enum TreeViewItemElement
        {
            None,
            Glyph,
            Image,
            Text
        }
    }
}
