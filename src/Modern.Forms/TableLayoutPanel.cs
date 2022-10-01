// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Modern.Forms.Layout;

namespace Modern.Forms;

/// <summary>
/// Represents a TableLayoutPanel control.
/// </summary>
[ProvideProperty ("ColumnSpan", typeof (Control))]
[ProvideProperty ("RowSpan", typeof (Control))]
[ProvideProperty ("Row", typeof (Control))]
[ProvideProperty ("Column", typeof (Control))]
[ProvideProperty ("CellPosition", typeof (Control))]
[DefaultProperty (nameof (ColumnCount))]
public class TableLayoutPanel : Panel, IExtenderProvider
{
    private readonly TableLayoutSettings _tableLayoutSettings;
    //private static readonly object s_eventCellPaint = new object ();

    /// <summary>
    /// Initializes a new instance of the TableLayoutPanel class.
    /// </summary>
    public TableLayoutPanel ()
    {
        _tableLayoutSettings = TableLayout.CreateSettings (this);
    }

    /// <inheritdoc/>
    public override LayoutEngine LayoutEngine => TableLayout.Instance;

    /// <summary>
    /// Gets the layout settings associated with this TableLayoutPanel.
    /// </summary>
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public TableLayoutSettings LayoutSettings {
        get => _tableLayoutSettings;
#if DESIGN_TIME
        set {
            if (value is not null && value.IsStub) {
                // WINRES only scenario.
                // we only support table layout settings that have been created from a type converter.
                // this is here for localization (WinRes) support.
                using (new LayoutTransaction (this, this, PropertyNames.LayoutSettings)) {
                    // apply RowStyles, ColumnStyles, Row & Column assignments.
                    _tableLayoutSettings.ApplySettings (value);
                }
            } else {
                throw new NotSupportedException (SR.TableLayoutSettingSettingsIsNotSupported);
            }
        }
#endif
    }

    //[Browsable(false)]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //[Localizable(true)]
    //public new BorderStyle BorderStyle
    //{
    //    get => base.BorderStyle;
    //    set
    //    {
    //        base.BorderStyle = value;
    //        Debug.Assert(BorderStyle == value, "BorderStyle should be the same as we set it");
    //    }
    //}

    //[DefaultValue (TableLayoutPanelCellBorderStyle.None)]
    //[Localizable (true)]
    //public TableLayoutPanelCellBorderStyle CellBorderStyle {
    //    get { return _tableLayoutSettings.CellBorderStyle; }
    //    set {
    //        _tableLayoutSettings.CellBorderStyle = value;

    //        // PERF: don't turn on ResizeRedraw unless we know we need it.
    //        //if (value != TableLayoutPanelCellBorderStyle.None)
    //        //{
    //        //    SetStyle(ControlStyles.ResizeRedraw, true);
    //        //}

    //        Invalidate ();
    //        Debug.Assert (CellBorderStyle == value, "CellBorderStyle should be the same as we set it");
    //    }
    //}

    //private int CellBorderWidth {
    //    get { return _tableLayoutSettings.CellBorderWidth; }
    //}

    /// <summary>
    /// Gets the collection of controls contained by the control.
    /// </summary>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    public new TableLayoutControlCollection Controls => (TableLayoutControlCollection)base.Controls;

    /// <summary>
    ///  This sets the maximum number of columns allowed on this table instead of allocating
    ///  actual spaces for these columns. So it is OK to set ColumnCount to Int32.MaxValue without
    ///  causing out of memory exception
    /// </summary>
    [DefaultValue (0)]
    [Localizable (true)]
    public int ColumnCount {
        get => _tableLayoutSettings.ColumnCount;
        set {
            _tableLayoutSettings.ColumnCount = value;
            Debug.Assert (ColumnCount == value, "ColumnCount should be the same as we set it");
        }
    }

    /// <summary>
    ///  Specifies if a TableLayoutPanel will gain additional rows or columns once its existing cells
    ///  become full.  If the value is 'FixedSize' then the TableLayoutPanel will throw an exception
    ///  when the TableLayoutPanel is over-filled.
    /// </summary>
    [DefaultValue (TableLayoutPanelGrowStyle.AddRows)]
    public TableLayoutPanelGrowStyle GrowStyle {
        get => _tableLayoutSettings.GrowStyle;
        set => _tableLayoutSettings.GrowStyle = value;
    }

    /// <summary>
    ///  This sets the maximum number of rows allowed on this table instead of allocating
    ///  actual spaces for these rows. So it is OK to set RowCount to Int32.MaxValue without
    ///  causing out of memory exception
    /// </summary>
    [DefaultValue (0)]
    [Localizable (true)]
    public int RowCount {
        get => _tableLayoutSettings.RowCount;
        set => _tableLayoutSettings.RowCount = value;
    }

