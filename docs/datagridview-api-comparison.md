# DataGridView: WinForms vs Modern.Forms API Comparison

This document compares every public member on the WinForms `System.Windows.Forms.DataGridView` with the current Modern.Forms `DataGridView` implementation.

**Legend:**
- ✅ = Implemented
- ⚠️ = Partially implemented / different signature
- ❌ = Not implemented

---

## Properties

| WinForms Property | Modern.Forms | Status | Notes |
|---|---|---|---|
| `AdjustedTopLeftHeaderCell` | — | ❌ | |
| `AdvancedCellBorderStyle` | — | ❌ | |
| `AdvancedColumnHeadersBorderStyle` | — | ❌ | |
| `AdvancedRowHeadersBorderStyle` | — | ❌ | |
| `AllowUserToAddRows` | — | ❌ | |
| `AllowUserToDeleteRows` | — | ❌ | |
| `AllowUserToOrderColumns` | — | ❌ | |
| `AllowUserToResizeColumns` | `AllowUserToResizeColumns` | ✅ | |
| `AllowUserToResizeRows` | `AllowUserToResizeRows` | ✅ | |
| `AlternatingRowsDefaultCellStyle` | `AlternatingRowsDefaultCellStyle` | ✅ | |
| `AutoGenerateColumns` | — | ❌ | Auto-generation happens implicitly via `DataSource` |
| `AutoSizeColumnsMode` | — | ❌ | |
| `AutoSizeRowsMode` | — | ❌ | |
| `BackgroundColor` | `Style.BackgroundColor` | ⚠️ | Via ControlStyle |
| `BorderStyle` | `Style.Border` | ⚠️ | Via ControlStyle |
| `CellBorderStyle` | — | ❌ | |
| `ClipboardCopyMode` | — | ❌ | |
| `ColumnCount` | `Columns.Count` | ⚠️ | No direct property; use collection |
| `ColumnHeadersBorderStyle` | — | ❌ | |
| `ColumnHeadersDefaultCellStyle` | `ColumnHeadersDefaultCellStyle` | ✅ | |
| `ColumnHeadersHeight` | `ColumnHeadersHeight` | ✅ | |
| `ColumnHeadersHeightSizeMode` | — | ❌ | |
| `ColumnHeadersVisible` | `ColumnHeadersVisible` | ✅ | |
| `Columns` | `Columns` | ✅ | |
| `CurrentCell` | `CurrentCell` | ✅ | |
| `CurrentCellAddress` | `CurrentCellAddress` | ✅ | Returns `Point` |
| `CurrentRow` | `CurrentRow` | ✅ | |
| `DataMember` | — | ❌ | |
| `DataSource` | `DataSource` | ✅ | |
| `DefaultCellStyle` | `DefaultCellStyle` | ✅ | |
| `DisplayedColumnCount` | — | ❌ | |
| `DisplayedRowCount` | `DisplayedRowCount` | ✅ | |
| `EditingCell` | — | ❌ | |
| `EditingControl` | — | ❌ | Internal `edit_textbox` field exists |
| `EditingPanel` | — | ❌ | |
| `EditMode` | — | ❌ | Currently double-click or F2 only |
| `EnableHeadersVisualStyles` | — | ❌ | |
| `FirstDisplayedCell` | — | ❌ | |
| `FirstDisplayedScrollingColumnHiddenWidth` | — | ❌ | |
| `FirstDisplayedScrollingColumnIndex` | — | ❌ | |
| `FirstDisplayedScrollingRowIndex` | `FirstDisplayedScrollingRowIndex` | ✅ | |
| `GridColor` | — | ❌ | Uses theme colors |
| `HorizontalScrollingOffset` | `HorizontalScrollOffset` | ⚠️ | Internal, named differently |
| `IsCurrentCellDirty` | — | ❌ | |
| `IsCurrentCellInEditMode` | `IsCurrentCellInEditMode` | ✅ | |
| `IsCurrentRowDirty` | — | ❌ | |
| `Item[Int32, Int32]` (indexer) | — | ❌ | |
| `MultiSelect` | — | ❌ | |
| `NewRowIndex` | — | ❌ | |
| `ReadOnly` | `ReadOnly` | ✅ | |
| `RowCount` | `Rows.Count` | ⚠️ | No direct property; use collection |
| `RowHeadersBorderStyle` | — | ❌ | |
| `RowHeadersDefaultCellStyle` | `RowHeadersDefaultCellStyle` | ✅ | |
| `RowHeadersVisible` | `RowHeadersVisible` | ✅ | |
| `RowHeadersWidth` | `RowHeadersWidth` | ✅ | |
| `RowHeadersWidthSizeMode` | — | ❌ | |
| `Rows` | `Rows` | ✅ | |
| `RowsDefaultCellStyle` | — | ❌ | |
| `RowTemplate` | — | ❌ | |
| `ScrollBars` | — | ❌ | Scrollbars are always auto-managed |
| `SelectedCells` | — | ❌ | |
| `SelectedColumns` | — | ❌ | |
| `SelectedRows` | — | ❌ | `SelectedRowIndex` provides single selection |
| `SelectionMode` | `SelectionMode` | ✅ | Uses `DataGridViewSelectionMode` enum |
| `ShowCellErrors` | — | ❌ | |
| `ShowCellToolTips` | — | ❌ | |
| `ShowEditingIcon` | — | ❌ | |
| `ShowRowErrors` | — | ❌ | |
| `SortedColumn` | — | ❌ | Sorting is implemented but no property to retrieve sorted column |
| `SortOrder` | — | ❌ | Available on individual columns |
| `StandardTab` | — | ❌ | |
| `TopLeftHeaderCell` | — | ❌ | |
| `UserSetCursor` | — | ❌ | |
| `VirtualMode` | — | ❌ | |

