using System;
using Avalonia;

namespace Modern.Forms
{
    public class PopupWindow : Window
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private readonly Form parent_form;

        public PopupWindow (Form parentForm) : base (AvaloniaGlobals.WindowingInterface.CreatePopup ())
        {
            parent_form = parentForm;
            parent_form.Deactivated += (o, e) => Hide ();
        }

        protected override System.Drawing.Size DefaultSize => new System.Drawing.Size (100, 100);
    }
}
