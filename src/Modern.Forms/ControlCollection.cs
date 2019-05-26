using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modern.Forms
{
    public class ControlCollection : IList<Control>
    {
        private readonly List<Control> controls = new List<Control> ();
        private readonly List<Control> implicit_controls = new List<Control> ();

        private readonly Control parent;

        internal ControlCollection (Control parent)
        {
            this.parent = parent;
        }

        public Control this[int index] {
            get => controls[index];
            set {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index));

                controls[index] = value;
                SetUpItem (value);
                parent.PerformLayout ();
            }
        }

        public int Count => controls.Count;

        public bool IsReadOnly => false;

        public void Add (Control item)
        {
            controls.Add (item);
            SetUpItem (item);
            parent.PerformLayout ();
        }

        internal void AddImplicitControl (Control item)
        {
            item.ImplicitControl = true;
            implicit_controls.Add (item);
            SetUpItem (item);
            parent.PerformLayout ();
        }

        public void AddRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                Add (c);

            parent.ResumeLayout (true);
        }

        internal void AddImplicitControlRange (params Control[] controls)
        {
            parent.SuspendLayout ();

            foreach (var c in controls)
                AddImplicitControl (c);

            parent.ResumeLayout (true);
        }

        public void Clear ()
        {
            while (controls.Count > 0)
                RemoveAt (0);
        }

        public bool Contains (Control item) => controls.Contains (item);

        public void CopyTo (Control[] array, int arrayIndex) => controls.CopyTo (array, arrayIndex);

        public IEnumerator<Control> GetEnumerator () => controls.GetEnumerator ();

        public int IndexOf (Control item) => controls.IndexOf (item);

        public void Insert (int index, Control item)
        {
            controls.Insert (index, item);
            SetUpItem (item);
            parent.PerformLayout ();
        }

        public bool Remove (Control item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            parent.PerformLayout ();

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            var item = controls[index];

            item.SetParentInternal (null);
            controls.RemoveAt (index);

            parent.PerformLayout ();
        }

        IEnumerator IEnumerable.GetEnumerator () => controls.GetEnumerator ();

        internal IEnumerable<Control> GetAllControls () => controls.Concat (implicit_controls);

        private void SetUpItem (Control item)
        {
            item.SetParentInternal (parent);
        }
    }
}
