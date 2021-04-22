/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

#if WINFORM
using RGFloat = System.Single;
using RGIntDouble = System.Int32;
#else
using RGFloat = System.Double;
using RGIntDouble = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Utility;
using System.Collections;

#if OUTLINE
using unvell.ReoGrid.Outline;
#endif // OUTLINE

using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Core
{
	#region HeaderOperationsHelper

	class RangeModifyHelper
	{
		internal static void ProcessAfterInsertRow(int row, int count, IRowRange range)
		{
			if (range.Row > row)
			{
				range.Row += count;
			}
			else if (range.EndRow > row)
			{
				range.Rows += count;
			}
		}

		internal static void ProcessAfterInsertColumn(int col, int count, IColumnRange range)
		{
			if (range.Col > col)
			{
				range.Col += count;
			}
			else if (range.EndCol > col)
			{
				range.Cols += count;
			}
		}

		internal static void ProcessAfterDeleteRow(int row, int count, int endRow, IRowRange rowRange, Action onChange, Action onRemove)
		{
			if (endRow - 1 < rowRange.Row)
			{
				rowRange.Row -= count;
			}
			else if (row < rowRange.Row)
			{
				int deleteRows = endRow - rowRange.Row;

				if (deleteRows >= rowRange.Rows)
				{
					onRemove();
				}
				else
				{
					onChange();

					rowRange.Row -= count - deleteRows;
					rowRange.Rows -= deleteRows;
				}
			}
			else if (row <= rowRange.EndRow)
			{
				int deleteRows = Math.Min(rowRange.EndRow - row + 1, count);

				if (deleteRows >= rowRange.Rows)
				{
					onRemove();
				}
				else
				{
					onChange();

					rowRange.Rows -= deleteRows;
				}
			}
		}

		internal static void ProcessAfterDeleteColumn(int col, int count, int endCol, IColumnRange colRange, Action onChange, Action onRemove)
		{
			if (endCol - 1 < colRange.Col)
			{
				colRange.Col -= count;
			}
			else if (col < colRange.Col)
			{
				int deleteCols = endCol - colRange.Col;

				if (deleteCols >= colRange.Cols)
				{
					onRemove();
				}
				else
				{
					onChange();

					colRange.Col -= count - deleteCols;
					colRange.Cols -= deleteCols;
				}
			}
			else if (col <= colRange.EndCol)
			{
				int deleteCols = Math.Min(colRange.EndCol - col + 1, count);

				if (deleteCols >= colRange.Cols)
				{
					onRemove();
				}
				else
				{
					onChange();

					colRange.Cols -= deleteCols;
				}
			}
		}
	}

	#endregion // HeaderOperationsHelper
}

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Array
		internal List<ColumnHeader> cols = new List<ColumnHeader>(DefaultCols);
		internal List<RowHeader> rows = new List<RowHeader>(DefaultRows);
		#endregion // Array

		#region Width & Height
		internal ushort colHeaderHeight = 18;
		internal ushort rowHeaderWidth = 40;
		internal ushort defaultColumnWidth = InitDefaultColumnWidth;
		internal ushort defaultRowHeight = InitDefaultRowHeight;
		private bool userRowHeaderWidth = false;

		/// <summary>
		/// Get or set width of row header in pixel.
		/// 
		/// Set to -1 to restore system default width;
		/// Set to 0 to hide the panel of row header;
		/// Set to other value to decide the width of row header in pixel;
		/// </summary>
		public int RowHeaderWidth
		{
			get { return this.rowHeaderWidth; }
			set
			{
				if (value == -1)
				{
					this.userRowHeaderWidth = false;
					AutoAdjustRowHeaderPanelWidth();
				}
				else if (value == 0)
				{
					this.SetSettings(WorksheetSettings.View_ShowRowHeader, false);
				}
				else
				{
					this.userRowHeaderWidth = true;
					this.rowHeaderWidth = (ushort)value;

					if (this.viewportController != null)
					{
						this.viewportController.UpdateController();
					}
				}
			}
		}

		private void AutoAdjustRowHeaderPanelWidth()
		{
			if (!this.userRowHeaderWidth)
			{
				this.rowHeaderWidth = (ushort)((this.rows.Count >= 100000) ? 50 : 40);
			}
		}

		/// <summary>
		/// Set width of specified columns
		/// </summary>
		/// <param name="col">Start column index to set</param>
		/// <param name="count">Number of columns to set</param>
		/// <param name="width">Width value of column</param>
		public void SetColumnsWidth(int col, int count, ushort width)
		{
			SetColumnsWidth(col, count, c => width);
		}

		internal void SetColumnsWidth(int col, int count, Func<int, int> widthGetter,
			bool processOutlines = true, bool updateMaxColumnHeader = true)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			int applyEndCol = col + count;
			int offset = 0;
			RGFloat scaledOffset = 0;

#if OUTLINE
			var colOutlines = GetOutlines(RowOrColumn.Column);
#endif // OUTLINE

			if (updateMaxColumnHeader && maxColumnHeader < applyEndCol - 1) maxColumnHeader = applyEndCol - 1;

			int maxRow = Math.Min(this.rows.Count, cells.MaxRow + 1);
			int sheetEndCol = this.cols.Count;

			for (int c = col; c < sheetEndCol; c++)
			{
				var colhead = this.cols[c];
				colhead.Left += offset;

				int w = colhead.InnerWidth;
				int width = 0;
				bool skipped = false;

				if (c < applyEndCol)
				{
					width = widthGetter(c);

					// skip this column when width < 0
					if (width >= 0)
					{
						// if both target width and current column's width are zero,
						// then skip adjusting column width
						if (width == 0 && colhead.InnerWidth <= 0)
						{
							skipped = true;
						}
						else
						{
							colhead.LastWidth = colhead.InnerWidth;
							colhead.InnerWidth = (ushort)width;

							#region Outline Automatic Process
#if OUTLINE
							if (width > 0)
							{
								#region Expand
								if (processOutlines && colOutlines != null)
								{
									colOutlines.IterateOutlines(o =>
									{
										if (o.End == c + 1 && o.InternalCollapsed)
										{
											o.InternalCollapsed = false;
											o.RaiseAfterExpandingEvent();
											return false;
										}
										return true;
									});
								}
								#endregion
							}
							else // if height >= 0 then collapse outlines
							{
								#region Collapse
								if (processOutlines && colOutlines != null)
								{
									colOutlines.IterateOutlines(o =>
									{
										if (o.End == c + 1 && !o.InternalCollapsed)
										{
											bool collapse = true;

											// check all rows are non-hide
											for (int k = o.Start; k < o.End; k++)
											{
												if (this.cols[k].InnerWidth > 0)
												{
													collapse = false;
													break;
												}
											}

											if (collapse)
											{
												o.InternalCollapsed = true;
												o.RaiseAfterCollapseEvent();
												return false;
											}
										}
										return true;
									});
								}
								#endregion
							}
#endif // OUTLINE
							#endregion // Outline Automatic Process
						}
					}
					else
					{
						// width must be >= zero
						width = 0;
					}
				}

				#region Offset Cells
				for (int r = 0; r < maxRow; r++)
				{
					Cell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.IsEndMergedCell)
						{
							Cell mergedStartCell = GetCell(cell.MergeStartPos);
							UpdateCellBounds(mergedStartCell);

							mergedStartCell.UpdateContentBounds();
						}
						else
						{
							cell.Left += offset;
							cell.TextBoundsLeft += scaledOffset;

							// update non-merged cell
							if (cell.InternalCol < applyEndCol && cell.Colspan == 1 && cell.Rowspan == 1)
							{
								cell.Width = width + 1;
								UpdateCellTextBounds(cell);
							}

							cell.UpdateContentBounds();
						}
					}
				}
				#endregion

				if (c < applyEndCol && !skipped)
				{
					offset += width - w;
					scaledOffset = offset * this.renderScaleFactor;
				}
			}

			#region Offset Floating Objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null && this.drawingCanvas.drawingObjects.Count > 0)
			{
				var left = this.cols[col].Left;
				var right = this.cols[col + count - 1].Right;

				foreach (var obj in this.drawingCanvas.drawingObjects)
				{
					// below
					if (obj.Left > left)
					{
						obj.X += offset;
					}
					else if (obj.Right > left)
					{
						obj.Width += offset;
					}
				}
			}
#endif // DRAWING
			#endregion // Offset Floating Objects

			UpdateViewportController();

			// Raise events
			if (!suspendingUIUpdates)
			{
				int minCol = Math.Min(applyEndCol, sheetEndCol);

				for (int c = col; c < minCol; c++)
				{
					this.cols[c].RaiseWidthChangedEvent();
				}
			}

			this.ColumnsWidthChanged?.Invoke(this, new ColumnsWidthChangedEventArgs(col, count, widthGetter(col)));

#if DEBUG
			watch.Stop();

			if (watch.ElapsedMilliseconds > 5)
			{
				Debug.WriteLine(string.Format("columns width change takes {0} ms.", watch.ElapsedMilliseconds));
			}
#endif // DEBUG
		}

		/// <summary>
		/// Set height of specified rows
		/// </summary>
		/// <param name="row">Start row index to set</param>
		/// <param name="count">Number of rows to set</param>
		/// <param name="height">Height value of row</param>
		public void SetRowsHeight(int row, int count, ushort height)
		{
			SetRowsHeight(row, count, r => height, true);
		}

		internal void SetRowsHeight(int row, int count, Func<int, int> heightGetter, bool processOutlines)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif // DEBUG

			int applyEndRow = row + count;
			int offset = 0;
			RGFloat scaledOffset = 0;
#if OUTLINE
			var rowOutlines = GetOutlines(RowOrColumn.Row);
