using System;
using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a ComboBox control.
    /// </summary>
    public class ComboBox : Control
    {
        private PopupWindow? popup;
        private readonly ListBox popup_listbox;
        private bool suppress_popup_close;

        /// <summary>
        /// Initializes a new instance of the ComboBox class.
        /// </summary>
        public ComboBox ()
        {
            popup_listbox = new ListBox { Dock = DockStyle.Fill, ShowHover = true };
            popup_listbox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
        }

        /// <inheritdoc/>
        protected override Cursor DefaultCursor => Cursors.Hand;

        /// <inheritdoc/>
        protected override Padding DefaultPadding => new Padding (4, 0, 3, 0);

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (120, 28);

        /// <summary>
        /// The default ControlStyle for all instances of ComboBox.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.Border.Width = 1;
                style.BackgroundColor = Theme.DarkNeutralGray;
            });

        /// <inheritdoc/>
        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);

            popup?.Close ();
            popup = null;

            popup_listbox.Dispose ();
        }

        /// <summary>
        /// Raised when the drop down portion of the ComboBox is closed.
        /// </summary>
        public event EventHandler? DropDownClosed;

        /// <summary>
        /// Raised when the drop down portion of the ComboBox is opened.
        /// </summary>
        public event EventHandler? DropDownOpened;

        /// <summary>
        /// Gets or sets whether the drop down portion of the ComboBox is currently shown.
        /// </summary>
        public bool DroppedDown {
            get => popup?.Visible == true;
            set {
                if (DroppedDown && !value) {
                    popup?.Hide ();
                    OnDropDownClosed (EventArgs.Empty);
                } else if (!DroppedDown && value) {
                    if (FindForm () is not Form form)
                        throw new InvalidOperationException ("Cannot drop down a ComboBox that is not parented to a Form");

                    popup ??= new PopupWindow (form) {
                        Size = new Size (Width, 102)
                    };

                    popup.Controls.Add (popup_listbox);
                    popup.Show (this, 1, Height);

                    OnDropDownOpened (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the collection of items contained by this ComboBox.
        /// </summary>
        public ListBoxItemCollection Items => popup_listbox.Items;

        // When the selected item of the popup ListBox changes, update the ComboBox
        private void ListBox_SelectedIndexChanged (object? sender, EventArgs e)
        {
            if (popup_listbox.SelectedIndex > -1) {
                if (!suppress_popup_close)
                    DroppedDown = false;

                Invalidate ();

                OnSelectedIndexChanged (e);
            }
        }

        /// <inheritdoc/>
        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);

            DroppedDown = !DroppedDown;
        }

        /// <inheritdoc/>
        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            DroppedDown = false;
        }

        /// <summary>
        /// Raises the DropDownOpened event.
        /// </summary>
        protected virtual void OnDropDownClosed (EventArgs e) => DropDownClosed?.Invoke (this, e);

        /// <summary>
        /// Raises the DropDownOpened event.
        /// </summary>
        protected virtual void OnDropDownOpened (EventArgs e) => DropDownOpened?.Invoke (this, e);

        /// <inheritdoc/>
        protected override void OnKeyUp (KeyEventArgs e)
        {
            // Alt+Up/Down toggles the dropdown
            if (e.Alt && e.KeyCode.In (Keys.Up, Keys.Down)) {
                DroppedDown = !DroppedDown;
                e.Handled = true;
                return;
            }

            // If dropdown is shown, Esc/Enter will close it
            if (e.KeyCode.In (Keys.Escape, Keys.Enter) && DroppedDown) {
                DroppedDown = false;
                e.Handled = true;
                return;
            }

            // If you mouse click an item we automatically close the dropdown,
            // we don't want that behavior when using the keyboard.
            suppress_popup_close = true;
            popup_listbox.RaiseKeyUp (e);
            suppress_popup_close = false;

            if (e.Handled)
                return;

            base.OnKeyUp (e);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        protected virtual void OnSelectedIndexChanged (EventArgs e) => SelectedIndexChanged?.Invoke (this, e);

        /// <summary>
        /// Gets or sets the index of the currently selected item.  Returns -1 if no item is selected.
        /// </summary>
        public int SelectedIndex {
            get => popup_listbox.SelectedIndex;
            set => popup_listbox.SelectedIndex = value;
        }

        /// <summary>
        /// Gets or sets the currently selected item, if any.
        /// </summary>
        public object? SelectedItem {
            get => popup_listbox.SelectedItem;
            set => popup_listbox.SelectedItem = value;
        }

        /// <summary>
        /// Raised when the value of the SelectedIndex property changes.
        /// </summary>
        public event EventHandler? SelectedIndexChanged;

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
