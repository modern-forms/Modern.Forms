using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    public class LiteControlCollection : IList<LiteControl>
    {
        private readonly List<LiteControl> items = new List<LiteControl> ();
        private readonly object owner;

        internal LiteControlCollection (object owner)
        {
            this.owner = owner;
        }

        public LiteControl this[int index] {
            get => items[index];
            set {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index));

                items[index] = value;
                SetUpItem (value);
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add (LiteControl item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public LiteControl Add (string text)
        {
            var item = new LiteControl {
                Text = text
            };

            Add (item);

            return item;
        }

        public void Clear ()
        {
            while (items.Count > 0)
                RemoveAt (0);
        }

        public bool Contains (LiteControl item) => items.Contains (item);

        public void CopyTo (LiteControl[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<LiteControl> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (LiteControl item) => items.IndexOf (item);

        public void Insert (int index, LiteControl item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (LiteControl item)
        {
            if (item == null)
                throw new NullReferenceException ();

            var index = IndexOf (item);

            if (index != -1)
                RemoveAt (index);

            return index != -1;
        }

        public void RemoveAt (int index)
        {
            var item = items[index];
            var parent = item.ParentForm;

            item.ParentForm = null;
            items.RemoveAt (index);

            if (parent != null)
                parent.Invalidate ();
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();

        private void SetUpItem (LiteControl item)
        {
            if (owner is LiteForm form) {
                item.ParentForm = form;
                form.DoLayout ();
            }
        }
    }
}