#endif // OUTLINE

			if (maxRowHeader < applyEndRow - 1) maxRowHeader = applyEndRow - 1;

			int sheetEndRow = this.rows.Count;
			int maxCol = Math.Min(this.cols.Count, cells.MaxCol + 1);

			for (int r = row; r < sheetEndRow; r++)
			{
				var rowhead = rows[r];
				rowhead.Top += offset;

				int h = rowhead.InnerHeight;
				int height = 0;
				bool skiped = false;

				if (r < applyEndRow)
				{
					height = heightGetter(r);

					// skip this row when height < 0
					if (height >= 0)
					{
						// if both target height and current row's height are zero,
						// then skip adjusting row height
						if (height == 0 && rowhead.InnerHeight <= 0)
						{
							skiped = true;
						}
						else
						{
							rowhead.LastHeight = rowhead.InnerHeight;
							rowhead.InnerHeight = (ushort)height;

							#region Outline Automatic Process
#if OUTLINE
							if (height > 0)
							{
								#region Expand
								if (processOutlines && rowOutlines != null)
								{
									rowOutlines.IterateOutlines(o =>
									{
										if (o.End == r + 1 && o.InternalCollapsed)
										{
											o.InternalCollapsed = false;
											o.RaiseAfterExpandingEvent();
											//return false;
										}
										return true;
									});
								}
								#endregion
							}
							else // if height <= 0 then collapse outlines
							{
								#region Collapse
								if (processOutlines && rowOutlines != null)
								{
									rowOutlines.IterateOutlines(o =>
									{
										if (o.End == r + 1 && !o.InternalCollapsed)
										{
											bool collapse = true;

											// check all rows are non-hide
											for (int k = o.Start; k < o.End; k++)
											{
												if (this.rows[k].InnerHeight > 0)
												{
													collapse = false;
													break;
												}
											}

											if (collapse)
											{
												o.InternalCollapsed = true;
												o.RaiseAfterCollapseEvent();
												return true;
											}
										}
										return true;
									});
								}
								#endregion
							}
#endif // OUTLINE
							#endregion // Outline Automatic Process
						}
					}
					else
					{
						// height must be >= zero
						height = 0;
					}
				}

				#region Offset Cells
				for (int c = 0; c < maxCol; c++)
				{
					Cell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.IsEndMergedCell)
						{
							Cell mergedStartCell = GetCell(cell.MergeStartPos);
							UpdateCellBounds(mergedStartCell);
						}
						else
						{
							cell.Top += offset;
							cell.TextBoundsTop += scaledOffset;

							// update unmerged cell
							if (cell.InternalRow < applyEndRow && cell.Colspan == 1 && cell.Rowspan == 1)
							{
								cell.Height = height + 1;
								UpdateCellTextBounds(cell);
							}

							cell.UpdateContentBounds();
						}
					}
				}
				#endregion

				if (r < applyEndRow && !skiped)
				{
					offset += height - h;
					scaledOffset = offset * this.renderScaleFactor;
				}
			}

			#region Offset Floating Objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null && this.drawingCanvas.drawingObjects.Count > 0)
			{
				var top = this.rows[row].Top;
				var bottom = this.rows[row + count - 1].Bottom;

				foreach (var obj in this.drawingCanvas.drawingObjects)
				{
					// below
					if (obj.Top > top)
					{
						obj.Y += offset;
					}
					else if (obj.Bottom > top)
					{
						obj.Height += offset;
					}
				}
			}
#endif // DRAWING
			#endregion // Offset Floating Objects

			UpdateViewportController();

			// Raise events
			if (!suspendingUIUpdates)
			{
				int minRow = Math.Min(applyEndRow, sheetEndRow);

				for (int r = row; r < minRow; r++)
				{
					this.rows[r].RaiseHeightChangedEvent();
				}
			}

			this.RowsHeightChanged?.Invoke(this, new RowsHeightChangedEventArgs(row, count, heightGetter(row)));

#if DEBUG
			watch.Stop();
			long ms = watch.ElapsedMilliseconds;

			if (ms > 10)
			{
				Debug.WriteLine(string.Format("row height changed: {0} ms.", ms));
			}
