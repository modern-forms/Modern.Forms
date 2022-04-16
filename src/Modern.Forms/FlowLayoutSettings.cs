// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Stores setting used by a FlowLayoutPanel.
/// </summary>
[DefaultProperty (nameof (FlowDirection))]
public class FlowLayoutSettings : LayoutSettings
{
    internal FlowLayoutSettings (IArrangedElement owner) : base (owner)
    {
    }

    /// <inheritdoc/>
    public override LayoutEngine LayoutEngine => FlowLayout.Instance;

    /// <summary>
    /// Gets or sets a value indicating which direction the control's children should be positioned.
    /// </summary>
    [DefaultValue (FlowDirection.LeftToRight)]
    public FlowDirection FlowDirection {
        get => FlowLayout.GetFlowDirection (Owner);
        set {
            FlowLayout.SetFlowDirection (Owner, value);
            Debug.Assert (FlowDirection == value, "FlowDirection should be the same as we set it");
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the control's children
    /// should wrap to new lines if more space is needed.
    /// </summary>
    [DefaultValue (true)]
    public bool WrapContents {
        get => FlowLayout.GetWrapContents (Owner);
        set {
            FlowLayout.SetWrapContents (Owner, value);
            Debug.Assert (WrapContents == value, "WrapContents should be the same as we set it");
        }
    }

    /// <summary>
    /// Sets a flow break for the specified control.
    /// </summary>
    public void SetFlowBreak (object child, bool value)
    {
        ArgumentNullException.ThrowIfNull (child);

        var element = FlowLayout.Instance.CastToArrangedElement (child);

        if (GetFlowBreak (child) != value)
            CommonProperties.SetFlowBreak (element, value);
    }

    /// <summary>
    /// Gets a value indicating if a flow break has been set for the specified control.
    /// </summary>
    public bool GetFlowBreak (object child)
    {
        ArgumentNullException.ThrowIfNull (child);

        var element = FlowLayout.Instance.CastToArrangedElement (child);
        return CommonProperties.GetFlowBreak (element);
    }
}
