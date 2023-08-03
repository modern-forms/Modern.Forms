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
        private static readonly double deviceDpi = LogicalDpi;

        /// <summary>
        ///  Returns whether scaling is required when converting between logical-device units,
        ///  if the application opted in the automatic scaling in the .config file.
        /// </summary>
        public static bool IsScalingRequired => deviceDpi != LogicalDpi;

        /// <summary>
        ///  Indicates, if rescaling becomes necessary, either because we are not 96 DPI or we're PerMonitorV2Aware.
        /// </summary>
        internal static bool IsScalingRequirementMet => IsScalingRequired;// || s_perMonitorAware;

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
        public static Size LogicalToDeviceUnits (Size logicalSize, int deviceDpi)
        {
            return new Size (LogicalToDeviceUnits (logicalSize.Width, deviceDpi),
                            LogicalToDeviceUnits (logicalSize.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Padding with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        public static Padding LogicalToDeviceUnits (Padding logicalPadding, int deviceDpi)
        {
            return new Padding (LogicalToDeviceUnits (logicalPadding.Left, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Top, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Right, deviceDpi),
                                LogicalToDeviceUnits (logicalPadding.Bottom, deviceDpi));
        }
    }
}
