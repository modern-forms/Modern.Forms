using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Layout
{
    internal class SR
    {
        /// <summary>Invisible or disabled control cannot be activated</summary>
        internal static string @CannotActivateControl => GetResourceString ("CannotActivateControl");

        /// <summary>A circular control reference has been made. A control cannot be owned by or parented to itself.</summary>
        internal static string @CircularOwner => GetResourceString ("CircularOwner");

        /// <summary>Parameter must be of type Control.</summary>
        internal static string @ControlBadControl => GetResourceString ("ControlBadControl");

        /// <summary>'child' is not a child control of this parent.</summary>
        internal static string @ControlNotChild => GetResourceString ("ControlNotChild");

        /// <summary>Key specified was either empty or null.</summary>
        internal static string @FindKeyMayNotBeEmptyOrNull => GetResourceString ("FindKeyMayNotBeEmptyOrNull");

        /// <summary>Index {0} is out of range.</summary>
        internal static string @IndexOutOfRange => GetResourceString ("IndexOutOfRange");

        /// <summary>LayoutEngine cannot arrange objects of type '{0}'.</summary>
        internal static string @LayoutEngineUnsupportedType => GetResourceString ("LayoutEngineUnsupportedType");

        private static string GetResourceString (string v)
        {
            return v;
        }
    }
}
