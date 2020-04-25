using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonItem : ILayoutable
    {
        private bool enabled = true;
        private string text = string.Empty;

        private const int IMAGE_SIZE = 32;
        private const int MINIMUM_ITEM_SIZE = 40;

        public SKBitmap? Image { get; set; }
        public bool Selected { get; set; }
        public bool Hovered { get; private set; }
        public RibbonItemGroup? Owner { get; set; }

        public Padding Margin { get; set; } = Padding.Empty;
        public Padding Padding { get; set; } = new Padding (3);

        public RibbonItem ()
        {
        }

        public RibbonItem (string text, SKBitmap? image = null, EventHandler<MouseEventArgs>? onClick = null)
        {
            Text = text;
            Image = image;

            Click += onClick;
        }

        public Rectangle Bounds { get; private set; }

        public event EventHandler<MouseEventArgs>? Click;
        public event EventHandler? EnabledChanged;
        public event EventHandler<MouseEventArgs>? MouseDown;
        public event EventHandler<MouseEventArgs>? MouseEnter;
        public event EventHandler? MouseLeave;
        public event EventHandler<MouseEventArgs>? MouseMove;
        public event EventHandler<MouseEventArgs>? MouseUp;
        public event EventHandler? TextChanged;

        public bool Enabled {
            get => enabled;
            set {
                if (enabled == value)
                    return;

                enabled = value;
                OnEnabledChanged (EventArgs.Empty);
                Invalidate ();
            }
        }

        public Ribbon? FindRibbon () => Owner?.Owner?.Owner;

        public void Invalidate ()
        {
            FindRibbon ()?.Invalidate (Bounds);
        }

        public void SetBounds (int x, int y, int width, int height, BoundsSpecified specified = BoundsSpecified.All)
        {
            var new_bounds = new Rectangle (x, y, width, height);

            if (Bounds != new_bounds) {
                Bounds = new_bounds;
                OnBoundsChanged ();
            }
        }

        public string Text {
            get => text;
            set {
                if (text == value)
                    return;

                text = value;
                OnTextChanged (EventArgs.Empty);
                Invalidate ();
            }
        }

        public void PerformClick ()
        {
            OnClick (new MouseEventArgs(MouseButtons.Left, 1, 0, 0, Point.Empty));
        }

        public Size GetPreferredSize (Size proposedSize)
        {
            var padding = LogicalToDeviceUnits (Padding.Horizontal);
            var font_size = LogicalToDeviceUnits (Theme.RibbonItemFontSize);
            var proposed_size = LogicalToDeviceUnits (new Size (40, 30));
            var text_size = (int)Math.Round (TextMeasurer.MeasureText (Text ?? string.Empty, Theme.UIFont, font_size, proposed_size).Width);

            return new Size (Math.Max (text_size + padding, LogicalToDeviceUnits (MINIMUM_ITEM_SIZE)), 0);
        }

        protected virtual void OnBoundsChanged ()
        {
            OnLayout (new LayoutEventArgs ((Control?)null, string.Empty));
        }

        protected virtual void OnClick (MouseEventArgs e) => Click?.Invoke (this, e);
        protected virtual void OnEnabledChanged (EventArgs e) => EnabledChanged?.Invoke (this, e);

        protected virtual void OnLayout (LayoutEventArgs e)
        {
        }

        protected virtual void OnMouseDown (MouseEventArgs e)
        {
            MouseDown?.Invoke (this, e);
        }

        protected virtual void OnMouseEnter (MouseEventArgs e)
        {
            Hovered = true;
            Invalidate ();

            MouseEnter?.Invoke (this, e);
        }

        protected virtual void OnMouseLeave (EventArgs e)
        {
            Hovered = false;
            Invalidate ();

            MouseLeave?.Invoke (this, e);
        }

        protected virtual void OnMouseMove (MouseEventArgs e)
        {
            MouseMove?.Invoke (this, e);
        }

        protected virtual void OnMouseUp (MouseEventArgs e)
        {
            MouseUp?.Invoke (this, e);
        }

        protected virtual void OnTextChanged (EventArgs e) => TextChanged?.Invoke (this, e);

        internal void FireEvent (EventArgs e, ItemEventType type)
        {
            switch (type) {
                case ItemEventType.MouseDown:
                    OnMouseDown ((MouseEventArgs)e);
                    break;
                case ItemEventType.MouseEnter:
                    OnMouseEnter ((MouseEventArgs)e);
                    break;
                case ItemEventType.MouseLeave:
                    OnMouseLeave ((EventArgs)e);
                    break;
                case ItemEventType.MouseMove:
                    OnMouseMove ((MouseEventArgs)e);
                    break;
                case ItemEventType.MouseUp:
                    OnMouseUp ((MouseEventArgs)e);
                    break;
                case ItemEventType.Click:
                    OnClick ((MouseEventArgs)e);
                    break;
            }
        }

        private int LogicalToDeviceUnits (int value) => FindRibbon ()?.LogicalToDeviceUnits (value) ?? value;
        private Size LogicalToDeviceUnits (Size value) => FindRibbon ()?.LogicalToDeviceUnits (value) ?? value;
        private Padding LogicalToDeviceUnits (Padding value) => FindRibbon ()?.LogicalToDeviceUnits (value) ?? value;
    }
}
