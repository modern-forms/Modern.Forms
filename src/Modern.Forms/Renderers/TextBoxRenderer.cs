using System;
using System.Drawing;
using Topten.RichTextKit;

namespace Modern.Forms.Renderers
{
    public class TextBoxRenderer : Renderer<TextBox>
    {
        protected override void Render (TextBox control, PaintEventArgs e)
        {
            var text = control.Text.Length > 0 ? control.Text : control.Placeholder;

            // Bail early if we don't need to draw anything
            if (text.Length == 0 && !control.Selected)
                return;

            var block = GetTextBlock (control);

            UpdateScrollBars (control, block);

            e.Canvas.Save ();
            e.Canvas.Clip (control.PaddedClientRectangle);

            if (text.Length > 0)
                e.Canvas.DrawTextBlock (block, GetTextOrigin (control), GetTextSelection (control));

            if (control.Selected) {
                var caret = TextMeasurer.GetCursorLocation (block, GetTextOrigin (control), GetCursorIndex (control), GetCurrentFontSize (control));
                e.Canvas.DrawRectangle (caret, Theme.DarkTextColor);
            }

            e.Canvas.Restore ();
        }

        protected int GetCurrentFontSize (TextBox control) => control.CurrentFontSize;

        protected int GetCursorIndex (TextBox control) => control.document.CursorIndex;

        protected TextBlock GetTextBlock (TextBox control) => control.document.GetTextBlock ();

        protected Point GetTextOrigin (TextBox control) => control.TextOrigin;

        protected TextSelection GetTextSelection (TextBox control) => control.document.GetTextSelection ();

        protected void UpdateScrollBars (TextBox control, TextBlock block) => control.UpdateScrollBars (block);
    }
}
