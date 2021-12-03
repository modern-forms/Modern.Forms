using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer
{
    internal static class MouseExtensions
    {
        public static Point Clamp (this Point point, Rectangle bounds)
        {
            var x = Math.Min (point.X, bounds.Right);
            x = Math.Max (x, 0);

            var y = Math.Min (point.Y, bounds.Bottom);
            y = Math.Max (y, 0);

            return new Point (x, y);
        }
    }
}
