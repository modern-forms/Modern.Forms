using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using Designer;
using Modern.Forms;
using SkiaSharp;

namespace Modern.Forms.Design
{
    public class FormDocumentDesigner : Control, IRootDesigner
    {
        private Form form;
        private ControlDesigner? selected_control;
        bool is_moving_control = false;
        Point? mousedown_anchor = null;
        //private ResizeAdornment? dragging_adornment = null;

        private GrabHandleGlyph? active_dragging_glyph = null;

        public FormDocumentDesigner ()
        {
            Dock = DockStyle.Fill;
        }

        public FormDocumentDesigner (Form form)
        {
            this.form = form;

            foreach (var control in form.Controls.ToArray ()) {
                if (control is not ControlDesigner) {
                    form.Controls.Remove (control);
                    var new_control = new ControlDesigner (control);
                    //new_control.Top -= form.TitleBar.Height;
                    Controls.Add (new_control);
                } else {
                    form.Controls.Remove (control);
                    Controls.Add (control);
                }
            }
        }

        public Rectangle GetFormBounds () => new Rectangle (0, 0, form.Size.Width + 1, form.Size.Height + 1);
        public Form Form => form;

        protected override void OnPaint (PaintEventArgs e)
        {
            // The Form isn't in our Controls collection, so we need to manually paint it
            var form_bounds = new Rectangle (0, 0, form.Size.Width + 1, form.Size.Height + 1);

            e.Canvas.FillRectangle (form_bounds, form.CurrentStyle.GetBackgroundColor ());

            // Now we can draw the ControlDesigners the normal way
            // (They are actually parented to the Designer, not the Form.)
            base.OnPaint (e);

            // Border and implicit controls like TitleBar
            e.Canvas.DrawBorder (form_bounds, form.CurrentStyle);
            e.Canvas.Save ();
            e.Canvas.ClipRect (form_bounds.ToSKRect ());
            form.adapter.DoPaint (e);
            e.Canvas.Restore ();

            // Draw any adorners needed for selected ControlDesigner on top
            if (selected_control is not null) {
                foreach (var glyph in selected_control.GetGlyphs ())
                    glyph.Paint (e);

                // Selected control resize handles
                //var adornment_bounds = new Rectangle (selected_control.Left, selected_control.Top, selected_control.Width, selected_control.Height);
                //e.Canvas.DrawResizeAdornment (adornment_bounds);
            }
        }

        protected override bool OnPreviewMouseDown (MouseEventArgs e)
        {
            //base.OnMouseDown (e);

            // Begin control resizing
            if (selected_control is not null)
                foreach (var glyph in selected_control.GetGlyphs ().OfType<GrabHandleGlyph> ())
                    if (glyph.GetHitTest (e.Location) is not null) {
                        active_dragging_glyph = glyph;
                        return true;
                    }

            // Begin control moving
            if (GetControlAtLocation (e.Location) is ControlDesigner c) {
                selected_control = c;

                if (c.Dock == DockStyle.None) {
                    is_moving_control = true;
                    mousedown_anchor = e.Location;
                }

                Invalidate ();
                return true;
            }


            //if (selected_control is not null && DrawingExtensions.GetResizeAdornmentAtPoint (selected_control.Bounds, e.Location) is ResizeAdornment adornment) {
            //    dragging_adornment = adornment;
            //    mousedown_anchor = e.Location;
            //    return true;
            //}

            if (selected_control is not null) {
                selected_control = null;
                Invalidate ();
            }

            return false;
        }

        protected override bool OnPreviewMouseMove (MouseEventArgs e)
        {
            //base.OnMouseMove (e);

            // Handle control resizing

            if (selected_control is not null && active_dragging_glyph is not null) {
                selected_control.Bounds = active_dragging_glyph.GetNewBounds (selected_control.Bounds, new Size (25, 25), e.Location);
                // TODO: Changing a control's bounds should trigger a parent layout
                form.PerformLayout ();
                selected_control.Invalidate ();
                return true;
            }

            //if (selected_control is not null && dragging_adornment is not null && mousedown_anchor.HasValue) {
            //    selected_control.Bounds = dragging_adornment.GetNewBounds (selected_control.Bounds, mousedown_anchor.Value, e.Location);
            //    mousedown_anchor = e.Location;
            //    Invalidate ();
            //    selected_control.Invalidate ();
            //    return true;
            //}

            // Handle control moving
            if (is_moving_control && selected_control is not null && mousedown_anchor.HasValue) {
                var delta = new Point (e.Location.X - mousedown_anchor.Value.X, e.Location.Y - mousedown_anchor.Value.Y);
                selected_control.Location = ClampToFormClientArea (new Point (selected_control.Left + delta.X, selected_control.Top + delta.Y));
                mousedown_anchor = e.Location;
                Invalidate ();
                return true;
            }

            return false;
        }

        protected override bool OnPreviewMouseUp (MouseEventArgs e)
        {
            //base.OnMouseUp (e);

            // End control moving
            is_moving_control = false;

            // End control resizing
            //dragging_adornment = null;
            active_dragging_glyph = null;

            return false;
        }

        private Control? GetControlAtLocation (Point location)
        {
            //var point = new Point (location.X - FORM_MARGIN, location.Y - FORM_MARGIN);
            //location.Offset (-FORM_MARGIN, -FORM_MARGIN);

            foreach (var c in Controls)
                if (c.Bounds.Contains (location))
                    return c;

            return null;
        }

        public override int Height { get => form.Size.Height; set => throw new Exception (); }
        public override int Width { get => form.Size.Width; set => throw new Exception (); }
        //public void SetFormSize (Size size) => form.Size = size;
        public override Size Size { get => form.Size; set { form.Size = value; form.adapter.PerformLayout (); } }
        public override Rectangle Bounds { get => GetFormBounds (); set => base.Bounds = value; }

        ViewTechnology[] IRootDesigner.SupportedTechnologies => new[] { ViewTechnology.Default };

        IComponent IDesigner.Component => form;

        DesignerVerbCollection? IDesigner.Verbs => null;

        private Rectangle GetFormClientArea () => new Rectangle (1, form.TitleBar.Bottom + 1, form.Size.Width - 2, form.Size.Height - form.TitleBar.Height - 2);
        private Point ClampToFormClientArea (Point rectangle)
        {
            var form = GetFormClientArea ();

            //if (rectangle.X < form.X)
            //    rectangle.X = form.X;
            //if (rectangle.Y < form.Y)
            //    rectangle.Y = form.Y;

            return rectangle;
        }

        public IEnumerable<Glyph> GetGlyphs ()
        {
            var bounds = GetFormBounds ();

            yield return new SelectionRectangleGlyph (bounds);

            foreach (var glyph in DrawingExtensions.GetGrabHandleGlyphs (bounds, true))
                yield return glyph;
        }

        object IRootDesigner.GetView (ViewTechnology technology)
        {
            return surf;
        }

        void IDesigner.DoDefaultAction ()
        {
            throw new NotImplementedException ();
        }

        private DesignerFrame? surf;

        void IDesigner.Initialize (IComponent component)
        {
            this.form = (Modern.Forms.Form)component;
            surf = new DesignerFrame (this);

            foreach (var control in form.Controls.ToArray ()) {
                if (control is not ControlDesigner) {
                    form.Controls.Remove (control);
                    var new_control = new ControlDesigner (control);
                    //new_control.Top -= form.TitleBar.Height;
                    Controls.Add (new_control);
                } else {
                    form.Controls.Remove (control);
                    Controls.Add (control);
                }
            }
        }
    }
}
