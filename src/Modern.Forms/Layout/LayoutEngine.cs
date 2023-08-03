// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Modern.Forms.Layout;

/// <summary>
/// Base class for layout engines.
/// </summary>
public abstract class LayoutEngine
{
    internal static IArrangedElement CastToArrangedElement (object obj)
    {
        if (obj is not IArrangedElement element)
            throw new NotSupportedException (string.Format (SR.LayoutEngineUnsupportedType, obj.GetType ()));

        return element;
    }

    internal virtual Size GetPreferredSize (IArrangedElement container, Size proposedConstraints)
    {
        return Size.Empty;
    }

    /// <summary>
    /// Initializes a layout.
    /// </summary>
    public virtual void InitLayout (object child, BoundsSpecified specified)
    {
        ArgumentNullException.ThrowIfNull (child);

        InitLayoutCore (CastToArrangedElement (child), specified);
    }

    private protected virtual void InitLayoutCore (IArrangedElement element, BoundsSpecified bounds)
    {
    }

    /// <summary>
    /// Performs a layout. Returns true is parent layout is required.
    /// </summary>
    public virtual bool Layout (object container, LayoutEventArgs layoutEventArgs)
    {
        ArgumentNullException.ThrowIfNull (container);
        return LayoutCore (CastToArrangedElement (container), layoutEventArgs);
    }

    private protected virtual bool LayoutCore (IArrangedElement container, LayoutEventArgs layoutEventArgs)
    {
        return false;
    }

    internal virtual void ProcessSuspendedLayoutEventArgs (IArrangedElement container, LayoutEventArgs args)
    {
    }
}
