using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms.Renderers
{
    public abstract class Renderer
    {
        public abstract Type Type { get; }
    }

    public abstract class Renderer<T> : Renderer
    {
        public override Type Type => typeof (T);

        public abstract void Render (T control, PaintEventArgs e);
    }
}
