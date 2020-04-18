using System;
using System.Drawing;
using Modern.Forms.Renderers;
using Topten.RichTextKit;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a TextBox control.
    /// </summary>
    public class TextBox : ScrollControl
    {
        internal readonly TextBoxDocument document;

        private bool is_highlighting;
        private int selection_anchor = -1;
        private int scroll_x = 0;
        private int scroll_y = 0;

        /// <summary>
        /// Initializes a new instance of the TextBox class.
        /// </summary>
        public TextBox ()
        {
            Cursor = Cursors.IBeam;

            document = new TextBoxDocument (this);

            VerticalScrollBar.Enabled = false;
            VerticalScrollBar.ValueChanged += (o, e) => DoScroll (0, (o as VerticalScrollBar)!.Value - scroll_y);
        }

        /// <summary>
        /// Copies the selected text of the TextBox to the clipboard.
        /// </summary>
        public void Copy ()
        {
            if (!document.IsTextSelected)
                return;

            var text = document.SelectedText;
            AsyncHelper.RunSync (() => Avalonia.AvaloniaGlobals.ClipboardInterface.SetTextAsync (text));
        }

        // The scaled height of the current font.
        internal int CurrentFontSize => LogicalToDeviceUnits (CurrentStyle.GetFontSize ());

        /// <summary>
        /// Copies the selected text of the TextBox to the clipboard and removes it from the TextBox.
        /// </summary>
        public void Cut ()
        {
            if (!document.IsTextSelected)
                return;

            var text = document.SelectedText;
            AsyncHelper.RunSync (() => Avalonia.AvaloniaGlobals.ClipboardInterface.SetTextAsync (text));

            document.DeleteSelection ();
        }

        /// <inheritdoc/>
        protected override Padding DefaultPadding => new Padding (1, 0, 0, 0);

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 25);

        /// <inheritdoc/>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.Border.Width = 1;
                style.BackgroundColor = Theme.LightNeutralGray;
            });

        // Scrolls the TextBox by the specified amounts.
        private void DoScroll (int x, int y)
        {
            scroll_x += x;
            scroll_y += y;

            Invalidate ();
        }

        // Gets the index of the character at the specified location.
        private int GetCharIndexFromPosition (Point location)
        {
            if (!document.Text.HasValue ())
                return 0;

            return document.GetCharIndexFromPosition (location.X - TextOrigin.X, location.Y - TextOrigin.Y).ClosestCodePointIndex;
        }

        // Handles key down events.
        private bool HandleKeyDown (KeyEventArgs e)
        {
            var need_refresh = false;

            try {
                switch (e.KeyData & Keys.KeyCode) {
                    case Keys.Left:
                        need_refresh = document.MoveCursor (ArrowDirection.Left, e.Shift, e.Control, false);
                        return true;
                    case Keys.Right:
                        need_refresh = document.MoveCursor (ArrowDirection.Right, e.Shift, e.Control, false);
                        return true;
                    case Keys.Home:
                        need_refresh = document.MoveCursor (ArrowDirection.Left, e.Shift, e.Control, true);
                        return true;
                    case Keys.End:
                        need_refresh = document.MoveCursor (ArrowDirection.Right, e.Shift, e.Control, true);
                        return true;
                    case Keys.Up:
                        need_refresh = document.MoveCursor (ArrowDirection.Up, e.Shift, e.Control, false);
                        return true;
                    case Keys.Down:
                        need_refresh = document.MoveCursor (ArrowDirection.Down, e.Shift, e.Control, false);
                        return true;
                    case Keys.Delete:
                        need_refresh = document.DeleteText (true, e.Control);
                        return true;
                    case Keys.Back:
                        need_refresh = document.DeleteText (false, e.Control);
                        return true;
                    case Keys.C:
                        if (e.Control)
                            Copy ();

                        return e.Control;
                    case Keys.X:
                        if (e.Control)
                            Cut ();

                        return e.Control;
                    case Keys.V:
                        if (e.Control)
                            Paste ();

                        return e.Control;
                    case Keys.A:
                        if (e.Control)
                            document.SelectAll ();

                        return e.Control;

                }
            } finally {
                if (need_refresh)
                    ScrollToCaret ();
            }

            return false;
        }

        /// <summary>
        /// Gets or sets a value indicating the maximum length of text the TextBox can hold.
        /// </summary>
        public int MaxLength {
            get => document.MaxLength;
            set => document.MaxLength = value;
        }

        /// <summary>
        /// Gets or sets a value indicating if the TextBox supports multiple lines of text.
        /// </summary>
        public bool MultiLine {
            get => document.IsMultiline;
            set {
                if (document.IsMultiline != value) {

                    if (Padding == DefaultPadding)
                        Padding = new Padding (value ? 4 : 1, 0, 0, 0);

                    document.IsMultiline = value;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            document.Deselect ();
        }

        /// <inheritdoc/>
        protected override void OnEnabledChanged (EventArgs e)
        {
            base.OnEnabledChanged (e);

            document.Enabled = Enabled;
        }

        /// <inheritdoc/>
        protected override void OnKeyDown (KeyEventArgs e)
        {
            base.OnKeyDown (e);

            e.Handled = HandleKeyDown (e);
        }

        /// <inheritdoc/>
        protected override void OnKeyPress (KeyPressEventArgs e)
        {
            base.OnKeyPress (e);

            // Enter = 13
            if (e.KeyChar == 13 && MultiLine) {
                if (document.InsertText ("\n"))
                    ScrollToCaret ();
            }

            // Printable characters (except backspace)
            if (e.KeyChar >= 32 && e.KeyChar != 127) {
                if (document.InsertText ((e.KeyChar).ToString ()))
                    ScrollToCaret ();
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (e.Button != MouseButtons.Left)
                return;

            SetCursorToCharIndex (GetCharIndexFromPosition (e.Location));

            is_highlighting = true;
            selection_anchor = document.CursorIndex;

            Invalidate ();
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (is_highlighting) {
                SetCursorToCharIndex (GetCharIndexFromPosition (e.Location));

                if (document.CursorIndex == selection_anchor) {
                    document.SelectionStart = -1;
                    document.SelectionEnd = -1;
                } else {
                    document.SelectionStart = selection_anchor;
                    document.SelectionEnd = document.CursorIndex;
                }

                Invalidate ();
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (e.Button != MouseButtons.Left)
                return;

            SetCursorToCharIndex (GetCharIndexFromPosition (e.Location));

            is_highlighting = false;

            if (document.CursorIndex == selection_anchor) {
                document.SelectionStart = -1;
                document.SelectionEnd = -1;
            } else {
                document.SelectionStart = selection_anchor;
                document.SelectionEnd = document.CursorIndex;
            }

            Invalidate ();
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <inheritdoc/>
        protected override void OnSizeChanged (EventArgs e)
        {
            base.OnSizeChanged (e);

            document.Width = PaddedClientRectangle.Width;
        }

        /// <summary>
        /// Inserts any text on the clipboard into the TextBox.
        /// </summary>
        public void Paste ()
        {
            if (document.ReadOnly)
                return;

            var text = AsyncHelper.RunSync (() => Avalonia.AvaloniaGlobals.ClipboardInterface.GetTextAsync ());

            if (!string.IsNullOrEmpty (text) && document.InsertText (text))
                    ScrollToCaret ();
        }

        /// <summary>
        /// Gets or sets text to display if the TextBox contains no text.
        /// </summary>
        public string Placeholder {
            get => document.Placeholder;
            set => document.Placeholder = value;
        }

        /// <summary>
        /// Gets or sets a value indicating if the text can be edited.
        /// </summary>
        public bool ReadOnly {
            get => document.ReadOnly;
            set => document.ReadOnly = value;
        }

        /// <summary>
        /// Scrolls the TextBox so that the caret is visible.
        /// </summary>
        public void ScrollToCaret ()
        {
            var caret = TextMeasurer.GetCursorLocation (document.GetTextBlock (), TextOrigin, document.CursorIndex, CurrentFontSize);

            if (caret.IsEmpty)
                return;

            caret.Offset (scroll_x, scroll_y);

            var dx = 0;
            var dy = 0;
            var viewport = TextViewport;

            if (caret.Top < viewport.Top)
                dy = caret.Top - viewport.Top - 1;
            else if (caret.Bottom > viewport.Bottom)
                dy = caret.Bottom - viewport.Bottom + 3;

            if (caret.Left < viewport.Left)
                dx = caret.Left - viewport.Left - 1;
            else if (caret.Right > viewport.Right)
                dx = caret.Right - viewport.Right + 3;

            DoScroll (dx, dy);
        }

        /// <summary>
        /// Gets or sets a value indicating the end of the TextBox's selected text.
        /// </summary>
        public int SelectionEnd {
            get => document.SelectionEnd;
            set => document.SelectionEnd = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the start of the TextBox's selected text.
        /// </summary>
        public int SelectionStart {
            get => document.SelectionStart;
            set => document.SelectionStart = value;
        }

        // Sets cursor to specified character index and scrolls TextBox to cursor.
        private void SetCursorToCharIndex (int index)
        {
            if (document.SetCursorToCharIndex (index))
                ScrollToCaret ();
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <inheritdoc/>
        public override string Text { 
            get => document.Text; 
            set {
                if (document.Text != value) {
                    document.Text = value;
                    ScrollToCaret ();
                }
            }
        }

        // Where the text starts, taking scrolling into account
        internal Point TextOrigin => new Point (PaddedClientRectangle.Location.X - scroll_x, PaddedClientRectangle.Location.Y - scroll_y);

        // The virtual bounds of what is currently shown to the user.
        private Rectangle TextViewport => new Rectangle (new Point (PaddedClientRectangle.Location.X + scroll_x, PaddedClientRectangle.Location.Y + scroll_y), PaddedClientRectangle.Size);

        // Enables and recalculates scrollbars as needed.
        internal void UpdateScrollBars (TextBlock block)
        {
            // TODO: Horizontal scrollbar not supported
            // Something about the document changed, so we need to update the scrollbars
            if ((int)block.MeasuredHeight - PaddedClientRectangle.Height > 0) {
                VerticalScrollBar.Enabled = true;
                VerticalScrollBar.Maximum = (int)block.MeasuredHeight - PaddedClientRectangle.Height;
                VerticalScrollBar.LargeChange = PaddedClientRectangle.Height;
                VerticalScrollBar.SmallChange = CurrentFontSize * 3;

                var new_value = Math.Min (scroll_y, VerticalScrollBar.Maximum);

                if (VerticalScrollBar.Value != new_value)
                    VerticalScrollBar.Value = new_value;
            } else {
                if (scroll_y > 0)
                    DoScroll (0, -scroll_y);

                VerticalScrollBar.Enabled = false;
            }
        }
    }
}
