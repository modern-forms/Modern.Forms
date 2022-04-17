// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Represents the row and column of a cell in a TableLayoutPanel.
/// </summary>
[TypeConverter (typeof (TableLayoutPanelCellPositionTypeConverter))]
public struct TableLayoutPanelCellPosition
{
    /// <summary>
    /// Initializes a new instance of the ComboBox class.
    /// </summary>
    public TableLayoutPanelCellPosition (int column, int row)
    {
        if (row < -1)
            throw new ArgumentOutOfRangeException (nameof (row), row, string.Format (SR.InvalidArgument, nameof (row), row));

        if (column < -1)
            throw new ArgumentOutOfRangeException (nameof (column), column, string.Format (SR.InvalidArgument, nameof (column), column));

        Row = row;
        Column = column;
    }

    /// <summary>
    /// Gets or sets the row of the table cell.
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Gets or sets the column of the table cell.
    /// </summary>
    public int Column { get; set; }

    /// <inheritdoc/>
    public override bool Equals (object? other)
    {
        if (other is not TableLayoutPanelCellPosition otherCellPosition)
            return false;

        return this == otherCellPosition;
    }

    /// <summary>
    /// Operator for comparing equality of TableLayoutPanelCellPosition.
    /// </summary>
    public static bool operator == (TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return p1.Row == p2.Row && p1.Column == p2.Column;
    }

    /// <summary>
    /// Operator for comparing inequality of TableLayoutPanelCellPosition.
    /// </summary>
    public static bool operator != (TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
    {
        return !(p1 == p2);
    }

    /// <inheritdoc/>
    public override string ToString () => $"{Column},{Row}";

    /// <inheritdoc/>
    public override int GetHashCode () => HashCode.Combine (Row, Column);
}

internal class TableLayoutPanelCellPositionTypeConverter : TypeConverter
{
    public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof (InstanceDescriptor))
            return true;

        return base.CanConvertTo (context, destinationType);
    }

    public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof (string))
            return true;

        return base.CanConvertFrom (context, sourceType);
    }

    public override object? ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue) {
            stringValue = stringValue.Trim ();

            if (stringValue.Length == 0)
                return null;

            // Parse 2 integer values.
            if (culture is null)
                culture = CultureInfo.CurrentCulture;

            var tokens = stringValue.Split (new char[] { culture.TextInfo.ListSeparator[0] });
            var values = new int[tokens.Length];

            var intConverter = TypeDescriptor.GetConverter (typeof (int));

            for (var i = 0; i < values.Length; i++) {
                // Note: ConvertFromString will raise exception if value cannot be converted.
                values[i] = (int)intConverter.ConvertFromString (context, culture, tokens[i])!;
            }

            if (values.Length != 2)
                throw new ArgumentException (string.Format (SR.TextParseFailedFormat, stringValue, "column, row"), nameof (value));

            return new TableLayoutPanelCellPosition (values[0], values[1]);
        }

        return base.ConvertFrom (context, culture, value);
    }

    public override object? ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof (InstanceDescriptor) && value is TableLayoutPanelCellPosition cellPosition) {
            return new InstanceDescriptor (
                typeof (TableLayoutPanelCellPosition).GetConstructor (new Type[] { typeof (int), typeof (int) }),
                new object[] { cellPosition.Column, cellPosition.Row }
            );
        }

        return base.ConvertTo (context, culture, value, destinationType);
    }

    public override object CreateInstance (ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        ArgumentNullException.ThrowIfNull (propertyValues);

        try {
            return new TableLayoutPanelCellPosition (
                (int)propertyValues[nameof (TableLayoutPanelCellPosition.Column)]!,
                (int)propertyValues[nameof (TableLayoutPanelCellPosition.Row)]!
            );
        } catch (InvalidCastException invalidCast) {
            throw new ArgumentException (SR.PropertyValueInvalidEntry, nameof (propertyValues), invalidCast);
        } catch (NullReferenceException nullRef) {
            throw new ArgumentException (SR.PropertyValueInvalidEntry, nameof (propertyValues), nullRef);
        }
    }

    public override bool GetCreateInstanceSupported (ITypeDescriptorContext? context) => true;

    public override PropertyDescriptorCollection GetProperties (ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        var props = TypeDescriptor.GetProperties (typeof (TableLayoutPanelCellPosition), attributes);
        return props.Sort (new string[] { nameof (TableLayoutPanelCellPosition.Column), nameof (TableLayoutPanelCellPosition.Row) });
    }

    public override bool GetPropertiesSupported (ITypeDescriptorContext? context) => true;
}
