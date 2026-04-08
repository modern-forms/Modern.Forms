using System.Collections.ObjectModel;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of DataGridViewColumn objects in a DataGridView control.
    /// </summary>
    public class DataGridViewColumnCollection : Collection<DataGridViewColumn>
    {
        private readonly DataGridView owner;

        /// <summary>
        /// Initializes a new instance of the DataGridViewColumnCollection class.
        /// </summary>
        internal DataGridViewColumnCollection (DataGridView owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds a column with the specified header text to the collection.
        /// </summary>
        public DataGridViewColumn Add (string headerText)
        {
            var column = new DataGridViewColumn (headerText);
            Add (column);
            return column;
        }

        /// <summary>
        /// Adds a column with the specified header text and width to the collection.
        /// </summary>
        public DataGridViewColumn Add (string headerText, int width)
        {
            var column = new DataGridViewColumn (headerText) { Width = width };
            Add (column);
            return column;
        }

        /// <inheritdoc/>
        protected override void ClearItems ()
        {
            foreach (var column in this)
                column.SetOwner (null);

            base.ClearItems ();
            owner.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, DataGridViewColumn item)
        {
            item.SetOwner (owner);
            base.InsertItem (index, item);
            owner.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            this[index].SetOwner (null);
            base.RemoveItem (index);
            owner.Invalidate ();
        }

        /// <inheritdoc/>
        protected override void SetItem (int index, DataGridViewColumn item)
        {
            this[index].SetOwner (null);
            item.SetOwner (owner);
            base.SetItem (index, item);
            owner.Invalidate ();
        }
    }
}
