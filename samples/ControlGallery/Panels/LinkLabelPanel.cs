using System.Drawing;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels
{
    /// <summary>
    /// Demonstrates the LinkLabel control with various configurations.
    /// </summary>
    public class LinkLabelPanel : Panel
    {
        private readonly Label output;

        public LinkLabelPanel ()
        {
            Padding = new Padding (20);

            output = new Label {
                Text = "Output: ",
                Location = new Point (20, 400),
                Width = 500,
                Height = 30
            };

            Controls.Add (output);

            // === Simple single link ===
            var simple = new LinkLabel {
                Text = "Click here to open documentation",
                Location = new Point (20, 20),
                Width = 400,
                Height = 30,
                HoverLinkColor = SKColors.Red,
                VisitedLinkColor = SKColors.Purple
            };

            simple.LinkClicked += (s, e) => {
                output.Text = "Simple link clicked";
            };

            Controls.Add (simple);

            // === Multiple links ===
            var multi = new LinkLabel {
                Text = "Visit docs or support page",
                Location = new Point (20, 70),
                Width = 400,
                Height = 30
            };

            multi.Links.Clear ();
            multi.Links.Add (0, 10, "docs");
            multi.Links.Add (14, 7, "support");

            multi.LinkClicked += (s, e) => {
                output.Text = $"Clicked: {e.Link?.LinkData}";
            };

            Controls.Add (multi);

            // === Visited links ===
            var visited = new LinkLabel {
                Text = "Visited link example",
                Location = new Point (20, 120),
                LinkVisited = true,
                Width = 400,
                Height = 30
            };

            visited.LinkClicked += (s, e) => {
                output.Text = "Visited link clicked";
            };

            Controls.Add (visited);

            // === Disabled link ===
            var disabled = new LinkLabel {
                Text = "Disabled link example",
                Location = new Point (20, 170),
                Width = 400,
                Height = 30
            };

            disabled.Links[0].Enabled = false;

            Controls.Add (disabled);

            // === Hover underline only ===
            var hover = new LinkLabel {
                Text = "Hover underline example",
                Location = new Point (20, 220),
                Width = 400,
                Height = 30,
                LinkBehavior = LinkBehavior.HoverUnderline,
                HoverLinkColor = SKColors.Red
            };

            hover.LinkClicked += (s, e) => {
                output.Text = "Hover link clicked";
            };

            Controls.Add (hover);

            // === Custom colors ===
            var custom = new LinkLabel {
                Text = "Custom color link",
                Location = new Point (20, 270),
                Width = 400,
                Height = 30,
                LinkColor = SKColors.Green,
                ActiveLinkColor = SKColors.Red,
                VisitedLinkColor = SKColors.Purple
            };

            custom.LinkClicked += (s, e) => {
                output.Text = "Custom color link clicked";
            };

            Controls.Add (custom);

            // === Keyboard navigation demo ===
            var keyboard = new LinkLabel {
                Text = "Use TAB / arrows to switch links",
                Location = new Point (20, 320),
                Width = 400,
                Height = 30
            };

            keyboard.Links.Clear ();
            keyboard.Links.Add (4, 3, "tab");
            keyboard.Links.Add (10, 6, "arrows");

            keyboard.LinkClicked += (s, e) => {
                output.Text = $"Keyboard link: {e.Link?.LinkData}";
            };

            Controls.Add (keyboard);
        }
    }
}
