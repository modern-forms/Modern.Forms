using System;
using System.Collections.Generic;
using System.Linq;

namespace Modern.Forms.Renderers
{
    public static class RenderManager
    {
        private static readonly Dictionary<Type, Renderer> renderers = new Dictionary<Type, Renderer> ();

        static RenderManager ()
        {
            SetRenderer<Button> (new ButtonRenderer ());
            SetRenderer<CheckBox> (new CheckBoxRenderer ());
            SetRenderer<ComboBox> (new ComboBoxRenderer ());
            SetRenderer<FormTitleBar> (new FormTitleBarRenderer ());
            SetRenderer<Label> (new LabelRenderer ());
            SetRenderer<ListBox> (new ListBoxRenderer ());
            SetRenderer<Panel> (new PanelRenderer ());
            SetRenderer<PictureBox> (new PictureBoxRenderer ());
            SetRenderer<ProgressBar> (new ProgressBarRenderer ());
            SetRenderer<RadioButton> (new RadioButtonRenderer ());
            SetRenderer<ScrollableControl> (new ScrollableControlRenderer ());
        }

        public static T? GetRenderer<T> () where T : Renderer
        {
            return renderers.Values.OfType<T> ().FirstOrDefault ();
        }

        public static void Render<T> (T control, PaintEventArgs e) where T : Control
        {
            var type = control.GetType ();

            while (type != null && type != typeof (object)) {
                if (renderers.TryGetValue (type, out var renderer)) {
                    var render = renderer as Renderer;
                    render?.Render (control, e);
                    return;
                }

                type = type.BaseType;
            }

            throw new InvalidOperationException ($"No renderer found for type {typeof (T).FullName}");
        }

        public static void SetRenderer<T> (Renderer renderer)
        {
            if (renderer.Type != typeof (T))
                throw new InvalidOperationException ($"Invalid renderer for type {typeof (T).FullName}");

            renderers[typeof (T)] = renderer;
        }
    }
}
