using System;
using System.Drawing;
using System.Linq;
using SkiaSharp;
using Topten.RichTextKit;

namespace Modern.Forms
{
    class TextBoxDocument
    {
        private readonly TextBox textbox;

        private string text = string.Empty;
        private string placeholder = string.Empty;

        private TextBlock? cached_text_block;

        private int cursor_index = 0;
        private bool read_only = false;
        private int selection_start = -1;
        private int selection_end = -1;
        private int max_length = int.MaxValue;
        private bool multiline = false;
        private int width = -1;
        private SKTypeface font = Theme.UIFont;
        private int font_size = Theme.FontSize;
        private TextAlignment alignment = TextAlignment.Left;
        private SKColor font_color = Theme.DarkTextColor;
        private SKColor placeholder_font_color = Theme.DisabledTextColor;
        private SKColor selection_color = new SKColor (153, 201, 239);

        private static readonly string[] invalid_singleline_characters = new[] { "\r", "\n" };

        public TextBoxDocument (TextBox textbox)
        {
            this.textbox = textbox;
            width = textbox.PaddedClientRectangle.Width;
        }

        public TextAlignment Alignment {
            get => alignment;
            set {
                if (alignment != value) {
                    alignment = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public bool AtBeginning => cursor_index == 0;

        public bool AtEnd => cursor_index == text.Length;

        public int CursorIndex => cursor_index;

        public bool DeleteSelection ()
        {
            if (!IsTextSelected || read_only)
                return false;

            var start = Math.Min (selection_start, selection_end);
            var end = Math.Max (selection_start, selection_end);

            SetCursorToCharIndex (start);

            RemoveText (start, end - start);

            Deselect ();

            return true;
        }

        public bool DeleteText (bool forward, bool wholeWord)
        {
            // TODO: wholeWord not implemented
            if (read_only)
                return false;

            if (DeleteSelection ())
                return true;

            if (forward && !AtEnd) {
                RemoveText (cursor_index, 1);
                return true;
            }

            if (!forward && !AtBeginning) {
                var block = GetTextBlock ();
                var current_caret = block.GetCaretInfo (cursor_index);

                SetCursorToCharIndex (current_caret.PreviousCodePointIndex);

                RemoveText (cursor_index - 1, 1);

                return true;
            }

            return false;
        }

        public bool Deselect ()
        {
            if (!IsTextSelected)
                return false;

            selection_start = -1;
            selection_end = -1;

            return true;
        }

        public string DisplayText => text.Length > 0 ? text : placeholder;

        public SKTypeface Font {
            get => font;
            set {
                if (font != value) {
                    font = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public SKColor FontColor {
            get => font_color;
            set {
                if (font_color != value) {
                    font_color = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public int FontSize {
            get => font_size;
            set {
                if (font_size != value) {
                    font_size = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public HitTestResult GetCharIndexFromPosition (int x, int y)
        {
            var hit = GetTextBlock ().HitTest (x, y);

            return hit;
        }

        public TextSelection GetTextSelection () => new TextSelection (selection_start, selection_end, selection_color);

        public TextBlock GetTextBlock ()
        {
            if (cached_text_block != null)
                return cached_text_block;

            var max_size = multiline ? new Size (width, int.MaxValue) : TextMeasurer.MaxSize;
            var color = Text.HasValue () ? font_color : placeholder_font_color;

            return cached_text_block = TextMeasurer.CreateTextBlock (DisplayText, font, font_size, max_size, alignment, color, MaxLines);
        }

        public bool InsertText (string str)
        {
            if (read_only)
                return false;

            // Delete any currently selected text
            DeleteSelection ();

            str = StripInvalidCharacters (str);

            if (text.Length + str.Length > max_length)
                str = str.Substring (0, max_length - text.Length);

            text = text.Insert (cursor_index, str);
            cached_text_block = null;

            // TODO: Need to properly handle code points
            SetCursorToCharIndex (cursor_index + str.Length);

            return true;
        }

        public void Invalidate ()
        {
            textbox.Invalidate ();
        }

        public bool IsMultiline {
            get => multiline;
            set {
                if (multiline != value) {
                    multiline = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public bool IsTextSelected => selection_start >= 0 && selection_end >= 0 && SelectionLength != 0;

        public int MaxLength {
            get => max_length == int.MaxValue ? 0 : max_length;
            set => max_length = max_length == 0 ? int.MaxValue : max_length;
        }

        private int? MaxLines => multiline ? (int?)null : 1;

        public bool MoveCursor (ArrowDirection direction, bool select, bool wholeWord, bool end)
        {
            if (!select)
                Deselect ();

            var new_index = -1;
            var block = GetTextBlock ();
            var current_caret = block.GetCaretInfo (cursor_index);
            
            switch (direction) {
                case ArrowDirection.Left:

                    // Ctrl-Home - Go to the beginning of the document
                    if (end && wholeWord)
                        new_index = block.CaretIndicies.First ();
                    // Home - Go to the beginning of the current line
                    else if (end)
                        new_index = block.HitTest (0, current_caret.CaretRectangle.MidY).ClosestCodePointIndex;
                    // Ctrol-Left - Go left one word
                    else if (wholeWord)
                        new_index =  TextMeasurer.FindNextSeparator (text, cursor_index, false);
                    // Left - Go left one character
                    else
                        new_index = current_caret.PreviousCodePointIndex;

                    break;

                case ArrowDirection.Up:

                    // Multiline - Go up one line
                    if (multiline)
                        new_index = GetCharIndexFromPosition ((int)current_caret.CaretXCoord, (int)current_caret.CaretRectangle.MidY - FontSize).ClosestCodePointIndex;
                    // Single line - Go left one character
                    else
                        new_index = current_caret.PreviousCodePointIndex;

                    break;

                case ArrowDirection.Right:

                    // Ctrl-End - Go to the end of the document
                    if (end && wholeWord)
                        new_index = block.CaretIndicies.Last ();
                    // End - Go to the end of the current line
                    else if (end)
                        new_index = block.HitTest (int.MaxValue, current_caret.CaretRectangle.MidY).ClosestCodePointIndex;
                    // Ctrl-Right - Go right one word
                    else if (wholeWord)
                        new_index = TextMeasurer.FindNextSeparator (text, cursor_index, true);
                    // Right - Go right one character
                    else
                        new_index = current_caret.NextCodePointIndex;

                    break;

                case ArrowDirection.Down:

                    // Multiline - Go down one line
                    if (multiline)
                        new_index = GetCharIndexFromPosition ((int)current_caret.CaretXCoord, (int)current_caret.CaretRectangle.MidY + FontSize).ClosestCodePointIndex;
                    // Single line - Go left one character
                    else
                        new_index = current_caret.NextCodePointIndex;

                    break;
            }

            if (new_index != -1 && new_index != cursor_index) {
                SetCursorToCharIndex (new_index);
                return true;
            }

            return false;
        }

        public string Placeholder {
            get => placeholder;
            set {
                if (placeholder != value) {
                    placeholder = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public SKColor PlaceholderFontColor {
            get => placeholder_font_color;
            set {
                if (placeholder_font_color != value) {
                    placeholder_font_color = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }

        public bool ReadOnly {
            get => read_only;
            set {
                if (read_only != value) {
                    read_only = value;
                    Invalidate ();
                }
            }
        }

        private void RemoveText (int start, int length)
        {
            text = text.Remove (start, length);
            cached_text_block = null;
        }

        public void SelectAll ()
        {
            selection_start = 0;
            selection_end = text.Length;

            Invalidate ();
        }

        public string SelectedText => IsTextSelected ? text.Substring (Math.Min (selection_start, selection_end), SelectionLength) : string.Empty;

        public SKColor SelectionColor {
            get => selection_color;
            set {
                if (selection_color != value) {
                    selection_color = value;
                    Invalidate ();
                }
            }
        }

        public int SelectionEnd {
            get => selection_end;
            set {
                if (selection_end != value) {
                    selection_end = value;
                    Invalidate ();
                }
            }
        }

        public int SelectionLength => Math.Abs (selection_end - selection_start);

        public int SelectionStart {
            get => selection_start;
            set {
                if (selection_start != value) {
                    selection_start = value;
                    Invalidate ();
                }
            }
        }

        public bool SetCursorToCharIndex (int index)
        {
            if (cursor_index == index)
                return false;

            cursor_index = index;

            return true;
        }

        private string StripInvalidCharacters (string text)
        {
            if (multiline)
                return text;

            foreach (var c in invalid_singleline_characters)
                text = text.Replace (c, string.Empty);

            return text;
        }

        public string Text {
            get => text;
            set {
                if (text != value) {
                    text = value;
                    cached_text_block = null;

                    // If the Text property is changed, we need to reset the cursor to the top
                    SetCursorToCharIndex (0);
                    Invalidate ();
                }
            }
        }

        public int Width {
            get => width;
            set {
                if (width != value) {
                    width = value;
                    cached_text_block = null;
                    Invalidate ();
                }
            }
        }
    }
}
