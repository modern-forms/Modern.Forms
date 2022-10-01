// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
///  This is a wrapper class to expose interesting properties of TableLayout
/// </summary>
#if DESIGN_TIME
[TypeConverter (typeof (TableLayoutSettingsTypeConverter))]
#endif
[Serializable]  // This class participates in resx serialization.
public sealed partial class TableLayoutSettings : LayoutSettings
#if DESIGN_TIME
    , ISerializable
#endif
{
    private static readonly int[] borderStyleToOffset =
    {
        /*None = */ 0,
        /*Single = */ 1,
        /*Inset = */ 2,
        /*InsetDouble = */ 3,
        /*Outset = */ 2,
        /*OutsetDouble = */ 3,
        /*OutsetPartial = */ 3
    };
    private TableLayoutPanelCellBorderStyle _borderStyle;
    private TableLayoutSettingsStub? _stub;

    // used by TableLayoutSettingsTypeConverter
    internal TableLayoutSettings () : base (null!)
    {
        _stub = new TableLayoutSettingsStub ();
    }

    internal TableLayoutSettings (IArrangedElement owner) : base (owner) { }

#if DESIGN_TIME
    private TableLayoutSettings (SerializationInfo serializationInfo, StreamingContext context) : this ()
    {
        var converter = TypeDescriptor.GetConverter (this);
        var stringVal = serializationInfo.GetString ("SerializedString");

        if (!string.IsNullOrEmpty (stringVal)) {
            if (converter.ConvertFromInvariantString (stringVal) is TableLayoutSettings tls) {
                ApplySettings (tls);
            }
        }
    }
#endif

    /// <inheritdoc/>
    public override LayoutEngine LayoutEngine => TableLayout.Instance;

    private TableLayout TableLayout => (TableLayout)LayoutEngine;

    /// <summary> internal as this is a TableLayoutPanel feature only </summary>
    [DefaultValue (TableLayoutPanelCellBorderStyle.None)]
    internal TableLayoutPanelCellBorderStyle CellBorderStyle {
        get => _borderStyle;
        set {
            //valid values are 0x0 to 0x6
            SourceGenerated.EnumValidator.Validate (value);
            _borderStyle = value;
            //set the CellBorderWidth according to the current CellBorderStyle.
            var containerInfo = TableLayout.GetContainerInfo (Owner);
            containerInfo.CellBorderWidth = borderStyleToOffset[(int)value];
            LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.CellBorderStyle);
            Debug.Assert (CellBorderStyle == value, "CellBorderStyle should be the same as we set");
        }
    }

    [DefaultValue (0)]
    internal int CellBorderWidth => TableLayout.GetContainerInfo (Owner).CellBorderWidth;

    /// <summary>
    ///  This sets the maximum number of columns allowed on this table instead of allocating
    ///  actual spaces for these columns. So it is OK to set ColumnCount to Int32.MaxValue without
    ///  causing out of memory exception
    /// </summary>
    [DefaultValue (0)]
    public int ColumnCount {
        get {
            var containerInfo = TableLayout.GetContainerInfo (Owner);
            return containerInfo.MaxColumns;
        }
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException (nameof (value), value, string.Format (SR.InvalidLowBoundArgumentEx, nameof (ColumnCount), value, 0));

            var containerInfo = TableLayout.GetContainerInfo (Owner);
            containerInfo.MaxColumns = value;
            LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.Columns);
            Debug.Assert (ColumnCount == value, "the max columns should equal to the value we set it to");
        }
    }

    /// <summary>
    ///  This sets the maximum number of rows allowed on this table instead of allocating
    ///  actual spaces for these rows. So it is OK to set RowCount to Int32.MaxValue without
    ///  causing out of memory exception
    /// </summary>
    [DefaultValue (0)]
    public int RowCount {
        get {
            var containerInfo = TableLayout.GetContainerInfo (Owner);
            return containerInfo.MaxRows;
        }
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException (nameof (value), value, string.Format (SR.InvalidLowBoundArgumentEx, nameof (RowCount), value, 0));

            var containerInfo = TableLayout.GetContainerInfo (Owner);
            containerInfo.MaxRows = value;
            LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.Rows);
            Debug.Assert (RowCount == value, "the max rows should equal to the value we set it to");
        }
    }

    /// <summary>
    /// The collection of RowStyles for the table layout.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    public TableLayoutRowStyleCollection RowStyles {
        get {
            if (IsStub) {
                return _stub!.RowStyles;
            } else {
                var containerInfo = TableLayout.GetContainerInfo (Owner);
                return containerInfo.RowStyles;
            }
        }
    }

    /// <summary>
    /// The collection of ColumnStyles for the table layout.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    public TableLayoutColumnStyleCollection ColumnStyles {
        get {
            if (IsStub) {
                return _stub!.ColumnStyles;
            } else {
                var containerInfo = TableLayout.GetContainerInfo (Owner);
                return containerInfo.ColumnStyles;
            }
        }
    }

    /// <summary>
    ///  Specifies if a TableLayoutPanel will gain additional rows or columns once its existing cells
    ///  become full.  If the value is 'FixedSize' then the TableLayoutPanel will throw an exception
    ///  when the TableLayoutPanel is over-filled.
    /// </summary>
    [DefaultValue (TableLayoutPanelGrowStyle.AddRows)]
    public TableLayoutPanelGrowStyle GrowStyle {
        get => TableLayout.GetContainerInfo (Owner).GrowStyle;
        set {
            //valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate (value);

            var containerInfo = TableLayout.GetContainerInfo (Owner);

            if (containerInfo.GrowStyle != value) {
                containerInfo.GrowStyle = value;
                LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.GrowStyle);
            }
        }
    }

    [MemberNotNullWhen (true, nameof (_stub))]
    internal bool IsStub => _stub is not null;

