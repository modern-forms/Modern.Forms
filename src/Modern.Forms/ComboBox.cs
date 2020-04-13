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
                    popup ??= new PopupWindow (FindForm ()) {
                        Size = new Size (Width, 102)
                    };

                    popup.Location = PointToScreen (new Point (1, ScaledHeight - 1));
                    popup.Controls.Add (popup_listbox);

                    popup.Show ();

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
            get => Items.SelectedIndex;
            set => Items.SelectedIndex = value;
        }

        /// <summary>
        /// Gets or sets the currently selected item, if any.
        /// </summary>
        public object? SelectedItem {
            get => Items.SelectedItem;
            set => Items.SelectedItem = value;
        }

        /// <summary>
        /// Raised when the value of the SelectedIndex property changes.
        /// </summary>
        public event EventHandler? SelectedIndexChanged;

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
