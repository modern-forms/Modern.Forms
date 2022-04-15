﻿//
// DefaultLayout.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Jonathan Pobst
//
// Authors:
//	Jonathan Pobst (monkey@jpobst.com)
//	Stefan Noack (noackstefan@googlemail.com)
//

using System;
using System.Linq;

namespace Modern.Forms
{
    //internal class DefaultLayout : LayoutEngine
    //{
    //    public static DefaultLayout Instance = new DefaultLayout ();

    //    public override bool Layout (object container, LayoutEventArgs args)
    //    {
    //        if (!(container is Control parent))
    //            return false;

    //        var controls = parent.Controls.GetAllControls ().ToArray ();

    //        LayoutDockedChildren (parent, controls);
    //        LayoutAnchoredChildren (parent, controls);
    //        //LayoutAutoSizedChildren (parent, controls);

    //        //if (parent is Form)
    //        //    LayoutAutoSizeContainer (parent);

    //        return false;
    //    }

    //    private void LayoutDockedChildren (Control parent, Control[] controls)
    //    {
    //        var space = parent.ClientRectangle;

    //        if (space.Width == 0 || space.Height == 0 || controls.Length == 0)
    //            return;

    //        // Deal with docking; go through in reverse, MS docs say that lowest Z-order is closest to edge
    //        for (var i = controls.Length - 1; i >= 0; i--) {
    //            var child = controls[i];
    //            var child_size = child.ScaledSize;

    //            //if (child.AutoSize)
    //            //	child_size = GetPreferredControlSize (child);

    //            if (!child.Visible || child.UseAnchorLayoutInternal)
    //                continue;

    //            switch (child.Dock) {
    //                case DockStyle.None:
    //                    // Do nothing
    //                    break;

    //                case DockStyle.Left:
    //                    child.SetScaledBounds (space.Left, space.Y, child_size.Width, space.Height, BoundsSpecified.Size);
    //                    space.X += child.ScaledBounds.Width;
    //                    space.Width -= child.ScaledBounds.Width;
    //                    break;

    //                case DockStyle.Top:
    //                    child.SetScaledBounds (space.Left, space.Y, space.Width, child_size.Height, BoundsSpecified.Size);
    //                    space.Y += child.ScaledBounds.Height;
    //                    space.Height -= child.ScaledBounds.Height;
    //                    break;

    //                case DockStyle.Right:
    //                    child.SetScaledBounds (space.Right - child_size.Width, space.Y, child_size.Width, space.Height, BoundsSpecified.Size);
    //                    space.Width -= child.ScaledBounds.Width;
    //                    break;

    //                case DockStyle.Bottom:
    //                    child.SetScaledBounds (space.Left, space.Bottom - child_size.Height, space.Width, child_size.Height, BoundsSpecified.Size);
    //                    space.Height -= child.ScaledBounds.Height;
    //                    break;

    //                case DockStyle.Fill:
    //                    child.SetScaledBounds (space.Left, space.Top, space.Width, space.Height, BoundsSpecified.Size);
    //                    break;
    //            }
    //        }
    //    }

    //    private void LayoutAnchoredChildren (Control parent, Control[] controls)
    //    {
    //        var space = parent.ClientRectangle;

    //        for (var i = 0; i < controls.Length; i++) {
    //            int left;
    //            int top;
    //            int width;
    //            int height;

    //            var child = controls[i];

    //            if (!child.Visible || !child.UseAnchorLayoutInternal)
    //                continue;

    //            var anchor = child.Anchor;

    //            left = child.Left;
    //            top = child.Top;

    //            width = child.ScaledWidth;
    //            height = child.ScaledHeight;

    //            if (anchor.HasFlag (AnchorStyles.Right)) {
    //                // Control is anchored to left and right, so we need to stretch it
    //                if (anchor.HasFlag (AnchorStyles.Left))
    //                    width = space.Width - child.dist_right - left;
    //                else
    //                    left = space.Width - child.dist_right - width;
    //            } else if (!anchor.HasFlag (AnchorStyles.Left)) {
    //                left += (space.Width - (left + width + child.dist_right)) / 2;
    //                child.dist_right = space.Width - (left + width);
    //            }

    //            if (anchor.HasFlag (AnchorStyles.Bottom)) {
    //                // Control is anchored to top and bottom, so we need to stretch it
    //                if (anchor.HasFlag (AnchorStyles.Top))
    //                    height = space.Height - child.dist_bottom - top;
    //                else
    //                    top = space.Height - child.dist_bottom - height;
    //            } else if (!anchor.HasFlag (AnchorStyles.Top)) {
    //                top += (space.Height - (top + height + child.dist_bottom)) / 2;
    //                child.dist_bottom = space.Height - (top + height);
    //            }

    //            // Sanity
    //            if (width < 0)
    //                width = 0;

    //            if (height < 0)
    //                height = 0;

    //            child.SetScaledBounds (child.LogicalToDeviceUnits (left), child.LogicalToDeviceUnits (top), width, height, BoundsSpecified.All);
    //        }
    //    }

