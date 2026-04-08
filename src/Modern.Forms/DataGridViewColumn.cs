using System.Drawing;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a column in a DataGridView control.
    /// </summary>
    public class DataGridViewColumn
    {
        private string header_text = string.Empty;
        private int width = 100;
        private DataGridView? owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumn class.
        /// </summary>
        public DataGridViewColumn ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumn class with the specified header text.
        /// </summary>
        public DataGridViewColumn (string headerText)
        {
            header_text = headerText;
        }

        /// <summary>
        /// Gets the bounding rectangle of the column header.
        /// </summary>
        internal Rectangle HeaderBounds { get; set; }

        /// <summary>
        /// Gets the header cell for this column.
        /// </summary>
        public DataGridViewColumnHeaderCell HeaderCell { get; } = new DataGridViewColumnHeaderCell ();

        /// <summary>
        /// Gets or sets the header text for this column.
        /// </summary>
        public string HeaderText {
            get => header_text;
            set {
                if (header_text != value) {
                    header_text = value;
                    owner?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Gets the index of this column in the DataGridView.
        /// </summary>
        public int Index => owner?.Columns.IndexOf (this) ?? -1;

        /// <summary>
        /// Gets or sets the minimum width, in pixels, of the column.
        /// </summary>
        public int MinimumWidth { get; set; } = 30;

        /// <summary>
        /// Gets the DataGridView control that contains this column.
        /// </summary>
        public DataGridView? DataGridView => owner;

        /// <summary>
        /// Gets or sets a value indicating whether the column is sortable.
        /// </summary>
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Gets or sets the sort order for this column.
        /// </summary>
        public SortOrder SortOrder { get; set; } = SortOrder.None;

        /// <summary>
        /// Gets or sets an object that contains data to associate with the column.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets whether the column is visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the width, in pixels, of the column.
        /// </summary>
        public int Width {
            get => width;
            set {
                value = Math.Max (value, MinimumWidth);

                if (width != value) {
                    width = value;
                    owner?.Invalidate ();
                }
            }
        }

        /// <summary>
        /// Sets the owning DataGridView.
        /// </summary>
        internal void SetOwner (DataGridView? dataGridView) => owner = dataGridView;
    }

    /// <summary>
    /// Specifies the sort order for a column.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// No sort order.
        /// </summary>
        None,
        /// <summary>
        /// Items are sorted in ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Items are sorted in descending order.
        /// </summary>
        Descending
    }
}
