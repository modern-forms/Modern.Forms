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
            SetRenderer<Menu> (new MenuRenderer ());
            SetRenderer<MenuDropDown> (new MenuDropDownRenderer ());
            SetRenderer<Panel> (new PanelRenderer ());
            SetRenderer<PictureBox> (new PictureBoxRenderer ());
            SetRenderer<ProgressBar> (new ProgressBarRenderer ());
            SetRenderer<RadioButton> (new RadioButtonRenderer ());
            SetRenderer<ScrollableControl> (new ScrollableControlRenderer ());
            SetRenderer<ScrollBar> (new ScrollBarRenderer ());
            SetRenderer<SplitContainer> (new SplitContainerRenderer ());
            SetRenderer<Splitter> (new SplitterRenderer ());
            SetRenderer<TabControl> (new TabControlRenderer ());
            SetRenderer<TabStrip> (new TabStripRenderer ());
            SetRenderer<TextBox> (new TextBoxRenderer ());
            SetRenderer<ToolBar> (new ToolBarRenderer ());
            SetRenderer<TreeView> (new TreeViewRenderer ());
        }

        public static T? GetRenderer<T> () where T : Renderer
        {
            return renderers.Values.OfType<T> ().FirstOrDefault ();
        }

        public static T? GetRenderer<T> (Control control) where T : Renderer
        {
            var type = (Type?)control.GetType ();

            while (type != null && type != typeof (object)) {
                if (renderers.TryGetValue (type, out var renderer)) {
                    return renderer as T;
                }

                type = type.BaseType;
            }

            throw new InvalidOperationException ($"No renderer found for type {typeof (T).FullName}");
        }

        public static void Render<T> (T control, PaintEventArgs e) where T : Control
        {
            var renderer = GetRenderer<Renderer> (control);
            renderer?.Render (control, e);
        }

        public static void SetRenderer<T> (Renderer renderer)
        {
            if (renderer.Type != typeof (T))
                throw new InvalidOperationException ($"Invalid renderer for type {typeof (T).FullName}");

            renderers[typeof (T)] = renderer;
        }
    }
}
