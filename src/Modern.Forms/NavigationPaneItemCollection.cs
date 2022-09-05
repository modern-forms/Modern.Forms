using System;
using System.Collections.ObjectModel;
using SkiaSharp;

namespace Modern.Forms;

/// <summary>
/// Represents a collection of NavigationPaneItem.
/// </summary>
public class NavigationPaneItemCollection : Collection<NavigationPaneItem>
{
    private readonly NavigationPane owner;
    private int focused_index = 0;
    private int hovered_index = -1;
    private int selected_index = -1;

    internal NavigationPaneItemCollection (NavigationPane owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// Adds the NavigationPaneItem to the collection.
    /// </summary>
    public new NavigationPaneItem Add (NavigationPaneItem item)
    {
        item.Parent = owner;
        base.Add (item);

        return item;
    }

    /// <summary>
    /// Adds a new NavigationPaneItem to the collection with the specified text.
    /// </summary>
    public NavigationPaneItem Add (SKBitmap image) => Add (new NavigationPaneItem (image));

    internal int FocusedIndex {
        get => focused_index;
        set {
            if (focused_index != value) {
                focused_index = value;
                owner.Invalidate ();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index of the item the mouse is currently hovered over.
    /// </summary>
    internal int HoveredIndex {
        get => hovered_index;
        set {
            if (hovered_index != value) {
                hovered_index = value;
                owner.Invalidate ();
            }
        }
    }

    /// <inheritdoc/>
    protected override void InsertItem (int index, NavigationPaneItem item)
    {
        item.Parent = owner;

        base.InsertItem (index, item);

        if (Count == 1)
            owner.SelectedItem = item;
        else
            owner.Invalidate ();
    }

    /// <inheritdoc/>
    protected override void RemoveItem (int index)
    {
        var item = this[index];

        item.Parent = null;

        var selected_item = owner.SelectedItem;

        base.RemoveItem (index);

        if (selected_item == item && Count > 0) {
            // Need to temporarily set this to nothing in case the index doesn't change,
            // we still want to force it to be treated like a new selection.
            selected_index = -1;
            owner.SelectedIndex = Math.Max (index - 1, 0);
        } 

        if (selected_item is null && Count > 0)
            owner.SelectedIndex = 0;
        else
            owner.Invalidate ();
    }

    /// <summary>
    /// Gets or sets the index of the currently selected item.
    /// </summary>
    internal int SelectedIndex {
        get => selected_index;
        set {
            if (value < -1 || value >= Count)
                throw new ArgumentOutOfRangeException (nameof (value));

            if (selected_index != value) {
                selected_index = value;
                focused_index = value;
            }
        }
    }

    /// <inheritdoc/>
    protected override void SetItem (int index, NavigationPaneItem item)
    {
        this[index].Parent = null;
        item.Parent = owner;

        base.SetItem (index, item);
    }
}
