// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Base class for collections of TableLayout styles.
/// </summary>
public abstract class TableLayoutStyleCollection : IList
{
    private readonly ArrayList _innerList = new ArrayList ();

    internal TableLayoutStyleCollection (IArrangedElement? owner)
    {
        Owner = owner;
    }

    internal IArrangedElement? Owner { get; private set; }

    internal virtual string? PropertyName => null;

    int IList.Add (object? style)
    {
        ArgumentNullException.ThrowIfNull (style);

        EnsureNotOwned ((TableLayoutStyle)style);
        ((TableLayoutStyle)style).Owner = Owner;
        var index = _innerList.Add (style);
        PerformLayoutIfOwned ();
        return index;
    }

    /// <summary>
    /// Add a style to the collection. 
    /// </summary>
    public int Add (TableLayoutStyle style) => ((IList)this).Add (style);

    void IList.Insert (int index, object? style)
    {
        ArgumentNullException.ThrowIfNull (style);

        EnsureNotOwned ((TableLayoutStyle)style);
        ((TableLayoutStyle)style).Owner = Owner;
        _innerList.Insert (index, style);
        PerformLayoutIfOwned ();
    }

    object? IList.this[int index] {
        get => _innerList[index];
        set {
            ArgumentNullException.ThrowIfNull (value);

            var style = (TableLayoutStyle)value;
            EnsureNotOwned (style);
            style.Owner = Owner;
            _innerList[index] = style;
            PerformLayoutIfOwned ();
        }
    }

    /// <summary>
    /// Indexer for collection.
    /// </summary>
    public TableLayoutStyle this[int index] {
        get => (TableLayoutStyle)((IList)this)[index]!;
        set => ((IList)this)[index] = value;
    }

    void IList.Remove (object? style)
    {
        if (style is null)
            return;

        ((TableLayoutStyle)style).Owner = null;
        _innerList.Remove (style);
        PerformLayoutIfOwned ();
    }

    /// <summary>
    /// Clears all styles from the collection.
    /// </summary>
    public void Clear ()
    {
        foreach (TableLayoutStyle style in _innerList)
            style.Owner = null;

        _innerList.Clear ();
        PerformLayoutIfOwned ();
    }

    /// <summary>
    /// Removes the style at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt (int index)
    {
        var style = (TableLayoutStyle)_innerList[index]!;
        style.Owner = null;
        _innerList.RemoveAt (index);
        PerformLayoutIfOwned ();
    }

    bool IList.Contains (object? style) => _innerList.Contains (style);

    int IList.IndexOf (object? style) => _innerList.IndexOf (style);

    bool IList.IsFixedSize => _innerList.IsFixedSize;

    bool IList.IsReadOnly => _innerList.IsReadOnly;

    void ICollection.CopyTo (Array array, int startIndex) => _innerList.CopyTo (array, startIndex);

    /// <summary>
    /// Get the count of styles in the collection.
    /// </summary>
    public int Count => _innerList.Count;

    bool ICollection.IsSynchronized => _innerList.IsSynchronized;

    object ICollection.SyncRoot => _innerList.SyncRoot;

    IEnumerator IEnumerable.GetEnumerator () => _innerList.GetEnumerator ();

    private static void EnsureNotOwned (TableLayoutStyle style)
    {
        if (style.Owner is not null)
            throw new ArgumentException (string.Format (SR.OnlyOneControl, style.GetType ().Name), nameof (style));
    }

    internal void EnsureOwnership (IArrangedElement owner)
    {
        Owner = owner;

        for (var i = 0; i < Count; i++)
            this[i].Owner = owner;
    }

    private void PerformLayoutIfOwned ()
    {
        if (Owner is not null)
            LayoutTransaction.DoLayout (Owner, Owner, PropertyName);
    }
}
