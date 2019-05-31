using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Modern.Forms
{
    public class ControlCollection : Collection<Control>
    {
        private readonly List<Control> implicit_controls = new List<Control> ();
        private readonly Control parent;

        internal ControlCollection (Control parent)
        {
            this.parent = parent;
        }

        public new Control Add (Control item)
        {
            base.Add (item);
            return item;
        }

        public void AddRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                Add (c);

            parent.ResumeLayout (true);
        }

        protected override void InsertItem (int index, Control item)
        {
            base.InsertItem (index, item);

            item.SetParentInternal (parent);
            parent.PerformLayout ();
        }

        protected override void RemoveItem (int index)
        {
            var item = this[index];

            base.RemoveItem (index);

            item.SetParentInternal (null);
            parent.PerformLayout ();
        }

        protected override void SetItem (int index, Control item)
        {
            var old_item = this.ElementAtOrDefault (index);
            old_item?.SetParentInternal (null);

            base.SetItem (index, item);

            item.SetParentInternal (parent);
            parent.PerformLayout ();
        }

        internal void AddImplicitControl (Control item)
        {
            item.ImplicitControl = true;
            implicit_controls.Add (item);
            item.SetParentInternal (parent);
            parent.PerformLayout ();
        }

        internal void AddImplicitControlRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                AddImplicitControl (c);

            parent.ResumeLayout (true);
        }

        internal IEnumerable<Control> GetAllControls () => this.Concat (implicit_controls);
    }
}
