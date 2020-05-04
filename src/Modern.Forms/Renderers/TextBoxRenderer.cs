using System;
using System.Drawing;
using Topten.RichTextKit;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a TextBox.
    /// </summary>
    public class TextBoxRenderer : Renderer<TextBox>
    {
        /// <inheritdoc/>
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
                e.Canvas.DrawRectangle (caret, Theme.PrimaryTextColor);
            }

            e.Canvas.Restore ();
        }

        /// <summary>
        /// Gets the TextBox's font size.
        /// </summary>
        protected int GetCurrentFontSize (TextBox control) => control.CurrentFontSize;

        /// <summary>
        /// Gets the current index of the TextBox cursor.
        /// </summary>
        protected int GetCursorIndex (TextBox control) => control.document.CursorIndex;

        /// <summary>
        /// Gets the TextBox's text block.
        /// </summary>
        protected TextBlock GetTextBlock (TextBox control) => control.document.GetTextBlock ();

        /// <summary>
        /// Gets the TextBox's text origin.
        /// </summary>
        protected Point GetTextOrigin (TextBox control) => control.TextOrigin;

        /// <summary>
        /// Gets the TextBox's text seleection.
        /// </summary>
        protected TextSelection GetTextSelection (TextBox control) => control.document.GetTextSelection ();

        /// <summary>
        /// Updates the TextBox's scroll bars.
        /// </summary>
        protected void UpdateScrollBars (TextBox control, TextBlock block) => control.UpdateScrollBars (block);
    }
}
