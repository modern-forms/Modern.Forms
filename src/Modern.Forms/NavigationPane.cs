using System;
using System.Drawing;
using System.Linq;
using Modern.Forms.Renderers;

namespace Modern.Forms;

/// <summary>
/// Represents a NavigationPane control.
/// </summary>
public class NavigationPane : Control
{
    /// <summary>
    /// Initializes a new instance of the NavigationPane class.
    /// </summary>
    public NavigationPane ()
    {
        Items = new NavigationPaneItemCollection (this);
        Dock = DockStyle.Left;
    }

    /// <inheritdoc/>
    protected override Size DefaultSize => new Size (49, 600);

    /// <inheritdoc/>
    public new static ControlStyle DefaultStyle = new ControlStyle (Control.DefaultStyle,
        (style) => {
            style.BackgroundColor = Theme.NeutralGray;
            style.Border.Right.Width = 1;
            style.Border.Right.Color = new SkiaSharp.SKColor (237, 235, 233);
        });

    // Returns the item at the specified location.
    private NavigationPaneItem? GetItemAtLocation (Point location) => Items.FirstOrDefault (tp => tp.Bounds.Contains (location));

    /// <summary>
    /// Gets the collection of items contained by this NavigationPane.
    /// </summary>
    public NavigationPaneItemCollection Items { get; }

    // Layout the items.
    private void LayoutItems ()
    {
        StackLayoutEngine.VerticalExpand.Layout (ClientRectangle, Items.Cast<ILayoutable> ());
    }

    /// <inheritdoc/>
    protected override void OnClick (MouseEventArgs e)
    {
        base.OnClick (e);

        var clicked_item = GetItemAtLocation (e.Location);

        // This does a null check
        if (clicked_item?.Enabled == true)
            SelectedItem = clicked_item;
    }

    /// <inheritdoc/>
    protected override void OnMouseLeave (EventArgs e)
    {
        base.OnMouseLeave (e);

        Items.HoveredIndex = -1;
    }

    /// <inheritdoc/>
    protected override void OnMouseMove (MouseEventArgs e)
    {
        base.OnMouseMove (e);

        var hover_item = GetItemAtLocation (e.Location);
        Items.HoveredIndex = hover_item is null ? -1 : Items.IndexOf (hover_item);
    }

    /// <inheritdoc/>
    protected override void OnPaint (PaintEventArgs e)
    {
        base.OnPaint (e);

        // TODO: This should only be done when items are added or removed, or the NavigationPane is resized.
        LayoutItems ();

        RenderManager.Render (this, e);
    }

    /// <summary>
    /// Raises the SelectedItemChanged event.
    /// </summary>
    protected virtual void OnSelectedItemChanged (EventArgs e) => SelectedItemChanged?.Invoke (this, e);

    /// <summary>
    /// Raised when the selected item changes.
    /// </summary>
    public event EventHandler? SelectedItemChanged;

    /// <inheritdoc/>
    public override ControlStyle Style { get; } = new ControlStyle (DefaultStyle);

    /// <summary>
    /// Gets or sets the index of the currently selected item.
    /// </summary>
    public int SelectedIndex {
        get => Items.SelectedIndex;
        set {
            if (Items.SelectedIndex != value) {
                Items.SelectedIndex = value;
                OnSelectedItemChanged (EventArgs.Empty);

                Invalidate ();
            }
        }
    }

    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public NavigationPaneItem? SelectedItem {
        get => SelectedIndex >= 0 ? Items[SelectedIndex] : null;
        set {
            if (value is null) {
                SelectedIndex = -1;
                return;
            }

            var index = Items.IndexOf (value);

            if (index == -1)
                throw new ArgumentException ("Item is not part of this list");

            SelectedIndex = index;
        }
    }
}
