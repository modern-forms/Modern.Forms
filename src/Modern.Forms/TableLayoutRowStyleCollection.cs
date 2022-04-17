// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Represents a collection of row styles for a TableLayoutPanel.
/// </summary>
public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
{
    internal TableLayoutRowStyleCollection (IArrangedElement Owner) : base (Owner) { }
    internal TableLayoutRowStyleCollection () : base (null) { }

    internal override string PropertyName => PropertyNames.RowStyles;

    /// <summary>
    /// Add a RowStyle to the collection. 
    /// </summary>
    public int Add (RowStyle rowStyle) { return ((IList)this).Add (rowStyle); }

    /// <summary>
    /// Insert a RowStyle at the specified index.
    /// </summary>
    public void Insert (int index, RowStyle rowStyle) { ((IList)this).Insert (index, rowStyle); }

    /// <summary>
    /// Indexer for collection.
    /// </summary>
    public new RowStyle this[int index] {
        get => (RowStyle)((IList)this)[index]!;
        set => ((IList)this)[index] = value;
    }

    /// <summary>
    /// Remove the specified RowStyle from the collection.
    /// </summary>
    public void Remove (RowStyle rowStyle) => ((IList)this).Remove (rowStyle);

    /// <summary>
    /// Returns a value indicating if the specified RowStyle is contained in the collection.
    /// </summary>
    public bool Contains (RowStyle rowStyle) => ((IList)this).Contains (rowStyle);

    /// <summary>
    /// Returns the index of the specified RowStyle.
    /// </summary>
    public int IndexOf (RowStyle rowStyle) => ((IList)this).IndexOf (rowStyle);
}
