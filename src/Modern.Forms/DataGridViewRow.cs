using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a row in a DataGridView control.
    /// </summary>
    public class DataGridViewRow
    {
        private int height = 25;
        private DataGridView? owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewRow class.
        /// </summary>
        public DataGridViewRow ()
        {
            Cells = new DataGridViewCellCollection (this);
        }

        /// <summary>
        /// Gets the bounding rectangle of the row.
        /// </summary>
        internal Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets the collection of cells in this row.
        /// </summary>
        public DataGridViewCellCollection Cells { get; }

        /// <summary>
        /// Gets the header cell for this row.
        /// </summary>
        public DataGridViewRowHeaderCell HeaderCell { get; } = new DataGridViewRowHeaderCell ();

        /// <summary>
        /// Gets the DataGridView that contains this row.
        /// </summary>
        public DataGridView? DataGridView => owner;

        /// <summary>
        /// Gets or sets the height, in pixels, of the row.
        /// </summary>
        public int Height {
            get => height;
            set {
                if (height != value) {
                    height = Math.Max (value, 10);
                    owner?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets the index of this row in the DataGridView.
        /// </summary>
        public int Index => owner?.Rows.IndexOf (this) ?? -1;

        /// <summary>
        /// Gets or sets whether this row is selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets an object that contains data to associate with the row.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Sets the owning DataGridView.
        /// </summary>
        internal void SetOwner (DataGridView? dataGridView) => owner = dataGridView;
    }
}