    /// <summary>
    /// Gets the collection of RowStyles for this TableLayoutPanel.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [DisplayName ("Rows")]
    [MergableProperty (false)]
    [Browsable (false)]
    public TableLayoutRowStyleCollection RowStyles => _tableLayoutSettings.RowStyles;

    /// <summary>
    /// Gets the collection of ColumnStyles for this TableLayoutPanel.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [DisplayName ("Columns")]
    [Browsable (false)]
    [MergableProperty (false)]
    public TableLayoutColumnStyleCollection ColumnStyles => _tableLayoutSettings.ColumnStyles;

    /// <inheritdoc/>
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    protected override ControlCollection CreateControlsInstance ()
    {
        return new TableLayoutControlCollection (this);
    }

    private bool ShouldSerializeControls ()
    {
        var collection = Controls;

        return collection is not null && collection.Count > 0;
    }

#region Extended Properties
    bool IExtenderProvider.CanExtend (object obj)
    {
        return obj is Control control && control.Parent == this;
    }

    /// <summary>
    /// Returns the column span value for the specified control.
    /// </summary>
    [DefaultValue (1)]
    [DisplayName ("ColumnSpan")]
    public int GetColumnSpan (Control control) => _tableLayoutSettings.GetColumnSpan (control);

    /// <summary>
    /// Sets the column span value for the specified control.
    /// </summary>
    public void SetColumnSpan (Control control, int value)
    {
        // layout.SetColumnSpan() throws ArgumentException if out of range.
        _tableLayoutSettings.SetColumnSpan (control, value);
        Debug.Assert (GetColumnSpan (control) == value, "GetColumnSpan should be the same as we set it");
    }

    /// <summary>
    /// Returns the row span value for the specified control.
    /// </summary>
    [DefaultValue (1)]
    [DisplayName ("RowSpan")]
    public int GetRowSpan (Control control) => _tableLayoutSettings.GetRowSpan (control);

    /// <summary>
    /// Sets the row span value for the specified control.
    /// </summary>
    public void SetRowSpan (Control control, int value)
    {
        // layout.SetRowSpan() throws ArgumentException if out of range.
        _tableLayoutSettings.SetRowSpan (control, value);
        Debug.Assert (GetRowSpan (control) == value, "GetRowSpan should be the same as we set it");
    }

    /// <summary>
    ///  Gets the row position of the specified control.
    /// </summary>
    [DefaultValue (-1)]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [DisplayName ("Row")]
    public int GetRow (Control control) => _tableLayoutSettings.GetRow (control);

    /// <summary>
    ///  Sets the row position of the specified control.
    /// </summary>
    public void SetRow (Control control, int row)
    {
        _tableLayoutSettings.SetRow (control, row);
        Debug.Assert (GetRow (control) == row, "GetRow should be the same as we set it");
    }

    /// <summary>
    ///  Gets the row and column position of the specified control.
    /// </summary>
#if DESIGN_TIME
    [DefaultValue (typeof (TableLayoutPanelCellPosition), "-1,-1")]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
#endif
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [DisplayName ("Cell")]    public TableLayoutPanelCellPosition GetCellPosition (Control control) => _tableLayoutSettings.GetCellPosition (control);

    /// <summary>
    ///  Sets the row and column position of the specified control.
    /// </summary>
    public void SetCellPosition (Control control, TableLayoutPanelCellPosition position) => _tableLayoutSettings.SetCellPosition (control, position);

    /// <summary>
    ///  Gets the column position of the specified control.
    /// </summary>
    [DefaultValue (-1)]  //if change this value, also change the SerializeViaAdd in TableLayoutControlCollectionCodeDomSerializer
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [DisplayName ("Column")]
    public int GetColumn (Control control) => _tableLayoutSettings.GetColumn (control);

    /// <summary>
    ///  Sets the column position of the specified control.
    /// </summary>
    public void SetColumn (Control control, int column)
    {
        _tableLayoutSettings.SetColumn (control, column);
        Debug.Assert (GetColumn (control) == column, "GetColumn should be the same as we set it");
    }

    /// <summary>
    ///  Gets the control which covers the specified row and column. return null if we can't find one
    /// </summary>
    public Control? GetControlFromPosition (int column, int row) => (Control?)_tableLayoutSettings.GetControlFromPosition (column, row);

    /// <summary>
    ///  Gets the row and column position of the specified control.
    /// </summary>
    public TableLayoutPanelCellPosition GetPositionFromControl (Control control) => _tableLayoutSettings.GetPositionFromControl (control);

