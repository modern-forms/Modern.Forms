using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    public class ScrollProperties
    {
        private ScrollBar scrollbar;

        internal ScrollProperties (ScrollBar scrollbar)
        {
            this.scrollbar = scrollbar;
            ParentContol = scrollbar.Parent as ScrollableControl;
        }

        public bool Enabled {
            get => scrollbar.Enabled;
            set => scrollbar.Enabled = value;
        }

        public int LargeChange {
            get => scrollbar.LargeChange;
            set => scrollbar.LargeChange = value;
        }

        public int Maximum {
            get => scrollbar.Maximum;
            set => scrollbar.Maximum = value;
        }

        public int SmallChange {
            get => scrollbar.SmallChange;
            set => scrollbar.SmallChange = value;
        }

        public int Value {
            get => scrollbar.Value;
            set => scrollbar.Value = value;
        }

        public ScrollableControl ParentContol { get; }
    }
}
