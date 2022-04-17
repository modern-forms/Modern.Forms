// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace Modern.Forms;

/// <summary>
///  Represents a collection of controls on the TableLayoutPanel.
/// </summary>
[ListBindable (false)]
public class TableLayoutControlCollection : Control.ControlCollection
{
    /// <summary>
    /// Initializes a new instance of the TableLayoutControlCollection class.
    /// </summary>
    public TableLayoutControlCollection (TableLayoutPanel container) : base (container.OrThrowIfNull ())
    {
        Container = container;
    }

    /// <summary>
    /// The container of this TableLayoutControlCollection
    /// </summary>
    public TableLayoutPanel Container { get; }

    /// <summary>
    ///  Add control to cell (x, y) on the table. The control becomes absolutely positioned if neither x nor y is equal to -1
    /// </summary>
    public virtual void Add (Control control, int column, int row)
    {
        base.Add (control);

        Container.SetColumn (control, column);
        Container.SetRow (control, row);
    }
}
