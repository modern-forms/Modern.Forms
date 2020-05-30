using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a collection of Controls.
    /// </summary>
    public class ControlCollection : Collection<Control>
    {
        private readonly List<Control> implicit_controls = new List<Control> ();
        private readonly Control parent;

        internal ControlCollection (Control parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Adds the Control to the collection.
        /// </summary>
        public T Add<T> (T item) where T : Control
        {
            base.Add (item);
            return item;
        }

        /// <summary>
        /// Efficiently add multiple Controls to the collection.
        /// </summary>
        public void AddRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                Add (c);

            parent.ResumeLayout (true);
        }

        /// <inheritdoc/>
        protected override void InsertItem (int index, Control item)
        {
            base.InsertItem (index, item);

            item.SetParentInternal (parent);
            parent.PerformLayout ();
        }

        /// <inheritdoc/>
        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.SetParentInternal (null);
            parent.PerformLayout ();
        }

        /// <summary>
        /// Moves a child control in this collection to a new location.
        /// </summary>
        public void SetChildIndex (Control item, int newIndex)
        {
            var index = IndexOf (item);

            if (index == -1)
                throw new ArgumentException ("'item' does not belong to this collection");

            base.RemoveItem (index);
            base.InsertItem (newIndex, item);

            parent.PerformLayout ();
        }

        /// <inheritdoc/>
        protected override void SetItem (int index, Control item)
        {
            var old_item = this.ElementAtOrDefault (index);
            old_item?.SetParentInternal (null);

            base.SetItem (index, item);

            item.SetParentInternal (parent);
            parent.PerformLayout ();
        }

        internal T AddImplicitControl<T> (T item) where T : Control
        {
            item.ImplicitControl = true;
            implicit_controls.Add (item);
            item.SetParentInternal (parent);
            parent.PerformLayout ();
            return item;
        }

        internal void AddImplicitControlRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                AddImplicitControl (c);

            parent.ResumeLayout (true);
        }

        internal IEnumerable<Control> GetAllControls (bool includeImplicit = true)
        {
            if (includeImplicit)
                return this.Concat (implicit_controls);

            return this;
        }
    }
}
