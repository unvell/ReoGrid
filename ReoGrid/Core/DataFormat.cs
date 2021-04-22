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

using System.Collections.Generic;
using System.Linq;

using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Set
		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate the cell and range on spreadsheet to be set.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		/// <exception cref="InvalidAddressException">Throw if specified address or name is illegal.</exception>
		public void SetRangeDataFormat(string addressOrName, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				SetRangeDataFormat(new RangePosition(addressOrName), format, dataFormatArgs);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var namedRange))
			{
				SetRangeDataFormat(namedRange, format, dataFormatArgs);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="row">Number of row to locate the range.</param>
		/// <param name="col">Number of column to locate the range.</param>
		/// <param name="rows">Number of rows contained in the range.</param>
		/// <param name="cols">Number of columns contained in the range.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		public void SetRangeDataFormat(int row, int col, int rows, int cols, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			SetRangeDataFormat(new RangePosition(row, col, rows, cols), format, dataFormatArgs);
		}

		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="range">Range to be set.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		public void SetRangeDataFormat(RangePosition range, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			RangePosition fixedRange = FixRange(range);

			int rend = fixedRange.EndRow;
			int cend = fixedRange.EndCol;

#if FORMULA
			List<Cell> formulaDirtyCells = new List<Cell>(10);
#else
			List<Cell> formulaDirtyCells = null;
#endif

			for (int r = fixedRange.Row; r <= rend; r++)
			{
				for (int c = fixedRange.Col; c <= cend;)
				{
					Cell cell = CreateAndGetCell(r, c);

					SetCellDataFormat(cell, format, ref dataFormatArgs, formulaDirtyCells);

					c += cell.Colspan > 1 ? cell.Colspan : 1;
				}
			}

#if FORMULA
			foreach (var cell in formulaDirtyCells)
			{
				RecalcCell(cell);
			}
#endif

			RequestInvalidate();
		}

		internal void SetCellDataFormat(CellPosition pos, CellDataFormatFlag format, ref object dataFormatArgs)
		{
			SetCellDataFormat(CreateAndGetCell(pos), format, ref dataFormatArgs);
		}

		internal void SetCellDataFormat(Cell cell, CellDataFormatFlag format,
			ref object dataFormatArgs, List<Cell> formulaDirtyCells = null)
		{
			cell.DataFormat = format;
			cell.DataFormatArgs = dataFormatArgs;

			//string oldDisplay = cell.Display;

			DataFormatterManager.Instance.FormatCell(cell);

			unvell.ReoGrid.Utility.StyleUtility.UpdateCellRenderAlign(this, cell);
			UpdateCellTextBounds(cell);

#if FORMULA
			if (formulaDirtyCells != null)
			{
				// reference ranges (including cells)
				foreach (var referencedRange in formulaRanges)
				{
					if (referencedRange.Value.Any(rr => rr.Contains(cell.InternalPos)))
					{
						if (!formulaDirtyCells.Contains(referencedRange.Key))
						{
							formulaDirtyCells.Add(referencedRange.Key);
						}
					}
				}
			}
#endif
		}
		#endregion // Set

		#region Get
		public CellDataFormatFlag GetCellDataFormat(string addressOrName, out object dataFormatArgs)
		{
			if (CellPosition.IsValidAddress(addressOrName))
			{
				return this.GetCellDataFormat(new CellPosition(addressOrName), out dataFormatArgs);
			}
			else if (this.TryGetNamedRangePosition(addressOrName, out var namedRange))
			{
				return this.GetCellDataFormat(namedRange.StartPos, out dataFormatArgs);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		public CellDataFormatFlag GetCellDataFormat(CellPosition pos, out object dataFormatArgs)
		{
			pos = this.FixPos(pos);

			var cell = this.cells[pos.Row, pos.Col];

			if (cell == null)
			{
				dataFormatArgs = null;
				return CellDataFormatFlag.General;
			}

			return GetCellDataFormat(cell, out dataFormatArgs);
		}

		internal CellDataFormatFlag GetCellDataFormat(Cell cell, out object dataFormatArgs)
		{
			dataFormatArgs = cell.DataFormatArgs;
			return cell.DataFormat;
		}
		#endregion // Get

		#region Delete
		/// <summary>
		/// Delete data format settings from specified range
		/// </summary>
		/// <param name="range">Range to be remove formats</param>
		public void DeleteRangeDataFormat(RangePosition range)
		{
			var fixedRange = this.FixRange(range);

			for (int r = fixedRange.Row; r <= fixedRange.EndRow; r++)
			{
				for (int c = fixedRange.Col; c <= fixedRange.EndCol;)
				{
					Cell cell = this.cells[r, c];

					if (cell == null)
					{
						c++;
					}
					else
					{
						// clear data format flags
						cell.DataFormat = CellDataFormatFlag.General;
						cell.DataFormatArgs = null;

						if (cell.IsValidCell)
						{
							// reformat cell
							DataFormatterManager.Instance.FormatCell(cell);

							// update cell render alignemnt (Number aligned to right might be restored to left)
							unvell.ReoGrid.Utility.StyleUtility.UpdateCellRenderAlign(this, cell);

							// update text bounds
							UpdateCellTextBounds(cell);

							c += cell.Colspan > 1 ? cell.Colspan : 1;
						}
						else
						{
							c++;
						}
					}
				}
			}
		}
		#endregion // Delete
	}
}