## Methods

| WinForms Method | Modern.Forms | Status | Notes |
|---|---|---|---|
| `AdjustColumnHeaderBorderStyle()` | — | ❌ | |
| `AreAllCellsSelected()` | — | ❌ | |
| `AutoResizeColumn()` | — | ❌ | |
| `AutoResizeColumnHeadersHeight()` | — | ❌ | |
| `AutoResizeColumns()` | — | ❌ | |
| `AutoResizeRow()` | — | ❌ | |
| `AutoResizeRowHeadersWidth()` | — | ❌ | |
| `AutoResizeRows()` | — | ❌ | |
| `BeginEdit()` | `BeginEdit(int, int)` | ⚠️ | WinForms takes `bool selectAll` only; Modern.Forms takes row/column indices |
| `CancelEdit()` | `CancelEdit()` | ✅ | |
| `ClearSelection()` | — | ❌ | |
| `CommitEdit()` | — | ❌ | `EndEdit()` serves a similar purpose |
| `CreateColumnsInstance()` | — | ❌ | |
| `CreateRowsInstance()` | — | ❌ | |
| `DisplayedColumnCount()` | — | ❌ | |
| `DisplayedRowCount()` | `DisplayedRowCount` | ⚠️ | Property instead of method |
| `EndEdit()` | `EndEdit()` | ✅ | |
| `GetCellCount()` | — | ❌ | |
| `GetCellDisplayRectangle()` | `GetCellBounds(int, int)` | ⚠️ | Named differently |
| `GetClipboardContent()` | — | ❌ | |
| `GetColumnDisplayRectangle()` | — | ❌ | |
| `GetRowDisplayRectangle()` | — | ❌ | |
| `HitTest()` | — | ❌ | `GetRowAtLocation`/`GetColumnAtLocation` are internal |
| `InvalidateCell()` | — | ❌ | Full `Invalidate()` only |
| `InvalidateColumn()` | — | ❌ | |
| `InvalidateRow()` | — | ❌ | |
| `IsInputChar()` | — | ❌ | |
| `IsInputKey()` | — | ❌ | |
| `NotifyCurrentCellDirty()` | — | ❌ | |
| `RefreshEdit()` | — | ❌ | |
| `SelectAll()` | — | ❌ | |
| `Sort()` | `SortByColumn(int, SortOrder)` | ⚠️ | Different signature; also auto-sorts on header click |
| `UpdateCellErrorText()` | — | ❌ | |
| `UpdateCellValue()` | — | ❌ | |
| `UpdateRowErrorText()` | — | ❌ | |
| `UpdateRowHeightInfo()` | — | ❌ | |

## Events

