using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    public class StatusBarRenderer : Renderer<StatusBar>
    {
        protected override void Render (StatusBar control, PaintEventArgs e)
        {
            e.Canvas.DrawText (control.Text, control.PaddedClientRectangle, control, ContentAlignment.MiddleLeft, maxLines: 1);
        }
    }
}
