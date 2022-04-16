// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Represents a control that positions its children in a sequential layout.
/// </summary>
[ProvideProperty ("FlowBreak", typeof (Control))]
[DefaultProperty (nameof (FlowDirection))]
public class FlowLayoutPanel : Panel, IExtenderProvider
{
    private readonly FlowLayoutSettings _flowLayoutSettings;

    /// <summary>
    /// Initializes a new instance of the FlowLayoutPanel class.
    /// </summary>
    public FlowLayoutPanel ()
    {
        _flowLayoutSettings = new FlowLayoutSettings (this);
    }

    /// <inheritdoc/>
    public override LayoutEngine LayoutEngine => FlowLayout.Instance;

    /// <summary>
    /// Gets or sets a value indicating which direction the control's children should be positioned.
    /// </summary>
    [DefaultValue (FlowDirection.LeftToRight)]
    [Localizable (true)]
    public FlowDirection FlowDirection {
        get => _flowLayoutSettings.FlowDirection;
        set {
            _flowLayoutSettings.FlowDirection = value;
            Debug.Assert (FlowDirection == value, "FlowDirection should be the same as we set it");
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the control's children
    /// should wrap to new lines if more space is needed.
    /// </summary>
    [DefaultValue (true)]
    [Localizable (true)]
    public bool WrapContents {
        get => _flowLayoutSettings.WrapContents;
        set {
            _flowLayoutSettings.WrapContents = value;
            Debug.Assert (WrapContents == value, "WrapContents should be the same as we set it");
        }
    }

    bool IExtenderProvider.CanExtend (object obj) => obj is Control control && control.Parent == this;

    /// <summary>
    /// Gets a value indicating if a flow break has been set for the specified control.
    /// </summary>
    [DefaultValue (false)]
    [DisplayName ("FlowBreak")]
    public bool GetFlowBreak (Control control)
    {
        ArgumentNullException.ThrowIfNull (control);

        return _flowLayoutSettings.GetFlowBreak (control);
    }

    /// <summary>
    /// Sets a flow break for the specified control.
    /// </summary>
    [DisplayName ("FlowBreak")]
    public void SetFlowBreak (Control control, bool value)
    {
        ArgumentNullException.ThrowIfNull (control);

        _flowLayoutSettings.SetFlowBreak (control, value);
    }
}
