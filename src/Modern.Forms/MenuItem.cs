using System;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    public class MenuItem : ILayoutable
    {
        private MenuItemCollection? items;
        private MenuDropDown? dropdown;
        private bool selected;

        public event EventHandler<MouseEventArgs> Click;

        public Rectangle Bounds { get; private set; }

        public virtual Size GetPreferredSize (Size proposedSize)
        {
            if (OwnerControl is Menu menu) {
                var padding = menu?.LogicalToDeviceUnits (Padding.Horizontal) ?? Padding.Horizontal;
                var font_size = menu?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;
                var text_size = (int)Math.Round (TextMeasurer.MeasureText (Text, Theme.UIFont, font_size));

                return new Size (text_size + padding, Bounds.Height);
            } 
            
            if (OwnerControl is MenuDropDown dropdown) {
                var padding = dropdown?.LogicalToDeviceUnits (Padding) ?? Padding;
                var font_size = dropdown?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;
                var text_size = TextMeasurer.MeasureText (Text, Theme.UIFont, font_size, new SKSize (0, font_size));

                return new Size ((int)Math.Round (text_size.Width, 0, MidpointRounding.AwayFromZero) + padding.Horizontal + 70, (int)Math.Round (text_size.Height, 0, MidpointRounding.AwayFromZero) + 12);
            }

            return proposedSize;
        }

        // Traverses MenuItems and MenuDropDowns to get the top menu
        internal MenuBase? GetTopMenu ()
        {
            var root = this;

            while (root.Parent != null)
                root = root.Parent;

            return (root.OwnerControl as MenuBase);
        }

        public bool HasItems => items?.Any () == true;

        public void HideDropDown ()
        {
            selected = false;
            dropdown?.Hide ();
            IsDropDownOpened = false;

            // Recursively close any child dropdowns
            foreach (var child in Items)
                child.HideDropDown ();
        }

        public bool Hovered { get; internal set; }

        public SKBitmap? Image { get; set; }

        public bool IsDropDownOpened { get; private set; }

        public MenuItemCollection Items => items ??= new MenuItemCollection (this);

        public Padding Margin { get; set; } = Padding.Empty;

        public virtual void OnClick (MouseEventArgs e)
        {
            Click?.Invoke (this, e);
        }

        public virtual void OnPaint (SKCanvas canvas)
        {
            if (OwnerControl is Menu menu) {
                // Background
                var background_color = Hovered || IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;
                canvas.FillRectangle (Bounds, background_color);

                // Text
                var font_color = Theme.DarkTextColor;
                var font_size = menu?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;

                canvas.DrawText (Text, Theme.UIFont, font_size, Bounds, font_color, ContentAlignment.MiddleCenter);

                return;
            }

            if (OwnerControl is MenuDropDown dropdown) {
                // Background
                var background_color = Hovered || IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.LightTextColor;
                canvas.FillRectangle (Bounds, background_color);

                // Image
                if (Image != null) {
                    var image_bounds = DrawingExtensions.CenterSquare (Bounds, 16);
                    var image_rect = new Rectangle (Bounds.Left + 6, image_bounds.Top, 16, 16);
                    canvas.DrawBitmap (Image, image_rect);
                }

                // Text
                var font_color = Theme.DarkTextColor;
                var font_size = dropdown?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;
                var bounds = Bounds;
                bounds.X += 28;
                canvas.DrawText (Text, Theme.UIFont, font_size, bounds, font_color, ContentAlignment.MiddleLeft);

                // Dropdown Arrow
                if (HasItems) {
                    var arrow_bounds = DrawingExtensions.CenterSquare (Bounds, 16);
                    var arrow_area = new Rectangle (Bounds.Right - 20, arrow_bounds.Top, 16, 16);
                    ControlPaint.DrawArrowGlyph (new PaintEventArgs (SKImageInfo.Empty, canvas, OwnerControl?.Scaling ?? 1), arrow_area, Theme.DarkTextColor, ArrowDirection.Right);
                }

                return;
            }
        }

        internal Control? OwnerControl {
            get {
                if (ParentControl != null)
                    return ParentControl;

                if (this is MenuRootItem root)
                    return root.Control;

                return Parent?.OwnerControl;
            }
        }

        public Padding Padding { get; set; } = new Padding (14, 3, 14, 3);

        // If this is a sub MenuItem, this is the parent MenuItem
        public MenuItem? Parent { get; internal set; }

        // The control this MenuItem is parented to, for example a MenuDropDown or a Menu
        internal Control? ParentControl { get; set; }

        public bool Selected {
            get => selected;
            internal set {
                if (selected != value) {
                    selected = value;

                    if (value)
                        ShowDropDown ();
                    else
                        HideDropDown ();
                }
            }
        }

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        public void ShowDropDown ()
        {
            if (HasItems && OwnerControl != null) {
                dropdown = dropdown ??= new MenuDropDown (this);

                var dropdown_location = Point.Empty;

                if (OwnerControl is Menu)
                    dropdown_location = OwnerControl.PointToScreen (new Point (Bounds.Left + 1, Bounds.Bottom));
                else if (OwnerControl is MenuDropDown)
                    dropdown_location = OwnerControl.PointToScreen (new Point (Bounds.Right - 1, Bounds.Top));

                dropdown.Show (dropdown_location);
                IsDropDownOpened = true;
            }
        }

        public string Text { get; set; } = string.Empty;
    }
}
