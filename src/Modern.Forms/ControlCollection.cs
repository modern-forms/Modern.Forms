using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    public class ControlCollection : IList<Control>
    {
        private readonly List<Control> items = new List<Control> ();
        private readonly Control parent;

        internal ControlCollection (Control parent)
        {
            this.parent = parent;
        }

        public Control this[int index] {
            get => items[index];
            set {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index));

                items[index] = value;
                SetUpItem (value);
                parent.DoLayout ();
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (Control item)
        {
            items.Add (item);
            SetUpItem (item);
            parent.DoLayout ();
        }

        public void Clear ()
        {
            while (items.Count > 0)
                RemoveAt (0);
        }

        public bool Contains (Control item) => items.Contains (item);

        public void CopyTo (Control[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<Control> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (Control item) => items.IndexOf (item);

        public void Insert (int index, Control item)
        {
            items.Insert (index, item);
            SetUpItem (item);
            parent.DoLayout ();
        }

        public bool Remove (Control item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            parent.DoLayout ();

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            var item = items[index];

            item.SetParentInternal (null);
            items.RemoveAt (index);

            parent.DoLayout ();
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();

        private void SetUpItem (Control item)
        {
            item.SetParentInternal (parent);
        }
    }
}
