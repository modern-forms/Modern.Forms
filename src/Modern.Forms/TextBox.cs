using System;
using System.Drawing;
using System.Linq;
using SkiaSharp;

namespace Modern.Forms
{
    // TODO:
    public class TextBox : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.Border.Width = 1;
                style.BackgroundColor = Theme.LightNeutralGray;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private int cursor_index = 0;
        private bool read_only = false;
        private string placeholder = string.Empty;
        private int selection_start = -1;
        private int selection_end = -1;
        private bool is_highlighting;
        private int selection_anchor = -1;
        private bool multiline = false;

        private static SKColor selection_color = new SKColor (153, 201, 239);
        private static SKColor selection_color_deselected = new SKColor (212, 220, 216);

        protected override Size DefaultSize => new Size (100, 28);

        public TextBox ()
        {
            SetControlBehavior (ControlBehaviors.InvalidateOnTextChanged);

            Cursor = Cursors.IBeam;
        }

        public bool MultiLine {
            get => multiline;
            set {
                if (multiline != value) {
                    multiline = value;
                    Invalidate ();
                }
            }
        }

        public string Placeholder {
            get => placeholder;
            set {
                if (placeholder != value) {
                    placeholder = value;
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

        public int GetCharIndexFromPosition (Point location)
        {
            if (CurrentText.Length == 0)
                return 0;

            var hit = TextMeasurer.HitTest (Text, ClientRectangle, CurrentStyle.GetFont (), LogicalToDeviceUnits (CurrentStyle.GetFontSize ()), new Size (1000, 1000), Alignment, location, MaxLines);

            return hit.ClosestCodePointIndex;
        }

        protected override void OnKeyDown (KeyEventArgs e)
        {
            base.OnKeyDown (e);

            switch (e.KeyData & Keys.KeyCode) {
                case Keys.Delete:
                    if (read_only || DeleteHighlightedText () || cursor_index >= CurrentText.Length)
                        return;

                    Text = Text.Remove (cursor_index, 1);
                    e.Handled = true;
                    return;
                case Keys.Left:
                    if (!Dehighlight () && cursor_index > 0)
                        cursor_index--;

                    Invalidate ();
                    e.Handled = true;

                    return;
                case Keys.Right:
                    if (!Dehighlight () && cursor_index < TextMeasurer.GetMaxCaretIndex (Text))
                        cursor_index++;

                    Invalidate ();
                    e.Handled = true;

                    return;
                case Keys.Home:
                case Keys.Up:
                    Dehighlight ();

                    cursor_index = 0;
                    Invalidate ();
                    e.Handled = true;

                    return;
                case Keys.End:
                case Keys.Down:
                    Dehighlight ();

                    cursor_index = CurrentText.Length;
                    Invalidate ();
                    e.Handled = true;

                    return;
            }
        }

        protected override void OnKeyPress (KeyPressEventArgs e)
        {
            base.OnKeyPress (e);

            if (read_only) {
                e.Handled = true;
                return;
            }

            // Backspace = 8
            if (e.KeyChar == 8) {
                if (!DeleteHighlightedText () && cursor_index != 0)
                    Text = CurrentText.Remove (--cursor_index, 1);

                e.Handled = true;
                return;
            }

            // Ctrl-Backspace = 127
            if (e.KeyChar == 127) {
                var new_index = TextMeasurer.FindNextSeparator (CurrentText, cursor_index, false);

                if (!DeleteHighlightedText () && cursor_index != 0) {
                    Text = CurrentText.Remove (new_index, cursor_index - new_index);
                    cursor_index = new_index;
                }

                e.Handled = true;
                return;
            }

            if (e.KeyChar >= 32) {
                DeleteHighlightedText ();
                Text = CurrentText.Insert (cursor_index++, (e.KeyChar).ToString ());
            }
        }

        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            Dehighlight ();
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (e.Button != MouseButtons.Left)
                return;

            cursor_index = GetCharIndexFromPosition (e.Location);

            is_highlighting = true;
            selection_anchor = cursor_index;

            Invalidate ();
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (is_highlighting) {
                cursor_index = GetCharIndexFromPosition (e.Location);

                if (cursor_index == selection_anchor) {
                    selection_start = -1;
                    selection_end = -1;
                } else {
                    selection_start = selection_anchor;
                    selection_end = cursor_index;
                }

                Invalidate ();
            }
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (e.Button != MouseButtons.Left)
                return;

            cursor_index = GetCharIndexFromPosition (e.Location);

            is_highlighting = false;

            if (cursor_index == selection_anchor) {
                selection_start = -1;
                selection_end = -1;
            } else {
                selection_start = selection_anchor;
                selection_end = cursor_index;
            }

            Invalidate ();
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            if (!string.IsNullOrEmpty (Text))
                e.Canvas.DrawText (Text, ClientRectangle, this, Alignment, selection_start, selection_end, Selected ? selection_color : selection_color_deselected, MaxLines);
            else if (!string.IsNullOrEmpty (placeholder))
                e.Canvas.DrawText (placeholder, CurrentStyle.GetFont (), CurrentFontSize, ClientRectangle, Theme.DisabledTextColor, Alignment, maxLines: MaxLines);

            if (Selected) {
                var caret = TextMeasurer.GetCursorLocation (Text, ClientRectangle, CurrentStyle.GetFont (), LogicalToDeviceUnits (CurrentStyle.GetFontSize ()), new Size (1000, 1000), Alignment, cursor_index, MaxLines);
                e.Canvas.DrawRectangle (caret, Theme.DarkTextColor);
            }
        }

        private bool DeleteHighlightedText ()
        {
            if (!IsTextHighlighted)
                return false;

            cursor_index = Math.Min (selection_start, selection_end);

            Text = CurrentText.Remove (Math.Min (selection_start, selection_end), SelectionLength);

            Dehighlight ();

            return true;
        }

        private bool Dehighlight ()
        {
            if (!IsTextHighlighted)
                return false;

            selection_start = -1;
            selection_end = -1;

            return true;
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

        public int SelectionStart {
            get => selection_start;
            set {
                if (selection_start != value) {
                    selection_start = value;
                    Invalidate ();
                }
            }
        }

        private string CurrentText => Text ?? string.Empty;

        private int CurrentFontSize => LogicalToDeviceUnits (CurrentStyle.GetFontSize ());

        private bool IsTextHighlighted => selection_start >= 0 && selection_end >= 0 && SelectionLength != 0;

        private int SelectionLength => Math.Abs (selection_end - selection_start);

        private int? MaxLines => multiline ? (int?)null : 1;

        private ContentAlignment Alignment => MultiLine ? ContentAlignment.TopLeft : ContentAlignment.TopLeft;
    }
}
