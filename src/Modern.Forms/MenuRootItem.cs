namespace Modern.Forms
{
    internal sealed class MenuRootItem : MenuItem
    {
        public Control Control { get; }

        public MenuRootItem (Control control) => Control = control;
    }
}
