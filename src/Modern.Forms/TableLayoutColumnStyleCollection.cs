// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Represents a collection of column styles for a TableLayoutPanel.
/// </summary>
public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
{
    internal TableLayoutColumnStyleCollection (IArrangedElement Owner) : base (Owner) { }
    internal TableLayoutColumnStyleCollection () : base (null) { }

    internal override string PropertyName => PropertyNames.ColumnStyles;

    /// <summary>
    /// Add a ColumnStyle to the collection. 
    /// </summary>
    public int Add (ColumnStyle columnStyle) => ((IList)this).Add (columnStyle);

    /// <summary>
    /// Insert a ColumnStyle at the specified index.
    /// </summary>
    public void Insert (int index, ColumnStyle columnStyle) => ((IList)this).Insert (index, columnStyle);

    /// <summary>
    /// Indexer for collection.
    /// </summary>
    public new ColumnStyle this[int index] {
        get => (ColumnStyle)((IList)this)[index]!;
        set => ((IList)this)[index] = value;
    }

    /// <summary>
    /// Remove the specified ColumnStyle from the collection.
    /// </summary>
    public void Remove (ColumnStyle columnStyle) => ((IList)this).Remove (columnStyle);

    /// <summary>
    /// Returns a value indicating if the specified ColumnStyle is contained in the collection.
    /// </summary>
    public bool Contains (ColumnStyle columnStyle) => ((IList)this).Contains (columnStyle);

    /// <summary>
    /// Returns the index of the specified ColumnStyle.
    /// </summary>
    public int IndexOf (ColumnStyle columnStyle) => ((IList)this).IndexOf (columnStyle);
}
