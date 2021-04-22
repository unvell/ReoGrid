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
using unvell.ReoGrid.Core;
using unvell.ReoGrid.Events;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Merge
		/// <summary>
		/// Merge a range by specified address or name
		/// </summary>
		/// <param name="addressOrName">address or name to locate a range, if a name of range is specified, 
		/// the name must be defined by DefineNamedRange first.</param>
		/// <exception cref="NamedRangeNotFoundException">throw when the specified name of range cannot be found.</exception>
		public void MergeRange(string addressOrName)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				MergeRange(new RangePosition(addressOrName));
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var refRange))
			{
				MergeRange(refRange);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Merge specified range into single cell
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start column</param>
		/// <param name="rows">number of rows to be merged</param>
		/// <param name="cols">number of columns to be merged</param>
		public void MergeRange(int row, int col, int rows, int cols)
		{
			MergeRange(new RangePosition(row, col, rows, cols));
		}

		/// <summary>
		/// Merge specified range into single cell
		/// </summary>
		/// <exception cref="RangeTooSmallException">thrown when specified range has only one cell.</exception>
		/// <exception cref="RangeIntersectionException">thrown when specified range intersectes with another one. </exception>
		/// <param name="range">Range to be merged</param>
		/// <seealso cref="UnmergeRange"/>
		public void MergeRange(RangePosition range)
		{
			MergeRange(range, true);
		}

		internal void MergeRange(RangePosition range, bool checkIntersection = true, bool updateUIAndEvent = true)
		{
			if (range.IsEmpty) return;

			RangePosition fixedRange = FixRange(range);

			if (fixedRange.Cols <= 1 && fixedRange.Rows <= 1)
			{
				return;
			}

			if (checkIntersection)
			{
				RangePosition intersectedRange = CheckIntersectedMergingRange(fixedRange);
				if (!intersectedRange.IsEmpty)
				{
					throw new RangeIntersectionException(intersectedRange);
				}
			}

			int row = fixedRange.Row;
			int col = fixedRange.Col;
			int torow = fixedRange.EndRow;
			int tocol = fixedRange.EndCol;

			// find start and end cell
			Cell startCell = cells[row, col];
			Cell endCell = cells[torow, tocol];

			if (startCell == null) startCell = CreateCell(row, col);
			if (endCell == null) endCell = CreateCell(torow, tocol);

			for (int r = row; r <= torow; r++)
			{
				for (int c = col; c <= tocol; c++)
				{
					Cell cell = CreateAndGetCell(r, c);

					// reference to start and end pos
					cell.MergeStartPos = startCell.InternalPos;
					cell.MergeEndPos = endCell.InternalPos;

					// close col and row span
					cell.Colspan = 0;
					cell.Rowspan = 0;

					// apply text to merged start cell
					if (cell != startCell) cell.InnerData = cell.InnerDisplay = null;

					if (r == row)
					{
						if (c > col) CutBeforeVBorder(r, c);
					}
					else
					{
						hBorders[r, c] = new ReoGridHBorder { Span = 0 };
					}

					if (c == col)
					{
						if (r > row) CutBeforeHBorder(r, c);
					}
					else
					{
						vBorders[r, c] = new ReoGridVBorder { Span = 0 };
					}
				}
			}

			// update the spans for start cell
			startCell.Rowspan = (short)fixedRange.Rows;
			startCell.Colspan = (short)fixedRange.Cols;

			// update content bounds
			UpdateCellBounds(startCell);

			// fix selection
			if (this.selectionRange.IntersectWith(fixedRange))
			{
				this.selectionRange = this.CheckMergedRange(fixedRange);
			}

			// fix focus pos
			if (fixedRange.Contains(this.focusPos))
			{
				this.focusPos = fixedRange.StartPos;
			}

			if (updateUIAndEvent)
			{
				RequestInvalidate();

				if (RangeMerged != null)
				{
					RangeMerged(this, new RangeEventArgs(fixedRange));
				}
			}
		}
		#endregion // Merge

		#region Unmerge
		/// <summary>
		/// Unmerge specified range.
		/// </summary>
		/// <param name="row">Number of row of range to be unmerged.</param>
		/// <param name="col">Number of column of range to be unmerged.</param>
		/// <param name="rows">Number of rows in the range to be unmerged.</param>
		/// <param name="cols">Number of columns in the range to be unmerged.</param>
		public void UnmergeRange(int row, int col, int rows, int cols)
		{
			UnmergeRange(new RangePosition(row, col, rows, cols));
		}

		/// <summary>
		/// Unmerge all cells contained in the specified range.
		/// </summary>
		/// <seealso cref="MergeRange"/>
		/// <param name="range">Range to be checked and all cells in this range will be unmerged.</param>
		public void UnmergeRange(RangePosition range)
		{
			if (range.IsEmpty) return;

			range = FixRange(range);

			int row = range.Row;
			int col = range.Col;
			int torow = range.Row + range.Rows - 1;
			int tocol = range.Col + range.Cols - 1;

			for (int r = row; r <= torow; r++)
			{
				for (int c = col; c <= tocol; c++)
				{
					Cell cell = CreateAndGetCell(r, c);

					if (cell.Colspan > 1 || cell.Rowspan > 1)
					{
						UnmergeCell(cell);
						c += cell.Colspan;
					}
				}
			}

			RequestInvalidate();
		}

		private void UnmergeCell(Cell source)
		{
			//WorksheetRangeStyle style = source.InnerStyle;
			int r2 = source.InternalRow + source.Rowspan;
			int c2 = source.InternalCol + source.Colspan;

			RangePosition range = new RangePosition(source.InternalRow, source.InternalCol, source.Rowspan, source.Colspan);

			for (int r = source.InternalRow; r < r2; r++)
			{
				for (int c = source.InternalCol; c < c2; c++)
				{
					Cell cell = CreateAndGetCell(r, c);

					cell.MergeStartPos = CellPosition.Empty;
					cell.MergeEndPos = CellPosition.Empty;
					cell.Colspan = 1;
					cell.Rowspan = 1;
					UpdateCellBounds(cell);

					if (r != source.InternalRow) hBorders[r, c] = null;
					if (c != source.InternalCol) vBorders[r, c] = null;

					if (r != source.InternalRow || c != source.InternalCol)
					{
						cell.InnerStyle = source.InnerStyle;
						cell.StyleParentKind = StyleParentKind.Range;
					}
				}
			}

			if (RangeUnmerged != null)
			{
				RangeUnmerged(this, new RangeEventArgs(range));
			}
		}
		#endregion // Unmerge

		#region Event
		/// <summary>
		/// Event raised when range merged
		/// </summary>
		public event EventHandler<RangeEventArgs> RangeMerged;

		/// <summary>
		/// Event raised when range unmerged
		/// </summary>
		public event EventHandler<RangeEventArgs> RangeUnmerged;

		#endregion Event

		#region Utility

		/// <summary>
		/// Check are there any merged range exist in specified range
		/// </summary>
		/// <param name="range">range to be checked</param>
		/// <returns>the intersected range with specified range</returns>
		public RangePosition CheckIntersectedMergingRange(RangePosition range)
		{
			RangePosition intersectedRange = RangePosition.Empty;

			cells.Iterate(range.Row, range.Col, range.Rows, range.Cols, true, (r, c, cell) =>
			{
				if (!cell.MergeStartPos.IsEmpty)
				{
					Cell checkStartCell = GetCell(cell.MergeStartPos);
					for (int rr = checkStartCell.InternalRow; rr <= checkStartCell.MergeEndPos.Row; rr++)
					{
						for (int cc = checkStartCell.InternalCol; cc <= checkStartCell.MergeEndPos.Col; cc++)
						{
							var targetCell = cells[rr, cc];
							if (targetCell != null && !range.Contains(targetCell.InternalPos))
							{
								intersectedRange = new RangePosition(checkStartCell.InternalPos, checkStartCell.MergeEndPos);
								break;
							}
						}

						if (!intersectedRange.IsEmpty) break;
					}

					return intersectedRange.IsEmpty ? 0 : cell.Colspan;
				}
				return cell.Colspan < 1 ? 1 : cell.Colspan;
			});

			return intersectedRange;
		}

		/// <summary>
		/// Check are there any merged range exist in specified range
		/// </summary>
		/// <param name="range">range to be checked</param>
		/// <returns>true if specified range can be merged</returns>
		public bool HasIntersectedMergingRange(RangePosition range)
		{
			return !CheckIntersectedMergingRange(range).IsEmpty;
		}

		/// <summary>
		/// Check whether a  range intersects with others merged range, return the most outside range which contains all these intersected range.
		/// </summary>
		/// <param name="range">Range to be checked.</param>
		/// <returns>Most outside range that contains all intersected range.</returns>
		public RangePosition CheckMergedRange(RangePosition range)
		{
			int minr = range.Row;
			int minc = range.Col;
			int maxr = range.EndRow;
			int maxc = range.EndCol;

			bool boundsChanged = false;
			Cell cell = null;

			do
			{
				boundsChanged = false;

				for (int r = minr; r <= maxr; r++)
				{
					cell = cells[r, minc];

					if (cell != null && cell.MergeStartPos != CellPosition.Empty)
					{
						int rr = cell.MergeStartPos.Row;
						if (minr > rr)
						{
							minr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeStartPos.Col;
						if (minc > cc)
						{
							minc = cc;
							boundsChanged = true;
						}
					}
				}

				for (int r = minr; r <= maxr; r++)
				{
					cell = cells[r, maxc];

					if (cell != null && cell.MergeEndPos != CellPosition.Empty)
					{
						int rr = cell.MergeEndPos.Row;
						if (maxr < rr)
						{
							maxr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeEndPos.Col;
						if (maxc < cc)
						{
							maxc = cc;
							boundsChanged = true;
						}
					}
				}

				for (int c = minc; c <= maxc; c++)
				{
					cell = cells[minr, c];

					if (cell != null && cell.MergeStartPos != CellPosition.Empty)
					{
						int rr = cell.MergeStartPos.Row;
						if (minr > rr)
						{
							minr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeStartPos.Col;
						if (minc > cc)
						{
							minc = cc;
							boundsChanged = true;
						}
					}
				}

				for (int c = minc; c <= maxc; c++)
				{
					cell = cells[maxr, c];

					if (cell != null && cell.MergeEndPos != CellPosition.Empty)
					{
						int rr = cell.MergeEndPos.Row;
						if (maxr < rr)
						{
							maxr = rr;
							boundsChanged = true;
						}
						int cc = cell.MergeEndPos.Col;
						if (maxc < cc)
						{
							maxc = cc;
							boundsChanged = true;
						}
					}
				}
			} while (boundsChanged);

			return new RangePosition(minr, minc, maxr - minr + 1, maxc - minc + 1);
		}

		private bool IsInsideSameMergedCell(int r1, int c1, int r2, int c2)
		{
			return IsInsideSameMergedCell(cells[r1, c1], cells[r2, c2]);
		}

		private static bool IsInsideSameMergedCell(Cell cell1, Cell cell2)
		{
			return cell1 != null && cell2 != null
				&& !cell1.MergeStartPos.IsEmpty
				&& CellPosition.Equals(cell1.MergeStartPos, cell2.MergeStartPos);
		}

		/// <summary>
		/// Check whether or not specified range contains only one cell, merged cell is treated as one cell.
		/// </summary>
		/// <param name="range">Range to be checked</param>
		/// <returns>True if the range contains only one cell or one merged cell; otherwise return false.</returns>
		public bool RangeIsMergedCell(RangePosition range)
		{
			Cell cell = cells[range.Row, range.Col];
			return (cell == null && range.Rows == 1 && range.Cols == 1)
				|| (cell != null && cell.Rowspan == range.Rows && cell.Colspan == range.Cols);
		}

		#endregion // Utility
	}
}