#endif // DEBUG
		}

		/// <summary>
		/// Get width from specified column. (in pixel)
		/// </summary>
		/// <param name="col">Column index to get.</param>
		/// <returns>Width in pixel of specified column.</returns>
		public ushort GetColumnWidth(int col)
		{
			if (col < 0 || col >= ColumnCount)
			{
				throw new ArgumentOutOfRangeException("col", "invalid column index");
			}
			return cols[col].InnerWidth;
		}

		/// <summary>
		/// Get height from specified row
		/// </summary>
		/// <param name="row">Row index to get</param>
		/// <returns>Height value of specified row</returns>
		public ushort GetRowHeight(int row)
		{
			if (row < 0 || row >= RowCount)
			{
				throw new ArgumentOutOfRangeException("row", "invalid row index");
			}

			return rows[row].InnerHeight;
		}

		internal bool ExpandRowHeightToFitCell(Cell cell)
		{
			if (!cell.IsValidCell) return false;

			if (cell.FontDirty)
			{
				UpdateCellFont(cell);
			}

			if (!string.IsNullOrEmpty(cell.DisplayText))
			{
				int textHeight = (int)Math.Ceiling(cell.TextBounds.Height / renderScaleFactor);
				if (textHeight > 65535) textHeight = 65535;

				if (cell.Height < textHeight)
				{
					SetRowsHeight(cell.InternalRow, 1, (ushort)(textHeight));
					return true;
				}
			}

			return false;
		}

		internal bool ExpandColumnWidthFitToCell(Cell cell)
		{
			if (!cell.IsValidCell) return false;

			if (cell.FontDirty)
			{
				UpdateCellFont(cell);
			}

			if (!string.IsNullOrEmpty(cell.DisplayText))
			{

				int textWidth = (int)Math.Ceiling(cell.TextBounds.Width / renderScaleFactor);
				if (textWidth > 65535) textWidth = 65535;

				if (cell.Width < textWidth)
				{
					SetColumnsWidth(cell.InternalCol, 1, (ushort)textWidth);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Make height of specified row header to fit the cells on that row automatically.
		/// </summary>
		/// <param name="row">Zero-based number of row to be adjusted.</param>
		/// <param name="byAction">Specify that whether or not this operation should 
		/// be done by performing action, that will be able to revoke this behavior.</param>
		/// <returns>Return true if operation actually done; Return false if nothing 
		/// happened. (cells are default height)</returns>
		public bool AutoFitRowHeight(int row, bool byAction = false)
		{
			if (row < 0 || row > this.rows.Count - 1)
			{
				throw new ArgumentOutOfRangeException("row");
			}

			RGFloat maxHeight = 0;

			for (int c = 0; c <= this.MaxContentCol; c++)
			{
				Cell cell = this.cells[row, c];

				if (cell != null && cell.Rowspan == 1)
				{
					if (cell.FontDirty)
					{
						this.UpdateCellFont(cell);
					}

#if DRAWING
					var rt = cell.Data as Drawing.RichText;

					if (rt != null)
					{
						var rtHeight = rt.TextSize.Height;

						if (maxHeight < rtHeight)
						{
							maxHeight = rtHeight;
						}
					}
					else
					{
#endif // DRAWING
						RGFloat textHeight = cell.TextBounds.Height / this.renderScaleFactor;

						if (maxHeight < textHeight)
						{
							maxHeight = textHeight;
						}

#if DRAWING
					}
#endif // DRAWING

				}
			}

			if (maxHeight > 0)
			{
				if (maxHeight < 0) maxHeight = 0;
				if (maxHeight > ushort.MaxValue - 2) maxHeight = ushort.MaxValue - 2;

				ushort targetHeight = (ushort)(maxHeight + 2);

				if (byAction)
				{
					this.DoAction(new SetRowsHeightAction(row, 1, targetHeight));
				}
				else
				{
					this.SetRowsHeight(row, 1, targetHeight);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Make width of specified column header to fit the cells on that column automatically.
		/// </summary>
		/// <param name="col">Zero-based number of column to be adjusted</param>
		/// <param name="byAction">Specify that whether or not this operation should 
		/// be done by performing action, that will be able to revoke this behavior.</param>
		/// <returns>Return true if operation actually done; Return false if nothing 
		/// need to do (cells are default width).</returns>
		public bool AutoFitColumnWidth(int col, bool byAction = false)
		{
			if (col < 0 || col > this.cols.Count - 1)
			{
				throw new ArgumentOutOfRangeException("col");
			}

			RGFloat maxWidth = 0;

			for (int r = 0; r <= this.MaxContentRow; r++)
			{
				Cell cell = this.cells[r, col];

				if (cell != null
					&& cell.Colspan == 1)
				{
					if (cell.FontDirty)
					{
						this.UpdateCellFont(cell);
					}

#if DRAWING
					var rt = cell.Data as Drawing.RichText;

					if (rt != null)
					{
						var rtWidth = rt.TextSize.Width;

						if (maxWidth < rtWidth)
						{
							maxWidth = rtWidth;
						}
					}
					else
					{
#endif // DRAWING

						RGFloat textWidth = cell.TextBounds.Width / this.renderScaleFactor;

						if (maxWidth < textWidth)
						{
							maxWidth = textWidth;
						}

#if DRAWING
					}
#endif // DRAWING
				}
			}

			if (maxWidth > 0)
			{
				if (maxWidth < 0) maxWidth = 0;
				if (maxWidth > ushort.MaxValue - 2) maxWidth = ushort.MaxValue - 2;

				ushort targetWidth = (ushort)(maxWidth + 2);

				if (byAction)
				{
					this.DoAction(new SetColumnsWidthAction(col, 1, targetWidth));
				}
				else
				{
					this.SetColumnsWidth(col, 1, targetWidth);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion // Width & Height

		#region Append

		/// <summary>
		/// Append specified columns at right of spreadsheet
		/// </summary>
		/// <param name="count">number of columns</param>
		public void AppendColumns(int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("count must be greater than zero");
			}

			if (this.cols.Count + count > this.cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count",
					"number of columns exceeds the maximum columns: " + this.cells.ColCapacity);
			}

			int x = cols.Count == 0 ? 0 : cols[cols.Count - 1].Right;
			int total = cols.Count + count;

			for (int i = cols.Count; i < total; i++)
			{
				cols.Add(new ColumnHeader(this)
				{
					InnerWidth = defaultColumnWidth,
					Col = i,
					RenderText = RGUtility.GetAlphaChar(i),
					Left = x,
					IsAutoWidth = true,
				});

				x += defaultColumnWidth;
			}

			UpdateViewportController();

			ColumnsInserted?.Invoke(this, new ColumnsInsertedEventArgs(this.cols.Count - count, count));
		}

		/// <summary>
		/// Append specified rows at bottom of grid
		/// </summary>
		/// <param name="count">number of rows</param>
		public void AppendRows(int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("count must be greater than zero");
			}

			if (this.rows.Count + count > this.cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count",
					"number of rows exceeds the maximum rows: " + this.cells.RowCapacity);
			}

			int y = rows.Count == 0 ? 0 : rows[rows.Count - 1].Bottom;
			int total = rows.Count + count;

			for (int i = rows.Count; i < total; i++)
			{
				rows.Add(new RowHeader(this)
				{
					InnerHeight = defaultRowHeight,
					Row = i,
					Top = y,
					IsAutoHeight = true,
				});

				y += defaultRowHeight;
			}

			UpdateViewportController();

			RowsInserted?.Invoke(this, new RowsInsertedEventArgs(this.rows.Count - count, count));
		}

		#endregion // Append

		#region Resize

		/// <summary>
		/// Resize grid to specified number of rows and cols.
		/// </summary>
		/// <param name="rows">Number of rows to be resized.</param>
		/// <param name="cols">Number of columns to be resized.</param>
		public void Resize(int rows, int cols)
		{
			if (cols > 0)
			{
				if (cols > this.cols.Count)
				{
					AppendColumns(cols - this.cols.Count);
				}
				else if (cols < this.cols.Count)
				{
					DeleteColumns(cols, this.cols.Count - cols);
				}
			}

			if (rows > 0)
			{
				if (rows > this.rows.Count)
				{
					AppendRows(rows - this.rows.Count);
				}
				else if (rows < this.rows.Count)
				{
					DeleteRows(rows, this.rows.Count - rows);
				}
			}
		}

		/// <summary>
		/// Set number of columns (up to 32768)
		/// </summary>
		/// <param name="colCount">Number of columns</param>
		public void SetCols(int colCount)
		{
			Resize(-1, colCount);
		}

		/// <summary>
		/// Set number of rows (up to 1048576)
		/// </summary>
		/// <param name="rowCount">Number of rows</param>
		public void SetRows(int rowCount)
		{
			Resize(rowCount, -1);
		}

		/// <summary>
		/// Get or set number of rows of current worksheet
		/// </summary>
		public int Rows
		{
			get { return this.rows.Count; }
			set { SetRows(value); }
		}

		/// <summary>
		/// Get or set number of columns of current worksheet
		/// </summary>
		public int Columns
		{
			get { return this.cols.Count; }
			set { SetCols(value); }
		}

		#endregion // Resize

		#region Insert

		/// <summary>
		/// Insert rows before specified row index
		/// </summary>
		/// <param name="row">index of row</param>
		/// <param name="count">number of rows</param>
		public void InsertRows(int row, int count)
		{
			#region Check
			if (row > this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("row");
			}

			if (rows.Count + count > cells.RowCapacity)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count < 1)
			{
				throw new ArgumentException("count must be >= 1");
			}
			#endregion // Check

			if (row >= this.rows.Count)
			{
				AppendRows(count);
				return;
			}

#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif
			#region insert headers
			int y = row == 0 ? 0 : rows[row - 1].Bottom;
			int top = y;

			ushort height = rows[row].InnerHeight;

			RowHeader[] headers = new RowHeader[count];

			for (int i = 0; i < count; i++)
			{
				headers[i] = new RowHeader(this)
				{
					Row = row + i,
					Top = y,
					InnerHeight = height,
					InnerStyle = rows[row].InnerStyle == null ? null : new WorksheetRangeStyle(rows[row].InnerStyle),
					IsAutoHeight = true,
				};

				y += height;
			}

			// insert row headers
			rows.InsertRange(row, headers);

			int totalHeight = height * count;

			#endregion

			#region move rows
			// TODO: can be optimized by moving entrie page in JaggedTreeArray
			for (int r = this.rows.Count - 1; r > row + count - 1; r--)
			{
				if (r != this.rows.Count)
				{
					rows[r].Row += count;
					rows[r].Top += totalHeight;
				}

				// move cells
				for (int c = this.cols.Count - 1; c >= 0; c--)
				{
					cells[r, c] = cells[r - count, c];

					Cell cell = cells[r, c];

					if (cell != null)
					{
						cell.InternalRow += count;
						cell.Top += totalHeight;
						cell.TextBoundsTop += totalHeight;

						// move start pos
						if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Row >= row)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(count, 0);
						}

						// move end pos
						if (!cell.MergeEndPos.IsEmpty)
						{
							cell.MergeEndPos = cell.MergeEndPos.Offset(count, 0);
						}
					}

					// move borders
					vBorders[r, c] = vBorders[r - count, c];
					hBorders[r, c] = hBorders[r - count, c];
				}
			}
			#endregion

			#region insert rows

			// TODO: can be optimized by moving entrie page via RegularTreeArray
			for (int r = row; r < row + count; r++)
			{
				for (int c = 0; c < this.cols.Count; c++)
				{
					hBorders[r, c] = null;
					vBorders[r, c] = null;
					cells[r, c] = null;
				}
			}

			//int colspan = 1;
			for (int c = this.cols.Count; c >= 0; c--)
			{
				if (row == 0)
				{
					cells[row, c] = null;
				}

				// clear old border
				vBorders[row, c] = null;
				hBorders[row, c] = null;

				#region insert vertial border

				// insert vertial border
				bool vhasTop = row > 0 && vBorders[row - 1, c] != null && vBorders[row - 1, c].Style != null;
				bool vhasBottom = vBorders[row + count, c] != null && vBorders[row + count, c].Style != null;

				// insert vertial border if cell has both top and bottom borders
				if (vhasTop && vhasBottom)
				{
					// set vertial border
					SetVBorders(row, c, count, vBorders[row - 1, c].Style, vBorders[row - 1, c].Pos);

					//for (int r = row; r < row + count; r++)
					//{
					//	// merge owner flag
					//	vBorders[r, c].Pos |= vBorders[row - 1, c].Pos;
					//}
				}

				#endregion

				// not last column
				if (c != this.cols.Count)
				{
					#region TODO: insert horizontal borders

					//// has old horizontal border?
					//if (hBorders[row + 1, c] != null && hBorders[row + 1, c].Border != null)
					//{
					//  // compare horizontal border from (+1,0) to (+1,+1)
					//  // if two borders are same, add colspan
					//  if (IsBorderSame(hBorders[row + 1, c], hBorders[row, c + 1])) colspan++;

					//  // get old border
					//  ReoGridHBorder hBorder = hBorders[row + 1, c];

					//  //
					//  // TODO: auto fix border
					//  //
					//  // old border is inner border of cell
					//  // need add a horizontal top border to current cell
					//  if (hBorder.Pos == ReoGridHBorderPosition.All)
					//  {
					//    //hBorders[row, c] = new ReoGridHBorder
					//    //{
					//    //  Border = hBorders[row + 1, c].Border,
					//    //  Cols = colspan,
					//    //  Pos = ReoGridHBorderPosition.All,
					//    //};
					//  }
					//  else if (hBorder.Pos == ReoGridHBorderPosition.Top)
					//  {

					//  }
					//  else if (hBorder.Pos == ReoGridHBorderPosition.Bottom)
					//  {
					//    //hBorders[row, c] = hBorders[row + 1, c];
					//    //hBorders[row + 1, c] = null;

					//    //for (int ck = c - 1; ck >= 0; ck--)
					//    //{
					//    //  if (hBorders[row + 1, ck] != null) hBorders[row + 1, ck].Cols--;
					//    //}
					//  }
					//}
					//else colspan = 1;

					#endregion

					#region fill merged cell
					Cell prevCell = row <= 0 ? null : cells[row - 1, c];
					Cell nextCell = cells[row + count, c];

					Cell cell = cells[row, c] = null;

					bool isTopMerged = prevCell != null && prevCell.Rowspan != 1;
					bool isBottomMerged = nextCell != null && nextCell.Rowspan != 1;
					bool insideMergedRange = IsInsideSameMergedCell(prevCell, nextCell);

					if (insideMergedRange)
					{
						// fill empty rows inside current range
						for (int r = row; r < row + count; r++)
						{
							cell = CreateCell(r, c);
							cell.Colspan = 0;
							cell.Rowspan = 0;
							cell.MergeEndPos = prevCell.MergeEndPos.Offset(count, 0);
							cell.MergeStartPos = prevCell.MergeStartPos;

							// cells inside range should be have an empty v-border and h-border
							if (c > cell.MergeStartPos.Col) vBorders[r, c] = new ReoGridVBorder();
							hBorders[r, c] = new ReoGridHBorder();
						}

						// find cells which in the top side of inserted row, and offset their's merge-end-pos 
						// (merge-end-pos += number of inserted rows)
						for (int r = cell.MergeStartPos.Row; r < row; r++)
						{
							cells[r, c].MergeEndPos = cells[r, c].MergeEndPos.Offset(count, 0);
						}

						// if range is splitted by inserted rows
						// the height of range should be expanded
						//
						// NOTE: only do this once by making sure c is merge-start-column
						//
						if (c == cell.MergeStartPos.Col)
						{
							Cell startCell = GetCell(cell.MergeStartPos);
							startCell.Rowspan += (short)count;
							startCell.Height += totalHeight;
						}
					}
					else
					{
						cells[row, c] = null;
					}

					#endregion
				}
			}

			#endregion

			#region Insert Outlines
#if OUTLINE
			var outlines = GetOutlines(RowOrColumn.Row);
			if (outlines != null)
			{
				outlines.IterateOutlines(o =>
				{
					RangeModifyHelper.ProcessAfterInsertRow(row, count, (IRowRange)o);
					return true;
				});
			}
#endif // OUTLINE
			#endregion // Insert Outlines

			#region insert before printable range
#if PRINT
			RangeModifyHelper.ProcessAfterInsertRow(row, count, this.printableRange);
#endif // PRINT
			#endregion

			#region move named ranges
			//foreach (var range in this.registeredNamedRanges.Values)
			//{
			//	RangeModifyHelper.ProcessAfterInsertRow(row, count, range);
			//}
			#endregion // move named ranges

			#region highlight ranges
			//foreach (var range in this.highlightRanges)
			//{
			//	RangeModifyHelper.ProcessAfterInsertRow(row, count, range);
			//}
			#endregion // move named ranges

			#region Floating objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null)
			{
				foreach (var child in this.drawingCanvas.drawingObjects)
				{
					if (child.Y >= top)
					{
						child.Y += totalHeight;
					}
					else if (child.Bottom > top)
					{
						child.Height += totalHeight;
					}
				}
			}
#endif // DRAWING
			#endregion // Floating objects

			#region Update frozen rows
			if (row < this.FreezePos.Row)
			{
				this.FreezePos = FixPos(new CellPosition(this.FreezePos.Row + count, this.FreezePos.Col));
			}
			#endregion // Update frozen rows

			UpdateViewportController();

			// raise event
			RowsInserted?.Invoke(this, new RowsInsertedEventArgs(row, count));

#if DEBUG
			watch.Stop();
			Debug.WriteLine("insert rows: " + watch.ElapsedMilliseconds + " ms.");
#endif
		}

		/// <summary>
		/// Insert rows before specified row index
		/// </summary>
		/// <param name="col">zero-based number of column start to insert columns</param>
		/// <param name="count">number of columns to be inserted</param>
		public void InsertColumns(int col, int count)
		{
			#region Check

			if (col > this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("col");
			}

			if (cols.Count + count > cells.ColCapacity)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count < 1)
			{
				throw new ArgumentException("count must be >= 1");
			}

			if (col >= this.cols.Count)
			{
				AppendColumns(count);
				return;
			}

			#endregion // Check

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			#region insert headers
			int x = col == 0 ? 0 : cols[col - 1].Right;
			ushort width = cols[col].InnerWidth;

			ColumnHeader[] headers = new ColumnHeader[count];

			int left = x;
			for (int i = 0; i < count; i++)
			{
				int index = col + i;

				headers[i] = new ColumnHeader(this)
				{
					Col = index,
					RenderText = RGUtility.GetAlphaChar(index),
					Left = x,
					InnerWidth = width,
					InnerStyle = cols[col].InnerStyle == null ? null : new WorksheetRangeStyle(cols[col].InnerStyle),
					IsAutoWidth = true,
				};

				x += width;
			}

			// insert row header
			cols.InsertRange(col, headers);

			int totalWidth = width * count;

			#endregion

			#region move columns
			// TODO: can be optimized by moving entrie page in RegularTreeArray
			for (int c = this.cols.Count - 1; c > col + count - 1; c--)
			{
				if (c != this.cols.Count)
				{
					int newCol = cols[c].Col + count;

					cols[c].Col = newCol;
					cols[c].Left += totalWidth;

					if (cols[c].Text == null)
					{
						cols[c].RenderText = RGUtility.GetAlphaChar(newCol);
					}
				}

				// move cells
				for (int r = this.rows.Count - 1; r >= 0; r--)
				{
					cells[r, c] = cells[r, c - count];

					Cell cell = cells[r, c];

					if (cell != null)
					{
						cell.InternalCol += count;
						cell.Left += totalWidth;
						cell.TextBoundsLeft += totalWidth;

						// move start pos
						if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Col >= col)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(0, count);
						}

						// move end pos
						if (!cell.MergeEndPos.IsEmpty)
						{
							cell.MergeEndPos = cell.MergeEndPos.Offset(0, count);
						}
					}

					// move borders
					vBorders[r, c] = vBorders[r, c - count];
					hBorders[r, c] = hBorders[r, c - count];
				}
			}
			#endregion

			#region insert cols

			// TODO: can be optimized by moving entrie page in RegularTreeArray
			for (int c = col; c < col + count; c++)
			{
				for (int r = 0; r < this.rows.Count; r++)
				{
					hBorders[r, c] = null;
					vBorders[r, c] = null;
					cells[r, c] = null;
				}
			}

			//int colspan = 1;
			for (int r = this.rows.Count; r >= 0; r--)
			{
				if (col == 0)
				{
					cells[r, col] = null;
				}

				// clear old border
				vBorders[r, col] = null;
				hBorders[r, col] = null;

				#region insert horizontal border

				// insert horizontal border
				bool hhasLeft = col > 0 && hBorders[r, col - 1] != null && hBorders[r, col - 1].Style != null;
				bool hhasRight = hBorders[r, col + count] != null && hBorders[r, col + count].Style != null;

				// insert horizontal border if cell has both top and bottom borders
				if (hhasLeft && hhasRight)
				{
					// set horizontal border
					SetHBorders(r, col, count, hBorders[r, col - 1].Style, hBorders[r, col - 1].Pos);
				}

				#endregion

				// not last row
				if (r != this.rows.Count)
				{
					#region TODO: insert horizontal borders

					#endregion

					#region fill merged cell
					Cell prevCell = col <= 0 ? null : cells[r, col - 1];
					Cell nextCell = cells[r, col + count];

					Cell cell = cells[r, col] = null;

					bool isTopMerged = prevCell != null && prevCell.Rowspan != 1;
					bool isBottomMerged = nextCell != null && nextCell.Rowspan != 1;
					bool insideMergedRange = IsInsideSameMergedCell(prevCell, nextCell);

					if (insideMergedRange)
					{
						// fill empty columns inside current range
						for (int c = col; c < col + count; c++)
						{
							cell = CreateCell(r, c);
							cell.Colspan = 0;
							cell.Rowspan = 0;
							cell.MergeEndPos = prevCell.MergeEndPos.Offset(0, count);
							cell.MergeStartPos = prevCell.MergeStartPos;

							// cells inside range should be have an empty v-border and h-border
							if (r > cell.MergeStartPos.Row) hBorders[r, c] = new ReoGridHBorder();
							vBorders[r, c] = new ReoGridVBorder();
						}

						// find cells which in the left side of inserted column, offset their's merge-end-pos  
						// (merge-end-pos += number of inserted rows)
						for (int c = cell.MergeStartPos.Col; c < col; c++)
						{
							cells[r, c].MergeEndPos = cells[r, c].MergeEndPos.Offset(0, count);
						}

						// if range is splitted by inserted rows
						// the width of range should be expanded
						//
						// NOTE: only do this once by making sure r is merge-start-row
						//
						if (r == cell.MergeStartPos.Row)
						{
							Cell startCell = GetCell(cell.MergeStartPos);
							startCell.Colspan += (short)count;
							startCell.Width += totalWidth;
						}
					}
					else
					{
						cells[r, col] = null;
					}

					#endregion
				}
			}

			#endregion // insert cols

			#region Insert before printable range
