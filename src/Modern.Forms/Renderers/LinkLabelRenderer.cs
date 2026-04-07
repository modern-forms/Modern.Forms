using System.Drawing;
using Modern.Forms.Layout;
using SkiaSharp;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Renders a <see cref="LinkLabel"/> control.
    /// </summary>
    /// <remarks>
    /// This renderer is responsible for drawing:
    /// <list type="bullet">
    /// <item><description>The optional image inherited from <see cref="Label"/>.</description></item>
    /// <item><description>Normal text segments and linked text segments separately.</description></item>
    /// <item><description>Hover, active, visited, and disabled link colors.</description></item>
    /// <item><description>Optional underlines.</description></item>
    /// <item><description>Focus cues for the focused link.</description></item>
    /// </list>
    /// </remarks>
    public class LinkLabelRenderer : Renderer<LinkLabel>
    {
        /// <inheritdoc/>
        protected override void Render (LinkLabel control, PaintEventArgs e)
        {
            var layout = LayoutTextAndImage (control);

            DrawImage (control, e, layout);

            if (string.IsNullOrEmpty (control.Text))
                return;

            DrawTextRuns (control, e, layout);

            if (control.Selected && control.ShowFocusCues && control.FocusLink is not null) {
                foreach (var bounds in control.FocusLink.VisualBounds)
                    e.Canvas.DrawFocusRectangle (bounds);
            }
        }

        /// <summary>
        /// Performs hit testing and returns the link located at the given client point.
        /// </summary>
        /// <param name="control">The associated control.</param>
        /// <param name="location">The client location to test.</param>
        /// <returns>The matching link, or <see langword="null"/> if no link was found.</returns>
        public LinkLabel.Link? HitTest (LinkLabel control, Point location)
        {
            EnsureLayoutCache (control);
            return control.Links.FirstOrDefault (link => link.Contains (location));
        }

        private static LinkLabelTextLayout LayoutTextAndImage (LinkLabel control)
        {
            var layout = TextImageLayoutEngine.Layout (control);
            return new LinkLabelTextLayout (layout.TextBounds, layout.ImageBounds);
        }

        private static void DrawImage (LinkLabel control, PaintEventArgs e, LinkLabelTextLayout layout)
        {
            if ((control as IHaveTextAndImageAlign).GetImage () is SKBitmap image)
                e.Canvas.DrawBitmap (image, layout.ImageBounds, !control.Enabled);
        }

        private void DrawTextRuns (LinkLabel control, PaintEventArgs e, LinkLabelTextLayout layout)
        {
            EnsureLayoutCache (control);

            using var paint = CreatePaint (control);
            var metrics = paint.FontMetrics;
            var line_height = (float)Math.Ceiling (metrics.Descent - metrics.Ascent + metrics.Leading);
            var baseline_offset = -metrics.Ascent;

            var lines = BuildLines (control.Text ?? string.Empty);
            var line_rectangles = MeasureLines (paint, lines, layout.TextBounds, line_height, control.TextAlign);

            var current_index = 0;

            for (var line_index = 0; line_index < lines.Count; line_index++) {
                var line = lines[line_index];
                var line_rect = line_rectangles[line_index];

                var x = line_rect.Left;
                var baseline = line_rect.Top + baseline_offset;

                for (var i = 0; i < line.Length; i++) {
                    var character = line[i].ToString ();
                    var width = Math.Max (1f, paint.MeasureText (character));

                    var text_index = current_index + i;
                    var link = GetLinkAtIndex (control, text_index);
                    var bounds = Rectangle.Round (new RectangleF (x, line_rect.Top, width, line_height));

                    if (link is not null) {
                        var color = control.ResolveLinkColor (link);
                        paint.Color = color;

                        e.Canvas.DrawText (character, x, baseline, paint);

                        if (control.ShouldUnderline (link)) {
                            var underline_offset = Math.Max (1, control.LogicalToDeviceUnits (1));
                            var underline_y = bounds.Bottom - underline_offset;
                            e.Canvas.DrawLine ((int)x, underline_y, (int)(x + width), underline_y, paint.Color);
                        }
                    } else {
                        paint.Color = control.Enabled
                            ? (control.Style.ForegroundColor ?? Theme.ForegroundColor)
                            : Theme.ForegroundDisabledColor;

                        e.Canvas.DrawText (character, x, baseline, paint);
                    }

                    x += width;
                }

                current_index += line.Length;

                if (line_index < lines.Count - 1)
                    current_index += 1;
            }
        }

        private void EnsureLayoutCache (LinkLabel control)
        {
            if (!control.IsLayoutInvalidated)
                return;

            control.Links.ClearVisualBounds ();

            using var paint = CreatePaint (control);
            var metrics = paint.FontMetrics;
            var line_height = (float)Math.Ceiling (metrics.Descent - metrics.Ascent + metrics.Leading);

            var layout = LayoutTextAndImage (control);
            var lines = BuildLines (control.Text ?? string.Empty);
            var line_rectangles = MeasureLines (paint, lines, layout.TextBounds, line_height, control.TextAlign);

            var current_index = 0;

            for (var line_index = 0; line_index < lines.Count; line_index++) {
                var line = lines[line_index];
                var line_rect = line_rectangles[line_index];

                var x = line_rect.Left;

                for (var i = 0; i < line.Length; i++) {
                    var character = line[i].ToString ();
                    var width = Math.Max (1f, paint.MeasureText (character));
                    var text_index = current_index + i;

                    var link = GetLinkAtIndex (control, text_index);
                    if (link is not null) {
                        var bounds = Rectangle.Round (new RectangleF (x, line_rect.Top, width, line_height));
                        link.AddVisualBounds (bounds);
                    }

                    x += width;
                }

                current_index += line.Length;

                if (line_index < lines.Count - 1)
                    current_index += 1;
            }

            control.ValidateLayout ();
        }

        private static SKPaint CreatePaint (LinkLabel control)
        {
            return new SKPaint {
                IsAntialias = true,
                Typeface = control.CurrentStyle.GetFont (),
                TextSize = control.LogicalToDeviceUnits (control.CurrentStyle.GetFontSize ())
            };
        }

        private static List<string> BuildLines (string text)
        {
            if (string.IsNullOrEmpty (text))
                return [string.Empty];

            return text.Replace ("\r\n", "\n").Split ('\n').ToList ();
        }

        private static LinkLabel.Link? GetLinkAtIndex (LinkLabel control, int index)
        {
            foreach (var link in control.Links) {
                var end = link.Start + link.Length;
                if (index >= link.Start && index < end)
                    return link;
            }

            return null;
        }

        private static List<RectangleF> MeasureLines (
            SKPaint paint,
            List<string> lines,
            Rectangle available_bounds,
            float line_height,
            ContentAlignment alignment)
        {
            var result = new List<RectangleF> (lines.Count);

            var total_height = line_height * lines.Count;
            var start_y = alignment switch {
                ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => available_bounds.Top,
                ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => available_bounds.Top + ((available_bounds.Height - total_height) / 2f),
                _ => available_bounds.Bottom - total_height
            };

            for (var i = 0; i < lines.Count; i++) {
                var width = paint.MeasureText (lines[i]);
                var x = alignment switch {
                    ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => available_bounds.Left + ((available_bounds.Width - width) / 2f),
                    ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight => available_bounds.Right - width,
                    _ => available_bounds.Left
                };

                result.Add (new RectangleF (x, start_y + (i * line_height), width, line_height));
            }

            return result;
        }

        private readonly record struct LinkLabelTextLayout (Rectangle TextBounds, Rectangle ImageBounds);
    }
}
