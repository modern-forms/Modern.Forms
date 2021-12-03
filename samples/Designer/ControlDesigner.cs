using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Designer
{
    public class ControlDesigner : Control
    {
        private readonly Control control;

        private bool is_selected;

        public ControlDesigner (Control control)
        {
            //control.Dock = DockStyle.Fill;
            this.control = control;

            //control.SetControlBehavior (ControlBehaviors.Selectable, false);
            control.Enabled = false;

            SetBounds (control.Left, control.Top, control.Width, control.Height);
            //Width = control.Width;
            //Height = control.Height;
            //Left = control.Left

            control.Dock = DockStyle.Fill;
            Controls.Add (control);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            //e.Canvas.Save ();
            //e.Canvas.Translate (FORM_MARGIN, FORM_MARGIN);

            var control_bounds = new Rectangle (0, 0, control.Size.Width + 1, control.Size.Height + 1);

            //e.Canvas.FillRectangle (form_bounds, form.CurrentStyle.GetBackgroundColor ());
            //e.Canvas.DrawBorder (form_bounds, form.CurrentStyle);

            //e.Canvas.Save ();
            //e.Canvas.ClipRect (form_bounds.ToSKRect ());
            //control.DoPaint (e);
            //e.Canvas.Restore ();
            //TypeDescriptor.CreateDesigner
            if (is_selected)
                e.Canvas.DrawResizeAdornment (control_bounds);

            //e.Canvas.Restore ();
        }

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            is_selected = true;
            Invalidate ();
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);
            is_selected = true;
            Invalidate ();
        }

        public IEnumerable<Glyph> GetGlyphs ()
        {
            var bounds = Bounds;

            yield return new SelectionRectangleGlyph (bounds);

            foreach (var glyph in Modern.Forms.Design.DrawingExtensions.GetGrabHandleGlyphs (bounds, false))
                yield return glyph;
        }

        public override DockStyle Dock { get => control.Dock; set => control.Dock = value; }
        //public override Size Size { get => control.Size; set => control.Size = value; }
        //public override AnchorStyles Anchor { get => control.Anchor; set => control.Anchor = value; }
        //public override Point Location { get => control.Location; set => control.Location = value; }
        //public override Rectangle Bounds { get => control.Bounds; set => control.Bounds = value; }
        //public override bool Visible { get => control.Visible; set => control.Visible = value; }
        //public override int Height { get => control.Height; set => control.Height = value; }
        //public override int Width { get => control.Width; set => control.Width = value; }
        //public override int Left { get => control.Left; set => control.Left = value; }
        //public override int Top { get => control.Top; set => control.Top = value; }
        //public override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        //{
        //    control.SetBoundsCore (x, y, width, height, specified);
        //}
    }
}