    /// <summary>
    ///  Gets an array representing the widths (in pixels) of the columns in the TableLayoutPanel.
    /// </summary>
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public int[] GetColumnWidths ()
    {
        var containerInfo = TableLayout.GetContainerInfo (this);

        if (containerInfo.Columns is null)
            return Array.Empty<int> ();

        var cw = new int[containerInfo.Columns.Length];

        for (var i = 0; i < containerInfo.Columns.Length; i++)
            cw[i] = containerInfo.Columns[i].MinSize;

        return cw;
    }

    /// <summary>
    ///  Gets an array representing the heights (in pixels) of the rows in the TableLayoutPanel.
    /// </summary>
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public int[] GetRowHeights ()
    {
        var containerInfo = TableLayout.GetContainerInfo (this);

        if (containerInfo.Rows is null)
            return Array.Empty<int> ();

        var rh = new int[containerInfo.Rows.Length];

        for (var i = 0; i < containerInfo.Rows.Length; i++)
            rh[i] = containerInfo.Rows[i].MinSize;

        return rh;
    }
#endregion

#region PaintCode
    ///// <summary>
    ///// Raised when a cell needs to be painted.
    ///// </summary>
    //public event EventHandler<TableLayoutCellPaintEventArgs> CellPaint {
    //    add => Events.AddHandler (s_eventCellPaint, value);
    //    remove => Events.RemoveHandler (s_eventCellPaint, value);
    //}

    ///// <summary>
    /////  When a layout fires, make sure we're painting all of our
    /////  cell borders.
    ///// </summary>
    //[EditorBrowsable (EditorBrowsableState.Advanced)]
    //protected override void OnLayout (LayoutEventArgs levent)
    //{
    //    base.OnLayout (levent);
    //    Invalidate ();
    //}

    ///// <summary>
    ///// Raises the CellPaint event.
    ///// </summary>
    //protected virtual void OnCellPaint (TableLayoutCellPaintEventArgs e)
    //{
    //    (Events[s_eventCellPaint] as EventHandler<TableLayoutCellPaintEventArgs>)?.Invoke (this, e);
    //}

    // TODO: Custom Cell Paint
    //protected override void OnPaintBackground(PaintEventArgs e)
    //{
    //    base.OnPaintBackground(e);

    //    // paint borderstyles on top of the background image in WM_ERASEBKGND

    //    int cellBorderWidth = CellBorderWidth;
    //    TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
    //    TableLayout.Strip[] colStrips = containerInfo.Columns;
    //    TableLayout.Strip[] rowStrips = containerInfo.Rows;
    //    TableLayoutPanelCellBorderStyle cellBorderStyle = CellBorderStyle;

    //    if (colStrips is null || rowStrips is null)
    //    {
    //        return;
    //    }

    //    int cols = colStrips.Length;
    //    int rows = rowStrips.Length;

    //    int totalColumnWidths = 0, totalColumnHeights = 0;

    //    Rectangle displayRect = DisplayRectangle;
    //    Rectangle clipRect = e.ClipRectangle;

    //    Graphics g = e.GraphicsInternal;

    //    // Leave the space for the border
    //    int startx;
    //    bool isRTL = RightToLeft == RightToLeft.Yes;
    //    if (isRTL)
    //    {
    //        startx = displayRect.Right - (cellBorderWidth / 2);
    //    }
    //    else
    //    {
    //        startx = displayRect.X + (cellBorderWidth / 2);
    //    }

    //    for (int i = 0; i < cols; i++)
    //    {
    //        int starty = displayRect.Y + (cellBorderWidth / 2);

    //        if (isRTL)
    //        {
    //            startx -= colStrips[i].MinSize;
    //        }

    //        for (int j = 0; j < rows; j++)
    //        {
    //            Rectangle outsideCellBounds = new Rectangle(
    //                startx,
    //                starty,
    //                colStrips[i].MinSize,
    //                rowStrips[j].MinSize);

    //            Rectangle insideCellBounds = new Rectangle(
    //                outsideCellBounds.X + (cellBorderWidth + 1) / 2,
    //                outsideCellBounds.Y + (cellBorderWidth + 1) / 2,
    //                outsideCellBounds.Width - (cellBorderWidth + 1) / 2,
    //                outsideCellBounds.Height - (cellBorderWidth + 1) / 2);

    //            if (clipRect.IntersectsWith(insideCellBounds))
    //            {
    //                // First, call user's painting code
    //                using (var pcea = new TableLayoutCellPaintEventArgs(e, clipRect, insideCellBounds, i, j))
    //                {
    //                    OnCellPaint(pcea);
    //                    if (!((IGraphicsHdcProvider)pcea).IsGraphicsStateClean)
    //                    {
    //                        // The Graphics object got touched, hit the public property on our original args
    //                        // to mark it as dirty as well.

    //                        g = e.Graphics;
    //                    }
    //                }

