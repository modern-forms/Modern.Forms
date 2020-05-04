using System;
using Avalonia;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a popup window used for things like ComboBoxes and context menus.
    /// </summary>
    public class PopupWindow : Window
    {
        private readonly Form? parent_form;

        /// <summary>
        /// Initializes a new instance of the PopupWindow class.
        /// </summary>
        public PopupWindow (Form? parentForm) : base (AvaloniaGlobals.WindowingInterface.CreatePopup ())
        {
            StartPosition = FormStartPosition.Manual;

            parent_form = parentForm;

            if (parent_form != null)
                parent_form.Deactivated += (o, e) => Hide ();
        }

        /// <inheritdoc/>
        protected override System.Drawing.Size DefaultSize => new System.Drawing.Size (100, 100);

        /// <summary>
        /// Gets the default style for all controls of this type.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle);

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control.
        /// </summary>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
