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
using System.Linq;
using System.Text;

using unvell.ReoGrid.Core;

#if FORMULA
using unvell.ReoGrid.Formula;
#endif // FORMULA

using unvell.ReoGrid.Utility;

#if WINFORM || WPF
//using CellArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.ReoGridCell>;
//using HBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridHBorder>;
//using VBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridVBorder>;
using CellArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Cell>;
using HBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridHBorder>;
using VBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridVBorder>;
#elif ANDROID || iOS
using CellArray = unvell.ReoGrid.Data.ReoGridCellArray;
using HBorderArray = unvell.ReoGrid.Data.ReoGridHBorderArray;
using VBorderArray = unvell.ReoGrid.Data.ReoGridVBorderArray;
#endif // ANDROID

namespace unvell.ReoGrid
{
	#region Partial Grid
	/// <summary>
	/// Partial spreadsheet copy operation flag
	/// </summary>
	public enum PartialGridCopyFlag : int
	{
		/// <summary>
		/// All content will be processed
		/// </summary>
		All = CellAll | BorderAll,

		/// <summary>
		/// Cell value and cell styles will be processed (CellData | CellFormula | CellFormat | CellStyle)
		/// </summary>
		CellAll = CellData | CellFormula | CellFormat | CellStyle,

		/// <summary>
		/// Copy cells data.
		/// </summary>
		CellData = 0x1,

		/// <summary>
		/// Copy cells formula.
		/// </summary>
		CellFormula = 0x4,

		/// <summary>
		/// Copy cells data format.
		/// </summary>
		CellFormat = 0x4,

		/// <summary>
		/// Copy cells style.
		/// </summary>
		CellStyle = 0x8,

		/// <summary>
		/// Copy all borders. (HBorder | VBorder)
		/// </summary>
		BorderAll = HBorder | VBorder,

		/// <summary>
		/// Only horizontal borders will be processed
		/// </summary>
		HBorder = 0x10,

		/// <summary>
		/// Only vertical borders will be processed
		/// </summary>
		VBorder = 0x20,
	}

	internal enum ExPartialGridCopyFlag
	{
		None,

		/// <summary>
		/// Copy all borders that around the cells (ignores border's owner property)
		/// </summary>
		BorderOutsideOwner = 0x10,

		// copy merged-range-info 

		//CellBounds,
	}

	/// <summary>
	/// Partial grid contains the cells and borders, including the data and styles from 
	/// </summary>
	[Serializable]
	public class PartialGrid
	{
		private int rows;

		/// <summary>
		/// Number of rows in this partial grid
		/// </summary>
		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		private int cols;

		/// <summary>
		/// Number of columns in this partial grid
		/// </summary>
		public int Columns
		{
			get { return cols; }
			set { cols = value; }
		}

		private CellArray cells;
#if DEBUG
		public
#else
		internal
#endif
		CellArray Cells
		{
			get { return cells; }
			set { cells = value; }
		}

		private HBorderArray hBorders;
		internal HBorderArray HBorders
		{
			get { return hBorders; }
			set { hBorders = value; }
		}

		private VBorderArray vBorders;
		internal VBorderArray VBorders
		{
			get { return vBorders; }
			set { vBorders = value; }
		}

		/// <summary>
		/// Create an empty partial grid without and data, borders and styles
		/// </summary>
		public PartialGrid()
		{
		}

		/// <summary>
		/// Create an empty partial grid with specified capacity
		/// </summary>
		/// <param name="initRows">capacity of rows</param>
		/// <param name="initCols">capacity of cols</param>
		public PartialGrid(int initRows, int initCols)
		{
			this.rows = initRows;
			this.cols = initCols;
		}