    //                // Paint the table border on top.
    //                ControlPaint.PaintTableCellBorder(cellBorderStyle, g, outsideCellBounds);
    //            }

    //            starty += rowStrips[j].MinSize;

    //            // Only sum this up once...
    //            if (i == 0)
    //            {
    //                totalColumnHeights += rowStrips[j].MinSize;
    //            }
    //        }

    //        if (!isRTL)
    //        {
    //            startx += colStrips[i].MinSize;
    //        }

    //        totalColumnWidths += colStrips[i].MinSize;
    //    }

    //    if (!HScroll && !VScroll && cellBorderStyle != TableLayoutPanelCellBorderStyle.None)
    //    {
    //        // Paint the border of the table if we are not auto scrolling.

    //        Rectangle tableBounds = new Rectangle(
    //            cellBorderWidth / 2 + displayRect.X,
    //            cellBorderWidth / 2 + displayRect.Y,
    //            displayRect.Width - cellBorderWidth,
    //            displayRect.Height - cellBorderWidth);

    //        // If the borderStyle is Inset or Outset, we can only paint the lower bottom half since otherwise we
    //        // will have 1 pixel loss at the border.
    //        if (cellBorderStyle == TableLayoutPanelCellBorderStyle.Inset)
    //        {
    //            g.DrawLine(
    //                SystemPens.ControlDark,
    //                tableBounds.Right,
    //                tableBounds.Y,
    //                tableBounds.Right,
    //                tableBounds.Bottom);

    //            g.DrawLine(
    //                SystemPens.ControlDark,
    //                tableBounds.X,
    //                tableBounds.Y + tableBounds.Height - 1,
    //                tableBounds.X + tableBounds.Width - 1,
    //                tableBounds.Y + tableBounds.Height - 1);
    //        }
    //        else if (cellBorderStyle == TableLayoutPanelCellBorderStyle.Outset)
    //        {
    //            g.DrawLine(
    //                SystemPens.Window,
    //                tableBounds.X + tableBounds.Width - 1,
    //                tableBounds.Y,
    //                tableBounds.X + tableBounds.Width - 1,
    //                tableBounds.Y + tableBounds.Height - 1);
    //            g.DrawLine(
    //                SystemPens.Window,
    //                tableBounds.X,
    //                tableBounds.Y + tableBounds.Height - 1,
    //                tableBounds.X + tableBounds.Width - 1,
    //                tableBounds.Y + tableBounds.Height - 1);
    //        }
    //        else
    //        {
    //            ControlPaint.PaintTableCellBorder(cellBorderStyle, g, tableBounds);
    //        }

    //        ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRect);
    //    }
    //    else
    //    {
    //        ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRect);
    //    }
    //}

    /// <inheritdoc/>
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected override void ScaleCore (float dx, float dy)
    {
        base.ScaleCore (dx, dy);
        ScaleAbsoluteStyles (new SizeF (dx, dy));
    }

    ///// <summary>
    /////  Scale this form.  Form overrides this to enforce a maximum / minimum size.
    ///// </summary>
    //protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    //{
    //    base.ScaleControl(factor, specified);
    //    ScaleAbsoluteStyles(factor);
    //}

    private void ScaleAbsoluteStyles (SizeF factor)
    {
        var containerInfo = TableLayout.GetContainerInfo (this);
        var i = 0;

        // The last row/column can be larger than the
        // absolutely styled column width.
        var lastRowHeight = -1;
        var lastRow = containerInfo.Rows.Length - 1;

        if (containerInfo.Rows.Length > 0)
            lastRowHeight = containerInfo.Rows[lastRow].MinSize;

        var lastColumnHeight = -1;
        var lastColumn = containerInfo.Columns.Length - 1;

        if (containerInfo.Columns.Length > 0)
            lastColumnHeight = containerInfo.Columns[containerInfo.Columns.Length - 1].MinSize;

        foreach (ColumnStyle cs in ColumnStyles) {
            if (cs.SizeType == SizeType.Absolute) {
                if (i == lastColumn && lastColumnHeight > 0) {
                    // the last column is typically expanded to fill the table. use the actual
                    // width in this case.
                    cs.Width = (float)Math.Round (lastColumnHeight * factor.Width);
                } else {
                    cs.Width = (float)Math.Round (cs.Width * factor.Width);
                }
            }

            i++;
        }

        i = 0;

        foreach (RowStyle rs in RowStyles) {
            if (rs.SizeType == SizeType.Absolute) {
                if (i == lastRow && lastRowHeight > 0) {
                    // the last row is typically expanded to fill the table. use the actual
                    // width in this case.
                    rs.Height = (float)Math.Round (lastRowHeight * factor.Height);
                } else {
                    rs.Height = (float)Math.Round (rs.Height * factor.Height);
                }
            }
        }
    }
#endregion
}
