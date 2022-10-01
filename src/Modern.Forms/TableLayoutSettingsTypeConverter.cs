// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;
using System.Xml;
using Modern.Forms.Layout;

namespace Modern.Forms;

#if DESIGN_TIME
/// <summary>
/// TypeConverter for TableLayoutSettings.
/// </summary>
public class TableLayoutSettingsTypeConverter : TypeConverter
{
    /// <summary>
    ///  Determines if this converter can convert an object in the given source
    ///  type to the native type of the converter.
    /// </summary>
    public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof (string)) 
            return true;

        return base.CanConvertFrom (context, sourceType);
    }

    /// <summary>
    ///  Gets a value indicating whether this converter can
    ///  convert an object to the given destination type using the context.
    /// </summary>
    public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof (InstanceDescriptor) || destinationType == typeof (string)) 
            return true;

        return base.CanConvertTo (context, destinationType);
    }

    /// <summary>
    ///  Converts the given object to the converter's native type.
    /// </summary>
    public override object? ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string val_string) {
            var tableLayoutSettingsXml = new XmlDocument ();
            tableLayoutSettingsXml.LoadXml (val_string);

            var settings = new TableLayoutSettings ();

            ParseControls (settings, tableLayoutSettingsXml.GetElementsByTagName ("Control"));
            ParseStyles (settings, tableLayoutSettingsXml.GetElementsByTagName ("Columns"), /*isColumn=*/true);
            ParseStyles (settings, tableLayoutSettingsXml.GetElementsByTagName ("Rows"), /*isColumn=*/false);

            return settings;
        }

        return base.ConvertFrom (context, culture, value);
    }

    /// <inheritdoc/>
    public override object? ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is TableLayoutSettings tls && destinationType == typeof (string)) {
            var xmlStringBuilder = new StringBuilder ();
            var xmlWriter = XmlWriter.Create (xmlStringBuilder);
            xmlWriter.WriteStartElement ("TableLayoutSettings");

            //
            // write controls
            //
            xmlWriter.WriteStartElement ("Controls");

            foreach (var c in tls.GetControlsInformation ()) {
                if (c.Name is null) 
                    throw new InvalidOperationException (SR.TableLayoutSettingsConverterNoName);

                xmlWriter.WriteStartElement ("Control");
                xmlWriter.WriteAttributeString ("Name", c.Name.ToString ());
                xmlWriter.WriteAttributeString ("Row", c.Row.ToString (CultureInfo.CurrentCulture));
                xmlWriter.WriteAttributeString ("RowSpan", c.RowSpan.ToString (CultureInfo.CurrentCulture));

                xmlWriter.WriteAttributeString ("Column", c.Column.ToString (CultureInfo.CurrentCulture));
                xmlWriter.WriteAttributeString ("ColumnSpan", c.ColumnSpan.ToString (CultureInfo.CurrentCulture));

                xmlWriter.WriteEndElement ();
            }

            xmlWriter.WriteEndElement (); // end Controls

            //
            // write columns
            //
            xmlWriter.WriteStartElement ("Columns");

            var columnStyles = new StringBuilder ();

            foreach (ColumnStyle colStyle in tls.ColumnStyles) 
                columnStyles.AppendFormat ("{0},{1},", colStyle.SizeType, colStyle.Width);

            if (columnStyles.Length > 0) 
                columnStyles.Remove (columnStyles.Length - 1, 1);

            xmlWriter.WriteAttributeString ("Styles", columnStyles.ToString ());
            xmlWriter.WriteEndElement (); // end columns

            //
            // write rows
            //
            xmlWriter.WriteStartElement ("Rows");

            var rowStyles = new StringBuilder ();

            foreach (RowStyle rowStyle in tls.RowStyles) 
                rowStyles.AppendFormat ("{0},{1},", rowStyle.SizeType, rowStyle.Height);

            if (rowStyles.Length > 0) 
                rowStyles.Remove (rowStyles.Length - 1, 1);

            xmlWriter.WriteAttributeString ("Styles", rowStyles.ToString ());
            xmlWriter.WriteEndElement (); // end Rows

            xmlWriter.WriteEndElement (); // end TableLayoutSettings

            xmlWriter.Close ();
            return xmlStringBuilder.ToString ();
        }

        return base.ConvertTo (context, culture, value, destinationType);
    }

