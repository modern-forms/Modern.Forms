using System;
using System.Drawing;
using System.Linq;
using Designer;
using Modern.Forms;
using Modern.Forms.Design;
using SkiaSharp;

namespace Modern.Forms.Design
{
    public class DesignerFrame : Control
    {
        private readonly FormDocumentDesigner designer;
        private const int FORM_MARGIN = 15;

        private GrabHandleGlyph? active_dragging_glyph = null;

        public DesignerFrame (FormDocumentDesigner designer)
        {
            this.designer = designer;
            Style.BackgroundColor = new SKColor (249, 249, 249);

            designer.Location = new Point (FORM_MARGIN, FORM_MARGIN);

            Controls.Add (designer);
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            // Form adorners
            e.Canvas.Save ();
            e.Canvas.Translate (FORM_MARGIN, FORM_MARGIN);

            base.OnPaint (e);

            foreach (var glyph in designer.GetGlyphs ())
                glyph.Paint (e);

            e.Canvas.Restore ();
        }

        // Changing this to OnPreviewMouseDown prevents the Form from being
        // resized larger than the DesignerSurface
        protected override void OnMouseDown (MouseEventArgs e)
        {
            var point = new Point (e.X - FORM_MARGIN, e.Y - FORM_MARGIN);

            foreach (var glyph in designer.GetGlyphs ().OfType<GrabHandleGlyph> ())
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
                designer.Size = active_dragging_glyph.GetNewSize (designer.Bounds, designer.Form.MinimumFormSize, point);
                designer.Invalidate ();
                return true;
            }

            foreach (var glyph in designer.GetGlyphs ())
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
