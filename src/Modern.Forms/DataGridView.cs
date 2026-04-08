using System.Drawing;
using Modern.Forms.Renderers;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a DataGridView control for displaying tabular data.
    /// </summary>
    public class DataGridView : Control
    {
        private int header_height = 30;
        private int row_height = 25;
        private int top_index;
        private int horizontal_scroll_offset;
        private int selected_row_index = -1;
        private int selected_column_index = -1;
        private int hovered_row_index = -1;
        private int resize_column_index = -1;
        private int resize_start_x;
        private int resize_start_width;
        private bool is_resizing;
        private bool column_headers_visible = true;
        private bool row_selection_mode = true;

        private readonly VerticalScrollBar vscrollbar;
        private readonly HorizontalScrollBar hscrollbar;

        /// <summary>
        /// Initializes a new instance of the DataGridView class.
        /// </summary>
        public DataGridView ()
        {
            Columns = new DataGridViewColumnCollection (this);
            Rows = new DataGridViewRowCollection (this);

            vscrollbar = new VerticalScrollBar {
                Minimum = 0,
                Maximum = 0,
                SmallChange = 1,
                LargeChange = 1,
                Visible = false,
                Dock = DockStyle.Right
            };

            vscrollbar.ValueChanged += (o, e) => {
                top_index = Math.Max (vscrollbar.Value, 0);
                Invalidate ();
            };

            hscrollbar = new HorizontalScrollBar {
                Minimum = 0,
                Maximum = 0,
                SmallChange = 10,
                LargeChange = 50,
                Visible = false,
                Dock = DockStyle.Bottom
            };

            hscrollbar.ValueChanged += (o, e) => {
                horizontal_scroll_offset = Math.Max (hscrollbar.Value, 0);
                Invalidate ();
            };

            Controls.AddImplicitControl (vscrollbar);
            Controls.AddImplicitControl (hscrollbar);
        }

        /// <summary>
        /// Gets or sets whether column headers are visible.
        /// </summary>
        public bool ColumnHeadersVisible {
            get => column_headers_visible;
            set {
                if (column_headers_visible != value) {
                    column_headers_visible = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets the collection of columns in the DataGridView.
        /// </summary>
        public DataGridViewColumnCollection Columns { get; }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (450, 300);

        /// <inheritdoc/>
        public new static readonly ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.ControlLowColor;
                style.Border.Width = 1;
            });

        /// <summary>
        /// Gets the first visible row index.
        /// </summary>
        public int FirstVisibleIndex {
            get => top_index;
            set {
                if (top_index == value)
                    return;

                if (value < 0 || value >= Rows.Count)
                    return;

                vscrollbar.Value = Math.Min (value, vscrollbar.Maximum);
            }
        }

        /// <summary>
        /// Gets the bounding rectangle for a cell.
        /// </summary>
        public Rectangle GetCellBounds (int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count || columnIndex < 0 || columnIndex >= Columns.Count)
                return Rectangle.Empty;

            var client = GetContentArea ();
            var row_top = client.Top + (ColumnHeadersVisible ? ScaledHeaderHeight : 0);
            var visible_row = rowIndex - top_index;

            if (visible_row < 0 || visible_row >= VisibleRowCount)
                return Rectangle.Empty;

            var y = row_top + visible_row * ScaledRowHeight;
            var x = client.Left - horizontal_scroll_offset;

            for (var i = 0; i < columnIndex; i++) {
                if (Columns[i].Visible)
                    x += LogicalToDeviceUnits (Columns[i].Width);
            }

            var col_width = LogicalToDeviceUnits (Columns[columnIndex].Width);
            return new Rectangle (x, y, col_width, ScaledRowHeight);
        }

        /// <summary>
        /// Gets the content area, accounting for scrollbars.
        /// </summary>
        internal Rectangle GetContentArea ()
        {
            var client = ClientRectangle;
            var w = client.Width - (vscrollbar.Visible ? vscrollbar.ScaledWidth : 0);
            var h = client.Height - (hscrollbar.Visible ? hscrollbar.ScaledHeight : 0);
            return new Rectangle (client.Left, client.Top, w, h);
        }

        /// <summary>
        /// Gets the column index at the specified location.
        /// </summary>
        internal int GetColumnAtLocation (Point location)
        {
            var client = GetContentArea ();
            var x = client.Left - horizontal_scroll_offset;

            for (var i = 0; i < Columns.Count; i++) {
                if (!Columns[i].Visible)
                    continue;

                var col_width = LogicalToDeviceUnits (Columns[i].Width);

                if (location.X >= x && location.X < x + col_width)
                    return i;

                x += col_width;
            }

            return -1;
        }

        /// <summary>
        /// Gets the resize column index if the mouse is near a column border.
        /// </summary>
        private int GetResizeColumnAtLocation (Point location)
        {
            var client = GetContentArea ();
            var header_rect = new Rectangle (client.Left, client.Top, client.Width, ScaledHeaderHeight);

            if (!header_rect.Contains (location))
                return -1;

            var x = client.Left - horizontal_scroll_offset;
            var resize_zone = LogicalToDeviceUnits (4);

            for (var i = 0; i < Columns.Count; i++) {
                if (!Columns[i].Visible)
                    continue;

                x += LogicalToDeviceUnits (Columns[i].Width);

                if (Math.Abs (location.X - x) <= resize_zone)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the row index at the specified location.
        /// </summary>
        internal int GetRowAtLocation (Point location)
        {
            var client = GetContentArea ();
            var row_top = client.Top + (ColumnHeadersVisible ? ScaledHeaderHeight : 0);

            if (location.Y < row_top)
                return -1;

            var relative_y = location.Y - row_top;
            var row_index = relative_y / ScaledRowHeight + top_index;

            if (row_index >= 0 && row_index < Rows.Count)
                return row_index;

            return -1;
        }

        /// <summary>
        /// Gets or sets the height, in pixels, of the column headers row.
        /// </summary>
        public int HeaderHeight {
            get => header_height;
            set {
                if (header_height != value) {
                    header_height = Math.Max (value, 10);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll offset.
        /// </summary>
        internal int HorizontalScrollOffset => horizontal_scroll_offset;

        /// <summary>
        /// Gets or sets the index of the currently hovered row.
        /// </summary>
        internal int HoveredRowIndex {
            get => hovered_row_index;
            set {
                if (hovered_row_index != value) {
                    hovered_row_index = value;
                    Invalidate ();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            if (!Enabled || !e.Button.HasFlag (MouseButtons.Left))
                return;

            // Check for column resize
            if (ColumnHeadersVisible) {
                var resize_col = GetResizeColumnAtLocation (e.Location);

                if (resize_col >= 0) {
                    is_resizing = true;
                    resize_column_index = resize_col;
                    resize_start_x = e.Location.X;
                    resize_start_width = LogicalToDeviceUnits (Columns[resize_col].Width);
                    return;
                }
            }

            // Check for header click (sorting)
            if (ColumnHeadersVisible) {
                var client = GetContentArea ();
                var header_rect = new Rectangle (client.Left, client.Top, client.Width, ScaledHeaderHeight);

                if (header_rect.Contains (e.Location)) {
                    var col = GetColumnAtLocation (e.Location);

                    if (col >= 0 && Columns[col].Sortable)
                        OnColumnHeaderClick (col);

                    return;
                }
            }

            // Select row/cell
            var row = GetRowAtLocation (e.Location);

            if (row >= 0) {
                if (row_selection_mode) {
                    SelectedRowIndex = row;
                } else {
                    var col = GetColumnAtLocation (e.Location);
                    SelectedRowIndex = row;
                    SelectedColumnIndex = col;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);
            HoveredRowIndex = -1;

            if (!is_resizing)
                Cursor = Cursors.Arrow;
        }

        /// <inheritdoc/>
        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            if (is_resizing) {
                var delta = e.Location.X - resize_start_x;
                var new_width = DeviceToLogicalUnits (resize_start_width + delta);
                Columns[resize_column_index].Width = new_width;
                UpdateScrollBars ();
                return;
            }

            // Update cursor for column resize zones
            if (ColumnHeadersVisible) {
                var resize_col = GetResizeColumnAtLocation (e.Location);
                Cursor = resize_col >= 0 ? Cursors.SizeWestEast : Cursors.Arrow;
            }

            // Update hovered row
            var row = GetRowAtLocation (e.Location);
            HoveredRowIndex = row;
        }

        /// <inheritdoc/>
        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            if (is_resizing) {
                is_resizing = false;
                resize_column_index = -1;
                Cursor = Cursors.Arrow;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel (MouseEventArgs e)
        {
            base.OnMouseWheel (e);

            if (vscrollbar.Visible)
                vscrollbar.RaiseMouseWheel (e);
        }

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Handles a column header click for sorting.
        /// </summary>
        private void OnColumnHeaderClick (int columnIndex)
        {
            var column = Columns[columnIndex];

            // Toggle sort order
            var new_order = column.SortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending;

            // Reset all other columns
            foreach (var col in Columns)
                col.SortOrder = SortOrder.None;

            column.SortOrder = new_order;

            // Raise the event
            ColumnHeaderClick?.Invoke (this, new EventArgs<DataGridViewColumn> (column));

            Invalidate ();
        }

        /// <summary>
        /// Raised when a column header is clicked.
        /// </summary>
        public event EventHandler<EventArgs<DataGridViewColumn>>? ColumnHeaderClick;

        /// <inheritdoc/>
        protected override void OnKeyUp (KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) {
                if (selected_row_index < Rows.Count - 1) {
                    SelectedRowIndex = selected_row_index + 1;
                    EnsureRowVisible (selected_row_index);
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyCode == Keys.Up) {
                if (selected_row_index > 0) {
                    SelectedRowIndex = selected_row_index - 1;
                    EnsureRowVisible (selected_row_index);
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyCode == Keys.PageDown) {
                var new_index = Math.Min (selected_row_index + VisibleRowCount, Rows.Count - 1);
                SelectedRowIndex = new_index;
                EnsureRowVisible (new_index);
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.PageUp) {
                var new_index = Math.Max (selected_row_index - VisibleRowCount, 0);
                SelectedRowIndex = new_index;
                EnsureRowVisible (new_index);
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Home) {
                SelectedRowIndex = 0;
                EnsureRowVisible (0);
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.End) {
                SelectedRowIndex = Rows.Count - 1;
                EnsureRowVisible (Rows.Count - 1);
                e.Handled = true;
                return;
            }

            if (!row_selection_mode) {
                if (e.KeyCode == Keys.Left && selected_column_index > 0) {
                    SelectedColumnIndex = selected_column_index - 1;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Right && selected_column_index < Columns.Count - 1) {
                    SelectedColumnIndex = selected_column_index + 1;
                    e.Handled = true;
                    return;
                }
            }

            base.OnKeyUp (e);
        }

        /// <summary>
        /// Called when the row collection changes.
        /// </summary>
        internal void OnRowsChanged ()
        {
            UpdateScrollBars ();
            Invalidate ();
        }

        /// <summary>
        /// Gets the collection of rows in the DataGridView.
        /// </summary>
        public DataGridViewRowCollection Rows { get; }

        /// <summary>
        /// Gets or sets the default height, in pixels, of each row.
        /// </summary>
        public int RowHeight {
            get => row_height;
            set {
                if (row_height != value) {
                    row_height = Math.Max (value, 10);
                    UpdateScrollBars ();
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether entire rows are selected when a cell is clicked.
        /// </summary>
        public bool RowSelectionMode {
            get => row_selection_mode;
            set {
                if (row_selection_mode != value) {
                    row_selection_mode = value;
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets the scaled height of the header row.
        /// </summary>
        public int ScaledHeaderHeight => LogicalToDeviceUnits (header_height);

        /// <summary>
        /// Gets the scaled height of each data row.
        /// </summary>
        public int ScaledRowHeight => LogicalToDeviceUnits (row_height);

        /// <summary>
        /// Gets or sets the index of the currently selected column.
        /// </summary>
        public int SelectedColumnIndex {
            get => selected_column_index;
            set {
                if (selected_column_index != value) {
                    selected_column_index = value;
                    OnSelectionChanged (EventArgs.Empty);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected row.
        /// </summary>
        public int SelectedRowIndex {
            get => selected_row_index;
            set {
                if (selected_row_index != value) {
                    // Deselect old row
                    if (selected_row_index >= 0 && selected_row_index < Rows.Count)
                        Rows[selected_row_index].Selected = false;

                    selected_row_index = value;

                    // Select new row
                    if (selected_row_index >= 0 && selected_row_index < Rows.Count)
                        Rows[selected_row_index].Selected = true;

                    OnSelectionChanged (EventArgs.Empty);
                    Invalidate ();
                }
            }
        }

        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        protected virtual void OnSelectionChanged (EventArgs e) => SelectionChanged?.Invoke (this, e);

        /// <summary>
        /// Raised when the selection changes.
        /// </summary>
        public event EventHandler? SelectionChanged;

        /// <inheritdoc/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore (x, y, width, height, specified);

            UpdateScrollBars ();
        }

        /// <inheritdoc/>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        /// <summary>
        /// Gets the total width needed to display all columns.
        /// </summary>
        internal int TotalColumnsWidth {
            get {
                var total = 0;

                foreach (var col in Columns)
                    if (col.Visible)
                        total += LogicalToDeviceUnits (col.Width);

                return total;
            }
        }

        /// <summary>
        /// Updates the scrollbars based on the current content.
        /// </summary>
        private void UpdateScrollBars ()
        {
            var client = ClientRectangle;
            var content_height = client.Height - (ColumnHeadersVisible ? ScaledHeaderHeight : 0);
            var visible_rows = content_height / ScaledRowHeight;

            // Vertical scrollbar
            if (Rows.Count > visible_rows) {
                vscrollbar.Visible = true;
                vscrollbar.Maximum = Rows.Count - visible_rows;
                vscrollbar.LargeChange = Math.Max (0, visible_rows);
            } else {
                vscrollbar.Visible = false;
                vscrollbar.Value = 0;
                top_index = 0;
            }

            // Horizontal scrollbar
            var available_width = client.Width - (vscrollbar.Visible ? vscrollbar.ScaledWidth : 0);

            if (TotalColumnsWidth > available_width) {
                hscrollbar.Visible = true;
                hscrollbar.Maximum = TotalColumnsWidth - available_width;
                hscrollbar.LargeChange = Math.Max (0, available_width);
            } else {
                hscrollbar.Visible = false;
                hscrollbar.Value = 0;
                horizontal_scroll_offset = 0;
            }
        }

        /// <summary>
        /// Gets the number of full rows that can be displayed at a time.
        /// </summary>
        public int VisibleRowCount {
            get {
                var content = GetContentArea ();
                var available = content.Height - (ColumnHeadersVisible ? ScaledHeaderHeight : 0);
                return Math.Max (0, available / ScaledRowHeight);
            }
        }

        /// <summary>
        /// Ensures the specified row is visible by scrolling if necessary.
        /// </summary>
        private void EnsureRowVisible (int index)
        {
            if (VisibleRowCount >= Rows.Count)
                return;

            if (index < top_index)
                FirstVisibleIndex = index;
            else if (index >= top_index + VisibleRowCount)
                FirstVisibleIndex = index - VisibleRowCount + 1;
        }

        /// <summary>
        /// Converts device units to logical units.
        /// </summary>
        internal int DeviceToLogicalUnits (int value)
        {
            var factor = Scaling;
            return factor > 0 ? (int)(value / factor) : value;
        }
    }
}
