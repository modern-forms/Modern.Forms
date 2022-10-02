using System;
using System.Drawing;
using Modern.Forms.Layout;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a Label.
    /// </summary>
    public class LabelRenderer : Renderer<Label>
    {
        /// <inheritdoc/>
        protected override void Render (Label control, PaintEventArgs e)
        {
            var face = LayoutUtils.DeflateRect (control.ClientRectangle, control.Padding);
            var i = control.Image;

            var layout = new TextImageRelationLayoutUtils {
                Bounds = face,
                Font = control.CurrentStyle.GetFont (),
                FontSize = control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ()),
                ImageAlign = control.ImageAlign,
                ImageSize = control.Image?.GetSize () ?? Size.Empty,
                Text = control.Text,
                TextAlign = control.TextAlign,
                TextImageRelation = control.TextImageRelation,
            };

            (var image_bounds, var text_bounds) = layout.Layout ();

            if (i is not null)
                e.Canvas.DrawBitmap (i, image_bounds, !control.Enabled);

            e.Canvas.DrawText (control.Text, text_bounds, control, control.TextAlign, maxLines: control.Multiline ? (int?)null : 1, ellipsis: control.AutoEllipsis);
        }
    }
}