		/// <summary>
		/// Create grid with specified data
		/// </summary>
		/// <param name="data">data to fill this partial grid</param>
		public PartialGrid(object[,] data)
		{
			this.cells = new CellArray();
			this.rows = data.GetLength(0);
			this.cols = data.GetLength(1);

			for (int r = 0; r < this.rows; r++)
			{
				for (int c = 0; c < this.cols; c++)
				{
					this.cells[r, c] = new Cell(null)
					{
						InternalRow = r,
						InternalCol = c,
						Rowspan = 1,
						Colspan = 1,
						InnerData = data[r, c],
					};
				}
			}
		}

		/// <summary>
		/// Compare this partial grid to another object
		/// </summary>
		/// <param name="obj">object to be compared</param>
		/// <returns>true if the object is same as this partial grid, otherwise false</returns>
		public override bool Equals(object obj)
		{
			if (obj is PartialGrid)
				return Equals((PartialGrid)obj, PartialGridCopyFlag.All);
			else
				return base.Equals(obj);
		}

		/// <summary>
		/// Compare this partial grid to another grid with specified comparison flag
		/// </summary>
		/// <param name="anotherPartialGrid">another partial grid to be compared</param>
		/// <param name="flag">comparison flag</param>
		/// <returns>true if two partial grid are same, otherwise return false</returns>
		public bool Equals(PartialGrid anotherPartialGrid, PartialGridCopyFlag flag)
		{
			if (anotherPartialGrid.rows != rows
				|| anotherPartialGrid.cols != cols)
				return false;

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					var a = cells[r, c];
					var b = anotherPartialGrid.cells[r, c];

					if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData)
					{
						if (a == null && b != null
							|| b == null && a != null
							|| a.InnerData == null || b.InnerData != null
							|| b.InnerData == null || a.InnerData != null)
							return false;

						if (Convert.ToString(a.DisplayText) != Convert.ToString(b.DisplayText))
							return false;

						if (!string.Equals(a.InnerFormula, b.InnerFormula, StringComparison.CurrentCultureIgnoreCase))
							return false;

						if (a.DataFormat != b.DataFormat)
							return false;

						if (a.DataFormatArgs == null && b.DataFormatArgs != null
							|| a.DataFormatArgs != null && b.DataFormatArgs == null)
							return false;

						if (!a.DataFormatArgs.Equals(b.DataFormatArgs))
							return false;
					}

