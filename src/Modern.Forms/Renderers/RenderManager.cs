using System;
using System.Collections.Generic;
using System.Linq;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class to manage rendering.
    /// </summary>
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
            SetRenderer<ListView> (new ListViewRenderer ());
            SetRenderer<Menu> (new MenuRenderer ());
            SetRenderer<MenuDropDown> (new MenuDropDownRenderer ());
            SetRenderer<NavigationPane> (new NavigationPaneRenderer ());
            SetRenderer<Panel> (new PanelRenderer ());
            SetRenderer<PictureBox> (new PictureBoxRenderer ());
            SetRenderer<ProgressBar> (new ProgressBarRenderer ());
            SetRenderer<RadioButton> (new RadioButtonRenderer ());
            SetRenderer<Ribbon> (new RibbonRenderer ());
            SetRenderer<ScrollableControl> (new ScrollableControlRenderer ());
            SetRenderer<ScrollBar> (new ScrollBarRenderer ());
            SetRenderer<SplitContainer> (new SplitContainerRenderer ());
            SetRenderer<Splitter> (new SplitterRenderer ());
            SetRenderer<StatusBar> (new StatusBarRenderer ());
            SetRenderer<TabControl> (new TabControlRenderer ());
            SetRenderer<TabStrip> (new TabStripRenderer ());
            SetRenderer<TextBox> (new TextBoxRenderer ());
            SetRenderer<ToolBar> (new ToolBarRenderer ());
            SetRenderer<TreeView> (new TreeViewRenderer ());
        }

        /// <summary>
        /// Gets a renderer of the requested type.
        /// </summary>
        public static T? GetRenderer<T> () where T : Renderer
        {
            return renderers.Values.OfType<T> ().FirstOrDefault ();
        }

        /// <summary>
        /// Gets a renderer for the requested control.
        /// </summary>
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

        /// <summary>
        /// Renders the specified control.
        /// </summary>
        public static void Render<T> (T control, PaintEventArgs e) where T : Control
        {
            var renderer = GetRenderer<Renderer> (control);
            renderer?.Render (control, e);
        }

        /// <summary>
        /// Registers a renderer for a control class.
        /// </summary>
        public static void SetRenderer<T> (Renderer renderer) where T : Control
        {
            if (renderer.Type != typeof (T))
                throw new InvalidOperationException ($"Invalid renderer for type {typeof (T).FullName}");

            renderers[typeof (T)] = renderer;
        }
    }
}
