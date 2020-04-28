using System;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Base class for rendering controls.
    /// </summary>
    public abstract class Renderer
    {
        /// <summary>
        /// Initializes a new instance of the renderer class.
        /// </summary>
        public Renderer ()
        {
        }

        /// <summary>
        /// The Control this class renders.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Renders the control.
        /// </summary>
        public abstract void Render (object control, PaintEventArgs e);
    }

    /// <summary>
    /// Base generic class for rendering controls.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Renderer<T> : Renderer where T : Control
    {
        /// <inheritdoc/>
        public override Type Type => typeof (T);

        /// <inheritdoc/>
        public override void Render (object control, PaintEventArgs e)
        {
            if (!(control is T c))
                throw new ArgumentException ($"Renderer {GetType ().Name} cannot render control of type {control.GetType ().FullName}", nameof (control));

            Render (c, e);
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        protected abstract void Render (T control, PaintEventArgs e);
    }
}
