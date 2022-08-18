using Xunit;

namespace Modern.Forms.Tests
{
    public class ControlTests
    {
        [Fact]
        public void GetNextControl_BasicTabIndex ()
        {
            var container = new Control ();
            var controls = new Control[5];

            for (var i = 0; i < 5; i++) {
                controls[i] = new Control {
                    TabIndex = i,
                    Text = "ctrl " + i
                };

                container.Controls.Add (controls[i]);
            }

            Assert.Equal (controls[0], container.GetNextControl (null, true));
            Assert.Equal (controls[4], container.GetNextControl (null, false));

            Assert.Equal (controls[1], container.GetNextControl (controls[0], true));
            Assert.Null (container.GetNextControl (controls[0], false));

            Assert.Equal (controls[2], container.GetNextControl (controls[1], true));
            Assert.Equal (controls[0], container.GetNextControl (controls[1], false));

            Assert.Equal (controls[3], container.GetNextControl (controls[2], true));
            Assert.Equal (controls[1], container.GetNextControl (controls[2], false));

            Assert.Equal (controls[4], container.GetNextControl (controls[3], true));
            Assert.Equal (controls[2], container.GetNextControl (controls[3], false));

            Assert.Null (container.GetNextControl (controls[4], true));
            Assert.Equal (controls[3], container.GetNextControl (controls[4], false));

            container.Dispose ();
        }

        [Fact]
        public void GetNextControl_ReverseTabIndex ()
        {
            var container = new Control ();
            var controls = new Control[5];

            for (var i = 0; i < 5; i++) {
                controls[i] = new Control {
                    TabIndex = 5 - i,
                    Text = "ctrl " + i
                };

                container.Controls.Add (controls[i]);
            }

            Assert.Equal ("ctrl 4", container.GetNextControl (null, true).Text);
            Assert.Equal ("ctrl 0", container.GetNextControl (null, false).Text);

            // Ignores passed in controls that are not child controls
            Assert.Equal ("ctrl 4", container.GetNextControl (new Control (), true).Text);
            Assert.Equal ("ctrl 0", container.GetNextControl (new Control (), false).Text);

            Assert.Null (container.GetNextControl (controls[0], true));
            Assert.Equal ("ctrl 1", container.GetNextControl (controls[0], false).Text);

            Assert.Equal ("ctrl 0", container.GetNextControl (controls[1], true).Text);
            Assert.Equal ("ctrl 2", container.GetNextControl (controls[1], false).Text);

            Assert.Equal ("ctrl 1", container.GetNextControl (controls[2], true).Text);
            Assert.Equal ("ctrl 3", container.GetNextControl (controls[2], false).Text);

            Assert.Equal ("ctrl 2", container.GetNextControl (controls[3], true).Text);
            Assert.Equal ("ctrl 4", container.GetNextControl (controls[3], false).Text);

            Assert.Equal ("ctrl 3", container.GetNextControl (controls[4], true).Text);
            Assert.Null (container.GetNextControl (controls[4], false));

            container.Dispose ();
        }

        [Fact]
        public void GetNextControl_DuplicateTabIndex ()
        {
            var container = new Control ();
            var controls = new Control[5];

            for (var i = 0; i < 5; i++) {
                controls[i] = new Control {
                    TabIndex = i,
                    Text = "ctrl " + i
                };

                container.Controls.Add (controls[i]);
            }

            controls[3].TabIndex = 2;

            Assert.Equal ("ctrl 0", container.GetNextControl (null, true).Text);
            Assert.Equal ("ctrl 4", container.GetNextControl (null, false).Text);

            Assert.Equal ("ctrl 1", container.GetNextControl (controls[0], true).Text);
            Assert.Null (container.GetNextControl (controls[0], false));

            Assert.Equal ("ctrl 2", container.GetNextControl (controls[1], true).Text);
            Assert.Equal ("ctrl 0", container.GetNextControl (controls[1], false).Text);

            Assert.Equal ("ctrl 3", container.GetNextControl (controls[2], true).Text);
            Assert.Equal ("ctrl 1", container.GetNextControl (controls[2], false).Text);

            Assert.Equal ("ctrl 4", container.GetNextControl (controls[3], true).Text);
            Assert.Equal ("ctrl 2", container.GetNextControl (controls[3], false).Text);

            Assert.Null (container.GetNextControl (controls[4], true));
            Assert.Equal ("ctrl 3", container.GetNextControl (controls[4], false).Text);

            container.Dispose ();
        }
        [Fact]
        public void GetNextControl_NestedControls ()
        {
            // - Form
            //   - Button 1
            //   - Panel 1  (Panels are not selectable and are not IContainerControl
            //     - Button 2
            //   - UserControl 1  (UserControls are selectable and is IContainerControl)
            //     - Button 3
            //   - Button 4

            var f = new Form ();

            var b1 = new Button { Text = "Button 1" };
            var b2 = new Button { Text = "Button 2" };
            var b3 = new Button { Text = "Button 3" };
            var b4 = new Button { Text = "Button 4", Top = 90 };

            f.Controls.Add (b1);

            var p1 = new Panel { Text = "Panel 1", Top = 30, Height = 30 };
            p1.Controls.Add (b2);
            f.Controls.Add (p1);

            var uc1 = new Control { Text = "UserControl 1", Top = 60, Height = 30 };
            uc1.Controls.Add (b3);
            f.Controls.Add (uc1);

            f.Controls.Add (b4);

            // Button 1 as "this"
            Assert.Null (b1.GetNextControl (b1, true));
            Assert.Null (b1.GetNextControl (p1, true));
            Assert.Null (b1.GetNextControl (b2, true));
            Assert.Null (b1.GetNextControl (uc1, true));
            Assert.Null (b1.GetNextControl (b3, true));
            Assert.Null (b1.GetNextControl (b4, true));

            // Panel 1 as "this"
            Assert.Equal ("Button 2", p1.GetNextControl (b1, true).Text);
            Assert.Equal ("Button 2", p1.GetNextControl (p1, true).Text);
            Assert.Null (p1.GetNextControl (b2, true));
            Assert.Equal ("Button 2", p1.GetNextControl (uc1, true).Text);
            Assert.Equal ("Button 2", p1.GetNextControl (b3, true).Text);
            Assert.Equal ("Button 2", p1.GetNextControl (b4, true).Text);

            // Form as "this"
            Assert.Equal ("Panel 1", f.GetNextControl (b1, true).Text);
            Assert.Equal ("Button 2", f.GetNextControl (p1, true).Text);
            Assert.Equal ("UserControl 1", f.GetNextControl (b2, true).Text);
            //Assert.Equal ("Button 4", f.GetNextControl (uc1, true).Text);
            Assert.Equal ("Button 4", f.GetNextControl (b3, true).Text);
            Assert.Null (f.GetNextControl (b4, true));
        }
    }
}
