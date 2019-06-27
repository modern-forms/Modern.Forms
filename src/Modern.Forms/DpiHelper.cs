using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modern.Forms
{
    internal class DpiHelper
    {
        internal const double LogicalDpi = 96.0;

        private static double logicalToDeviceUnitsScalingFactor = 0.0;
        private static double deviceDpi = LogicalDpi;

        private static double LogicalToDeviceUnitsScalingFactor {
            get {
                if (logicalToDeviceUnitsScalingFactor == 0.0)
                    logicalToDeviceUnitsScalingFactor = deviceDpi / LogicalDpi;

                return logicalToDeviceUnitsScalingFactor;
            }
        }

        /// <summary>
        /// Transforms a horizontal or vertical integer coordinate from logical to device units
        /// by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">value in logical units</param>
        /// <returns>value in device units</returns>
        public static int LogicalToDeviceUnits (int value, int devicePixels)
        {
            if (devicePixels == 0)
                return (int)Math.Round (LogicalToDeviceUnitsScalingFactor * value);

            var scalingFactor = devicePixels / LogicalDpi;
            return (int)Math.Round (scalingFactor * value);
        }

        /// <summary>
        /// Returns a new Size with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <returns>Size in device units</returns>
        public static Size LogicalToDeviceUnits (Size logicalSize, int deviceDpi)
        {
            return new Size (LogicalToDeviceUnits (logicalSize.Width, deviceDpi),
                            LogicalToDeviceUnits (logicalSize.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Padding with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalPadding">Padding in logical units</param>
        /// <returns>Padding in device units</returns>
        public static Padding LogicalToDeviceUnits (Padding logicalPadding, int deviceDpi)
        {
            return new Padding (LogicalToDeviceUnits (logicalPadding.Left, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Top, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Right, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Bottom, deviceDpi));
        }
    }
}
