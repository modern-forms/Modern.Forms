using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    public class ComboBox : Control
    {
        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
            (style) => {
                style.Border.Width = 1;
                style.BackgroundColor = Theme.DarkNeutralGray;
            });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        private PopupWindow? popup;
        private readonly ListBox popup_listbox;

        public ComboBox ()
        {
            popup_listbox = new ListBox { Dock = DockStyle.Fill, ShowHover = true };
            popup_listbox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
        }

        public ListBoxItemCollection Items => popup_listbox.Items;

        protected override Padding DefaultPadding => new Padding (3, 0, 0, 0);
        protected override Size DefaultSize => new Size (121, 28);

        protected override void OnClick (MouseEventArgs e)
        {
            base.OnClick (e);
            
            if (popup?.Visible == true)
                ClosePopup ();
            else
                ShowPopup ();
        }

        protected override void OnDeselected (EventArgs e)
        {
            base.OnDeselected (e);

            ClosePopup ();
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            if (Items.SelectedItem != null)
                e.Canvas.DrawText (Items.SelectedItem.ToString (), TextArea, this, ContentAlignment.MiddleLeft, maxLines: 1);

            var button_bounds = DropDownButtonArea;
            button_bounds.Width = LogicalToDeviceUnits (12);

            ControlPaint.DrawArrowGlyph (e, button_bounds, Theme.DarkTextColor, ArrowDirection.Down);
        }

        private void ClosePopup ()
        {
            popup?.Hide ();
        }

        private Rectangle DropDownButtonArea => new Rectangle (ScaledWidth - LogicalToDeviceUnits (15), 0, LogicalToDeviceUnits (15), ScaledHeight);

        private void ListBox_SelectedIndexChanged (object sender, EventArgs e)
        {
            if (popup_listbox.SelectedIndex > -1) {
                ClosePopup ();
                Invalidate ();
            }
        }

        private void ShowPopup ()
        {
            popup ??= new PopupWindow (FindForm ()) {
                Size = new Size (Width, 102)
            };

            popup.Location = PointToScreen (new Point (1, ScaledHeight - 1));
            popup.Controls.Add (popup_listbox);

            popup.Show ();
        }

        private Rectangle TextArea {
            get {
                var area = ClientRectangle;

                area.Width -= LogicalToDeviceUnits (15);
                return area;
            }
        }

        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);

            popup?.Close ();
            popup = null;

            popup_listbox.Dispose ();
        }
    }
}