| WinForms Event | Modern.Forms | Status | Notes |
|---|---|---|---|
| `AllowUserToAddRowsChanged` | — | ❌ | |
| `AllowUserToDeleteRowsChanged` | — | ❌ | |
| `AllowUserToOrderColumnsChanged` | — | ❌ | |
| `AllowUserToResizeColumnsChanged` | — | ❌ | |
| `AllowUserToResizeRowsChanged` | — | ❌ | |
| `AlternatingRowsDefaultCellStyleChanged` | — | ❌ | |
| `AutoGenerateColumnsChanged` | — | ❌ | |
| `AutoSizeColumnModeChanged` | — | ❌ | |
| `AutoSizeColumnsModeChanged` | — | ❌ | |
| `AutoSizeRowsModeChanged` | — | ❌ | |
| `BackgroundColorChanged` | — | ❌ | |
| `BorderStyleChanged` | — | ❌ | |
| `CancelRowEdit` | — | ❌ | |
| `CellBeginEdit` | `CellBeginEdit` | ✅ | |
| `CellBorderStyleChanged` | — | ❌ | |
| `CellClick` | — | ❌ | Mouse events + hit testing available |
| `CellContentClick` | — | ❌ | |
| `CellContentDoubleClick` | — | ❌ | |
| `CellContextMenuStripChanged` | — | ❌ | |
| `CellContextMenuStripNeeded` | — | ❌ | |
| `CellDoubleClick` | — | ❌ | |
| `CellEndEdit` | `CellEndEdit` | ✅ | |
| `CellEnter` | — | ❌ | |
| `CellErrorTextChanged` | — | ❌ | |
| `CellErrorTextNeeded` | — | ❌ | |
| `CellFormatting` | — | ❌ | |
| `CellLeave` | — | ❌ | |
| `CellMouseClick` | — | ❌ | |
| `CellMouseDoubleClick` | — | ❌ | |
| `CellMouseDown` | — | ❌ | |
| `CellMouseEnter` | — | ❌ | |
| `CellMouseLeave` | — | ❌ | |
| `CellMouseMove` | — | ❌ | |
| `CellMouseUp` | — | ❌ | |
| `CellPainting` | — | ❌ | Custom renderer can override |
| `CellParsing` | — | ❌ | |
| `CellStateChanged` | — | ❌ | |
| `CellStyleChanged` | — | ❌ | |
| `CellStyleContentChanged` | — | ❌ | |
| `CellToolTipTextNeeded` | — | ❌ | |
| `CellValidated` | — | ❌ | |
| `CellValidating` | — | ❌ | |
| `CellValueChanged` | `CellValueChanged` | ✅ | |
| `CellValueNeeded` | — | ❌ | |
| `CellValuePushed` | — | ❌ | |
| `ColumnAdded` | — | ❌ | |
| `ColumnContextMenuStripChanged` | — | ❌ | |
| `ColumnDataPropertyNameChanged` | — | ❌ | |
| `ColumnDefaultCellStyleChanged` | — | ❌ | |
| `ColumnDisplayIndexChanged` | — | ❌ | |
| `ColumnDividerDoubleClick` | — | ❌ | |
| `ColumnDividerWidthChanged` | — | ❌ | |
| `ColumnHeaderCellChanged` | — | ❌ | |
| `ColumnHeaderMouseClick` | `ColumnHeaderClick` | ⚠️ | Different event args type |
| `ColumnHeaderMouseDoubleClick` | — | ❌ | |
| `ColumnHeadersBorderStyleChanged` | — | ❌ | |
| `ColumnHeadersDefaultCellStyleChanged` | — | ❌ | |
| `ColumnHeadersHeightChanged` | — | ❌ | |
| `ColumnHeadersHeightSizeModeChanged` | — | ❌ | |
| `ColumnMinimumWidthChanged` | — | ❌ | |
| `ColumnNameChanged` | — | ❌ | |
| `ColumnRemoved` | — | ❌ | |
| `ColumnSortModeChanged` | — | ❌ | |
| `ColumnStateChanged` | — | ❌ | |
| `ColumnToolTipTextChanged` | — | ❌ | |
| `ColumnWidthChanged` | — | ❌ | |
| `CurrentCellChanged` | — | ❌ | |
| `CurrentCellDirtyStateChanged` | — | ❌ | |
| `DataBindingComplete` | — | ❌ | |
| `DataError` | — | ❌ | |
| `DataMemberChanged` | — | ❌ | |
| `DataSourceChanged` | — | ❌ | |
| `DefaultCellStyleChanged` | — | ❌ | |
| `DefaultValuesNeeded` | — | ❌ | |
| `EditingControlShowing` | — | ❌ | |
| `EditModeChanged` | — | ❌ | |
| `GridColorChanged` | — | ❌ | |
| `MultiSelectChanged` | — | ❌ | |
| `NewRowNeeded` | — | ❌ | |
| `ReadOnlyChanged` | — | ❌ | |
| `RowContextMenuStripChanged` | — | ❌ | |
| `RowContextMenuStripNeeded` | — | ❌ | |
| `RowDefaultCellStyleChanged` | — | ❌ | |
| `RowDirtyStateNeeded` | — | ❌ | |
| `RowDividerDoubleClick` | — | ❌ | |
| `RowDividerHeightChanged` | — | ❌ | |
| `RowEnter` | — | ❌ | |
| `RowErrorTextChanged` | — | ❌ | |
| `RowErrorTextNeeded` | — | ❌ | |
| `RowHeaderCellChanged` | — | ❌ | |
| `RowHeaderMouseClick` | — | ❌ | |
| `RowHeaderMouseDoubleClick` | — | ❌ | |
| `RowHeadersBorderStyleChanged` | — | ❌ | |
| `RowHeadersDefaultCellStyleChanged` | — | ❌ | |
| `RowHeadersWidthChanged` | — | ❌ | |
| `RowHeadersWidthSizeModeChanged` | — | ❌ | |
| `RowHeightChanged` | — | ❌ | |
| `RowHeightInfoNeeded` | — | ❌ | |
| `RowHeightInfoPushed` | — | ❌ | |
| `RowLeave` | — | ❌ | |
| `RowMinimumHeightChanged` | — | ❌ | |
| `RowPostPaint` | — | ❌ | |
| `RowPrePaint` | — | ❌ | |
| `RowsAdded` | — | ❌ | |
| `RowsDefaultCellStyleChanged` | — | ❌ | |
| `RowsRemoved` | — | ❌ | |
| `RowStateChanged` | — | ❌ | |
| `RowUnshared` | — | ❌ | |
| `RowValidated` | — | ❌ | |
| `RowValidating` | — | ❌ | |
| `Scroll` | — | ❌ | |
| `SelectionChanged` | `SelectionChanged` | ✅ | |
| `SortCompare` | — | ❌ | |
| `Sorted` | — | ❌ | |
| `UserAddedRow` | — | ❌ | |
| `UserDeletedRow` | — | ❌ | |
| `UserDeletingRow` | — | ❌ | |