#if DESIGN_TIME
    internal void ApplySettings (TableLayoutSettings settings)
    {
        if (settings.IsStub) {
            if (!IsStub) {
                // we're the real-live thing here, gotta walk through and touch controls
                settings._stub.ApplySettings (this);
            } else {
                // we're just copying another stub into us, just replace the member
                _stub = settings._stub;
            }
        }
    }
#endif

#region Extended Properties
    /// <summary>
    /// Return the column span value for the specified control.
    /// </summary>
    public int GetColumnSpan (object control)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (IsStub) {
            return _stub.GetColumnSpan (control);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);
            return TableLayout.GetLayoutInfo (element).ColumnSpan;
        }
    }

    /// <summary>
    /// Sets the column span value for the specified control.
    /// </summary>
    public void SetColumnSpan (object control, int value)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (value < 1)
            throw new ArgumentOutOfRangeException (nameof (value), value, string.Format (SR.InvalidArgument, nameof (value), value));

        if (IsStub) {
            _stub.SetColumnSpan (control, value);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);

            if (element.Container is not null)
                TableLayout.ClearCachedAssignments (TableLayout.GetContainerInfo (element.Container));

            TableLayout.GetLayoutInfo (element).ColumnSpan = value;
            LayoutTransaction.DoLayout (element.Container, element, PropertyNames.ColumnSpan);
            Debug.Assert (GetColumnSpan (element) == value, "column span should equal to the value we set");
        }
    }

    /// <summary>
    /// Return the row span value for the specified control.
    /// </summary>
    public int GetRowSpan (object control)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (IsStub) {
            return _stub.GetRowSpan (control);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);
            return TableLayout.GetLayoutInfo (element).RowSpan;
        }
    }

    /// <summary>
    /// Sets the row span value for the specified control.
    /// </summary>
    public void SetRowSpan (object control, int value)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (value < 1)
            throw new ArgumentOutOfRangeException (nameof (value), value, string.Format (SR.InvalidArgument, nameof (value), value));

        if (IsStub) {
            _stub.SetRowSpan (control, value);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);

            if (element.Container is not null)
                TableLayout.ClearCachedAssignments (TableLayout.GetContainerInfo (element.Container));

            TableLayout.GetLayoutInfo (element).RowSpan = value;
            LayoutTransaction.DoLayout (element.Container, element, PropertyNames.RowSpan);
            Debug.Assert (GetRowSpan (element) == value, "row span should equal to the value we set");
        }
    }

    /// <summary>
    ///  Get the row position of the element
    /// </summary>
    [DefaultValue (-1)]
    public int GetRow (object control)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (IsStub) {
            return _stub.GetRow (control);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);
            var layoutInfo = TableLayout.GetLayoutInfo (element);
            return layoutInfo.RowPosition;
        }
    }

    /// <summary>
    ///  Set the row position of the element
    ///  If we set the row position to -1, it will automatically switch the control from
    ///  absolutely positioned to non-absolutely positioned
    /// </summary>
    public void SetRow (object control, int row)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (row < -1)
            throw new ArgumentOutOfRangeException (nameof (row), row, string.Format (SR.InvalidArgument, nameof (row), row));

        SetCellPosition (control, row, -1, rowSpecified: true, colSpecified: false);
    }

    /// <summary>
    ///  Get the column position of the element
    /// </summary>
    [DefaultValue (-1)]
    public TableLayoutPanelCellPosition GetCellPosition (object control)
    {
        ArgumentNullException.ThrowIfNull (control);

        return new TableLayoutPanelCellPosition (GetColumn (control), GetRow (control));
    }

    /// <summary>
    ///  Set the column position of the element
    /// </summary>
    [DefaultValue (-1)]
    public void SetCellPosition (object control, TableLayoutPanelCellPosition cellPosition)
    {
        ArgumentNullException.ThrowIfNull (control);

        SetCellPosition (control, cellPosition.Row, cellPosition.Column, rowSpecified: true, colSpecified: true);
    }

    /// <summary>
    ///  Get the column position of the element
    /// </summary>
    [DefaultValue (-1)]
    public int GetColumn (object control)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (IsStub) {
            return _stub.GetColumn (control);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);
            var layoutInfo = TableLayout.GetLayoutInfo (element);
            return layoutInfo.ColumnPosition;
        }
    }

    /// <summary>
    ///  Set the column position of the element
    ///  If we set the column position to -1, it will automatically switch the control from
    ///  absolutely positioned to non-absolutely positioned
    /// </summary>
    public void SetColumn (object control, int column)
    {
        ArgumentNullException.ThrowIfNull (control);

        if (column < -1)
            throw new ArgumentOutOfRangeException (nameof (column), column, string.Format (SR.InvalidArgument, nameof (column), column));

        if (IsStub)
            _stub.SetColumn (control, column);
        else
            SetCellPosition (control, -1, column, rowSpecified: false, colSpecified: true);
    }

    private void SetCellPosition (object control, int row, int column, bool rowSpecified, bool colSpecified)
    {
        if (IsStub) {
            if (colSpecified)
                _stub.SetColumn (control, column);

            if (rowSpecified)
                _stub.SetRow (control, row);
        } else {
            var element = LayoutEngine.CastToArrangedElement (control);

            if (element.Container is not null)
                TableLayout.ClearCachedAssignments (TableLayout.GetContainerInfo (element.Container));

            var layoutInfo = TableLayout.GetLayoutInfo (element);

            if (colSpecified)
                layoutInfo.ColumnPosition = column;

            if (rowSpecified)
                layoutInfo.RowPosition = row;

            LayoutTransaction.DoLayout (element.Container, element, PropertyNames.TableIndex);
            Debug.Assert (!colSpecified || GetColumn (element) == column, "column position shoule equal to what we set");
            Debug.Assert (!rowSpecified || GetRow (element) == row, "row position shoule equal to what we set");
        }
    }

    /// <summary>
    ///  Get the element which covers the specified row and column. return null if we can't find one
    /// </summary>
    internal IArrangedElement? GetControlFromPosition (int column, int row) => TableLayout.GetControlFromPosition (Owner, column, row);

    internal TableLayoutPanelCellPosition GetPositionFromControl (IArrangedElement element) => TableLayout.GetPositionFromControl (Owner, element);

