using System;
using System.Collections.Generic;

namespace Modern.Forms.Renderers
{
    public static class RenderManager
    {
        private static Dictionary<Type, Renderer> renderers = new Dictionary<Type, Renderer> ();

        static RenderManager ()
        {
            SetRenderer<CheckBox> (new CheckBoxRenderer ());
        }

        public static void Render<T> (T control, PaintEventArgs e)
        {
            if (!renderers.TryGetValue (typeof (T), out var renderer))
                throw new InvalidOperationException ($"No renderer found for type {typeof (T).FullName}");

            var render = renderer as Renderer<T>;

            render?.Render (control, e);
        }

        public static void SetRenderer<T> (Renderer renderer)
        {
            if (renderer.Type != typeof (T))
                throw new InvalidOperationException ($"Invalid renderer for type {typeof (T).FullName}");

            renderers[typeof (T)] = renderer;
        }
    }
}
