using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class LabelRenderer : Renderer<Label>
    {
        public override void Render (Label control, PaintEventArgs e)
        {
            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, control.TextAlign, maxLines: control.Multiline ? (int?)null : 1, ellipsis: control.AutoEllipsis);
        }
    }
}
