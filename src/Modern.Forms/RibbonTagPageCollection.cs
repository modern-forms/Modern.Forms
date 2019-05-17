using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Modern.Forms
{
    public class RibbonTagPageCollection : IList<RibbonTabPage>
    {
        private readonly List<RibbonTabPage> items = new List<RibbonTabPage> ();
        private readonly Ribbon owner;

        internal RibbonTagPageCollection (Ribbon owner)
        {
            this.owner = owner;
        }

        public RibbonTabPage this[int index] {
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

        public void Add (RibbonTabPage item)
        {
            items.Add (item);
            SetUpItem (item);
        }

        public RibbonTabPage Add (string text)
        {
            var item = new RibbonTabPage {
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

        public bool Contains (RibbonTabPage item) => items.Contains (item);

        public void CopyTo (RibbonTabPage[] array, int arrayIndex) => items.CopyTo (array, arrayIndex);

        public IEnumerator<RibbonTabPage> GetEnumerator () => items.GetEnumerator ();

        public int IndexOf (RibbonTabPage item) => items.IndexOf (item);

        public void Insert (int index, RibbonTabPage item)
        {
            items.Insert (index, item);
            SetUpItem (item);
        }

        public bool Remove (RibbonTabPage item)
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
            items.RemoveAt (index);
        }

        IEnumerator IEnumerable.GetEnumerator () => items.GetEnumerator ();

        private void SetUpItem (RibbonTabPage item)
        {
            item.Owner = owner;
            owner.TabStrip.Tabs.Add (new TabStripItem { Text = item.Text });

            if (owner.TabStrip.Tabs.Count == 1)
                owner.TabStrip.SelectedTab = owner.TabStrip.Tabs[0];

            owner.Invalidate ();
        }
    }
}