## Modern.Forms Extra Members (not in WinForms)

| Modern.Forms Member | Type | Notes |
|---|---|---|
| `ColumnHeaderClick` | Event | WinForms uses `ColumnHeaderMouseClick` with different args |
| `FirstVisibleIndex` | Property | WinForms uses `FirstDisplayedScrollingRowIndex` |
| `GetCellBounds(int, int)` | Method | WinForms uses `GetCellDisplayRectangle(int, int, bool)` |
| `RefreshDataSource()` | Method | Re-reads the current `DataSource` |
| `RowHeight` | Property | Default row height; WinForms uses `RowTemplate.Height` |
| `ScaledHeaderHeight` | Property | DPI-scaled header height (internal use) |
| `ScaledRowHeight` | Property | DPI-scaled row height (internal use) |
| `SelectedColumnIndex` | Property | Single-selection index; WinForms uses `SelectedCells` collection |
| `SelectedRowIndex` | Property | Single-selection index; WinForms uses `SelectedRows` collection |
| `SortByColumn(int, SortOrder)` | Method | WinForms uses `Sort(DataGridViewColumn, ListSortDirection)` |
| `VisibleRowCount` | Property | WinForms uses `DisplayedRowCount(bool)` method |

## Summary

| Category | WinForms | Modern.Forms ✅ | Modern.Forms ⚠️ | Modern.Forms ❌ |
|---|---|---|---|---|
| Properties | ~55 | 12 | 6 | ~37 |
| Methods | ~30 | 3 | 3 | ~24 |
| Events | ~100 | 4 | 1 | ~95 |

**Overall coverage:** ~18% of WinForms public API members are implemented (fully or partially).

### Key areas implemented:
- Basic column/row management and display
- Cell editing (begin, end, cancel)
- Column sorting (click headers)
- Column resizing (mouse drag)
- Data binding via `DataSource`
- Selection (single row/cell with `DataGridViewSelectionMode`)
- Keyboard navigation (arrows, Page Up/Down, Home/End, Tab/Shift-Tab)
- Scrolling (vertical + horizontal)
- Cell styles (`DefaultCellStyle`, `ColumnHeadersDefaultCellStyle`, `RowHeadersDefaultCellStyle`)
- Header cells (`DataGridViewColumnHeaderCell`, `DataGridViewRowHeaderCell`)

### Key areas not yet implemented:
- Multi-selection
- Row headers
- Virtual mode
- Clipboard support
- Cell validation/formatting/error handling
- Auto-sizing
- Column reordering
- Most granular events (cell mouse, cell state, row lifecycle)
- Edit mode configuration
- Row/column templates
