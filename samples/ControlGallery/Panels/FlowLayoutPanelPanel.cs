using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels;

public class FlowLayoutPanelPanel : Panel
{
    private readonly SKColor[] colors = new[] {
        SKColors.CornflowerBlue,
        SKColors.LightPink,
        SKColors.LightSeaGreen,
        SKColors.LightYellow,
        SKColors.LightCoral,
        SKColors.LightGray,
        SKColors.LightGreen,
        SKColors.LightGoldenrodYellow
    };

    public FlowLayoutPanelPanel ()
    {
        var container = Controls.Add (new SplitContainer { Orientation = Orientation.Vertical, SplitterColor = SKColors.DarkGray });

        var ltr = container.Panel1.Controls.Add (new FlowLayoutPanel { Dock = DockStyle.Fill });
        var ttb = container.Panel2.Controls.Add (new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown });

        foreach (var color in colors)
            ltr.Controls.Add (CreatePanel (color));

        foreach (var color in colors)
            ttb.Controls.Add (CreatePanel (color));
    }

    private static Panel CreatePanel (SKColor color)
    {
        var panel = new Panel { Height = 100, Width = 100 };
        panel.Style.BackgroundColor = color;

        return panel;
    }
}
