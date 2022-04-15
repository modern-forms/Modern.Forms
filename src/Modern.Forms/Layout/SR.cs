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

    internal static string LayoutEngineUnsupportedType => "LayoutEngine cannot arrange objects of type '{0}'";
}
