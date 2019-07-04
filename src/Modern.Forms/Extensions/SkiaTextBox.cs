// From https://github.com/mono/SkiaSharp/issues/692

using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

/// <summary>
/// A text box that handles multi-line text wrapping and layout.
/// </summary>
public static class SkiaTextBox
{
    /// <summary>
    /// Draw the specified text in the region defined by x, y, width, height wrapping and breaking lines
    /// to fit in that region
    /// </summary>
    /// <returns>The draw.</returns>
    /// <param name="text">Text.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="canvas">Canvas.</param>
    /// <param name="paint">Paint.</param>
    public static void Draw (string text, double x, double y, double width, double height, SKCanvas canvas, SKPaint paint, bool ellipsize)
    {
        if (text == null) {
            return;
        }

        double textY = 0, textX = 0;

        switch (paint.TextAlign) {
            case SKTextAlign.Center:
                textX = x + width / 2;
                break;
            case SKTextAlign.Left:
                textX = x;
                break;
            case SKTextAlign.Right:
                textX = x + width;
                break;
        }

        var lines = BreakLines (text, paint, width);

        var metrics = paint.FontMetrics;
        var lineHeight = metrics.Bottom - metrics.Top;

        float textHeight = lines.Count * lineHeight - metrics.Leading;

        //if (textHeight > height) {
            textY = y - metrics.Top;
        //} else {
        //    textY = y - metrics.Top + (height - textHeight) / 2;
        //}
        if (ellipsize && lines.Count > height / lineHeight) {
            var ellipsizedLine = $"{lines.FirstOrDefault ()}...";
            canvas.DrawText (ellipsizedLine, (float)textX, (float)textY, paint);
        } else {
            for (int i = 0; i < lines.Count; i++) {
                canvas.DrawText (lines[i], (float)textX, (float)textY, paint);
                textY += lineHeight;
                if (textY + metrics.Descent > y + height) {
                    break;
                }
            }
        }
    }

    static List<string> BreakLines (string text, SKPaint paint, double width)
    {
        List<string> lines = new List<string> ();

        string remainingText = text.Trim ();

        do {
            int idx = LineBreak (remainingText, paint, width);
            if (idx == 0) {
                break;
            }
            var lastLine = remainingText.Substring (0, idx).Trim ();
            lines.Add (lastLine);
            remainingText = remainingText.Substring (idx).Trim ();
        } while (!string.IsNullOrEmpty (remainingText));
        return lines;
    }

    static int LineBreak (string text, SKPaint paint, double width)
    {
        int idx = 0, last = 0;
        int lengthBreak = (int)paint.BreakText (text, (float)width);

        while (idx < text.Length) {
            int next = text.IndexOfAny (new char[] { ' ', '\n' }, idx);
            if (next == -1) {
                if (idx == 0) {
                    // Word is too long, we will have to break it
                    return lengthBreak;
                } else {
                    // Ellipsize if it's the last line
                    if (lengthBreak == text.Length
                    // || text.IndexOfAny (new char [] { ' ', '\n' }, lengthBreak + 1) == -1
                    ) {
                        return lengthBreak;
                    }
                    // Split at the last word;
                    return last;
                }
            }
            if (text[idx] == '\n') {
                return idx;
            }
            if (next > lengthBreak) {
                return idx;
            }
            last = next;
            idx = next + 1;
        }
        return last;
    }
}