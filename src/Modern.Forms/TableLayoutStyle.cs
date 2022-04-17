// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Base class for TableLayout styles.
/// </summary>
[TypeConverter (typeof (TableLayoutSettings.StyleConverter))]
public abstract class TableLayoutStyle
{
    private SizeType _sizeType = SizeType.AutoSize;
    private float _size;

    /// <summary>
    /// Gets or sets a value indicating the size type of the style.
    /// </summary>
    [DefaultValue (SizeType.AutoSize)]
    public SizeType SizeType {
        get => _sizeType;
        set {
            if (_sizeType != value) {
                _sizeType = value;

                if (Owner is not null) {
                    LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.Style);

                    if (Owner is Control owner)
                        owner.Invalidate ();
                }
            }
        }
    }

    internal float Size {
        get => _size;
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException (nameof (value), value, string.Format (SR.InvalidLowBoundArgumentEx, nameof (Size), value, 0));

            if (_size != value) {
                _size = value;

                if (Owner is not null) {
                    LayoutTransaction.DoLayout (Owner, Owner, PropertyNames.Style);

                    if (Owner is Control owner)
                        owner.Invalidate ();
                }
            }
        }
    }

    private bool ShouldSerializeSize ()
    {
        return SizeType != SizeType.AutoSize;
    }

    internal IArrangedElement? Owner { get; set; }

    //set the size without doing a layout
    internal void SetSize (float size)
    {
        Debug.Assert (size >= 0);
        _size = size;
    }
}