#endregion

#if DESIGN_TIME
    void ISerializable.GetObjectData (SerializationInfo si, StreamingContext context)
    {
        var converter = TypeDescriptor.GetConverter (this);
        var stringVal = converter.ConvertToInvariantString (this);

        if (!string.IsNullOrEmpty (stringVal))
            si.AddValue ("SerializedString", stringVal);
    }

    internal List<ControlInformation> GetControlsInformation ()
    {
        if (IsStub) {
            return _stub.GetControlsInformation ();
        } else {
            var controlsInfo = new List<ControlInformation> (Owner.Children.Count ());

            foreach (IArrangedElement element in Owner.Children) {
                if (element is Control c) {
                    var controlInfo = new ControlInformation ();

                    // We need to go through the PropertyDescriptor for the Name property
                    // since it is shadowed.
                    var prop = TypeDescriptor.GetProperties (c)["Name"];

                    if (prop is not null && prop.PropertyType == typeof (string))
                        controlInfo.Name = prop.GetValue (c);

                    controlInfo.Row = GetRow (c);
                    controlInfo.RowSpan = GetRowSpan (c);
                    controlInfo.Column = GetColumn (c);
                    controlInfo.ColumnSpan = GetColumnSpan (c);
                    controlsInfo.Add (controlInfo);
                }
            }

            return controlsInfo;
        }
    }
#endif
}
