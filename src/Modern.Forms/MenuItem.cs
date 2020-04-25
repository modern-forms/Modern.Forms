using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a MenuItem menu item.
    /// </summary>
    public class MenuItem : ILayoutable
    {
        private MenuItemCollection? items;
        private MenuDropDown? dropdown;
        private bool enabled = true;
        private bool selected;

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        public MenuItem ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        public MenuItem (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            Text = text;
            Image = image;
            Click += onClick;
        }

        /// <summary>
        /// Gets the bounding box of this menu item.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Raised when the menu item is clicked.
        /// </summary>
        public event EventHandler<MouseEventArgs>? Click;

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled.
        /// </summary>
        public bool Enabled {
            get => enabled && OwnerControl?.Enabled == true;
            set {
                if (enabled != value) {
                    enabled = value;
                    OwnerControl?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Returns a preferred size the menu item would like to be.
        /// </summary>
        public virtual Size GetPreferredSize (Size proposedSize)
        {
            var owner = OwnerControl;

            if (owner is null)
                return proposedSize;

            var renderer = RenderManager.GetRenderer<Renderer> (owner);

            if (owner is Menu menu && renderer is MenuRenderer menu_renderer)
                return menu_renderer.GetPreferredItemSize (menu, this, proposedSize);

            if (owner is ToolBar tb && renderer is ToolBarRenderer tb_renderer)
                return tb_renderer.GetPreferredItemSize (tb, this, proposedSize);

            if (owner is MenuDropDown mdd && renderer is MenuDropDownRenderer mdd_renderer)
                return mdd_renderer.GetPreferredItemSize (mdd, this, proposedSize);

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

        /// <summary>
        /// Gets a value indicating if this menu item has any child items.
        /// </summary>
        public bool HasItems => items?.Any () == true;

        /// <summary>
        /// Closes the menu item's drop down.
        /// </summary>
        public void HideDropDown ()
        {
            selected = false;
            dropdown?.Hide ();
            IsDropDownOpened = false;

            // Recursively close any child dropdowns
            foreach (var child in Items)
                child.HideDropDown ();
        }

        /// <summary>
        /// Gets a value indicating the mouse cursor is currently hovering over this menu item.
        /// </summary>
        public bool Hovered { get; internal set; }

        /// <summary>
        /// Gets or sets an image to be displayed on the menu item.
        /// </summary>
        public SKBitmap? Image { get; set; }

        /// <summary>
        /// Gets a value indicating this menu item's drop down is currently open.
        /// </summary>
        public bool IsDropDownOpened { get; private set; }

        /// <summary>
        /// Gets the collection of menu items contained by this menu item.
        /// </summary>
        public MenuItemCollection Items => items ??= new MenuItemCollection (this);

        /// <summary>
        /// Gets or sets the margin of this menu item.
        /// </summary>
        public Padding Margin { get; set; } = Padding.Empty;

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        protected internal virtual void OnClick (MouseEventArgs e)
        {
            Click?.Invoke (this, e);
        }

        // The Control that owns this menu item.
        private Control? OwnerControl {
            get {
                if (ParentControl != null)
                    return ParentControl;

                if (this is MenuRootItem root)
                    return root.Control;

                return Parent?.OwnerControl;
            }
        }

        /// <summary>
        /// Gets or sets the amount of padding to apply to the menu item.
        /// </summary>
        public Padding Padding { get; set; } = new Padding (14, 3, 14, 3);

        /// <summary>
        /// The parent menu item this item belongs to, if any.
        /// </summary>
        public MenuItem? Parent { get; internal set; }

        // The control this MenuItem is parented to, for example a MenuDropDown or a Menu
        internal Control? ParentControl { get; set; }

        /// <summary>
        /// Gets a value indicating if this menu item is currently selected.
        /// </summary>
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

        /// <summary>
        /// Sets the bounds of the menu item. This API is considered internal and is not intended for public use.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        /// <summary>
        /// Shows this menu items drop down, if any.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the text of the menu item.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
