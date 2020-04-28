using System;
using System.Collections.Generic;
using System.Text;

namespace Modern.Forms
{
    /// <summary>
    /// Specifies the checked state of a control.
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// The control is not checked.
        /// </summary>
        Unchecked,
        /// <summary>
        /// The control is checked.
        /// </summary>
        Checked,
        /// <summary>
        /// The checked state of the control is indeterminate.
        /// </summary>
        Indeterminate
    }
}
