using System;
using Xunit;

namespace Modern.Forms.Tests
{
    public class ScrollableControlTests
    {
        [Fact]
        public void ClientSize ()
        {
            var control = new ScrollableControl {
                Width = 100,
                Height = 100
            };

            Assert.Equal (100, control.ClientSize.Width);
            Assert.Equal (100, control.ClientSize.Height);

            control.Padding = new Padding (15);

            Assert.Equal (100, control.ClientSize.Width);
            Assert.Equal (100, control.ClientSize.Height);
        }

        [Fact]
        public void ClientRectangle ()
        {
            var control = new ScrollableControl {
                Width = 100,
                Height = 100
            };

            Assert.Equal (0, control.ClientRectangle.Left);
            Assert.Equal (0, control.ClientRectangle.Top);
            Assert.Equal (100, control.ClientRectangle.Width);
            Assert.Equal (100, control.ClientRectangle.Height);

            control.Padding = new Padding (15);

            Assert.Equal (0, control.ClientRectangle.Left);
            Assert.Equal (0, control.ClientRectangle.Top);
            Assert.Equal (100, control.ClientRectangle.Width);
            Assert.Equal (100, control.ClientRectangle.Height);
        }

        [Fact]
        public void DisplayRectangle ()
        {
            var control = new ScrollableControl {
                Width = 100,
                Height = 100
            };

            Assert.Equal (0, control.DisplayRectangle.Left);
            Assert.Equal (0, control.DisplayRectangle.Top);
            Assert.Equal (100, control.DisplayRectangle.Width);
            Assert.Equal (100, control.DisplayRectangle.Height);

            control.Padding = new Padding (15);

            Assert.Equal (15, control.DisplayRectangle.Left);
            Assert.Equal (15, control.DisplayRectangle.Top);
            Assert.Equal (70, control.DisplayRectangle.Width);
            Assert.Equal (70, control.DisplayRectangle.Height);
        }
    }
}