					if ((flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle
						&& WorksheetRangeStyle.Equals(a.InnerStyle, b.InnerStyle))
						return false;

					if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
					{
						var ba = hBorders[r, c];
						var bb = anotherPartialGrid.hBorders[r, c];

						if (ba == null && bb != null
							|| bb == null && ba != null)
							return false;

						if (ba.Span != bb.Span
							|| !ba.Style.Equals(bb.Style)
							|| !ba.Pos.Equals(bb.Pos))
							return false;
					}

					if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
					{
						var ba = vBorders[r, c];
						var bb = anotherPartialGrid.vBorders[r, c];

						if (ba == null && bb != null
							|| bb == null && ba != null)
							return false;

						if (ba.Span != bb.Span
							|| !ba.Style.Equals(bb.Style)
							|| !ba.Pos.Equals(bb.Pos))
							return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Get hash code of this prtail grid object
		/// </summary>
		/// <returns>generated hash code</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
	#endregion

	partial class Worksheet
	{
		#region Partial Grid

		/// <summary>
		/// Copy a part of worksheet from specified range that identified by address or name.
		/// </summary>
		/// <param name="addressOrName">The address or name to locate a range position on worksheet.</param>
		/// <returns>A part of worksheet that is copied from original worksheet.</returns>
		public PartialGrid GetPartialGrid(string addressOrName)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				return this.GetPartialGrid(new RangePosition(addressOrName));
			}
			else if (this.TryGetNamedRange(addressOrName, out var namedRange))
			{
				return this.GetPartialGrid(namedRange.Position);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Copy a part of worksheet from specified range that identified by address value.
		/// </summary>
		/// <param name="row">Number of start row.</param>
		/// <param name="col">Number of start col.</param>
		/// <param name="rows">Number of rows to be copied.</param>
		/// <param name="cols">Number of columns to be copied.</param>
		/// <returns>A part of worksheet that is copied from original worksheet.</returns>
		public PartialGrid GetPartialGrid(int row, int col, int rows, int cols)
		{
			return GetPartialGrid(new RangePosition(row, col, rows, cols));
		}

		/// <summary>
		/// Copy a part of worksheet from specified range.
		/// </summary>
		/// <param name="range">The range to be copied.</param>
		/// <returns>A part of worksheet that is copied from original worksheet.</returns>
		public PartialGrid GetPartialGrid(RangePosition range)
		{
			return GetPartialGrid(range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		internal PartialGrid GetPartialGrid(RangePosition range, PartialGridCopyFlag flag, ExPartialGridCopyFlag exFlag,
			bool checkIntersectedRange = false)
		{
			range = FixRange(range);

			if (checkIntersectedRange)
			{
				var intersectedRange = CheckIntersectedMergingRange(range);
				if (intersectedRange != RangePosition.Empty)
				{
					throw new RangeIntersectionException(intersectedRange);
				}
			}

			int rows = range.Rows;
			int cols = range.Cols;

			PartialGrid data = new PartialGrid()
			{
				Columns = cols,
				Rows = rows,
			};

			if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData
				|| (flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle)
			{
				data.Cells = new CellArray();

				for (int r = range.Row; r <= range.EndRow; r++)
				{
					for (int c = range.Col; c <= range.EndCol; c++)
					{
						var cell = this.cells[r, c];

						int toRow = r - range.Row;
						int toCol = c - range.Col;

						//if (cell == null && data.Cells[toRow, toCol] == null)
						//{
						//	c++;
						//	continue;
						//}

						Cell toCell = null;

						if (cell != null)
						{
							toCell = new Cell(this);
							CellUtility.CopyCell(toCell, cell);
						}
						else
						{
							StyleParentKind pKind = StyleParentKind.Own;
							var style = StyleUtility.FindCellParentStyle(this, r, c, out pKind);

							style = StyleUtility.DistinctStyle(style, Worksheet.DefaultStyle);

							if (style != null)
							{
								toCell = new Cell(this);
								toCell.Colspan = 1;
								toCell.Rowspan = 1;
								toCell.InnerStyle = style;
								toCell.StyleParentKind = StyleParentKind.Own;
							}
						}

						if (toCell != null)
						{
							data.Cells[toRow, toCol] = toCell;
						}

						//c += (cell == null || cell.Colspan < 1) ? 1 : cell.Colspan;
					}
				}
			}

			if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
			{
				data.HBorders = new HBorderArray();

				hBorders.Iterate(range.Row, range.Col, rows + 1, cols, true, (r, c, hBorder) =>
				{
					// only copy borders they belong to the cell (unless BorderOutsideOwner is specified)
					if (((exFlag & ExPartialGridCopyFlag.BorderOutsideOwner) == ExPartialGridCopyFlag.BorderOutsideOwner)
						|| (hBorder != null && hBorder.Pos == HBorderOwnerPosition.None)
						|| (
								(r != range.Row
								|| (hBorder != null
								&& (hBorder.Pos & HBorderOwnerPosition.Top) == HBorderOwnerPosition.Top))
							&&
								(r != range.EndRow + 1
								|| (hBorder != null
								&& (hBorder.Pos & HBorderOwnerPosition.Bottom) == HBorderOwnerPosition.Bottom)))
					)
					{
						int toCol = c - range.Col;
						ReoGridHBorder thBorder = ReoGridHBorder.Clone(hBorder);
						if (thBorder != null && thBorder.Span > cols - toCol) thBorder.Span = cols - toCol;
						data.HBorders[r - range.Row, toCol] = thBorder;
					}
					return 1;
				});
			}

			if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
			{
				data.VBorders = new VBorderArray();

				vBorders.Iterate(range.Row, range.Col, rows, cols + 1, true, (r, c, vBorder) =>
				{
					// only copy borders they belong to the cell (unless BorderOutsideOwner is specified)
					if (((exFlag & ExPartialGridCopyFlag.BorderOutsideOwner) == ExPartialGridCopyFlag.BorderOutsideOwner)
						|| (vBorder != null && vBorder.Pos == VBorderOwnerPosition.None)
						|| (
								(c != range.Col
								|| (vBorder != null
								&& (vBorder.Pos & VBorderOwnerPosition.Left) == VBorderOwnerPosition.Left))
							&&
								(c != range.EndCol + 1
								|| (vBorder != null
								&& (vBorder.Pos & VBorderOwnerPosition.Right) == VBorderOwnerPosition.Right)))
					)
					{
						int toRow = r - range.Row;
						ReoGridVBorder tvBorder = ReoGridVBorder.Clone(vBorder);
						if (tvBorder != null && tvBorder.Span > rows - toRow) tvBorder.Span = rows - toRow;
						data.VBorders[toRow, c - range.Col] = tvBorder;
					}
					return 1;
				});
			}

			return data;
		}

		/// <summary>
		/// Copy a part of worksheet into current worksheet.
		/// </summary>
		/// <param name="addressOrName">The target range position specified by address or name.</param>
		/// <param name="data">A part of worksheet to be copied.</param>
		/// <returns>The range position that is the range filled actually.</returns>
		public RangePosition SetPartialGrid(string addressOrName, PartialGrid data)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				return this.SetPartialGrid(new RangePosition(addressOrName), data);
			}
			else if (this.TryGetNamedRange(addressOrName, out var namedRange))
			{
				return this.SetPartialGrid(namedRange.Position, data);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Copy from a separated grid into current grid.
		/// </summary>
		/// <param name="data">Partial grid to be copied.</param>
		/// <param name="toRange">Range to be copied.</param>
		/// <returns>Range has been copied</returns>
		public RangePosition SetPartialGrid(RangePosition toRange, PartialGrid data)
		{
			return this.SetPartialGrid(toRange, data, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
		}

		internal RangePosition SetPartialGrid(RangePosition toRange, PartialGrid data,
			PartialGridCopyFlag flag)
		{
			return this.SetPartialGrid(toRange, data, flag, ExPartialGridCopyFlag.None);
		}

		internal RangePosition SetPartialGrid(RangePosition toRange, PartialGrid data,
			PartialGridCopyFlag flag, ExPartialGridCopyFlag exFlag)
		{
			if (toRange.IsEmpty) return toRange;

			toRange = FixRange(toRange);

			int rows = data.Rows;
			int cols = data.Columns;

			if (rows + toRange.Row > this.rows.Count) rows = this.rows.Count - toRange.Row;
			if (cols + toRange.Col > this.cols.Count) cols = this.cols.Count - toRange.Col;

			if (((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData
				|| (flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle))
			{
				for (int r = 0; r < rows; r++)
				{
					for (int c = 0; c < cols; c++)
					{
						Cell fromCell = data.Cells == null ? null : data.Cells[r, c];

						int tr = toRange.Row + r;
						int tc = toRange.Col + c;

						bool processed = false;

						if (fromCell != null)
						{
							#region Merge from right side
							// from cell copied as part of merged cell
							if (
								// is part of merged cell
								!fromCell.MergeStartPos.IsEmpty

								&& fromCell.MergeStartPos.Col < toRange.Col
								// is right side     -------+--      (undo from delete column at end of merged range) 
								&& (fromCell.InternalCol - fromCell.MergeStartPos.Col > tc - toRange.Col
								// not inside       --+----+--     
								&& fromCell.MergeEndPos.Col <= toRange.EndCol))
							{
								// from cell inside existed merged range
								// these two ranges should be merged
								// the original range must be expanded
								Cell fromMergedStart = CreateAndGetCell(fromCell.MergeStartPos);
								fromMergedStart.MergeEndPos = new CellPosition(fromMergedStart.MergeEndPos.Row, tc);
								fromMergedStart.Colspan = (short)(tc - fromMergedStart.InternalCol + 1);

								for (int ic = fromMergedStart.InternalCol; ic < fromMergedStart.InternalCol + fromMergedStart.Colspan; ic++)
								{
									var insideCell = cells[tr, ic];
									if (insideCell != null)
									{
										insideCell.MergeEndPos = new CellPosition(insideCell.MergeEndPos.Row, tc);
									}
								}

								Cell tocell = CreateAndGetCell(tr, tc);
								tocell.MergeStartPos = fromMergedStart.InternalPos;
								tocell.MergeEndPos = new CellPosition(fromMergedStart.MergeEndPos.Row, tc);
								tocell.Colspan = 0;
								tocell.Rowspan = 0;

								if (tocell.IsEndMergedCell)
								{
									fromMergedStart.Bounds = GetRangePhysicsBounds(new RangePosition(
										fromMergedStart.InternalPos, fromMergedStart.MergeEndPos));
								}

								processed = true;

							}
							#endregion
							#region Merge from left side
							// usually used when undo action: when deleting column from left side of merged cell
							else if (
								!fromCell.MergeEndPos.IsEmpty
							
								// added 3/15/2016: check two unrelated ranges 
								&& toRange.ContainsRow(fromCell.Row)

								&& fromCell.MergeEndPos.Col > toRange.EndCol
								&& fromCell.MergeStartPos.Col <= toRange.EndCol
								)
							{
								// target partial range will override exsited range
								// need to update existed range at right side
								int rightCol = Math.Min(fromCell.MergeEndPos.Col, this.cols.Count - 1);

								Cell tocell = CreateAndGetCell(tr, tc);
								tocell.MergeStartPos = new CellPosition(fromCell.MergeStartPos.Row, fromCell.MergeStartPos.Col + tc - fromCell.InternalCol);
								tocell.MergeEndPos = new CellPosition(fromCell.MergeEndPos.Row, rightCol);

								for (int ic = toRange.EndCol + 1; ic <= rightCol; ic++)
								{
									var existedEndCell = CreateAndGetCell(tr, ic);
									existedEndCell.MergeStartPos = tocell.MergeStartPos;
									existedEndCell.Rowspan = 0;
									existedEndCell.Colspan = 0;
								}

								if (tocell.IsStartMergedCell)
								{
									tocell.Rowspan = (short)(tocell.MergeEndPos.Row - tocell.MergeStartPos.Row + 1);
									tocell.Colspan = (short)(tocell.MergeEndPos.Col - tocell.MergeStartPos.Col + 1);

									tocell.Bounds = GetRangeBounds(tocell.InternalPos, tocell.MergeEndPos);

									// copy cell content
									CellUtility.CopyCellContent(tocell, fromCell);

									UpdateCellFont(tocell);
								}
								else
								{
									tocell.Rowspan = 0;
									tocell.Colspan = 0;
								}

								processed = true;
							}
							#endregion // Merge from left side
							#region Merge from bottom
							else if (
								!fromCell.MergeStartPos.IsEmpty
								// above
								&& fromCell.MergeStartPos.Row < toRange.Row
								// merged start row in the above of target fill range
								&& fromCell.InternalRow - fromCell.MergeStartPos.Row > tr - toRange.Row
								// not inside current merged range
								&& fromCell.MergeEndPos.Row <= toRange.EndRow)
							{
								var mergedStartCell = CreateAndGetCell(fromCell.MergeStartPos);
								mergedStartCell.Rowspan = (short)(tr - mergedStartCell.InternalRow + 1);

								for (int ir = fromCell.MergeStartPos.Row; ir < tr; ir++)
								{
									var existedCell = CreateAndGetCell(ir, tc);
									existedCell.MergeEndPos = new CellPosition(tr, fromCell.MergeEndPos.Col);
								}

								var tocell = CreateAndGetCell(tr, tc);
								tocell.MergeStartPos = mergedStartCell.InternalPos;
								tocell.MergeEndPos = new CellPosition(tr, fromCell.MergeEndPos.Col);
								tocell.Rowspan = 0;
								tocell.Colspan = 0;

								if (tocell.IsEndMergedCell)
								{
									mergedStartCell.Bounds = GetRangeBounds(mergedStartCell.InternalPos, mergedStartCell.MergeEndPos);
								}

								processed = true;
							}
							#endregion // Merge from bottom
							#region Merge from top
							// usually used when undo action: when deleting column from top side of merged cell
							else if (
								!fromCell.MergeEndPos.IsEmpty

								// added 3/15/2016: check two unrelated ranges 
								&& toRange.ContainsColumn(fromCell.Column)

								&& fromCell.MergeEndPos.Row > toRange.EndRow
								&& fromCell.MergeStartPos.Row <= toRange.EndRow)
							{
								// target partial range will override exsited range
								// need to update existed range at right side
								int bottomRow = Math.Min(fromCell.MergeEndPos.Row, this.rows.Count - 1);

								for (int ir = toRange.EndRow + 1; ir <= bottomRow; ir++)
								{
									var existedEndCell = CreateAndGetCell(ir, tc);
									existedEndCell.MergeStartPos = new CellPosition(fromCell.MergeStartPos.Row, existedEndCell.MergeStartPos.Col);
									existedEndCell.Rowspan = 0;
									existedEndCell.Colspan = 0;
								}

								Cell tocell = CreateAndGetCell(tr, tc);
								tocell.MergeStartPos = fromCell.MergeStartPos;
								tocell.MergeEndPos = new CellPosition(bottomRow, fromCell.MergeEndPos.Col);

								if (tocell.IsStartMergedCell)
								{
									tocell.Rowspan = (short)(tocell.MergeEndPos.Row - tocell.MergeStartPos.Row + 1);
									tocell.Colspan = (short)(tocell.MergeEndPos.Col - tocell.MergeStartPos.Col + 1);

									tocell.Bounds = GetRangeBounds(tocell.InternalPos, tocell.MergeEndPos);

									// copy cell content
									CellUtility.CopyCellContent(tocell, fromCell);

									UpdateCellFont(tocell);
								}
								else
								{
									tocell.Rowspan = 0;
									tocell.Colspan = 0;
								}

								processed = true;
							}
							#endregion // Merge from top
						}

						if (!processed)
						{
							Cell toCell = CreateAndGetCell(tr, tc);

							if (toCell.Rowspan == 0 && toCell.Colspan == 0)
							{
								continue;
							}

							if (fromCell != null)
							{
								#region Copy Data
								if ((flag & PartialGridCopyFlag.CellData) == PartialGridCopyFlag.CellData)
								{
									CellUtility.CopyCellContent(toCell, fromCell);
								}
								#endregion // Copy Data

								#region Format Formula
#if FORMULA
								if ((flag & PartialGridCopyFlag.CellFormula) == PartialGridCopyFlag.CellFormula)
								{
									if (fromCell.HasFormula)
									{
										if (fromCell.formulaTree == null)
										{
											try
											{
												fromCell.formulaTree = Formula.Parser.Parse(this.workbook, fromCell, fromCell.InnerFormula);
											}
											catch
											{
												fromCell.formulaStatus = FormulaStatus.SyntaxError;
											}
										}

										if (fromCell.formulaTree != null)
										{
											var rs = new ReplacableString(fromCell.InnerFormula);
											Stack<List<Cell>> dirtyCells = new Stack<List<Cell>>();
											FormulaRefactor.CopyFormula(fromCell.Position, fromCell.formulaTree, toCell, rs, dirtyCells);
										}

										toCell.FontDirty = true;
									}
								}
								else
								{
									toCell.InnerFormula = null;
								}
#endif // FORMULA
#endregion // Formula Formula

											#region Copy Merged info

											// is single cell
											if (toCell.Rowspan == 1 && toCell.Colspan == 1)
								{
									// then copy span info
									toCell.Rowspan = fromCell.Rowspan;
									toCell.Colspan = fromCell.Colspan;

									if (!fromCell.MergeStartPos.IsEmpty)
									{
										toCell.MergeStartPos = fromCell.MergeStartPos.Offset(tr - fromCell.InternalRow, tc - fromCell.InternalCol);

#if DEBUG
										Debug.Assert(toCell.MergeStartPos.Row >= 0 && toCell.MergeStartPos.Row < this.rows.Count);
										Debug.Assert(toCell.MergeStartPos.Col >= 0 && toCell.MergeStartPos.Col < this.cols.Count);
#endif
									}

									if (!fromCell.MergeEndPos.IsEmpty)
									{
										toCell.MergeEndPos = fromCell.MergeEndPos.Offset(tr - fromCell.InternalRow, tc - fromCell.InternalCol);

#if DEBUG
										Debug.Assert(toCell.MergeEndPos.Row >= 0 && toCell.MergeEndPos.Row < this.rows.Count);
										Debug.Assert(toCell.MergeEndPos.Col >= 0 && toCell.MergeEndPos.Col < this.cols.Count);
#endif
									}
								}
								else
								{
									UpdateCellFont(toCell);
								}
#endregion // Copy Merged info

								#region Cell Styles
								if (((flag & PartialGridCopyFlag.CellStyle) == PartialGridCopyFlag.CellStyle)
									&& fromCell.InnerStyle != null)
								{
									if (fromCell.StyleParentKind == StyleParentKind.Own)
									{
										// from cell has own style, need copy the style
										toCell.InnerStyle = new WorksheetRangeStyle(fromCell.InnerStyle);
									}
									else
									{
										// from cell doesn't have own style, copy the reference of style
										toCell.InnerStyle = fromCell.InnerStyle;
									}

									// copy style parent flag
									toCell.StyleParentKind = fromCell.StyleParentKind;

									// TODO: render alignment is not contained in cell's style
									// copy the style may also need copy the render alignment
									// or we need to update the cell format again?
									if (fromCell.InnerStyle.HAlign == ReoGridHorAlign.General)
									{
										toCell.RenderHorAlign = fromCell.RenderHorAlign;
									}
								}
								#endregion // Cell Styles

								if (toCell.IsEndMergedCell)
								{
									Cell cell = GetCell(toCell.MergeStartPos);
									Debug.Assert(cell != null);

									UpdateCellBounds(cell);
								}
								else if (toCell.Rowspan == 1 && toCell.Colspan == 1)
								{
									UpdateCellFont(toCell);
								}
							}
							else
							{
								cells[tr, tc] = null;
							}
						}
					}
				}
			}

			// h-borders
			if ((flag & PartialGridCopyFlag.HBorder) == PartialGridCopyFlag.HBorder)
			{
				if (data.HBorders == null)
				{
					// cut left side border
					if (toRange.Col > 0)
					{
						for (int r = toRange.Row; r <= toRange.EndRow; r++)
						{
							this.CutBeforeHBorder(r, toRange.Col);
						}
					}

					// set borders to null
					this.hBorders.Iterate(toRange.Row, toRange.Col, rows, cols, true, (r, c, fromHBorder) =>
						{
							this.hBorders[r, c] = null;
							return 1;
						}
					);
				}
				else
				{
					// TODO: need to improve performance
					for (int r = 0; r < rows + 1; r++)
					{
						for (int c = 0; c < cols; c++)
						{
							int tr = toRange.Row + r;
							int tc = toRange.Col + c;

							this.CutBeforeHBorder(tr, tc);

							var fromHBorder = data.HBorders[r, c];

							if (fromHBorder == null)
							{
								hBorders[tr, tc] = null;
							}
							else
							{
								RangeBorderStyle style = fromHBorder.Style;

								int hcols = fromHBorder.Span;
								if (hcols > cols - c) hcols = cols - c;

								this.GetHBorder(tr, tc).Span = hcols;

								if (data.HBorders[r, c].Style != null)
								{
									// in last col
									//if (c == cols - 1)
									//	SetHBorders(tr, tc, hcols, style, fromHBorder.Pos);
									//else
									//	hBorders[tr, tc].Border = style;

									SetHBorders(tr, tc, hcols, style, fromHBorder.Pos);
								}
								else
								{
									hBorders[tr, tc].Style = RangeBorderStyle.Empty;
								}
							}
						}
					}
				}
			}

			// v-borders
			if ((flag & PartialGridCopyFlag.VBorder) == PartialGridCopyFlag.VBorder)
			{
				if (data.VBorders == null)
				{
					// cut top side border
					if (toRange.Row > 0)
					{
						for (int c = toRange.Col; c <= toRange.EndCol; c++)
						{
							CutBeforeVBorder(toRange.Row, c);
						}
					}

					// set border to null
					this.vBorders.Iterate(toRange.Row, toRange.Col, rows, cols, true, (r, c, fromVBorder) =>
						{
							this.vBorders[r, c] = null;
							return 1;
						}
					);
				}
				else
				{
					// TODO: need to improve performance
					for (int r = 0; r < rows; r++)
					{
						for (int c = 0; c < cols + 1; c++)
						{
							int tr = toRange.Row + r;
							int tc = toRange.Col + c;

							this.CutBeforeVBorder(tr, tc);

							var fromVBorder = data.VBorders[r, c];

							if (fromVBorder == null)
							{
								vBorders[tr, tc] = null;
							}
							else
							{
								RangeBorderStyle style = fromVBorder.Style;

								int vrows = fromVBorder.Span;
								if (vrows > rows - r) vrows = rows - r;
								GetVBorder(tr, tc).Span = vrows;

								if (data.VBorders[r, c].Style != null)
								{
									// is last row
									//if (r == rows - 1)
									//	SetVBorders(tr, tc, vrows, style, fromVBorder.Pos);
									//else
									//	vBorders[tr, tc].Border = fromVBorder.Border;
									this.SetVBorders(tr, tc, vrows, style, fromVBorder.Pos);
								}
								else
								{
									this.vBorders[tr, tc].Style = RangeBorderStyle.Empty;
								}
							}
						}
					}
				}
			}

			return new RangePosition(toRange.Row, toRange.Col, rows, cols);
		}

		public RangePosition SetPartialGridRepeatly(string addressOrName, PartialGrid grid)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				return this.SetPartialGridRepeatly(new RangePosition(addressOrName), grid);
			}
			else if (this.TryGetNamedRange(addressOrName, out namedRange))
			{
				return this.SetPartialGridRepeatly(namedRange.Position, grid);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Repeat to copy from a separated grid to fit specified range
		/// </summary>
		/// <param name="grid">Partial grid to be copied</param>
		/// <param name="range">Range to be copied</param>
		/// <returns></returns>
		public RangePosition SetPartialGridRepeatly(RangePosition range, PartialGrid grid)
		{
			if (grid.Rows <= 0 || grid.Columns <= 0) return RangePosition.Empty;

			for (int r = range.Row; r <= range.EndRow; r += grid.Rows)
			{
				for (int c = range.Col; c <= range.EndCol; c += grid.Columns)
				{
					SetPartialGrid(new RangePosition(r, c, grid.Rows, grid.Columns), grid);
				}
			}

			return range;
		}
#endregion // Partial Grid
	}
}