private string? GetAttributeValue (XmlNode node, string attribute)
    {
        var attr = node.Attributes![attribute];

        if (attr is not null) 
            return attr.Value;

        return null;
    }

    private int GetAttributeValue (XmlNode node, string attribute, int valueIfNotFound)
    {
        var attributeValue = GetAttributeValue (node, attribute);

        if (!string.IsNullOrEmpty (attributeValue)) 
            if (int.TryParse (attributeValue, out var result)) 
                return result;

        return valueIfNotFound;
    }

    private void ParseControls (TableLayoutSettings settings, XmlNodeList controlXmlFragments)
    {
        foreach (XmlNode controlXmlNode in controlXmlFragments) {
            var name = GetAttributeValue (controlXmlNode, "Name");

            if (!string.IsNullOrEmpty (name)) {
                var row = GetAttributeValue (controlXmlNode, "Row",       /*default*/-1);
                var rowSpan = GetAttributeValue (controlXmlNode, "RowSpan",   /*default*/1);
                var column = GetAttributeValue (controlXmlNode, "Column",    /*default*/-1);
                var columnSpan = GetAttributeValue (controlXmlNode, "ColumnSpan",/*default*/1);

                settings.SetRow (name, row);
                settings.SetColumn (name, column);
                settings.SetRowSpan (name, rowSpan);
                settings.SetColumnSpan (name, columnSpan);
            }
        }
    }

    private void ParseStyles (TableLayoutSettings settings, XmlNodeList controlXmlFragments, bool columns)
    {
        foreach (XmlNode styleXmlNode in controlXmlFragments) {
            var styleString = GetAttributeValue (styleXmlNode, "Styles");
            var sizeTypeType = typeof (SizeType);

            // styleString will consist of N Column/Row styles serialized in the following format
            // (Percent | Absolute | AutoSize), (24 | 24.4 | 24,4)
            // Two examples:
            // Percent,23.3,Percent,46.7,Percent,30
            // Percent,23,3,Percent,46,7,Percent,30
            // Note we get either . or , based on the culture the TableLayoutSettings were serialized in

            if (!string.IsNullOrEmpty (styleString)) {
                var currentIndex = 0;
                int nextIndex;
                while (currentIndex < styleString.Length) {
                    // ---- SizeType Parsing -----------------
                    nextIndex = currentIndex;
                    while (char.IsLetter (styleString[nextIndex])) 
                        nextIndex++;

                    var type = (SizeType)Enum.Parse (sizeTypeType, styleString.AsSpan (currentIndex, nextIndex - currentIndex), true);

                    // ----- Float Parsing --------------
                    // Find the next Digit (start of the float)
                    while (!char.IsDigit (styleString[nextIndex])) 
                        nextIndex++;

                    // Append digits left of the decimal delimiter(s)
                    var floatStringBuilder = new StringBuilder ();

                    while (nextIndex < styleString.Length && char.IsDigit (styleString[nextIndex])) {
                        floatStringBuilder.Append (styleString[nextIndex]);
                        nextIndex++;
                    }

                    // Append culture invariant delimiter
                    floatStringBuilder.Append ('.');

                    // Append digits right of the decimal delimiter(s)
                    while (nextIndex < styleString.Length && !char.IsLetter (styleString[nextIndex])) {
                        if (char.IsDigit (styleString[nextIndex])) floatStringBuilder.Append (styleString[nextIndex]);

                        nextIndex++;
                    }

                    var floatString = floatStringBuilder.ToString ();

                    if (!float.TryParse (floatString, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out var width)) 
                        width = 0F;

                    // Add new Column/Row Style
                    if (columns) 
                        settings.ColumnStyles.Add (new ColumnStyle (type, width));
                    else 
                        settings.RowStyles.Add (new RowStyle (type, width));

                    // Go to the next Column/Row Style
                    currentIndex = nextIndex;
                }
            }
        }
    }
}
#endif
