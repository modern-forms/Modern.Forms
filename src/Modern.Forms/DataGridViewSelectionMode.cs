namespace Modern.Forms
{
    /// <summary>
    /// Describes the selection behavior of a DataGridView.
    /// </summary>
    public enum DataGridViewSelectionMode
    {
        /// <summary>
        /// One or more individual cells can be selected.
        /// </summary>
        CellSelect = 0,
        /// <summary>
        /// The entire row will be selected by clicking its row's header or a cell contained in that row.
        /// </summary>
        FullRowSelect = 1,
        /// <summary>
        /// The entire column will be selected by clicking the column's header or a cell contained in that column.
        /// </summary>
        FullColumnSelect = 2,
        /// <summary>
        /// The row is selected by clicking in the row's header cell. An individual cell can be selected by clicking that cell.
        /// </summary>
        RowHeaderSelect = 3,
        /// <summary>
        /// The column is selected by clicking in the column's header cell. An individual cell can be selected by clicking that cell.
        /// </summary>
        ColumnHeaderSelect = 4
    }
}