#if PRINT
			RangeModifyHelper.ProcessAfterInsertColumn(col, count, this.printableRange);
#endif // PRINT
			#endregion // Insert before printable range

			#region insert outlines
#if OUTLINE
			var outlines = GetOutlines(RowOrColumn.Column);
			if (outlines != null)
			{
				outlines.IterateOutlines(o =>
				{
					RangeModifyHelper.ProcessAfterInsertColumn(col, count, (IColumnRange)o);
					return true;
				});
			}
#endif // OUTLINE
			#endregion

			#region move named ranges
			//foreach (var range in this.registeredNamedRanges.Values)
			//{
			//	RangeModifyHelper.ProcessAfterInsertColumn(col, count, range);
			//}
			#endregion // move named ranges

			#region highlight ranges
			//foreach (var range in this.highlightRanges)
			//{
			//	RangeModifyHelper.ProcessAfterInsertColumn(col, count, range);
			//}
			#endregion // move named ranges

			#region Floating objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null)
			{
				foreach (var child in this.drawingCanvas.drawingObjects)
				{
					if (child.Left >= left)
					{
						child.X += totalWidth;
					}
					else if (child.Right > left)
					{
						child.Width += totalWidth;
					}
				}
			}
#endif // DRAWING
			#endregion // Floating objects

			#region Update frozen column
			if (col < this.FreezePos.Col)
			{
				this.FreezePos = FixPos(new CellPosition(FreezePos.Row, this.FreezePos.Col + count));
			}
			#endregion // Update frozen rows

			this.selectionRange = FixRange(selectionRange);

			UpdateViewportController();

			// raise event
			ColumnsInserted?.Invoke(this, new ColumnsInsertedEventArgs(col, count));

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 15)
			{
				Debug.WriteLine("insert cols: " + ms + " ms.");
			}
