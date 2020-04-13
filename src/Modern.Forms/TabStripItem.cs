using System;
using System.Drawing;

namespace Modern.Forms
{
    public class TabStripItem : ILayoutable
    {
        private bool enabled = true;
        private string text = string.Empty;

        /// <summary>
        /// Gets the current bounding box of the tab.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tab is enabled.
        /// </summary>
        public bool Enabled {
            get => enabled && Parent?.Enabled == true;
            set {
                if (enabled != value) {
                    enabled = value;
                    Parent?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets the preferred size of the tab.
        /// </summary>
        public Size GetPreferredSize (Size proposedSize)
        {
            var padding = Parent?.LogicalToDeviceUnits (Padding.Horizontal) ?? Padding.Horizontal;
            var font_size = Parent?.LogicalToDeviceUnits (Theme.FontSize) ?? Theme.FontSize;
            var text_size = (int)Math.Round (TextMeasurer.MeasureText (Text, Theme.UIFont, font_size).Width);

            return new Size (text_size + padding, Bounds.Height);
        }

        /// <summary>
        /// Gets a value indicating if the tab currently has the mouse hovered over it.
        /// </summary>
        public bool Hovered => Parent?.Tabs.HoveredIndex == Index;

        // Gets the current index in the parent TabStrip, if parented to a TabStrip.
        private int Index => Parent?.Tabs.IndexOf (this) ?? -1;

        /// <summary>
        /// Gets or sets the amount of space to leave between this tab and other elements.
        /// </summary>
        public Padding Margin { get; set; } = Padding.Empty;

        /// <summary>
        /// Gets or sets the amount of space to leave between the text and the border of the tab.
        /// </summary>
        public Padding Padding { get; set; } = new Padding (14, 0, 14, 0);

        /// <summary>
        /// Gets the TabStrip this tab is currently a part of.
        /// </summary>
        public TabStrip? Parent { get; internal set; }

        /// <summary>
        /// Gets a value indicating if the tab is currently the selected tab.
        /// </summary>
        public bool Selected => Parent?.SelectedTab == this;

        /// <summary>
        /// Sets the bounding box of the tab. This is internal API and should not be called.
        /// </summary>
        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            Bounds = new Rectangle (x, y, width, height);
        }

        /// <summary>
        /// Gets or sets an object with additional user data about this tab.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the text displayed on the tab.
        /// </summary>
        public string Text {
            get => text;
            set {
                if (text != value) {
                    text = value;
                    Parent?.Invalidate ();
                }
            }
        }
    }
}
