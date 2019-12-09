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

        public event EventHandler<MouseEventArgs>? Click;

        public MenuItem ()
        {
        }

        public MenuItem (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            Text = text;
            Image = image;
            Click += onClick;
        }

        public Rectangle Bounds { get; private set; }

        public virtual Size GetPreferredSize (Size proposedSize)
        {
            var owner = OwnerControl;

            if (owner is Menu menu) {
                var padding = menu.LogicalToDeviceUnits (Padding.Horizontal);
                var font_size = menu.LogicalToDeviceUnits (Theme.FontSize);
                var text_size = (int)Math.Round (TextMeasurer.MeasureText (Text, Theme.UIFont, font_size).Width);

                return new Size (text_size + padding, Bounds.Height);
            }

            if (owner is ToolBar bar) {
                var width = bar.LogicalToDeviceUnits (Padding.Horizontal);
                var font_size = bar.LogicalToDeviceUnits (Theme.FontSize);
                width += (int)Math.Round (TextMeasurer.MeasureText (Text, Theme.UIFont, font_size).Width);

                if (!(Image is null))
                    width += bar.LogicalToDeviceUnits (20);

                if (HasItems)
                    width += bar.LogicalToDeviceUnits (14);

                return new Size (width, Bounds.Height);
            }

            if (owner is MenuDropDown dropdown) {
                var padding = dropdown.LogicalToDeviceUnits (Padding);
                var font_size = dropdown.LogicalToDeviceUnits (Theme.FontSize);
                var text_size = TextMeasurer.MeasureText (Text, Theme.UIFont, font_size);

                return new Size ((int)Math.Round (text_size.Width, 0, MidpointRounding.AwayFromZero) + padding.Horizontal + dropdown.LogicalToDeviceUnits (70), (int)Math.Round (text_size.Height, 0, MidpointRounding.AwayFromZero) + dropdown.LogicalToDeviceUnits (8));
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
            var owner = OwnerControl;

            if (owner is Menu menu) {
                // Background
                var background_color = Hovered || IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;
                canvas.FillRectangle (Bounds, background_color);

                // Text
                var font_color = Theme.DarkTextColor;
                var font_size = menu.LogicalToDeviceUnits (Theme.FontSize);

                canvas.DrawText (Text, Theme.UIFont, font_size, Bounds, font_color, ContentAlignment.MiddleCenter);

                return;
            }

            if (owner is ToolBar bar) {
                // Background
                var background_color = Hovered || IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;
                canvas.FillRectangle (Bounds, background_color);

                var bounds = Bounds;
                bounds.X += bar.LogicalToDeviceUnits (8);

                // Image
                if (Image != null) {
                    var image_size = bar.LogicalToDeviceUnits (20);
                    var image_bounds = DrawingExtensions.CenterSquare (Bounds, image_size);
                    var image_rect = new Rectangle (bounds.Left, image_bounds.Top, image_size, image_size);
                    canvas.DrawBitmap (Image, image_rect);

                    bounds.X += bar.LogicalToDeviceUnits (28);
                } else {
                    bounds.X += bar.LogicalToDeviceUnits (4);
                }

                // Text
                var font_color = Theme.DarkTextColor;
                var font_size = bar.LogicalToDeviceUnits (Theme.FontSize);

                bounds.Y += 1;
                canvas.DrawText (Text, Theme.UIFont, font_size, bounds, font_color, ContentAlignment.MiddleLeft);
                bounds.Y -= 1;

                // Dropdown Arrow
                if (HasItems) {
                    var arrow_bounds = DrawingExtensions.CenterSquare (Bounds, 16);
                    var arrow_area = new Rectangle (Bounds.Right - bar.LogicalToDeviceUnits (16) - 4, arrow_bounds.Top, 16, 16);
                    ControlPaint.DrawArrowGlyph (new PaintEventArgs (SKImageInfo.Empty, canvas, bar.Scaling), arrow_area, Theme.DarkTextColor, ArrowDirection.Down);
                }

                return;
            }

            if (owner is MenuDropDown dropdown) {
                // Background
                var background_color = Hovered || IsDropDownOpened ? Theme.RibbonItemHighlightColor : Theme.LightTextColor;
                canvas.FillRectangle (Bounds, background_color);

                // Image
                if (Image != null) {
                    var image_size = dropdown.LogicalToDeviceUnits (16);
                    var image_bounds = DrawingExtensions.CenterSquare (Bounds, image_size);
                    var image_rect = new Rectangle (Bounds.Left + dropdown.LogicalToDeviceUnits (6), image_bounds.Top, image_size, image_size);
                    canvas.DrawBitmap (Image, image_rect);
                }

                // Text
                var font_color = Theme.DarkTextColor;
                var font_size = dropdown.LogicalToDeviceUnits (Theme.FontSize);
                var bounds = Bounds;
                bounds.X += dropdown.LogicalToDeviceUnits (28);
                canvas.DrawText (Text, Theme.UIFont, font_size, bounds, font_color, ContentAlignment.MiddleLeft);

                // Dropdown Arrow
                if (HasItems) {
                    var arrow_bounds = DrawingExtensions.CenterSquare (Bounds, 16);
                    var arrow_area = new Rectangle (Bounds.Right - dropdown.LogicalToDeviceUnits (16) - 4, arrow_bounds.Top, 16, 16);
                    ControlPaint.DrawArrowGlyph (new PaintEventArgs (SKImageInfo.Empty, canvas, dropdown.Scaling), arrow_area, Theme.DarkTextColor, ArrowDirection.Right);
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

                if (OwnerControl is Menu || OwnerControl is ToolBar)
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
