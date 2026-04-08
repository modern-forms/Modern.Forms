using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a cell in a DataGridView control.
    /// </summary>
    public class DataGridViewCell
    {
        private string value = string.Empty;
        private DataGridViewRow? owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewCell class.
        /// </summary>
        public DataGridViewCell ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewCell class with the specified value.
        /// </summary>
        public DataGridViewCell (string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the bounding rectangle of the cell.
        /// </summary>
        internal Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets the column index of this cell.
        /// </summary>
        public int ColumnIndex => owner?.Cells.IndexOf (this) ?? -1;

        /// <summary>
        /// Gets the DataGridView that contains this cell.
        /// </summary>
        public DataGridView? DataGridView => owner?.DataGridView;

        /// <summary>
        /// Gets the row that contains this cell.
        /// </summary>
        public DataGridViewRow? OwningRow => owner;

        /// <summary>
        /// Gets the row index of this cell.
        /// </summary>
        public int RowIndex => owner?.Index ?? -1;

        /// <summary>
        /// Gets or sets whether this cell is selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the style for this cell.
        /// </summary>
        public ControlStyle Style { get; set; } = new ControlStyle (DataGridView.DefaultCellStyleInternal);

        /// <summary>
        /// Gets or sets an object that contains data to associate with the cell.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the value of this cell.
        /// </summary>
        public string Value {
            get => value;
            set {
                if (this.value != value) {
                    this.value = value;
                    owner?.DataGridView?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Sets the owning row.
        /// </summary>
        internal void SetOwner (DataGridViewRow? row) => owner = row;
    }
}