#endif
		}
		#endregion // Insert

		#region Delete

		/// <summary>
		/// Delete rows from speicifed number of row
		/// </summary>
		/// <param name="row">number of row start to be deleted</param>
		/// <param name="count">number of rows to be deleted</param>
		public void DeleteRows(int row, int count)
		{
			this.DeleteRows(row, count, null);
		}

		internal void DeleteRows(int row, int count, RemoveRowsAction action)
		{
			#region Check

			if (row < 0 || row >= this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count >= this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (row + count > this.rows.Count)
			{
				// at least remain 1 rows
				throw new ArgumentOutOfRangeException("row + count, at least one row should be left");
			}

			#endregion // Check

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			int top = this.rows[row].Top;
			int endrow = row + count;
			int totalHeight = this.rows[endrow - 1].Bottom - this.rows[row].Top;
			RGFloat scaledTotalHeight = totalHeight * this.renderScaleFactor;

#if DEBUG
			Debug.Assert(totalHeight > 0);
#endif

			int maxrow = MaxContentRow + 1;
			int maxcol = MaxContentCol + 1;

			suspendingUIUpdates = true;

			#region delete headers

			this.rows.RemoveRange(row, count);

			for (int r = row; r < this.rows.Count; r++)
			{
				rows[r].Row -= count;
				rows[r].Top -= totalHeight;

#if DEBUG
				Debug.Assert(rows[r].Row >= 0);
#endif
			}

			#endregion // delete headers

			#region delete top side
			for (int c = 0; c <= this.cols.Count; c++)
			{
				// TODO: bounds test
				Cell cell = cells[row, c];

				if (c < this.cols.Count && cell != null)
				{
					if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Row < row)
					{
						// update colspan for range
						if (cell.MergeStartPos.Col == c)
						{
							Cell mergedStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];

							Debug.Assert(mergedStartCell != null);
							Debug.Assert(mergedStartCell.Colspan > 0);

							int span = Math.Min(count, cell.MergeEndPos.Row - row + 1);
							mergedStartCell.Rowspan -= (short)span;

							Debug.Assert(mergedStartCell.Rowspan > 0);

							mergedStartCell.Height =
								this.rows[mergedStartCell.InternalRow + mergedStartCell.Rowspan - 1].Bottom -
								this.rows[mergedStartCell.InternalRow].Top;
						}

						// update merge-end-col for range
						for (int r = cell.MergeStartPos.Row; r < row; r++)
						{
							Cell topCell = cells[r, c];
							int span = Math.Min(count, topCell.MergeEndPos.Row - row + 1);
							topCell.MergeEndPos = topCell.MergeEndPos.Offset(-span, 0);
						}
					}
				}

				// if any borders exist in left side, it's need to update the span of borders.
				// from columns from 0 col no need to do this
				if (row > 0
					// is border at start column 
					&& this.vBorders[row - 1, c] != null && this.vBorders[row - 1, c].Span > 0)
				{
					// find border to merge in right side
					int addspan = 0;

					// border exists in right side?
					if (this.vBorders[endrow, c] != null
						// this is not a same border range
						&& this.vBorders[endrow, c].Span + count + 1 != this.vBorders[row - 1, c].Span
						// does they have same styles?
						&& this.vBorders[endrow, c].Style.Equals(this.vBorders[row - 1, c].Style)
						// does they have same owner position flags?
						&& this.vBorders[endrow, c].Pos == this.vBorders[row - 1, c].Pos)
					{
						addspan = this.vBorders[endrow, c].Span;
					}

					// update borders in left side
					int subspan = 0;

					// calc how many borders in delete target range,
					// it need be subtract from left side border.
					if (this.vBorders[row, c] != null && this.vBorders[row, c].Span > 0
						&& this.vBorders[row, c].Span == this.vBorders[row - 1, c].Span - 1)
					{
						subspan = Math.Min(this.vBorders[row, c].Span, count);
					}

					// set reference span
					int refspan = this.vBorders[row - 1, c].Span;

					this.vBorders[row - 1, c].Span += addspan - subspan;

					if (row > 1)
					{
						// update all span in left side
						for (int r = row - 2; r >= 0; r--)
						{
							if (this.vBorders[r, c] != null && this.vBorders[r, c].Span == refspan + 1)
							{
								this.vBorders[r, c].Span += addspan - subspan;
								refspan++;
							}
							else
								break;
						}
					}
				}
			}
			#endregion // delete top side

			#region delete bottom side
			int bottomBounds = Math.Min(this.rows.Count + count, rows.Capacity);

			// bottom
			for (int c = 0; c < this.cols.Count; c++)
			{
				for (int r = endrow; r < bottomBounds; r++)
				{
					Cell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.MergeStartPos.Row >= endrow)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(-count, 0);
							cell.Top -= totalHeight;
							cell.TextBoundsTop -= scaledTotalHeight;
						}
						else if (cell.InternalRow >= endrow && cell.IsValidCell)
						{
							cell.Top -= totalHeight;
							cell.TextBoundsTop -= scaledTotalHeight;
						}

						// Case:
						//
						//       col          ec
						//     +-----------+
						//     |           |
						//   0 |  1  |  2  |  3  |
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//
						else if (cell.MergeStartPos.Row >= row && cell.MergeStartPos.Row < endrow)
						{
							if (r == endrow && c == cell.MergeStartPos.Col)
							{
								Cell startCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
								Debug.Assert(startCell != null);

								// create a new merged cell
								cell.Rowspan = (short)(startCell.Rowspan - endrow + cell.MergeStartPos.Row);
								cell.Colspan = (short)(cell.MergeEndPos.Col - cell.MergeStartPos.Col + 1);

								cell.Bounds = GetRangeBounds(cell.MergeStartPos.Row, c, cell.Rowspan, cell.Colspan);

								// copy cell content
								CellUtility.CopyCellContent(cell, startCell);
							}

							//int sspan = endcol - cell.MergeStartPos.Col - count;
							cell.MergeStartPos = new CellPosition(row, cell.MergeStartPos.Col);
						}

						// update merge-end-pos
						int espan = Math.Min(count, cell.MergeEndPos.Row - cell.MergeStartPos.Row);
						cell.MergeEndPos = cell.MergeEndPos.Offset(-espan, 0);
					}
				}
			}
			#endregion // delete bottom side

			#region move rows

			for (int c = 0; c <= maxcol; c++)
			{
				#region move cells
				// move cells
				for (int r = row; r <= maxrow; r++)
				{
					var cell = cells[r + count, c];
					cells[r, c] = cell;

					if (cell != null)
					{
						cell.InternalRow -= count;
					}

					hBorders[r, c] = hBorders[r + count, c];
					vBorders[r, c] = vBorders[r + count, c];
				}
				#endregion // move cells

				// remove border to force show grid line
				if (row == 0 || !IsInsideSameMergedCell(row - 1, c, row, c))
				{
					if (hBorders[row, c] != null && hBorders[row, c].Span == 0)
					{
						hBorders[row, c] = null;
					}
				}
			}
			#endregion // move columns

			#region delete outlines
#if OUTLINE
			var rowOutlines = GetOutlines(RowOrColumn.Row);

			if (rowOutlines != null)
			{
				List<IReoGridOutline> removingOutlines = null;

				if (action != null)
				{
					action.deletedOutlines = removingOutlines;
				}

				rowOutlines.IterateOutlines(o =>
				{
					RangeModifyHelper.ProcessAfterDeleteRow(row, count, endrow, (IRowRange)o,
						() =>
						{
							if (action != null)
							{
								if (action.changedOutlines == null)
								{
									action.changedOutlines = new Dictionary<IReoGridOutline, BackupRangeInfo>();
								}

								action.changedOutlines[o] = new BackupRangeInfo(o.Start, o.Count);
							}
						},
						() =>
						{
							if (removingOutlines == null)
							{
								removingOutlines = new List<IReoGridOutline>();
							}

							removingOutlines.Add(o);
						});

					return true;
				});

				if (removingOutlines != null)
				{
					if (action != null)
					{
						action.deletedOutlines = removingOutlines;
					}

					// remove outlines which count <= 0
					foreach (var o in removingOutlines)
					{
						RemoveOutline(o);
					}
				}

				// when any outlines size changed, there is may cause some outlines have same position and count
				// it's nesscaray to found them out, and remove them from current worksheet
				//
				List<IReoGridOutline> deletedOutlines2 = null;

				rowOutlines.IterateReverseOutlines(o =>
				{
					if (rowOutlines.HasSame(o, deletedOutlines2))
					{
						if (deletedOutlines2 == null)
						{
							deletedOutlines2 = new List<IReoGridOutline>();
						}

						deletedOutlines2.Add(o);
					}

					return true;
				});

				if (deletedOutlines2 != null)
				{
					// add them into the backup list of action
					if (action != null)
					{
						if (action.deletedOutlines == null)
						{
							action.deletedOutlines = deletedOutlines2;
						}
						else
						{
							action.deletedOutlines.AddRange(deletedOutlines2);
						}
					}

					// remove outlines which count <= 0
					foreach (var o in deletedOutlines2)
					{
						RemoveOutline(o);
					}
				}
			}
#endif // OUTLINE
			#endregion // delete outlines

			#region move named ranges
			List<NamedRange> removedNamedRange = null;

			foreach (var name in this.registeredNamedRanges.Keys)
			{
				var range = this.registeredNamedRanges[name];

				RangeModifyHelper.ProcessAfterDeleteRow(row, count, endrow, range,
					() =>
					{
						if (action != null)
						{
							if (action.changedNamedRange == null)
							{
								action.changedNamedRange = new Dictionary<NamedRange, BackupRangeInfo>();
							}

							action.changedNamedRange[range] = new BackupRangeInfo(range.Row, range.Rows);
						}
					},
					() =>
					{
						if (removedNamedRange == null)
						{
							removedNamedRange = new List<NamedRange>(1);
						}

						removedNamedRange.Add(range);
					});
			}

			// add into action backup list
			if (action != null)
			{
				action.deletedNamedRanges = removedNamedRange;
			}

			if (removedNamedRange != null)
			{
				foreach (var range in removedNamedRange)
				{
					this.UndefineNamedRange(range.Name);
				}
			}
			#endregion // move named ranges

			#region highlight ranges
			for (int i = 0; i < this.highlightRanges.Count; i++)
			{
				var range = this.highlightRanges[i];

				RangeModifyHelper.ProcessAfterDeleteRow(row, count, endrow, range,
					() =>
					{
						if (action != null)
						{
							if (action.changedHighlightRanges == null)
							{
								action.changedHighlightRanges = new Dictionary<HighlightRange, BackupRangeInfo>();
							}

							action.changedHighlightRanges[range] = new BackupRangeInfo(range.Row, range.Rows);
						}
					},
					() =>
					{
						if (action != null)
						{
							if (action.deletedHighlightRanges == null)
							{
								action.deletedHighlightRanges = new List<HighlightRange>();
							}

							action.deletedHighlightRanges.Add(range);
						}

						RemoveHighlightRange(range);
					});
			}
			#endregion // move named ranges

			#region Floating objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null)
			{
				foreach (var child in this.drawingCanvas.drawingObjects)
				{
					if (child.Y >= top)
					{
						child.Y -= totalHeight;
					}
					else if (child.Bottom > top)
					{
						var height = child.Height - totalHeight;
						if (height < 0) height = 0;
						child.Height = height;
					}
				}
			}
#endif // DRAWING
			#endregion // Floating objects

			#region Update used range
			// bug: rgf will save the rows has been removed, error happens when next time loading
			// https://reogrid.net/forum/viewtopic.php?id=277
			if (this.cells.MaxRow >= endrow)
			{
				this.cells.MaxRow -= count;
			}
			if (this.hBorders.MaxRow >= endrow)
			{
				this.hBorders.MaxRow -= count;
			}
			if (this.vBorders.MaxRow >= endrow)
			{
				this.vBorders.MaxRow -= count;
			}
			#endregion // Update used range

			#region Update frozen rows
			if (row < this.FreezePos.Row)
			{
				this.FreezePos = FixPos(new CellPosition(this.FreezePos.Row - count, this.FreezePos.Col));

				// remain the first row to be frozen
				if (this.FreezePos.Row < 1)
				{
					if (this.rows.Count > 1)
					{
						this.FreezePos = new CellPosition(1, this.FreezePos.Col);
					}
					else
					{
						this.FreezePos = new CellPosition(0, this.FreezePos.Col);
					}
				}
			}
			#endregion // Update frozen rows

			suspendingUIUpdates = false;

			UpdateViewportController();

			var selRange = FixRange(selectionRange);
			ApplyRangeSelection(selRange.StartPos, selRange.EndPos, false);

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 20)
			{
				Debug.WriteLine("delete rows takes " + ms + " ms.");
			}
