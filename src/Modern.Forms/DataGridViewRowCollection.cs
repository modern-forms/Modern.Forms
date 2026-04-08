using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of DataGridViewRow objects in a DataGridView control.
    /// </summary>
    public class DataGridViewRowCollection : Collection<DataGridViewRow>
    {
        private readonly DataGridView owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewRowCollection class.
        /// </summary>
        internal DataGridViewRowCollection (DataGridView owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds a new row with the specified cell values.
        /// </summary>
        public DataGridViewRow Add (params string[] values)
        {
            var row = new DataGridViewRow ();

            foreach (var value in values)
                row.Cells.Add (value);

            Add (row);
            return row;
        }

        /// <inheritdoc/>
        protected override void ClearItems ()
        {
            foreach (var row in this)
                row.SetOwner (null);

            base.ClearItems ();
            owner.OnRowsChanged ();
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, DataGridViewRow item)
        {
            item.SetOwner (owner);
            base.InsertItem (index, item);
            owner.OnRowsChanged ();
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            this[index].SetOwner (null);
            base.RemoveItem (index);
            owner.OnRowsChanged ();
        }

        /// <summary>
        /// Replaces all rows with the specified list, raising a single change notification.
        /// </summary>
        internal void ReplaceAll (List<DataGridViewRow> rows)
        {
            // Clear without per-item notifications
            foreach (var row in this)
                row.SetOwner (null);

            Items.Clear ();

            foreach (var row in rows) {
                row.SetOwner (owner);
                Items.Add (row);
            }

            owner.OnRowsChanged ();
        }

        /// <inheritdoc/>
        protected override void SetItem (int index, DataGridViewRow item)
        {
            this[index].SetOwner (null);
            item.SetOwner (owner);
            base.SetItem (index, item);
            owner.OnRowsChanged ();
        }
    }
}
