using System;
using System.Drawing;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    ///  Provides data for the Paint event.
    /// </summary>
    public class PaintEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the PaintEventArgs class.
        /// </summary>
        public PaintEventArgs (SKImageInfo info, SKCanvas canvas, double scaling)
        {
            Info = info;
            Canvas = canvas;
            Scaling = scaling;
        }

        /// <summary>
        /// Gets the canvas needed to paint the control.
        /// </summary>
        public SKCanvas Canvas { get; }

        /// <summary>
        /// Gets information about the image canvas.
        /// </summary>
        public SKImageInfo Info { get; }

        /// <summary>
        /// Gets the current scale factor of the form.
        /// </summary>
        public double Scaling { get; }

        /// <summary>
        /// Transforms a horizontal or vertical integer coordinate from logical to device units
        /// by scaling it up for current DPI and rounding to nearest integer value.
        /// </summary>
        /// <param name="value">Value in logical units</param>
        /// <returns>Value in device units</returns>
        public int LogicalToDeviceUnits (int value) => (int)Math.Round (Scaling * value);

        /// <summary>
        /// Transforms a Size from logical to device units
        /// by scaling it up for current DPI and rounding to nearest integer value.
        /// </summary>
        /// <param name="value">Value in logical units</param>
        /// <returns>Value in device units</returns>
        public Size LogicalToDeviceUnits (Size value) => new Size (LogicalToDeviceUnits (value.Width), LogicalToDeviceUnits (value.Height));

        /// <summary>
        /// Transforms a Padding from logical to device units
        /// by scaling it up for current DPI and rounding to nearest integer value.
        /// </summary>
        /// <param name="value">Value in logical units</param>
        /// <returns>Value in device units</returns>
        public Padding LogicalToDeviceUnits (Padding value)
        {
            return new Padding (LogicalToDeviceUnits (value.Left),
                                LogicalToDeviceUnits (value.Top),
                                LogicalToDeviceUnits (value.Right),
                                LogicalToDeviceUnits (value.Bottom));
        }
    }
}
