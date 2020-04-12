using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class ButtonRenderer : Renderer<Button>
    {
        protected override void Render (Button control, PaintEventArgs e)
        {
            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, control.TextAlign);
        }
    }
}
