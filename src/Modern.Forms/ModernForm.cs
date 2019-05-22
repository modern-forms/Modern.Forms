using System;
using System.Drawing;
using System.Windows.Forms;

namespace Modern.Forms
{
    public class ModernForm : Form, IForm
    {
        private LiteControlAdapter adapter;

        public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
         (style) => {
             style.BackgroundColor = ModernTheme.FormBackgroundColor;
             style.Border.Color = ModernTheme.RibbonColor;
             style.Border.Width = 1;
         });

        public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

        public ModernForm ()
        {
            DoubleBuffered = true;
            SetStyle (ControlStyles.ResizeRedraw, true);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding (1);

            adapter = new LiteControlAdapter (this);
        }

        protected override Size DefaultSize => new Size (1080, 720);

        public new LiteControlCollection Controls => adapter.Controls;

        public void DoLayout ()
        {
            adapter.DoLayout ();
        }

        public override Rectangle DisplayRectangle => new Rectangle (Style.Border.Left.GetWidth (), Style.Border.Top.GetWidth (), ClientRectangle.Width - Style.Border.Right.GetWidth () - Style.Border.Left.GetWidth (), ClientRectangle.Height - Style.Border.Top.GetWidth () - Style.Border.Bottom.GetWidth ());

        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;

                cp.ClassStyle |= (int)XplatUIWin32.ClassStyle.CS_DBLCLKS;

                return cp;
            }
        }

        protected override void OnKeyDown (KeyEventArgs e)
        {
            base.OnKeyDown (e);

            adapter.RaiseKeyDown (e);
        }

        protected override void OnKeyPress (KeyPressEventArgs e)
        {
            base.OnKeyPress (e);

            adapter.RaiseKeyPress (e);
        }

        protected override void OnPaint (SKPaintEventArgs e)
        {
            base.OnPaint (e);

            adapter.RaisePaint (e);
        }

        protected override void OnResize (EventArgs e)
        {
            base.OnResize (e);

            adapter.SetBounds (DisplayRectangle.Left, DisplayRectangle.Top, DisplayRectangle.Width, DisplayRectangle.Height);
        }

        public MouseEventArgs MouseEventsForControl (MouseEventArgs e, LiteControl control)
        {
            if (control == null)
                return e;

            return new MouseEventArgs (e.Button, e.Clicks, e.Location.X - control.Left, e.Location.Y - control.Top, e.Delta);
        }

        protected override void OnMouseClick (MouseEventArgs e)
        {
            base.OnMouseClick (e);

            adapter?.RaiseClick (MouseEventsForControl (e, adapter));
        }

        protected override void OnMouseDoubleClick (MouseEventArgs e)
        {
            base.OnMouseDoubleClick (e);

            adapter?.RaiseDoubleClick (MouseEventsForControl (e, adapter));
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            base.OnMouseDown (e);

            adapter?.RaiseMouseDown (MouseEventsForControl (e, adapter));
        }

        protected override void OnMouseLeave (EventArgs e)
        {
            base.OnMouseLeave (e);

            adapter?.RaiseMouseLeave (e);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            base.OnMouseMove (e);

            adapter?.RaiseMouseMove (MouseEventsForControl (e, adapter));
        }

        protected override void OnMouseUp (MouseEventArgs e)
        {
            base.OnMouseUp (e);

            adapter?.RaiseMouseUp (MouseEventsForControl (e, adapter));
        }

        protected override void OnVisibleChanged (EventArgs e)
        {
            base.OnVisibleChanged (e);

            adapter.SetBounds (DisplayRectangle.Left, DisplayRectangle.Top, DisplayRectangle.Width, DisplayRectangle.Height);
        }

        public void BeginMoveDrag () { }
    }
}
