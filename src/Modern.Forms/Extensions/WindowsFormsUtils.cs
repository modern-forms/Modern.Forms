using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms
{
    internal static class WindowsFormsUtils
    {
        // ExecutionEngineException is obsolete and shouldn't be used (to catch, throw or reference) anymore.
        // Pragma added to prevent converting the "type is obsolete" warning into build error.
        // File owner should fix this.
        public static bool IsCriticalException (Exception ex)
        {
            return ex is NullReferenceException
                    || ex is StackOverflowException
                    || ex is OutOfMemoryException
                    || ex is System.Threading.ThreadAbortException
                    || ex is ExecutionEngineException
                    || ex is IndexOutOfRangeException
                    || ex is AccessViolationException;
        }

        /// <summary>
        ///  Compares the strings using invariant culture for Turkish-I support. Returns true if they match.
        ///
        ///  If your strings are symbolic (returned from APIs, not from user) the following calls
        ///  are faster than this method:
        ///
        ///  String.Equals(s1, s2, StringComparison.Ordinal)
        ///  String.Equals(s1, s2, StringComparison.OrdinalIgnoreCase)
        /// </summary>
        public static bool SafeCompareStrings (string? string1, string? string2, bool ignoreCase)
        {
            if ((string1 is null) || (string2 is null)) {
                // if either key is null, we should return false
                return false;
            }

            // Because String.Compare returns an ordering, it can not terminate early if lengths are not the same.
            // Also, equivalent characters can be encoded in different byte sequences, so it can not necessarily
            // terminate on the first byte which doesn't match. Hence this optimization.
            if (string1.Length != string2.Length) {
                return false;
            }

            return string.Compare (string1, string2, ignoreCase, CultureInfo.InvariantCulture) == 0;
        }
    }
}
