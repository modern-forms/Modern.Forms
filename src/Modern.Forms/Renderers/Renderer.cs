using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms.Renderers
{
    public abstract class Renderer
    {
        public abstract Type Type { get; }

        public abstract void Render (object control, PaintEventArgs e);
    }

    public abstract class Renderer<T> : Renderer where T : Control
    {
        public override Type Type => typeof (T);

        public override void Render (object control, PaintEventArgs e)
        {
            var c = control as T;

            if (c is null)
                throw new ArgumentException ($"Renderer {GetType ().Name} cannot render control of type {control.GetType ().FullName}", nameof (control));

            Render ((T)control, e);
        }

        protected abstract void Render (T control, PaintEventArgs e);
    }
}
