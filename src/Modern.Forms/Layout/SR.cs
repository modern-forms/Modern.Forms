// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Modern.Forms.Layout;

internal class SR
{
    internal static string CannotActivateControl => "Invisible or disabled control cannot be activated.";

    internal static string CircularOwner => "A circular control reference has been made. A control cannot be owned by or parented to itself.";

    internal static string ControlNotChild => "'child' is not a child control of this parent.";

    internal static string FindKeyMayNotBeEmptyOrNull => "Key specified was either empty or null.";

    internal static string IndexOutOfRange => "Index {0} is out of range.";

    internal static string InvalidArgument => "'{1}' is not a valid value for '{0}'.";

    internal static string InvalidLowBoundArgumentEx => "Value of '{1}' is not valid for '{0}'. '{0}' must be greater than or equal to {2}.";

    internal static string LayoutEngineUnsupportedType => "LayoutEngine cannot arrange objects of type '{0}'";

    internal static string OnlyOneControl => "Cannot add or insert the item '{0}' in more than one place. You must first remove it from its current location or clone it.";

    internal static string PropertyValueInvalidEntry => "One or more entries are not valid in the IDictionary parameter. Verify that all values match up to the object's properties.";

    internal static string TableLayoutPanelFullDesc => "Additional Rows or Columns cannot be created.  TableLayoutPanel is full and GrowStyle is 'FixedSize'.";

    internal static string TableLayoutPanelSpanDesc => "TableLayoutPanel cannot expand to contain the control, because the panel's GrowStyle property is set to 'FixedSize'.";

    internal static string TableLayoutSettingsConverterNoName => "Cannot convert TableLayoutSettings to string: could not find a 'Name' string property on a control.";

    internal static string TableLayoutSettingSettingsIsNotSupported => "Directly setting TableLayoutSettings is not supported.  Use individual properties instead.";

    internal static string TextParseFailedFormat => "Parse of Text('{0}') expected text in the format '{1}' did not succeed.";
}
