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
// Copyright (c) 2004 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//
//

using System;
using System.ComponentModel;

namespace Modern.Forms
{
    /// <summary>
    ///  Provides data for the Layout event.
    /// </summary>
    public sealed class LayoutEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the LayoutEventArgs class.
        /// </summary>
        public LayoutEventArgs (Control? affectedControl, string affectedProperty)
        {
            AffectedControl = affectedControl;
            AffectedProperty = affectedProperty;
        }

        /// <summary>
        ///  Initializes a new instance of the LayoutEventArgs class.
        /// </summary>
        public LayoutEventArgs (IComponent? affectedComponent, string affectedProperty)
        {
            AffectedComponent = affectedComponent;
            AffectedProperty = affectedProperty;
        }

        /// <summary>
        /// Gets the component affected by this layout event.
        /// </summary>
        public IComponent? AffectedComponent { get; }

        /// <summary>
        /// Gets the control affected by this layout event.
        /// </summary>
        public Control? AffectedControl { get; }

        /// <summary>
        /// Gets the property affected by this layout event.
        /// </summary>
        public string AffectedProperty { get; }
    }
}
