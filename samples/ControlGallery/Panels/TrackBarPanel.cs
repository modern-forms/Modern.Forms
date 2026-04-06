using Modern.Forms;

namespace ControlGallery.Panels
{
    /// <summary>
    /// Demonstrates different <see cref="TrackBar"/> configurations in the control gallery.
    /// </summary>
    /// <remarks>
    /// This panel showcases:
    /// <list type="bullet">
    /// <item><description>A basic horizontal track bar.</description></item>
    /// <item><description>A horizontal track bar with tick marks on both sides.</description></item>
    /// <item><description>A vertical track bar.</description></item>
    /// <item><description>A snapped track bar that rounds values to tick marks.</description></item>
    /// <item><description>A disabled track bar.</description></item>
    /// </list>
    /// </remarks>
    public class TrackBarPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBarPanel"/> class.
        /// </summary>
        public TrackBarPanel ()
        {
            CreateHorizontalExamples ();
            CreateVerticalExamples ();
            CreateOptionsSection ();
        }

        private void CreateHorizontalExamples ()
        {
            Controls.Add (new Label {
                Text = "Horizontal",
                Left = 10,
                Top = 10,
                Width = 200,
                Height = 25
            });

            var horizontal_value_label = Controls.Add (new Label {
                Text = "Value: 25",
                Left = 320,
                Top = 45,
                Width = 120,
                Height = 25
            });

            var horizontal = Controls.Add (new TrackBar {
                Left = 10,
                Top = 40,
                Width = 280,
                Minimum = 0,
                Maximum = 100,
                Value = 25,
                TickFrequency = 10,
                TickStyle = TickStyle.BottomRight
            });

            horizontal.ValueChanged += (sender, e) => {
                horizontal_value_label.Text = $"Value: {horizontal.Value}";
            };

            var both_ticks_value_label = Controls.Add (new Label {
                Text = "Value: 60",
                Left = 320,
                Top = 105,
                Width = 120,
                Height = 25
            });

            var both_ticks = Controls.Add (new TrackBar {
                Left = 10,
                Top = 100,
                Width = 280,
                Minimum = 0,
                Maximum = 100,
                Value = 60,
                TickFrequency = 5,
                TickStyle = TickStyle.Both
            });

            both_ticks.ValueChanged += (sender, e) => {
                both_ticks_value_label.Text = $"Value: {both_ticks.Value}";
            };

            Controls.Add (new Label {
                Text = "Snap to ticks",
                Left = 10,
                Top = 160,
                Width = 200,
                Height = 25
            });

            var snap_value_label = Controls.Add (new Label {
                Text = "Value: 30",
                Left = 320,
                Top = 195,
                Width = 120,
                Height = 25
            });

            var snapped = Controls.Add (new TrackBar {
                Left = 10,
                Top = 190,
                Width = 280,
                Minimum = 0,
                Maximum = 100,
                Value = 30,
                TickFrequency = 10,
                TickStyle = TickStyle.TopLeft,
                SnapToTicks = true
            });

            snapped.ValueChanged += (sender, e) => {
                snap_value_label.Text = $"Value: {snapped.Value}";
            };

            Controls.Add (new Label {
                Text = "Disabled",
                Left = 10,
                Top = 250,
                Width = 200,
                Height = 25
            });

            Controls.Add (new TrackBar {
                Left = 10,
                Top = 280,
                Width = 280,
                Minimum = 0,
                Maximum = 100,
                Value = 45,
                TickFrequency = 10,
                TickStyle = TickStyle.BottomRight,
                Enabled = false
            });
        }

        private void CreateVerticalExamples ()
        {
            Controls.Add (new Label {
                Text = "Vertical",
                Left = 500,
                Top = 10,
                Width = 100,
                Height = 25
            });

            var vertical_value_label = Controls.Add (new Label {
                Text = "Value: 75",
                Left = 470,
                Top = 45,
                Width = 120,
                Height = 25
            });

            var vertical = Controls.Add (new TrackBar {
                Left = 500,
                Top = 80,
                Width = 32,
                Height = 220,
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = 100,
                Value = 75,
                TickFrequency = 10,
                TickStyle = TickStyle.Both
            });

            vertical.ValueChanged += (sender, e) => {
                vertical_value_label.Text = $"Value: {vertical.Value}";
            };
        }

        private void CreateOptionsSection ()
        {
            Controls.Add (new Label {
                Text = "Interactive example",
                Left = 10,
                Top = 360,
                Width = 200,
                Height = 25
            });

            var interactive = Controls.Add (new TrackBar {
                Left = 10,
                Top = 390,
                Width = 280,
                Minimum = 0,
                Maximum = 50,
                Value = 10,
                TickFrequency = 5,
                TickStyle = TickStyle.BottomRight
            });

            var value_label = Controls.Add (new Label {
                Text = "Value: 10",
                Left = 320,
                Top = 395,
                Width = 120,
                Height = 25
            });

            var snap_checkbox = Controls.Add (new CheckBox {
                Text = "Snap to ticks",
                Left = 320,
                Top = 425,
                Width = 150,
                Checked = false
            });

            var tick_style_combo = Controls.Add (new ComboBox {
                Left = 320,
                Top = 455,
                Width = 150
            });

            tick_style_combo.Items.AddRange (Enum.GetNames<TickStyle> ());
            tick_style_combo.SelectedItem = TickStyle.BottomRight.ToString ();

            interactive.ValueChanged += (sender, e) => {
                value_label.Text = $"Value: {interactive.Value}";
            };

            snap_checkbox.CheckedChanged += (sender, e) => {
                interactive.SnapToTicks = snap_checkbox.Checked;
            };

            tick_style_combo.SelectedIndexChanged += (sender, e) => {
                if (tick_style_combo.SelectedItem is string selected_text)
                    interactive.TickStyle = Enum.Parse<TickStyle> (selected_text);
            };
        }
    }
}
