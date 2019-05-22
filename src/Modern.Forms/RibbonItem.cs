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
        private string text;

        public SKBitmap Image { get; set; }
        public bool Selected { get; set; }
        public bool Highlighted { get; set; }
        public RibbonItemGroup Owner { get; set; }

        public Rectangle Bounds { get; private set; }

        public Padding Margin { get; set; } = Padding.Empty;
        public Padding Padding { get; set; } = new Padding (3);

        public RibbonItem ()
        {
        }

        public RibbonItem (string text, SKBitmap image = null, EventHandler onClick = null)
        {
            Text = text;
            Image = image;

            Click += onClick;
        }

        public event EventHandler Click;
        public event EventHandler EnabledChanged;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseEnter;
        public event EventHandler MouseLeave;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler TextChanged;

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

        public Ribbon FindRibbon () => Owner?.Owner?.Owner;

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
            OnClick (new MouseEventArgs (MouseButtons.Left, 1, 0, 0, 0));
        }

        public Size GetPreferredSize (Size proposedSize)
        {
            var text_width = TextMeasurer.MeasureText (Text ?? string.Empty, Theme.UIFont, 12, new SKSize (40, 30)).Width;
            var measured_width = (int)Math.Ceiling (text_width) + Padding.Horizontal;

            return new Size (Math.Max (measured_width, 40), 0);
        }

        protected virtual void OnBoundsChanged ()
        {
            OnLayout (new LayoutEventArgs ((Control)null, string.Empty));
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
            Highlighted = true;
            Invalidate ();

            MouseEnter?.Invoke (this, e);
        }

        protected virtual void OnMouseLeave (EventArgs e)
        {
            Highlighted = false;
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

        protected virtual void OnPaint (SKPaintEventArgs e)
        {
            var canvas = e.Canvas;
            var background_color = Selected ? Theme.RibbonItemSelectedColor : Highlighted ? Theme.RibbonItemHighlightColor : Theme.NeutralGray;

            canvas.FillRectangle (Bounds, background_color);

            if (Image != null) {
                if (Enabled)
                    canvas.DrawBitmap (Image, Bounds.Left + ((Bounds.Width - Image.Width) / 2), Bounds.Top + 3);
                else
                    canvas.DrawDisabledBitmap (Image, Bounds.Left + ((Bounds.Width - Image.Width) / 2), Bounds.Top + 3);
            }

            canvas.Save ();
            canvas.ClipRect (new SKRect (Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom));

            var lines = Text.Split (' ');

            canvas.DrawCenteredText (lines[0].Trim (), Theme.UIFont, 12, Bounds.Left + Bounds.Width / 2, Bounds.Top + 50, Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor);

            if (lines.Length > 1)
                canvas.DrawCenteredText (lines[1].Trim (), Theme.UIFont, 12, Bounds.Left + Bounds.Width / 2, Bounds.Top + 66, Enabled ? Theme.DarkTextColor : Theme.DisabledTextColor);

            canvas.Restore ();
        }

        protected virtual void OnTextChanged (EventArgs e) => TextChanged?.Invoke (this, e);

        internal void FireEvent (EventArgs e, ToolStripItemEventType type)
        {
            if (!Enabled && type != ToolStripItemEventType.Paint)
                return;

            switch (type) {
                case ToolStripItemEventType.MouseDown:
                    OnMouseDown ((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.MouseEnter:
                    OnMouseEnter ((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.MouseLeave:
                    OnMouseLeave ((EventArgs)e);
                    break;
                case ToolStripItemEventType.MouseMove:
                    OnMouseMove ((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.MouseUp:
                    OnMouseUp ((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.Paint:
                    OnPaint ((SKPaintEventArgs)e);
                    break;
                case ToolStripItemEventType.Click:
                    OnClick ((MouseEventArgs)e);
                    break;
            }
        }
    }
}