    //    //void LayoutAutoSizedChildren (Control parent, Control[] controls)
    //    //{
    //    //	for (var i = 0; i < controls.Length; i++) {
    //    //		int left;
    //    //		int top;

    //    //		Control child = controls[i];
    //    //		if (!child.VisibleInternal
    //    //		    || child.ControlLayoutType == Control.LayoutType.Dock
    //    //		    || !child.AutoSize)
    //    //			continue;

    //    //		AnchorStyles anchor = child.Anchor;
    //    //		left = child.Left;
    //    //		top = child.Top;

    //    //		Size preferredsize = GetPreferredControlSize (child);

    //    //		if (((anchor & AnchorStyles.Left) != 0) || ((anchor & AnchorStyles.Right) == 0))
    //    //			child.dist_right += child.Width - preferredsize.Width;
    //    //		if (((anchor & AnchorStyles.Top) != 0) || ((anchor & AnchorStyles.Bottom) == 0))
    //    //			child.dist_bottom += child.Height - preferredsize.Height;

    //    //		child.SetBoundsInternal (left, top, preferredsize.Width, preferredsize.Height, BoundsSpecified.None);
    //    //	}
    //    //}

    //    //void LayoutAutoSizeContainer (Control container)
    //    //{
    //    //	int left;
    //    //	int top;
    //    //	int width;
    //    //	int height;

    //    //	if (!container.VisibleInternal || container.ControlLayoutType == Control.LayoutType.Dock || !container.AutoSize)
    //    //		return;

    //    //	left = container.Left;
    //    //	top = container.Top;

    //    //	Size preferredsize = container.PreferredSize;

    //    //	if (container.GetAutoSizeMode () == AutoSizeMode.GrowAndShrink) {
    //    //		width = preferredsize.Width;
    //    //		height = preferredsize.Height;
    //    //	} else {
    //    //		width = container.ExplicitBounds.Width;
    //    //		height = container.ExplicitBounds.Height;
    //    //		if (preferredsize.Width > width)
    //    //			width = preferredsize.Width;
    //    //		if (preferredsize.Height > height)
    //    //			height = preferredsize.Height;
    //    //	}

    //    //	// Sanity
    //    //	if (width < container.MinimumSize.Width)
    //    //		width = container.MinimumSize.Width;

    //    //	if (height < container.MinimumSize.Height)
    //    //		height = container.MinimumSize.Height;

    //    //	if (container.MaximumSize.Width != 0 && width > container.MaximumSize.Width)
    //    //		width = container.MaximumSize.Width;

    //    //	if (container.MaximumSize.Height != 0 && height > container.MaximumSize.Height)
    //    //		height = container.MaximumSize.Height;

    //    //	container.SetBoundsInternal (left, top, width, height, BoundsSpecified.None);
    //    //}

    //    //private Size GetPreferredControlSize (Control child)
    //    //{
    //    //    int width;
    //    //    int height;
    //    //    var preferredsize = child.GetPreferredSize (Size.Empty);

    //    //    //if (child.GetAutoSizeMode () == AutoSizeMode.GrowAndShrink || (child.Dock != DockStyle.None && !(child is Button) && !(child is FlowLayoutPanel))) {
    //    //    //	width = preferredsize.Width;
    //    //    //	height = preferredsize.Height;
    //    //    //} else {
    //    //    //width = child.ExplicitBounds.Width;
    //    //    //height = child.ExplicitBounds.Height;
    //    //    //if (preferredsize.Width > width)
    //    //    width = preferredsize.Width;
    //    //    //if (preferredsize.Height > height)
    //    //    height = preferredsize.Height;
    //    //    //}
    //    //    //if (child.AutoSize && child is FlowLayoutPanel && child.Dock != DockStyle.None) {
    //    //    //	switch (child.Dock) {
    //    //    //	case DockStyle.Left:
    //    //    //	case DockStyle.Right:
    //    //    //		if (preferredsize.Width < child.ExplicitBounds.Width && preferredsize.Height < child.Parent.PaddingClientRectangle.Height)
    //    //    //			width = preferredsize.Width;
    //    //    //		break;
    //    //    //	case DockStyle.Top:
    //    //    //	case DockStyle.Bottom:
    //    //    //		if (preferredsize.Height < child.ExplicitBounds.Height && preferredsize.Width < child.Parent.PaddingClientRectangle.Width)
    //    //    //			height = preferredsize.Height;
    //    //    //		break;
    //    //    //	}
    //    //    //}
    //    //    // Sanity
    //    //    //if (width < child.MinimumSize.Width)
    //    //    //	width = child.MinimumSize.Width;

    //    //    //if (height < child.MinimumSize.Height)
    //    //    //	height = child.MinimumSize.Height;

    //    //    //if (child.MaximumSize.Width != 0 && width > child.MaximumSize.Width)
    //    //    //	width = child.MaximumSize.Width;

    //    //    //if (child.MaximumSize.Height != 0 && height > child.MaximumSize.Height)
    //    //    //	height = child.MaximumSize.Height;

    //    //    return new Size (width, height);
    //    //}
    //}
}
