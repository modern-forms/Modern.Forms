namespace Modern.Forms
{
    /// <summary>
    /// Represents a header cell in a DataGridView control.
    /// </summary>
    public class DataGridViewHeaderCell : DataGridViewCell
    {
        /// <summary>
        /// Initializes a new instance of the DataGridViewHeaderCell class.
        /// </summary>
        public DataGridViewHeaderCell ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewHeaderCell class with the specified value.
        /// </summary>
        public DataGridViewHeaderCell (string value) : base (value)
        {
        }
    }

    /// <summary>
    /// Represents a column header cell in a DataGridView control.
    /// </summary>
    public class DataGridViewColumnHeaderCell : DataGridViewHeaderCell
    {
        /// <summary>
        /// Initializes a new instance of the DataGridViewColumnHeaderCell class.
        /// </summary>
        public DataGridViewColumnHeaderCell ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumnHeaderCell class with the specified value.
        /// </summary>
        public DataGridViewColumnHeaderCell (string value) : base (value)
        {
        }
    }

    /// <summary>
    /// Represents a row header cell in a DataGridView control.
    /// </summary>
    public class DataGridViewRowHeaderCell : DataGridViewHeaderCell
    {
        /// <summary>
        /// Initializes a new instance of the DataGridViewRowHeaderCell class.
        /// </summary>
        public DataGridViewRowHeaderCell ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataGridViewRowHeaderCell class with the specified value.
        /// </summary>
        public DataGridViewRowHeaderCell (string value) : base (value)
        {
        }
    }
}