#endif // DEBUG

			// raise column deleted event
			RowsDeleted?.Invoke(this, new RowsDeletedEventArgs(row, count));
		}

		/// <summary>
		/// Delete columns from specified number of column
		/// </summary>
		/// <param name="col">number of column start to be deleted</param>
		/// <param name="count">number of columns to be deleted</param>
		public void DeleteColumns(int col, int count)
		{
			DeleteColumns(col, count, null);
		}

		internal void DeleteColumns(int col, int count, RemoveColumnsAction action)
		{
			#region Check

			if (col < 0 || col >= this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (count >= this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (col + count > this.cols.Count)
			{
				// at least remain 1 cols
				throw new ArgumentOutOfRangeException("col + count, at least one column should be left");
			}

			#endregion // Check

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			int maxrow = MaxContentRow + 1;
			int maxcol = MaxContentCol + 1;

			int left = this.cols[col].Left;
			int endcol = col + count;
			int totalWidth = this.cols[endcol - 1].Right - this.cols[col].Left;
			RGFloat scaledTotalWidth = totalWidth * this.renderScaleFactor;

			suspendingUIUpdates = true;

			#region delete headers

			this.cols.RemoveRange(col, count);

			for (int c = col; c < this.cols.Count; c++)
			{
				cols[c].Col -= count;
				cols[c].Left -= totalWidth;

				if (cols[c].Text == null)
				{
					cols[c].RenderText = RGUtility.GetAlphaChar(c);
				}

				Debug.Assert(cols[c].Col >= 0);
			}

			#endregion // delete headers

			#region delete cells
			// left 
			for (int r = 0; r <= this.rows.Count; r++)
			{
				// TODO: bounds test
				Cell cell = cells[r, col];

				if (r < this.rows.Count && cell != null)
				{
					if (!cell.MergeStartPos.IsEmpty && cell.MergeStartPos.Col < col)
					{
						// update colspan for range
						if (cell.MergeStartPos.Row == r)
						{
							Cell mergedStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
#if DEBUG
							Debug.Assert(mergedStartCell.Colspan > 0);
#endif

							int span = Math.Min(count, cell.MergeEndPos.Col - col + 1);
							mergedStartCell.Colspan -= (short)span;

							mergedStartCell.Width =
								this.cols[mergedStartCell.InternalCol + mergedStartCell.Colspan - 1].Right -
								this.cols[mergedStartCell.InternalCol].Left;

#if DEBUG
							Debug.Assert(mergedStartCell.Colspan > 0);
#endif
						}

						// update merge-end-col for range
						for (int c = cell.MergeStartPos.Col; c < col; c++)
						{
							Cell leftCell = cells[r, c];
							int span = Math.Min(count, leftCell.MergeEndPos.Col - col + 1);
							leftCell.MergeEndPos = leftCell.MergeEndPos.Offset(0, -span);
						}
					}
				}

				// if any borders exist in left side, it's need to update the span of borders.
				// from columns from 0 col no need to do this
				if (col > 0
					// is border at start column 
					&& this.hBorders[r, col - 1] != null && this.hBorders[r, col - 1].Span > 0)
				{
					// find border to merge in right side
					int addspan = 0;

					// border exists in right side?
					if (this.hBorders[r, endcol] != null
						// this is not a same border range
						&& this.hBorders[r, endcol].Span + count + 1 != this.hBorders[r, col - 1].Span
						// does they have same style?
						&& this.hBorders[r, endcol].Style.Equals(this.hBorders[r, col - 1].Style)
						// does they have same owner position flag?
						&& this.hBorders[r, endcol].Pos == this.hBorders[r, col - 1].Pos)
					{
						addspan = this.hBorders[r, endcol].Span;
					}

					// update borders in left side
					int subspan = 0;

					// calc how many borders in delete target range,
					// it need be subtract from left side border.
					if (this.hBorders[r, col] != null && this.hBorders[r, col].Span > 0
						&& this.hBorders[r, col].Span == this.hBorders[r, col - 1].Span - 1)
					{
						subspan = Math.Min(this.hBorders[r, col].Span, count);
					}

					// set reference span
					int refspan = this.hBorders[r, col - 1].Span;

					this.hBorders[r, col - 1].Span += addspan - subspan;

					if (col > 1)
					{
						// update all span in left side
						for (int c = col - 2; c >= 0; c--)
						{
							if (this.hBorders[r, c] != null && this.hBorders[r, c].Span == refspan + 1)
							{
								this.hBorders[r, c].Span += addspan - subspan;
								refspan++;
							}
							else
								break;
						}
					}
				}
			}

			int rightBounds = Math.Min(this.cols.Count + count, cols.Capacity);

			// right
			for (int r = 0; r < this.rows.Count; r++)
			{
				for (int c = endcol; c < rightBounds; c++)
				{
					Cell cell = cells[r, c];

					if (cell != null)
					{
						if (cell.MergeStartPos.Col >= endcol)
						{
							cell.MergeStartPos = cell.MergeStartPos.Offset(0, -count);
							cell.Left -= totalWidth;
							cell.TextBoundsLeft -= scaledTotalWidth;
						}
						else if (cell.InternalCol >= endcol && cell.IsValidCell)
						{
							cell.Left -= totalWidth;
							cell.TextBoundsLeft -= scaledTotalWidth;
						}

						// Case:
						//
						//       col          ec
						//     +-----------+
						//     |           |
						//   0 |  1  |  2  |  3  |
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//     |     +-----|-----|
						//     |     |     |     |
						//
						else if (cell.MergeStartPos.Col >= col && cell.MergeStartPos.Col < endcol)
						{
							if (c == endcol && r == cells[r, c].MergeStartPos.Row)
							{
								Cell startCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
								Debug.Assert(startCell != null);

								// create a new merged cell
								cell.Rowspan = (short)(cell.MergeEndPos.Row - cell.MergeStartPos.Row + 1);
								cell.Colspan = (short)(startCell.Colspan - endcol + cell.MergeStartPos.Col);

								cell.Bounds = GetRangeBounds(r, cell.MergeStartPos.Col, cell.Rowspan, cell.Colspan);

								// copy cell content
								CellUtility.CopyCellContent(cell, startCell);
							}

							cell.MergeStartPos = new CellPosition(cell.MergeStartPos.Row, col);
						}

						// update merge-end-pos
						int espan = Math.Min(count, cell.MergeEndPos.Col - cell.MergeStartPos.Col);
						cell.MergeEndPos = cell.MergeEndPos.Offset(0, -espan);
					}
				}
			}
			#endregion // delete cells

			#region move cols

			for (int r = 0; r <= maxrow; r++)
			{
				#region move cells
				// move cells
				for (int c = col; c <= maxcol; c++)
				{
					var cell = cells[r, c + count];
					cells[r, c] = cell;

					if (cell != null)
					{
						cell.InternalCol -= count;
					}

					hBorders[r, c] = hBorders[r, c + count];
					vBorders[r, c] = vBorders[r, c + count];
				}
				#endregion // move cells

				// clear up borders (when most left cell or inside merged cell)
				//
				if (col == 0 || !IsInsideSameMergedCell(r, col - 1, r, col))
				{
					if (vBorders[r, col] != null && vBorders[r, col].Span == 0)
						vBorders[r, col] = null;
				}
			}
			#endregion // move cols

			#region delete outlines
#if OUTLINE
			var colOutlines = GetOutlines(RowOrColumn.Column);

			if (colOutlines != null)
			{
				List<IReoGridOutline> removingOutlines = null;

				if (action != null)
				{
					action.deletedOutlines = removingOutlines;
				}

				colOutlines.IterateOutlines(o =>
				{
					RangeModifyHelper.ProcessAfterDeleteColumn(col, count, endcol, (IColumnRange)o,
						() =>
						{
							if (action != null)
							{
								if (action.changedOutlines == null)
								{
									action.changedOutlines = new Dictionary<IReoGridOutline, BackupRangeInfo>();
								}

								action.changedOutlines[o] = new BackupRangeInfo(o.Start, o.Count);
							}
						},
						() =>
						{
							if (removingOutlines == null)
							{
								removingOutlines = new List<IReoGridOutline>();
							}

							removingOutlines.Add(o);
						});

					return true;
				});

				if (removingOutlines != null)
				{
					if (action != null)
					{
						action.deletedOutlines = removingOutlines;
					}

					// remove outlines which count <= 0
					foreach (var o in removingOutlines)
					{
						RemoveOutline(o);
					}
				}

				// when any outlines size changed, there is may cause some outlines have same position and count
				// it's nesscaray to found them out, and remove them from current worksheet
				//
				List<IReoGridOutline> deletedOutlines2 = null;

				colOutlines.IterateReverseOutlines(o =>
				{
					if (colOutlines.HasSame(o, deletedOutlines2))
					{
						if (deletedOutlines2 == null)
						{
							deletedOutlines2 = new List<IReoGridOutline>();
						}

						deletedOutlines2.Add(o);
					}

					return true;
				});

				if (deletedOutlines2 != null)
				{
					// add them into the backup list of action
					if (action != null)
					{
						if (action.deletedOutlines == null)
						{
							action.deletedOutlines = deletedOutlines2;
						}
						else
						{
							action.deletedOutlines.AddRange(deletedOutlines2);
						}
					}

					// remove outlines which count <= 0
					foreach (var o in deletedOutlines2)
					{
						RemoveOutline(o);
					}
				}
			}
#endif // OUTLINE
			#endregion // delete outlines

			#region move named ranges
			List<NamedRange> removedNamedRange = null;

			foreach (var name in this.registeredNamedRanges.Keys)
			{
				var range = this.registeredNamedRanges[name];

				RangeModifyHelper.ProcessAfterDeleteColumn(col, count, endcol, range,
					() =>
					{
						if (action != null)
						{
							if (action.changedNamedRange == null)
							{
								action.changedNamedRange = new Dictionary<NamedRange, BackupRangeInfo>();
							}

							action.changedNamedRange[range] = new BackupRangeInfo(range.Col, range.Cols);
						}
					},
					() =>
					{
						if (removedNamedRange == null)
						{
							removedNamedRange = new List<NamedRange>(1);
						}

						removedNamedRange.Add(range);
					});
			}

			// add into action backup list
			if (action != null)
			{
				action.deletedNamedRanges = removedNamedRange;
			}

			if (removedNamedRange != null)
			{
				foreach (var range in removedNamedRange)
				{
					this.UndefineNamedRange(range.Name);
				}
			}

			#endregion // move named ranges

			#region highlight ranges
			for (int i = 0; i < this.highlightRanges.Count; i++)
			{
				var range = this.highlightRanges[i];

				RangeModifyHelper.ProcessAfterDeleteColumn(col, count, endcol, range,
					() =>
					{
						if (action != null)
						{
							if (action.changedHighlightRanges == null)
							{
								action.changedHighlightRanges = new Dictionary<HighlightRange, BackupRangeInfo>();
							}

							action.changedHighlightRanges[range] = new BackupRangeInfo(range.Col, range.Cols);
						}
					},
					() =>
					{
						if (action != null)
						{
							if (action.deletedHighlightRanges == null)
							{
								action.deletedHighlightRanges = new List<HighlightRange>();
							}

							action.deletedHighlightRanges.Add(range);
						}

						RemoveHighlightRange(range);
					});
			}
			#endregion // move named ranges

			#region Floating objects
#if DRAWING
			if (this.drawingCanvas.drawingObjects != null)
			{

				foreach (var child in this.drawingCanvas.drawingObjects)
				{
					if (child.X >= left)
					{
						child.X -= totalWidth;
					}
					else if (child.Right > left)
					{
						var width = child.Width - totalWidth;
						if (width < 0) width = 0;
						child.Width = width;
					}
				}
			}
#endif // DRAWING
			#endregion // Floating objects

			#region Update used range
			// bug: rgf will save the rows has been removed, error happens when next time loading
			// https://reogrid.net/forum/viewtopic.php?id=277
			if (this.cells.MaxCol >= endcol)
			{
				this.cells.MaxCol -= count;
			}
			if (this.hBorders.MaxCol >= endcol)
			{
				this.hBorders.MaxCol -= count;
			}
			if (this.vBorders.MaxCol >= endcol)
			{
				this.vBorders.MaxCol -= count;
			}
			#endregion // Update used range

			#region Update frozen rows
			if (col < this.FreezePos.Col)
			{
				this.FreezePos = FixPos(new CellPosition(this.FreezePos.Col, this.FreezePos.Col - count));

				// remain the first column to be frozen
				if (this.FreezePos.Col < 1)
				{
					if (this.cols.Count > 1)
					{
						this.FreezePos = new CellPosition(this.FreezePos.Row, 1);
					}
					else
					{
						this.FreezePos = new CellPosition(this.FreezePos.Row, 0);
					}
				}
			}
			#endregion

			suspendingUIUpdates = false;

			UpdateViewportController();

			var selRange = FixRange(selectionRange);
			ApplyRangeSelection(selRange.StartPos, selRange.EndPos, false);
			
#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 20)
			{
				Debug.WriteLine("deleting columns takes " + ms + " ms.");
			}
#endif

			// raise column deleted event
			ColumnsDeleted?.Invoke(this, new ColumnsDeletedEventArgs(col, count));
		}

		#endregion // Delete

		#region Visible

		/// <summary>
		/// Hide specified rows.
		/// </summary>
		/// <seealso cref="ShowRows(int, int)"/>
		/// <param name="row">Index of row start to hide.</param>
		/// <param name="count">Number of rows to be hidden.</param>
		public void HideRows(int row, int count)
		{
			SetRowsHeight(row, count, 0);
		}

		/// <summary>
		/// Show specified rows.
		/// </summary>
		/// <seealso cref="HideRows(int, int)"/>
		/// <param name="row">Number of row start to show.</param>
		/// <param name="count">Number of rows to show.</param>
		public void ShowRows(int row, int count)
		{
			SetRowsHeight(row, count, r =>
			{
				var rowhead = this.rows[r];

				// just show row which is hidden
				return rowhead.IsVisible ? rowhead.InnerHeight : rowhead.LastHeight;
			}, true);
		}

		/// <summary>
		/// Hide specified columns.
		/// </summary>
		/// <seealso cref="ShowColumns(int, int)"/>
		/// <param name="col">index of start column to hide</param>
		/// <param name="count">number of columns to be hidden</param>
		public void HideColumns(int col, int count)
		{
			SetColumnsWidth(col, count, 0);
		}

		/// <summary>
		/// Show specified columns.
		/// </summary>
		/// <seealso cref="HideColumns(int, int)"/>
		/// <param name="col">Number of column start to show.</param>
		/// <param name="count">Number of columns to show.</param>
		public void ShowColumns(int col, int count)
		{
			SetColumnsWidth(col, count, c =>
			{
				var colhead = this.cols[c];

				// just show column which is hidden
				return colhead.IsVisible ? colhead.InnerWidth : colhead.LastWidth;
			});
		}

		/// <summary>
		/// Check whether or not a specified row is visible.
		/// </summary>
		/// <seealso cref="IsColumnVisible(int)"/>
		/// <param name="row">Zero-based row index to check.</param>
		/// <returns>True if the specified row on worksheet is visible; otherwise return false.</returns>
		public bool IsRowVisible(int row)
		{
			if (row < 0 || row >= this.RowCount) return false;
			return this.rows[row].IsVisible;
		}

		/// <summary>
		/// Check whether or not a specified column is visible.
		/// </summary>
		/// <seealso cref="IsRowVisible(int)"/>
		/// <param name="col">Zero-based row index to check.</param>
		/// <returns>True if the specified column on worksheet is visible; otherwise return false.</returns>
		public bool IsColumnVisible(int col)
		{
			if (col < 0 || col >= this.ColumnCount) return false;
			return this.cols[col].IsVisible;
		}

		#endregion // Visible

		#region Collection

		/// <summary>
		/// Get or set number of columns for current worksheet (must at least one column left)
		/// </summary>
		public int ColumnCount { get { return this.cols.Count; } set { SetCols(value); } }

		/// <summary>
		/// Get or set number of rows for current worksheet (must at least one row left)
		/// </summary>
		public int RowCount { get { return this.rows.Count; } set { SetRows(value); } }

		/// <summary>
		/// Get instance of row header from specified number of row
		/// (internal method, no boundary check)
		/// </summary>
		/// <param name="index">number of row to be get</param>
		/// <returns>instance of row header</returns>
		internal RowHeader RetrieveRowHeader(int index)
		{
			return this.rows[index];
		}

		/// <summary>
		/// Get the instance of column header from specified number of column
		/// (internal method, no boundary check)
		/// </summary>
		/// <param name="index">number of column to be get</param>
		/// <returns>instance of column header</returns>
		internal ColumnHeader RetrieveColumnHeader(int index)
		{
			return this.cols[index];
		}

		/// <summary>
		/// Get instance of row header from specified number of row.
		/// </summary>
		/// <param name="index">number of row to be get.</param>
		/// <returns>instance of row header.</returns>
		public RowHeader GetRowHeader(int index)
		{
			return (index < 0 || index >= this.rows.Count) ? null : this.rows[index];
		}

		/// <summary>
		/// Get the instance of column header from specified number of column.
		/// </summary>
		/// <param name="index">Number of column to be get.</param>
		/// <returns>Instance of column header.</returns>
		public ColumnHeader GetColumnHeader(int index)
		{
			return (index < 0 || index >= this.cols.Count) ? null : this.cols[index];
		}

		private RowHeaderCollection rowHeaderCollection;

		/// <summary>
		/// Get the collection of row header.
		/// </summary>
		public RowHeaderCollection RowHeaders
		{
			get
			{
				if (this.rowHeaderCollection == null)
				{
					this.rowHeaderCollection = new RowHeaderCollection(this);
				}
				return this.rowHeaderCollection;
			}
		}

		private ColumnHeaderCollection colHeaderCollection;

		/// <summary>
		/// Get the collection of column header.
		/// </summary>
		public ColumnHeaderCollection ColumnHeaders
		{
			get
			{
				if (this.colHeaderCollection == null)
				{
					this.colHeaderCollection = new ColumnHeaderCollection(this);
				}
				return this.colHeaderCollection;
			}
		}

		/// <summary>
		/// Row header collection
		/// </summary>
		public class RowHeaderCollection : IEnumerable<RowHeader>
		{
			internal Worksheet GridControl { get; set; }

			internal RowHeaderCollection(Worksheet grid)
			{
				this.GridControl = grid;
			}

			/// <summary>
			/// Get row header by zero-based index of number of row
			/// </summary>
			/// <param name="index">Zero-based number of row</param>
			/// <returns>Row header instance</returns>
			public RowHeader this[int index]
			{
				get
				{
					var grid = GridControl;
					return (index < 0 || index >= grid.rows.Count) ? null : grid.rows[index];
				}
			}

			private IEnumerator<RowHeader> GetEnum()
			{
				return this.GridControl.rows.GetEnumerator();
			}

			public IEnumerator<RowHeader> GetEnumerator()
			{
				return this.GetEnum();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnum();
			}
		}

		/// <summary>
		/// Column header collection
		/// </summary>
		public class ColumnHeaderCollection : IEnumerable<ColumnHeader>
		{
			internal Worksheet GridControl { get; set; }

			internal ColumnHeaderCollection(Worksheet grid)
			{
				this.GridControl = grid;
			}

			/// <summary>
			/// Get column header by zero-based index of number of column
			/// </summary>
			/// <param name="index">Zero-based number of column</param>
			/// <returns>Column header instance</returns>
			public ColumnHeader this[int index]
			{
				get
				{
					var grid = GridControl;
					return (index < 0 || index >= grid.cols.Count) ? null : grid.cols[index];
				}
			}

			/// <summary>
			/// Get column header by address code (e.g. A, B, Z)
			/// </summary>
			/// <param name="address">Address code to get column header</param>
			/// <returns>Column header instance</returns>
			public ColumnHeader this[string address]
			{
				get
				{
					int index = RGUtility.GetNumberOfChar(address);
					return this[index];
				}
			}

			private IEnumerator<ColumnHeader> GetEnum()
			{
				return this.GridControl.cols.GetEnumerator();
			}

			public IEnumerator<ColumnHeader> GetEnumerator()
			{
				return this.GetEnum();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnum();
			}
		}

		#endregion // Collection

		#region Events

		/// <summary>
		/// Event raised on row inserted at given index of row.
		/// </summary>
		public event EventHandler<RowsInsertedEventArgs> RowsInserted;

		/// <summary>
		/// Event raised on row deleted at given index of row.
		/// </summary>
		public event EventHandler<RowsDeletedEventArgs> RowsDeleted;

		/// <summary>
		/// Event raised on column inserted at given index of column.
		/// </summary>
		public event EventHandler<ColumnsInsertedEventArgs> ColumnsInserted;

		/// <summary>
		/// Event raised on column deleted at given index of column.
		/// </summary>
		public event EventHandler<ColumnsDeletedEventArgs> ColumnsDeleted;

		/// <summary>
		/// Event raised when row's height changed.
		/// </summary>
		public event EventHandler<RowsHeightChangedEventArgs> RowsHeightChanged;

		/// <summary>
		/// Event raised when column's width changed.
		/// </summary>
		public event EventHandler<ColumnsWidthChangedEventArgs> ColumnsWidthChanged;

		#endregion // Events
	}

	#region Header Defines
	/// <summary>
	/// Represents a base class for header instances of worksheet.
	/// </summary>
	public abstract class ReoGridHeader
	{
		internal Worksheet Worksheet { get; set; }

		internal ReoGridHeader(Worksheet sheet)
		{
			this.Worksheet = sheet;
		}

		/// <summary>
		/// Get or set whether or not to make the header visible on worksheet.
		/// </summary>
		public abstract bool IsVisible { get; set; }

		/// <summary>
		/// Zero-bsed number used to locate the header on worksheet.
		/// </summary>
		public abstract int Index { get; }

		/// <summary>
		/// Get or set user data.
		/// </summary>
		public object Tag { get; set; }
		
		private IHeaderBody body;

		/// <summary>
		/// Header body
		/// </summary>
		public IHeaderBody Body
		{
			get { return this.body; }
			set
			{
				this.body = value;
				this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Get or set the default cell body type for all cells on this column.
		/// If this value is not null, when an new instance of cells on this column is created,
		/// the cell will have a body automatically that is the instance of the type specified by this value.
		/// </summary>
		public Type DefaultCellBody { get; set; }
	}

	/// <summary>
	/// Represents a column header on worksheet.
	/// </summary>
	public class ColumnHeader : ReoGridHeader
	{
		internal ColumnHeader(Worksheet sheet)
			: base(sheet)
		{
		}

		/// <summary>
		/// Get the left position of this column header. (in pixel)
		/// </summary>
		public int Left { get; internal set; }

		internal ushort InnerWidth { get; set; }

		/// <summary>
		/// Get or set the width of this column header. (in pixel)
		/// </summary>
		public ushort Width
		{
			get { return this.InnerWidth; }
			set
			{
				this.VerifyWorksheet();
				this.Worksheet.SetColumnsWidth(this.Col, 1, value);
			}
		}

		internal ushort LastWidth { get; set; }

		internal int Col { get; set; }

		/// <summary>
		/// Get the number of column. (index cannot be changed, it managed by grid control)
		/// </summary>
		public override int Index { get { return this.Col; } }

		internal string RenderText { get; set; }

		private string text = null;

		/// <summary>
		/// Get or set the text of column header.
		/// </summary>
		public string Text
		{
			get { return this.text; }
			set
			{
				this.text = value;

				this.RenderText = (value == null) ? RGUtility.GetAlphaChar(this.Col) : value;

				if (this.Worksheet != null) this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Get the right position of column header. (in pixel)
		/// </summary>
		public int Right
		{
			get { return this.Left + this.InnerWidth; }

			internal set
			{
				int width = value - Left;
				if (width < 0) width = 0;
				this.InnerWidth = (ushort)width;
			}
		}

		internal WorksheetRangeStyle InnerStyle { get; set; }

		private ColumnHeaderStyle refStyle;

		/// <summary>
		/// Get style set of column header, modify any style in this set will affect all cells on this column.
		/// </summary>
		public ColumnHeaderStyle Style
		{
			get
			{
				if (this.refStyle == null)
				{
					this.refStyle = new ColumnHeaderStyle(this.Worksheet, this);
				}

				return this.refStyle;
			}
		}

		/// <summary>
		/// Get or set whether or not to auto adjust the width of this column.
		/// </summary>
		public bool IsAutoWidth { get; set; }

		/// <summary>
		/// Get or set whether or not to hide this column.
		/// </summary>
		public override bool IsVisible
		{
			get { return this.InnerWidth != 0; }
			set
			{
				VerifyWorksheet();

				if (value)
				{
					this.Worksheet.ShowColumns(this.Col, 1);
				}
				else
				{
					this.Worksheet.HideColumns(this.Col, 1);
				}
			}
		}

		private void VerifyWorksheet()
		{
			if (this.Worksheet == null)
			{
				throw new Exception("Column header must be associated to worksheet instance.");
			}
		}

		/// <summary>
		/// Get or set color for display the header text on spreadsheet.
		/// </summary>
		public SolidColor? TextColor { get; set; }

		/// <summary>
		/// Auto fit column width to largest cell on this column.
		/// </summary>
		/// <param name="byAction">Determines whether or not this operation 
		/// performed by doing action, which will provide the ability to undo this operation.</param>
		public void FitWidthToCells(bool byAction = false)
		{
			this.Worksheet.AutoFitColumnWidth(this.Col, byAction);
		}

		internal ColumnHeader Clone(Worksheet newSheet)
		{
			return new ColumnHeader(newSheet)
			{
				Left = this.Left,
				InnerWidth = this.InnerWidth,
				InnerStyle = this.InnerStyle == null ? null : new WorksheetRangeStyle(this.InnerStyle),
				IsAutoWidth = this.IsAutoWidth,
				TextColor = this.TextColor,
				text = this.text,
				RenderText = this.RenderText,
				Body = this.Body,
				DefaultCellBody = this.DefaultCellBody,
				Col = this.Col,
				LastWidth = this.LastWidth,
			};
		}

		/// <summary>
		/// Event raised when width changed of this column.
		/// </summary>
		public event EventHandler<ColumnsWidthChangedEventArgs> WidthChanged;

		internal void RaiseWidthChangedEvent()
		{
			if (WidthChanged != null)
			{
				WidthChanged(this, new ColumnsWidthChangedEventArgs(this.Col, 1, this.InnerWidth));
			}
		}
	}

	/// <summary>
	/// Represents a row header instance of worksheet.
	/// </summary>
	public class RowHeader : ReoGridHeader
	{
		internal RowHeader(Worksheet sheet)
			: base(sheet) { }

		/// <summary>
		/// Get the top position of header. (in pixel)
		/// </summary>
		public int Top { get; internal set; }

		internal ushort InnerHeight { get; set; }

		/// <summary>
		/// Get or set height of row. (in pixel)
		/// </summary>
		public ushort Height
		{
			get
			{
				return this.InnerHeight;
			}
			set
			{
				VerifyWorksheet();
				this.Worksheet.SetRowsHeight(this.Row, 1, value);
			}
		}

		internal ushort LastHeight { get; set; }

		/// <summary>
		/// Get the bottom position of header. (in pixel)
		/// </summary>
		public int Bottom
		{
			get { return this.Top + this.InnerHeight; }

			internal set
			{
				int height = value - Top;
				if (height < 0) height = 0;
				this.InnerHeight = (ushort)height;
			}
		}

		internal int Row { get; set; }

		/// <summary>
		/// Get the number of row. (index cannot be changed, it managed by grid control)
		/// </summary>
		public override int Index { get { return this.Row; } }

		private string text;

		/// <summary>
		/// Get or set display text for the row header.
		/// </summary>
		public string Text
		{
			get { return this.text; }
			set
			{
				this.text = value;

				if (this.Worksheet != null)
				{
					this.Worksheet.RequestInvalidate();
				}
			}
		}

		private SolidColor? textColor;

		/// <summary>
		/// Get or set the color that is used to display the header text.
		/// </summary>
		public SolidColor? TextColor
		{
			get { return this.textColor; }
			set
			{
				if (this.textColor != value)
				{
					this.textColor = value;

					if (this.Worksheet != null)
					{
						this.Worksheet.RequestInvalidate();
					}
				}
			}
		}

		internal WorksheetRangeStyle InnerStyle { get; set; }

		private RowHeaderStyle refStyle = null;

		/// <summary>
		/// Get style set of row header, modify any style in this set will affect all cells on this row.
		/// </summary>
		public RowHeaderStyle Style
		{
			get
			{
				if (this.refStyle == null)
				{
					this.refStyle = new RowHeaderStyle(this.Worksheet, this);
				}

				return this.refStyle;
			}
		}

		/// <summary>
		/// Get or set whether or not allow to automatically adjust the height in order to fit the largest cell.
		/// </summary>
		public bool IsAutoHeight { get; set; }

		/// <summary>
		/// Get or set whether or not to hide this row.
		/// </summary>
		public override bool IsVisible
		{
			get { return this.InnerHeight != 0; }
			set
			{
				VerifyWorksheet();

				if (value)
				{
					this.Worksheet.ShowRows(this.Row, 1);
				}
				else
				{
					this.Worksheet.HideRows(this.Row, 1);
				}
			}
		}

		private void VerifyWorksheet()
		{
			if (this.Worksheet == null)
			{
				throw new Exception("Row header must be associated to a grid instance.");
			}
		}

		/// <summary>
		/// Auto fit column width to largest cell on this column.
		/// </summary>
		/// <param name="byAction">Determines whether or not this operation 
		/// performed by doing action, which will provide the ability to undo this operation.</param>
		public void FitHeightToCells(bool byAction = false)
		{
			this.Worksheet.AutoFitRowHeight(this.Row, byAction);
		}

		internal RowHeader Clone(Worksheet newSheet)
		{
			return new RowHeader(newSheet)
			{
				Top = this.Top,
				InnerHeight = this.InnerHeight,
				InnerStyle = this.InnerStyle == null ? null : WorksheetRangeStyle.Clone(this.InnerStyle),
				IsAutoHeight = this.IsAutoHeight,
				TextColor = this.TextColor,
				text = this.text,
				Body = this.Body,
				Row = this.Row,
				LastHeight = this.LastHeight,
			};
		}

		/// <summary>
		/// Event raised when width changed of this column.
		/// </summary>
		public event EventHandler<RowsHeightChangedEventArgs> HeightChanged;

		internal void RaiseHeightChangedEvent()
		{
			if (this.HeightChanged != null)
			{
				this.HeightChanged(this, new RowsHeightChangedEventArgs(this.Row, 1, this.InnerHeight));
			}
		}
	}

	/// <summary>
	/// Flag to decide which orientation will be handled
	/// </summary>
	public enum RowOrColumn : byte
	{
		/// <summary>
		/// Row orientation
		/// </summary>
		Row = 1,

		/// <summary>
		/// Column orientation
		/// </summary>
		Column = 2,

		/// <summary>
		/// Both row and column (some approaches do not work with 'Both')
		/// </summary>
		Both = 3,
	}

	#endregion
}
