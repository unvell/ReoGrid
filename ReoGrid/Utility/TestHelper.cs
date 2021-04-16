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

#if WINFORM

using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using unvell.Common;
using unvell.ReoGrid.Graphics;

#if DEBUG
namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		/// <summary>
		/// [Debug Method]
		/// Validate whether border span is correct after border changes
		/// </summary>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_BorderSpan()
		{
			bool result = true;

			int span = 0;
			RangeBorderStyle lastBorder = RangeBorderStyle.Empty;

			// check h-borders
			for (int r = this.hBorders.MaxRow; r >= 0; r--)
			{
				for (int c = this.hBorders.MaxCol; c >= 0; c--)
				{
					if (hBorders[r, c] == null || hBorders[r, c].Style == null)
					{
						span = 0;
						lastBorder = RangeBorderStyle.Empty;
					}
					else
					{
						if (lastBorder != null && hBorders[r, c].Style.Equals(lastBorder))
						{
							span++;
						}
						else
						{
							lastBorder = hBorders[r, c].Style;
							span = 1;
						}

						if (hBorders[r, c].Span != span)
						{
							_Debug_MarkCellError(r, c, "hborder colspan", hBorders[r, c].Span, span);
							result = false;
						}

						//if (span == 1 && hBorders[r, c].Pos == HBorderOwnerPosition.None)
						//{
						//	_Debug_MarkCellError(r, c, "hborder has no owner pos", "none", "any pos");
						//	result = false;
						//}
					}
				}

				span = 0;
			}

			span = 0;
			lastBorder = RangeBorderStyle.Empty;

			// check v-borders
			for (int c = this.vBorders.MaxCol; c >= 0; c--)
			{
				for (int r = this.vBorders.MaxRow; r >= 0; r--)
				{
					if (vBorders[r, c] == null || vBorders[r, c].Style == null)
					{
						span = 0;
						lastBorder = RangeBorderStyle.Empty;
					}
					else
					{
						if (lastBorder != null && vBorders[r, c].Style.Equals(lastBorder))
						{
							span++;
						}
						else
						{
							lastBorder = vBorders[r, c].Style;
							span = 1;
						}

						if (vBorders[r, c].Span != span)
						{
							_Debug_MarkCellError(r, c, "vborder rowspan", vBorders[r, c].Span, span);
							result = false;
						}

						//if (span == 1 && vBorders[r, c].Pos == VBorderOwnerPosition.None)
						//{
						//	_Debug_MarkCellError(r, c, "vborder has no owner pos", "none", "any pos");
						//	result = false;
						//}
					}
				}

				span = 0;
			}

			return result;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether merged cell is correct after range merging
		/// </summary>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_MergedCells()
		{
			bool rs = true;

			int rows = Math.Min(this.rows.Count - 1, cells.MaxRow + 1);
			int cols = Math.Min(this.cols.Count - 1, cells.MaxCol + 1);

			cells.Iterate(0, 0, rows, cols, true, (row, col, cell) =>
			{
				if (row != cell.InternalRow || col != cell.InternalCol)
				{
					_Debug_MarkCellError(row, col, "cell pos", cell.InternalRow + "," + cell.InternalCol, row + "," + col);
					rs = false;
				}

				// is merged start cell
				if (rs && cell.IsStartMergedCell)
				{
					#region From start-merge-cell

					int rowspan = cell.MergeEndPos.Row - cell.MergeStartPos.Row + 1;
					int colspan = cell.MergeEndPos.Col - cell.MergeStartPos.Col + 1;

					// merged start cell rowspan
					if (cell.Rowspan != rowspan || cell.Rowspan != cell.MergeEndPos.Row - cell.InternalRow + 1)
					{
						_Debug_MarkCellError(cell.InternalRow, cell.InternalCol, "start cell rowspan", cell.Rowspan, rowspan);
						rs = false;
					}

					// merged start cell colspan
					if (cell.Colspan != colspan || cell.Colspan != cell.MergeEndPos.Col - cell.InternalCol + 1)
					{
						_Debug_MarkCellError(cell.InternalRow, cell.InternalCol, "start cell colspan", cell.Colspan, colspan);
						rs = false;
					}

					#region Validate children cells
					for (int r = row; r <= cell.MergeEndPos.Row; r++)
					{
						for (int c = col; c <= cell.MergeEndPos.Col; c++)
						{
							if (cells[r, c] == null)
							{
								_Debug_MarkCellError(r, c, "children cell null");
								rs = false;
								continue;
							}

							Cell gc = cells[r, c];

							if (!gc.IsStartMergedCell)
							{
								// merged middle cell
								if (gc.Rowspan != 0)
								{
									_Debug_MarkCellError(r, c, "cell rowspan", gc.Rowspan, 0);
									rs = false;
								}
								if (rs && gc.Colspan != 0)
								{
									_Debug_MarkCellError(r, c, "cell colspan", gc.Colspan, 0);
									rs = false;
								}
							}

							if (rs && (!gc.IsStartMergedCell && gc.MergeStartPos.Row > cell.InternalRow && gc.MergeEndPos.Col > cell.InternalCol))
							{
								_Debug_MarkCellError(r, c, "merged start pos after cell", gc.MergeStartPos, cell.InternalPos);
								rs = false;
							}

							// check cell merged start pos
							if (rs && gc.MergeStartPos != cell.InternalPos)
							{
								_Debug_MarkCellError(r, c, "cell merged start pos", gc.MergeStartPos, cell.InternalPos);
								rs = false;
							}

							// check cell merged end pos
							if (rs && gc.MergeEndPos != cell.MergeEndPos)
							{
								_Debug_MarkCellError(r, c, "cell merged end pos", gc.MergeEndPos, cell.MergeEndPos);
								rs = false;
							}

							// check right border in merged cell
							if (rs && c < this.cols.Count - 1 && c > col)
							{
								Cell rightCell = cells[r, c + 1];

								if (IsInsideSameMergedCell(gc, rightCell))
								{
									if (vBorders[r, c] == null || (vBorders[r, c] != null && vBorders[r, c].Span != 0))
									{
										_Debug_MarkCellError(r, c, "merged cell right border should be null", null, null);
									}
								}
							}

							// check bottom border in merged cell
							if (rs && r < this.rows.Count - 1 && r > row)
							{
								Cell downCell = cells[r + 1, c];

								if (IsInsideSameMergedCell(gc, downCell))
								{
									if (hBorders[r, c] == null || (hBorders[r, c] != null && hBorders[r, c].Span != 0))
									{
										_Debug_MarkCellError(r, c, "merged cell bottom border should be null", null, null);
									}
								}
							}

							if (!rs) break;
						}

						if (!rs) break;
					}
					#endregion // Validate child cells

					//Rectangle rangeBounds = GetRangeBounds(new ReoGridRange(cell.Row, cell.Col, cell.Rowspan, cell.Colspan));
					//rangeBounds.Width++;
					//rangeBounds.Height++;

					//if (cell.Bounds != rangeBounds)
					//{
					//_Debug_MarkCellError(cell.Row, cell.Col, "mismatched merged cell size", cell.Bounds, rangeBounds);
					//rs = false;
					//}

					#endregion // from start-merge-cell
				}
				// not a merged start cell, check its merged start cell
				else if (rs && (cell.Rowspan == 0 && cell.Colspan == 0))
				{
					var mergeStartCell = cells[cell.MergeStartPos.Row, cell.MergeStartPos.Col];
					if (mergeStartCell == null)
					{
						_Debug_MarkCellError(row, col, "cell merged start is null", null, "cell");
						rs = false;
					}
					else
					{
						if (mergeStartCell.MergeEndPos.Row < cell.InternalRow
							|| mergeStartCell.MergeEndPos.Col < cell.InternalCol)
						{
							_Debug_MarkCellError(row, col, "mismatched merged-start-pos");
							rs = false;
						}
					}
				}

				return rs ? 1 : 0;
			});

			return rs;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether unmerged cells are correct after range unmerging
		/// </summary>
		/// <param name="range">Range to be tested</param>
		/// <returns>True if test passed</returns>
		public bool _Debug_Validate_Unmerged_Range(RangePosition range)
		{
			if (this.rows.Count <= 0 || this.cols.Count <= 0) return true;
			//if (range.IsEmpty)
			//{
			//	range = new ReoGridRange(0, 0, rows.Count, cols.Count);
			//}
			range = FixRange(range);

			bool rs = true;

			cells.Iterate(range.Row, range.Col, range.Rows, range.Cols, true, (r, c, cell) =>
			{
				if (!cell.MergeStartPos.IsEmpty) return 1;

				// unmerged cell rowspan should be 1
				if (cell.Rowspan != 1)
				{
					_Debug_MarkCellError(r, c, "cell rowspan", cell.Rowspan, 1);
					rs = false;
				}

				// unmerged cell colspan should be 1
				if (rs && cell.Colspan != 1)
				{
					_Debug_MarkCellError(r, c, "cell colspan", cell.Colspan, 1);
					rs = false;
				}

				if (rs)
				{
					// bounds of cell must be single cell
					Rectangle bounds = GetCellRectFromHeader(r, c);
					if (cell.Bounds != bounds)
					{
						_Debug_MarkCellError(r, c, "cell bounds", cell.Bounds, bounds);
						rs = false;
					}
				}

				return rs ? 1 : 0;
			});

			return rs;
		}

		/// <summary>
		/// [Debug Method]
		/// Validate whether all cells are correct after some operations
		/// </summary>
		/// <returns></returns>
		public bool _Debug_Validate_All()
		{
			bool rs = _Debug_Validate_BorderSpan();
			if (rs) rs = _Debug_Validate_MergedCells();
			if (rs) rs = _Debug_Validate_Unmerged_Range(RangePosition.EntireRange);

			return rs;
		}

		public void _Debug_MarkCellError(int r, int c, string msg)
		{
			_Debug_MarkCellError(r, c, msg, null, null);
		}

		/// <summary>
		/// [Debug Method]
		/// Set specified cell to warning style
		/// </summary>
		/// <param name="r">Index of row</param>
		/// <param name="c">Index of column</param>
		/// <param name="msg">Message to be printed out in log</param>
		/// <param name="but">Test value to be printed</param>
		/// <param name="expect">Expect value to be printed</param>
		public void _Debug_MarkCellError(int r, int c, string msg, object but, object expect)
		{
			this.SetCellStyleOwn(r, c, new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = SolidColor.LightCoral,
			});

			if (but == null)
			{
				Logger.Log("rgdebug", string.Format("[{0},{1}] {2}", r, c, msg));
			}
			else
			{
				Logger.Log("rgdebug", string.Format("[{0},{1}] {2}, expect {3} but {4}", r, c, msg, expect, but));
				Debug.WriteLine("rgdebug: " + string.Format("[{0},{1}] {2}, expect {3} but {4}", r, c, msg, expect, but));
			}
		}
	}
}

namespace unvell.ReoGrid.Tests
{
	public enum DumpFlag : int
	{
		HBorder = 0x10,
		VBorder = 0x20,
	}

	public static class TestHelper
	{
		public static void DumpGrid(Worksheet grid, string file)
		{
			using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
			{
				DumpGrid(grid, fs);
			}
		}

		public static void DumpGrid(Worksheet grid, Stream stream)
		{
			var sw = new StreamWriter(stream);
			
			for (int row = 0; row < grid.RowCount; row++)
			{
				for (int col = 0; col < grid.ColumnCount; col++)
				{
					var hborder = grid.RetrieveHBorder(row, col);
					if (hborder != null)
					{
						sw.Write(hborder.Span);
					}
						
					sw.Write("\t");
				}

				sw.WriteLine("\n");
			}
			
		}
	}
}
#endif // DEBUG

#endif // WINFORM