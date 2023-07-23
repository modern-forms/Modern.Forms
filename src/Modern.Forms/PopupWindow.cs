using System.Drawing;
using Modern.WindowKit.Controls.Primitives.PopupPositioning;
using Modern.WindowKit.Platform;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a popup window used for things like ComboBoxes and context menus.
    /// </summary>
    public class PopupWindow : WindowBase
    {
        private readonly Form parent_form;

        /// <summary>
        /// Initializes a new instance of the PopupWindow class.
        /// </summary>
        public PopupWindow (Form parentForm) : base (parentForm.window.CreatePopup ()!) // NRT - This would only be null if we were using WindowKit overlaw popups
        {
            StartPosition = FormStartPosition.Manual;

            parent_form = parentForm;
            parent_form.Deactivated += (o, e) => Hide ();
        }

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 100);

        /// <summary>
        /// Gets the default style for all controls of this type.
        /// </summary>
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.BackgroundColor = Theme.ControlMidColor;
            });

        private IPopupImpl PopupImpl => (IPopupImpl)window;

        /// <summary>
        /// Show the PopupWindow at the specified screen coordinates
        /// </summary>
        public void Show (int x, int y)
        {
            var point = parent_form.PointToClient (new Point (x, y));

            var ppp = new PopupPositionerParameters {
                AnchorRectangle = new WindowKit.Rect (point.X, point.Y, 1, 1),
                Anchor = PopupAnchor.TopLeft,
                Gravity = PopupGravity.BottomRight,
                ConstraintAdjustment = PopupPositionerConstraintAdjustment.All,
                Size = Size.ToAvaloniaSize ()
            };

            PopupImpl.PopupPositioner.Update (ppp);

            Show ();
        }

        /// <summary>
        /// Show the PopupWindow at the specified screen coordinates
        /// </summary>
        public void Show (Point screenLocation) => Show (screenLocation.X, screenLocation.Y);

        /// <summary>
        /// Show the PopupWindow at the specified coordinates relative to the provided Control
        /// </summary>
        public void Show (Control control, int x, int y)
        {
            var pos = control.GetPositionInForm ();

            Show (parent_form.PointToScreen (new Point (pos.X + x, pos.Y + y)));
        }

        /// <summary>
        /// Gets or sets the unscaled size of the window.
        /// </summary>
        public new Size Size { get; set; }

        /// <summary>
        /// Gets the ControlStyle properties for this instance of the Control.
        /// </summary>
        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);
    }
}
