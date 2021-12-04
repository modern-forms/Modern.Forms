using System;
using System.Drawing;
using System.Linq;
using Designer;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Modern.Forms.Design
{
    public class DesignerFrame : Control//, IOverlayService
    {
        private readonly DocumentDesigner designer;
        internal readonly FormHostAdapter design_adapter;
        private const int FORM_MARGIN = 15;

        private GrabHandleGlyph? active_dragging_glyph = null;

        public DesignerFrame (DocumentDesigner designer)
        {
            this.designer = designer;

            Dock = DockStyle.Fill;
            Style.BackgroundColor = new SKColor (249, 249, 249);

            design_adapter = new FormHostAdapter (designer);
            design_adapter.Location = new Point (FORM_MARGIN, FORM_MARGIN);

            Controls.Add (design_adapter);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            // Form adorners
            e.Canvas.Save ();
            e.Canvas.Translate (FORM_MARGIN, FORM_MARGIN);

            base.OnPaint (e);

            foreach (var glyph in design_adapter.GetGlyphs ())
                glyph.Paint (e);

            e.Canvas.Restore ();
        }

        internal void DoMouseDown (MouseEventArgs e) => OnMouseDown (e);

        // Changing this to OnPreviewMouseDown prevents the Form from being
        // resized larger than the DesignerSurface
        protected override void OnMouseDown (MouseEventArgs e)
        {
            var point = new Point (e.X - FORM_MARGIN, e.Y - FORM_MARGIN);

            foreach (var glyph in design_adapter.GetGlyphs ().OfType<GrabHandleGlyph> ())
                if (glyph.GetHitTest (point) is not null) {
                    active_dragging_glyph = glyph;
                    return;
                }
        }

        protected override bool OnPreviewMouseMove (MouseEventArgs e)
        {
            // Account for the offset of the designed Form's origin
            var point = new Point (e.Location.X - FORM_MARGIN, e.Location.Y - FORM_MARGIN);

            if (active_dragging_glyph is not null) {
                design_adapter.Size = active_dragging_glyph.GetNewSize (design_adapter.Bounds, designer.Form.MinimumFormSize, point);
                design_adapter.Invalidate ();
                return true;
            }

            foreach (var glyph in design_adapter.GetGlyphs ())
                if (glyph.GetHitTest (point) is Cursor cursor) {
                    Cursor = cursor;
                    return true;
                }

            Cursor = Cursor.Default;

            return false;
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            active_dragging_glyph = null;
        }
    }
}
