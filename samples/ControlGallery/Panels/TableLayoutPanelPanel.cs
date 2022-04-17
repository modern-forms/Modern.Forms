using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels;

public class TableLayoutPanelPanel : TableLayoutPanel
{
    private readonly SKColor[] colors = new[] { 
        SKColors.CornflowerBlue, 
        SKColors.LightPink, 
        SKColors.LightSeaGreen, 
        SKColors.LightYellow, 
        SKColors.LightCoral,
        SKColors.LightGray,
        SKColors.LightGreen,
        SKColors.LightGoldenrodYellow,
        SKColors.Violet,
        SKColors.Orange
    };
    
    public TableLayoutPanelPanel ()
    {
        ColumnCount = 2;
        RowCount = 5;

        ColumnStyles.Add (new ColumnStyle (SizeType.Percent, 50));
        ColumnStyles.Add (new ColumnStyle (SizeType.Percent, 50));

        RowStyles.Add (new RowStyle (SizeType.Percent, 20));
        RowStyles.Add (new RowStyle (SizeType.Percent, 20));
        RowStyles.Add (new RowStyle (SizeType.Percent, 20));
        RowStyles.Add (new RowStyle (SizeType.Percent, 20));
        RowStyles.Add (new RowStyle (SizeType.Percent, 20));

        for (var row = 0; row < RowCount; row++)
            for (var col = 0; col < ColumnCount; col++)
                Controls.Add (CreatePanel (colors[(row * ColumnCount) + col], row, col));
    }

    private Panel CreatePanel (SKColor color, int row, int col)
    {
        var panel = new Panel { Height = 100, Width = 100 };
        panel.Style.BackgroundColor = color;

        LayoutSettings.SetColumn (panel, col);
        LayoutSettings.SetRow (panel, row);

        return panel;
    }
}
