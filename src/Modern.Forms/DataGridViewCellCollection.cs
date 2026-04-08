using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of DataGridViewCell objects in a DataGridViewRow.
    /// </summary>
    public class DataGridViewCellCollection : Collection<DataGridViewCell>
    {
        private readonly DataGridViewRow owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewCellCollection class.
        /// </summary>
        internal DataGridViewCellCollection (DataGridViewRow owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds a cell with the specified value to the collection.
        /// </summary>
        public DataGridViewCell Add (string value)
        {
            var cell = new DataGridViewCell (value);
            Add (cell);
            return cell;
        }

        /// <inheritdoc/>
        protected override void ClearItems ()
        {
            foreach (var cell in this)
                cell.SetOwner (null);

            base.ClearItems ();
            owner.DataGridView?.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, DataGridViewCell item)
        {
            item.SetOwner (owner);
            base.InsertItem (index, item);
            owner.DataGridView?.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            this[index].SetOwner (null);
            base.RemoveItem (index);
            owner.DataGridView?.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void SetItem (int index, DataGridViewCell item)
        {
            this[index].SetOwner (null);
            item.SetOwner (owner);
            base.SetItem (index, item);
            owner.DataGridView?.Invalidate ();
        }
    }
}
