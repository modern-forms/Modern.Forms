using System.Drawing;
using SkiaSharp;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a DataGridView.
    /// </summary>
    public class DataGridViewRenderer : Renderer<DataGridView>
    {
        /// <inheritdoc/>
        protected override void Render (DataGridView control, PaintEventArgs e)
        {
            var content = control.GetContentArea ();

            e.Canvas.Save ();
            e.Canvas.Clip (content);

            // Draw column headers
            if (control.ColumnHeadersVisible)
                RenderColumnHeaders (control, e, content);

            // Draw rows
            RenderRows (control, e, content);

            e.Canvas.Restore ();
        }

        /// <summary>
        /// Renders the column headers.
        /// </summary>
        protected virtual void RenderColumnHeaders (DataGridView control, PaintEventArgs e, Rectangle contentArea)
        {
            var header_height = control.ScaledHeaderHeight;
            var x = contentArea.Left - control.HorizontalScrollOffset;
            var y = contentArea.Top;

            // Draw header background
            var header_rect = new Rectangle (contentArea.Left, y, contentArea.Width, header_height);
            e.Canvas.FillRectangle (header_rect, Theme.ControlMidColor);

            for (var i = 0; i < control.Columns.Count; i++) {
                var column = control.Columns[i];

                if (!column.Visible)
                    continue;

                var col_width = control.LogicalToDeviceUnits (column.Width);
                var cell_rect = new Rectangle (x, y, col_width, header_height);

                column.HeaderBounds = cell_rect;

                RenderColumnHeader (control, column, i, cell_rect, e);

                x += col_width;
            }

            // Draw header bottom border
            e.Canvas.DrawLine (contentArea.Left, y + header_height - 1, contentArea.Right, y + header_height - 1, Theme.BorderMidColor);
        }

        /// <summary>
        /// Renders a single column header.
        /// </summary>
        protected virtual void RenderColumnHeader (DataGridView control, DataGridViewColumn column, int columnIndex, Rectangle bounds, PaintEventArgs e)
        {
            // Draw right border
            e.Canvas.DrawLine (bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom, Theme.BorderLowColor);

            // Draw text
            var text_bounds = bounds;
            text_bounds.Inflate (-6, 0);

            e.Canvas.DrawText (column.HeaderText, Theme.UIFontBold, control.LogicalToDeviceUnits (Theme.ItemFontSize), text_bounds, Theme.ForegroundColor, ContentAlignment.MiddleLeft, maxLines: 1);

            // Draw sort indicator
            if (column.SortOrder != SortOrder.None)
                RenderSortGlyph (e, bounds, column.SortOrder);
        }

        /// <summary>
        /// Renders the sort direction glyph.
        /// </summary>
        protected virtual void RenderSortGlyph (PaintEventArgs e, Rectangle bounds, SortOrder sortOrder)
        {
            var glyph_size = 6;
            var glyph_x = bounds.Right - glyph_size - 8;
            var glyph_y = bounds.Top + (bounds.Height - glyph_size) / 2;

            using var path = new SKPath ();

            if (sortOrder == SortOrder.Ascending) {
                path.MoveTo (glyph_x, glyph_y + glyph_size);
                path.LineTo (glyph_x + glyph_size / 2, glyph_y);
                path.LineTo (glyph_x + glyph_size, glyph_y + glyph_size);
                path.Close ();
            } else {
                path.MoveTo (glyph_x, glyph_y);
                path.LineTo (glyph_x + glyph_size / 2, glyph_y + glyph_size);
                path.LineTo (glyph_x + glyph_size, glyph_y);
                path.Close ();
            }

            using var paint = new SKPaint { Color = Theme.ForegroundColor, IsAntialias = true };
            e.Canvas.DrawPath (path, paint);
        }

        /// <summary>
        /// Renders the data rows.
        /// </summary>
        protected virtual void RenderRows (DataGridView control, PaintEventArgs e, Rectangle contentArea)
        {
            var row_height = control.ScaledRowHeight;
            var header_offset = control.ColumnHeadersVisible ? control.ScaledHeaderHeight : 0;
            var y = contentArea.Top + header_offset;

            for (var i = control.FirstVisibleIndex; i < control.Rows.Count; i++) {
                if (y >= contentArea.Bottom)
                    break;

                var row = control.Rows[i];
                var row_rect = new Rectangle (contentArea.Left, y, contentArea.Width, Math.Min (row_height, contentArea.Bottom - y));

                row.Bounds = row_rect;

                RenderRow (control, row, i, row_rect, e);

                y += row_height;
            }
        }

        /// <summary>
        /// Renders a single row.
        /// </summary>
        protected virtual void RenderRow (DataGridView control, DataGridViewRow row, int rowIndex, Rectangle bounds, PaintEventArgs e)
        {
            // Draw selection background
            if (control.SelectedRowIndex == rowIndex)
                e.Canvas.FillRectangle (bounds, Theme.ControlHighlightLowColor);
            // Draw hover background
            else if (control.HoveredRowIndex == rowIndex)
                e.Canvas.FillRectangle (bounds, Theme.ControlMidColor);
            // Draw alternating row background
            else if (rowIndex % 2 == 1)
                e.Canvas.FillRectangle (bounds, AlternatingRowColor ());

            // Draw cells
            var x = bounds.Left - control.HorizontalScrollOffset;

            for (var i = 0; i < control.Columns.Count; i++) {
                var column = control.Columns[i];

                if (!column.Visible)
                    continue;

                var col_width = control.LogicalToDeviceUnits (column.Width);
                var cell_rect = new Rectangle (x, bounds.Top, col_width, bounds.Height);

                var cell_value = i < row.Cells.Count ? row.Cells[i].Value : string.Empty;

                if (i < row.Cells.Count)
                    row.Cells[i].Bounds = cell_rect;

                RenderCell (control, cell_value, rowIndex, i, cell_rect, e);

                x += col_width;
            }

            // Draw row bottom border
            e.Canvas.DrawLine (bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1, Theme.BorderLowColor);
        }

        /// <summary>
        /// Renders a single cell.
        /// </summary>
        protected virtual void RenderCell (DataGridView control, string value, int rowIndex, int columnIndex, Rectangle bounds, PaintEventArgs e)
        {
            // Draw cell right border
            e.Canvas.DrawLine (bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom, Theme.BorderLowColor);

            // Draw cell selection for cell mode
            if (control.SelectionMode != DataGridViewSelectionMode.FullRowSelect && control.SelectedRowIndex == rowIndex && control.SelectedColumnIndex == columnIndex)
                e.Canvas.DrawRectangle (bounds, Theme.AccentColor, 2);

            // Draw text
            var text_bounds = bounds;
            text_bounds.Inflate (-4, 0);
            text_bounds.Height = control.ScaledRowHeight;  // Ensure consistent text positioning

            e.Canvas.DrawText (value, Theme.UIFont, control.LogicalToDeviceUnits (Theme.ItemFontSize), text_bounds, Theme.ForegroundColor, ContentAlignment.MiddleLeft, maxLines: 1);
        }

        /// <summary>
        /// Gets the alternating row background color.
        /// </summary>
        private static SKColor AlternatingRowColor ()
        {
            // Slightly different from the default background
            var bg = Theme.ControlLowColor;
            return new SKColor (
                (byte)Math.Max (0, bg.Red - 5),
                (byte)Math.Max (0, bg.Green - 5),
                (byte)Math.Max (0, bg.Blue - 5),
                bg.Alpha
            );
        }
    }
}
