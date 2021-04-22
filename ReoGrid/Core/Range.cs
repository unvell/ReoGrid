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
using System.Linq;
using System.Data;

#if EX_SCRIPT
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Views;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Range Collection

		private ReoGridRangeCollection rangeCollection;

		/// <summary>
		/// Virtual collection range of spreadsheet
		/// </summary>
		public ReoGridRangeCollection Ranges
		{
			get
			{
				if (this.rangeCollection == null)
				{
					this.rangeCollection = new ReoGridRangeCollection(this);
				}

				return this.rangeCollection;
			}
		}

		/// <summary>
		/// Virtual collection range of spreadsheet
		/// </summary>
		public class ReoGridRangeCollection
		{
			private Worksheet worksheet;

			internal ReoGridRangeCollection(Worksheet grid)
			{
				this.worksheet = grid;
			}

			/// <summary>
			/// Retrieve logical range by specified address
			/// </summary>
			/// <param name="address">address to locate a range</param>
			/// <returns>range position information</returns>
			public ReferenceRange this[string address]
			{
				get { return new ReferenceRange(this.worksheet, address); }
			}

			/// <summary>
			/// Retrieve logical range by specified address
			/// </summary>
			/// <param name="row">zero-based number of row of range</param>
			/// <param name="col">zero-based number of column of range</param>
			/// <param name="rows">number of rows contained in range</param>
			/// <param name="cols">number of columns contained in range</param>
			/// <returns>range position information</returns>
			public ReferenceRange this[int row, int col, int rows, int cols]
			{
				get { return new ReferenceRange(this.worksheet, this.worksheet.FixRange(new RangePosition(row, col, rows, cols))); }
			}

			/// <summary>
			/// Retrieve logical range by specified start cell position and end cell position
			/// </summary>
			/// <param name="startPos">start cell position of range</param>
			/// <param name="endPos">end cell position of range</param>
			/// <returns>range position information</returns>
			public ReferenceRange this[CellPosition startPos, CellPosition endPos]
			{
				get
				{
					return new ReferenceRange(this.worksheet,
						this.worksheet.CreateAndGetCell(startPos), this.worksheet.CreateAndGetCell(endPos));
				}
			}

			/// <summary>
			/// Retrieve logical range by specified range position
			/// </summary>
			/// <param name="range">The range position to be get</param>
			/// <returns>Instance of referenced range to worksheet</returns>
			public ReferenceRange this[RangePosition range]
			{
				get
				{
					if (range.IsEmpty)
					{
						throw new ArgumentException("range position is empty", nameof(range));
					} 

					return new ReferenceRange(this.worksheet, this.worksheet.FixRange(range));
				}
			}
		}

		#endregion // Range Collection

		#region Range Data

		/// <summary>
		/// Get data array from specified range
		/// </summary>
		/// <param name="range">Range to get data array</param>
		/// <returns>Data array of range</returns>
		public object[,] GetRangeData(RangePosition range)
		{
			RangePosition fixedRange = FixRange(range);
			object[,] data = new object[fixedRange.Rows, fixedRange.Cols];
			for (int r = fixedRange.Row, r2 = 0; r <= fixedRange.EndRow; r++, r2++)
			{
				for (int c = fixedRange.Col, c2 = 0; c <= fixedRange.EndCol; c++, c2++)
				{
					data[r2, c2] = (cells[r, c] == null) ? null : cells[r, c].InnerData;
				}
			}
			return data;
		}

		/// <summary>
		/// Remove all cell's data from specified range.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate range on worksheet.</param>
		public void DeleteRangeData(string addressOrName)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.DeleteRangeData(new RangePosition(addressOrName), true);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var refRange))
			{
				this.DeleteRangeData(refRange, true);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Remove all cell's data from specified range.
		/// </summary>
		/// <param name="range">Range to remove data array.</param>
		public void DeleteRangeData(RangePosition range)
		{
			this.DeleteRangeData(range, false);
		}

		/// <summary>
		/// Remove all cell's data from specified range.
		/// </summary>
		/// <param name="range">Range to remove data array</param>
		/// <param name="checkReadonly">if this flag is set to true, the value from 
		/// readonly cells will not be deleted</param>
		public void DeleteRangeData(RangePosition range, bool checkReadonly = false)
		{
			var fixedRange = FixRange(range);

			int maxcol = range.Col;

#if FORMULA
			List<Cell> formulaDirtyCells = new List<Cell>();
#endif // FORMULA

			IterateCells(fixedRange, (row, col, cell) =>
			{
				if (maxcol < col) maxcol = col;

				if (!checkReadonly || !cell.IsReadOnly)
				{
					cell.InnerData = null;
					cell.InnerDisplay = string.Empty;

					cell.TextBounds = new Rectangle();
					cell.RenderColor = SolidColor.Transparent;
					cell.RenderScaleFactor = this.renderScaleFactor;

					cell.InnerFormula = null;
#if FORMULA
					cell.FormulaTree = null;
					this.ClearCellReferenceList(cell);

					if (formulaDirtyCells.Contains(cell))
					{
						formulaDirtyCells.Remove(cell);
					}

#endif // FORMULA

					cell.FontDirty = false;
#if WINFORM
					cell.RenderFont = null;
#endif // WINFORM
				}

				// TODO: auto adjust row height

#if FORMULA
				foreach (var referecedRange in this.formulaRanges)
				{
					if (referecedRange.Value.Any(r => r.Contains(cell.InternalPos))
						&& !formulaDirtyCells.Contains(referecedRange.Key))
					{
						formulaDirtyCells.Add(referecedRange.Key);
					}
				}
#endif // FORMULA

				return true;
			});

#if FORMULA
			foreach (var dirtyCell in formulaDirtyCells)
			{
				RecalcCell(dirtyCell);
			}
#endif // FORMULA

			// only update the changed columns (up to maxcol)
			for (int i = fixedRange.Col; i <= maxcol; i++)
			{
				var header = this.cols[i];
				if (header.Body != null) header.Body.OnDataChange(fixedRange.Row, fixedRange.EndRow);
			}

			this.RaiseRangeDataChangedEvent(fixedRange);

			this.RequestInvalidate();
		}

		/// <summary>
		/// Set cell's data to fill specified range.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate range on worksheet.</param>
		/// <param name="data">Data to be set.</param>
		public void SetRangeData(string addressOrName, object data)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.SetRangeData(new RangePosition(addressOrName), data);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var refRange))
			{
				this.SetRangeData(refRange, data);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Set cell's data to fill specified range.
		/// </summary>
		/// <param name="row">Zero-based number of row.</param>
		/// <param name="col">Zero-based number of column.</param>
		/// <param name="rows">Number of rows in the range.</param>
		/// <param name="cols">Number of columns in the range.</param>
		/// <param name="data">Data to be set.</param>
		public void SetRangeData(int row, int col, int rows, int cols, object data)
		{
			SetRangeData(new RangePosition(row, col, rows, cols), data);
		}

		/// <summary>
		/// Set cell's data to fill specified range.
		/// </summary>
		/// <param name="range">Range position on worksheet to set the data.</param>
		/// <param name="data">Data to be set.</param>
		public void SetRangeData(RangePosition range, object data)
		{
			SetRangeData(range, data, false);
		}

		/// <summary>
		/// Set data into specified range.
		/// </summary>
		/// <param name="range">Range to set data.</param>
		/// <param name="data">Data to be set into the range.</param>
		/// <param name="checkReadonly">Ignore to update read-only cells if this flag is true; 
		/// Set this flag as fasle to update all cells in specified range. (Default is true)</param>
		public void SetRangeData(RangePosition range, object data, bool checkReadonly)
		{
			if (range.IsEmpty) return;

			range = FixRange(range);

			if (data is Array)
			{
				var arr = (Array)data;

				if (arr.Rank == 2)
				{
					int dataRows = arr.GetLength(0);
					int dataCols = arr.GetLength(1);

					int maxRows = Math.Max(dataRows, range.Rows);
					int maxCols = Math.Max(dataCols, range.Cols);

					for (int r = 0; r < maxRows && r < dataRows; r++)
					{
						for (int cc = 0, index = 0; cc < maxCols && index < dataCols; index++)
						{
							int targetRow = range.Row + r;
							int targetCol = range.Col + cc;

							if (targetRow < this.rows.Count && targetCol < this.cols.Count)
							{
								var cell = this.CreateAndGetCell(targetRow, targetCol);

								if (!checkReadonly || !cell.IsReadOnly)
								{
									SetSingleCellData(cell, arr.GetValue(r, index));
								}

								cc += Math.Max((short)1, cell.Colspan);
							}
						}
					}
				}
				else if (arr.Rank == 1)
				{
					bool nestedList = arr.Length > 0 && (arr.GetValue(0) is IEnumerable<object>);

					if (nestedList)
					{
						for (int r = range.Row, i = 0; r <= range.EndRow && i < arr.Length; r++, i++)
						{
							int c = range.Col;

							var elements = arr.GetValue(i) as IEnumerable<object>;

							if (elements != null)
							{
								foreach (var ele in elements)
								{
									SetCellData(r, c, ele);
									c++;
									if (c > range.EndCol || c >= this.cols.Count) break;
								}
							}
						}
					}
					else
					{
						int r = range.Row, c = range.Col;

						for (int i = 0; i < arr.Length; i++)
						{
							var rowdata = arr.GetValue(i);
							
							SetCellData(r, c, rowdata);

							c++;
							if (c > range.EndCol)
							{
								r++;
								if (r > range.EndRow)
								{
									break;
								}
								c = range.Col;
							}
						}
					}
				}
				else
					throw new ArgumentException("Array with more than 2 ranks is not supported.");
			}
			//else if (data is object[])
			//{
			//	var arr = (object[])data;

			//	if (arr[0] is IEnumerable<object>)
			//	{
			//		for (int r = range.Row, i = 0; r <= range.EndRow && i < arr.Length; r++, i++)
			//		{
			//			var item = arr[i] as IEnumerable<object>;

			//			if (item != null)
			//			{
			//				int c = range.Col;
			//				foreach (var ele in item)
			//				{
			//					SetCellData(r, c, ele);
			//					if (c++ > range.EndCol) break;
			//				}
			//			}
			//		}
			//	}
			//	else
			//	{

			//	}
			//}
			else if (data is IEnumerable<object>)
			{
				var arr = (IEnumerable<object>)data;

				int r = range.Row, c = range.Col;

				foreach (var element in arr)
				{
					SetCellData(r, c, element);

					if (++r > range.EndRow)
					{
						r = range.Row;
						if (++c > range.EndCol) break;
					}
				}
			}
			else if (data is DataTable)
			{
				SetRangeData(range, (DataTable)data);
			}
			else if (data is PartialGrid)
			{
				SetPartialGrid(range, (PartialGrid)data);
			}
			else
			{
				// only set one cell if value is single object
				SetCellData(range.StartPos, data);
			}
		}

		/// <summary>
		/// Set range data copied from data table.
		/// </summary>
		/// <param name="range">Range to fill the data read from data table</param>
		/// <param name="table">Read data from this data table</param>
		public void SetRangeData(RangePosition range, System.Data.DataTable table)
		{
			for (int r = range.Row, dr = 0; r <= range.EndRow && dr < table.Rows.Count; r++, dr++)
			{
				for (int c = range.Col, dc = 0; c <= range.EndCol && dc < table.Columns.Count; c++, dc++)
				{
					SetCellData(r, c, table.Rows[dr][dc]);
				}
			}
		}

		#endregion // Range Data

		#region Event
		internal void RaiseRangeDataChangedEvent(RangePosition range)
		{
			if (!this.suspendDataChangedEvent)
			{
				if (RangeDataChanged != null)
				{
					RangeDataChanged(this, new RangeEventArgs(range));
				}

#if EX_SCRIPT
				if (this.Srm != null && this.worksheetObj != null)
				{
					var ondatachange = Srm.DefaultContext.EvaluatePropertyGet(this.worksheetObj, "ondatachange");

					if (ondatachange != null)
					{
						Srm.InvokeFunctionIfExisted(this.worksheetObj, "ondatachange", new RSRangeObject(this, range));
					}
				}
#endif
			}
		}

		/// <summary>
		/// Event raised when entire data from a range is changed
		/// </summary>
		public event EventHandler<RangeEventArgs> RangeDataChanged;

		#endregion // Event

		#region Utilities

		/// <summary>
		/// Check the boundary of cell position and return a safe position.
		/// </summary>
		/// <param name="pos">The cell position to check.</param>
		/// <returns>Safe cell position.</returns>
		public CellPosition FixPos(CellPosition pos)
		{
			if (pos.Row < 0) pos.Row = 0;
			if (pos.Col < 0) pos.Col = 0;
			if (pos.Row > this.rows.Count - 1) pos.Row = this.rows.Count - 1;
			if (pos.Col > this.cols.Count - 1) pos.Col = this.cols.Count - 1;
			return pos;
		}

		/// <summary>
		/// Check the boundary of range position and return normalized range from this worksheet.
		/// </summary>
		/// <param name="range">The range position to be checked.</param>
		/// <returns>Normalized range position from this worksheet.</returns>
		public RangePosition FixRange(RangePosition range)
		{
			range.StartPos = this.FixPos(range.StartPos);
			if (range.Rows == -1) range.EndRow = this.rows.Count;
			if (range.Cols == -1) range.EndCol = this.cols.Count;
			if (range.EndRow > this.rows.Count - 1 || range.Rows == -1) range.EndRow = this.rows.Count - 1;
			if (range.EndCol > this.cols.Count - 1 || range.Cols == -1) range.EndCol = this.cols.Count - 1;
			return range;
		}

		/// <summary>
		/// Check whether or not the specified range contains any read-only cells.
		/// </summary>
		/// <remarks>
		/// This method ingores worksheet Readonly setting. If worksheet contains Readonly setting, 
		/// cells including cells data from this worksheet should not be changed. 
		/// To check the worksheet Readonly setting, use:
		/// <code>
		/// bool hasReadonly = worksheet.HasSettings(WorksheetSettings.Edit_Readonly);
		/// </code>
		/// </remarks>
		/// <param name="range">Range position to be checked.</param>
		/// <returns>True if the range contains read-only cells; Otherwise return false.</returns>
		public bool CheckRangeReadonly(RangePosition range)
		{
			bool foundReadonlyCell = false;

			this.IterateCells(range, (r, c, cell) =>
			{
				if (cell.IsReadOnly)
				{
					foundReadonlyCell = true;
					return false;
				}
				else return true;
			});

			return (foundReadonlyCell);
		}

		/// <summary>
		/// Scroll view to make the specified cell visible.
		/// </summary>
		/// <param name="address">address to locate the specified cell</param>
		public void ScrollToCell(string address)
		{
			this.ScrollToCell(new CellPosition(address));
		}

		/// <summary>
		/// Scroll view to make the specified cell visible.
		/// </summary>
		/// <param name="pos">index position to locate specified cell</param>
		public void ScrollToCell(CellPosition pos)
		{
			this.ScrollToRange(new RangePosition(pos), pos);
		}

		/// <summary>
		/// Scroll view to make the specified cell visible.
		/// </summary>
		/// <param name="cell">cell instance to be displayed</param>
		public void ScrollToCell(Cell cell)
		{
			if (cell.Worksheet != this)
			{
				throw new ReferenceObjectNotAssociatedException("Specified cell does not belong to this worksheet.");
			}

			this.ScrollToCell(cell.InternalPos);
		}

		/// <summary>
		/// Scroll to specified cell position
		/// </summary>
		/// <param name="row">Number of row</param>
		/// <param name="col">Number of column</param>
		public void ScrollToCell(int row, int col)
		{
			this.ScrollToRange(this.FixRange(new RangePosition(row, col, 1, 1)));
		}

		/// <summary>
		/// Scroll view to make the specified range visible.
		/// </summary>
		/// <param name="addressOrName">address or name to locate a range</param>
		public void ScrollToRange(string addressOrName)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.ScrollToRange(new RangePosition(addressOrName));
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var refRange))
			{
				this.ScrollToRange(refRange);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Scroll view to make the specified cell visible.
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of column</param>
		/// <param name="rows">number of rows in the range</param>
		/// <param name="cols">number of columns in the range</param>
		public void ScrollToRange(int row, int col, int rows, int cols)
		{
			this.ScrollToRange(new RangePosition(row, col, rows, cols));
		}

		/// <summary>
		/// Scroll view to make the specified cell visible.
		/// </summary>
		/// <param name="range">range index to locate a range on spreadsheet</param>
		public void ScrollToRange(RangePosition range)
		{
			this.ScrollToRange(range, range.EndPos);
		}

		/// <summary>
		/// Scroll to specified range based on a position to the range
		/// </summary>
		/// <param name="range">Range to be displayed</param>
		/// <param name="basePos">Base point to scroll views</param>
		public void ScrollToRange(RangePosition range, CellPosition basePos)
		{
			if (this.viewportController is IScrollableViewportController svc)
			{
				svc.ScrollToRange(this.FixRange(range), basePos);
			}
		}

		#region Drag and drop Range

		/// <summary>
		/// Move cells from specified range to another range.
		/// </summary>
		/// <param name="fromRangeAddress"></param>
		/// <param name="toRangeAddress"></param>
		public void MoveRange(string fromRangeAddress, string toRangeAddress)
		{
			RangePosition fromRange, toRange;

			if (RangePosition.IsValidAddress(fromRangeAddress))
			{
				fromRange = new RangePosition(fromRangeAddress);
			}
			else if (this.TryGetNamedRangePosition(fromRangeAddress, out fromRange))
			{
				// nothing to do
			}
			else
				throw new InvalidAddressException(fromRangeAddress);

			if (RangePosition.IsValidAddress(toRangeAddress))
			{
				toRange = new RangePosition(toRangeAddress);
			}
			else if (this.TryGetNamedRangePosition(toRangeAddress, out toRange))
			{
				// nothing to do
			}
			else
				throw new InvalidAddressException(toRangeAddress);

			this.MoveRange(fromRange, toRange);
		}

		/// <summary>
		/// Move cells from specified range to another range.
		/// </summary>
		/// <param name="fromRange">Cells moved from this range.</param>
		/// <param name="toRange">Cells moved to this range.</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied to that contains a part of another merged cell.</exception>
		public void MoveRange(RangePosition fromRange, RangePosition toRange)
		{
			MoveRange(fromRange, toRange, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Move cells from specified range to another range.
		/// </summary>
		/// <param name="fromRange">Cells moved from this range.</param>
		/// <param name="toRange">Cells moved to this range.</param>
		/// <param name="flags">Select the types of content to be moved or copied.</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied to that contains a part of another merged cell.</exception>
		public void MoveRange(RangePosition fromRange, RangePosition toRange, PartialGridCopyFlag flags = PartialGridCopyFlag.All)
		{
			CopyRange(fromRange, toRange, flags, true);

			//if (fromRange.Rows == this.rows.Count
			//	&& this.HasSettings(WorksheetSettings.Behavior_DragToMoveColumnHeader))
			//{

			//}
		}

		/// <summary>
		/// Copy content from specified range to another range.
		/// </summary>
		/// <param name="fromRangeAddress">Address to locate the range to read data.</param>
		/// <param name="toRangeAddress">Address to put copied data.</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied contains a part of another merged cell.</exception>
		public void CopyRange(string fromRangeAddress, string toRangeAddress)
		{
			RangePosition fromRange, toRange;

			if (RangePosition.IsValidAddress(fromRangeAddress))
			{
				fromRange = new RangePosition(fromRangeAddress);
			}
			else if (!this.TryGetNamedRangePosition(fromRangeAddress, out fromRange))
			{
				throw new InvalidAddressException(fromRangeAddress);
			}

			if (RangePosition.IsValidAddress(toRangeAddress))
			{
				toRange = new RangePosition(toRangeAddress);
			}
			else if (!this.TryGetNamedRangePosition(toRangeAddress, out toRange))
			{
				throw new InvalidAddressException(toRangeAddress);
			}

			this.CopyRange(fromRange, toRange);
		}

		/// <summary>
		/// Copy content from specified range to another range.
		/// </summary>
		/// <remarks>
		/// The range copied from cannot be a part of another range, <para>fromRange</para> contains a part of 
		/// another range will cause RangeIntersectionException.
		/// </remarks>
		/// <param name="fromRange">The range to be copied from.</param>
		/// <param name="toRange">Target range that is used to put copied content.</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied contains a part of another merged cell.</exception>
		/// <exception cref="RangeContainsReadonlyCellsException">Thrown when specified target range contains any read-only cell that cannot be overwritten.</exception>
		public void CopyRange(RangePosition fromRange, RangePosition toRange)
		{
			CopyRange(fromRange, toRange, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Copy content from specified range to another range.
		/// </summary>
		/// <remarks>
		/// The range copied from cannot be a part of another range, <para>fromRange</para> contains a part of 
		/// another range will cause RangeIntersectionException.
		/// </remarks>
		/// <param name="fromRange">The range to be copied from.</param>
		/// <param name="toRange">Target position that is used to put copied content.</param>
		/// <param name="flags">Content flags that specifiy the types of content to be copied. (Value of this flag can be data, borders, styles and etc.)</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied to that contains a part of another merged cell.</exception>
		/// <exception cref="RangeContainsReadonlyCellsException">Thrown when specified target range contains any read-only cell that cannot be overwritten.</exception>
		public void CopyRange(RangePosition fromRange, RangePosition toRange, PartialGridCopyFlag flags = PartialGridCopyFlag.All)
		{
			CopyRange(fromRange, toRange, flags, false);
		}

		/// <summary>
		/// Copy content from specified range to another range.
		/// </summary>
		/// <remarks>
		/// The range copied from cannot be a part of another range, <para>fromRange</para> contains a part of 
		/// another range will cause <code>RangeIntersectionException</code>.
		/// Operation will be cancelled if <para>fromRange</para> is same as <para>toRange</para>
		/// </remarks>
		/// <param name="fromRange">Range to be copied from.</param>
		/// <param name="toRange">Target position that is used to put copied content.</param>
		/// <param name="flags">Content flags that specifiy the types of content to be copied. (Value of this flag can be data, borders, styles and etc.)</param>
		/// <param name="moveRange">Determines that whether or not to remove content from original range. Set true to move range; set false to copy range.</param>
		/// <exception cref="CellDataReadonlyException">Throw when current worksheet is read-only.</exception>
		/// <exception cref="RangeIntersectionException">Range to be moved or copied contains a part of another merged cell.</exception>
		/// <exception cref="RangeContainsReadonlyCellsException">Thrown when specified target range contains any read-only cell that cannot be overwritten.</exception>
		internal void CopyRange(RangePosition fromRange, RangePosition toRange, PartialGridCopyFlag flags = PartialGridCopyFlag.All,
			bool moveRange = false)
		{
			if (this.HasSettings(WorksheetSettings.Edit_Readonly))
			{
				throw new CellDataReadonlyException(fromRange.StartPos);
			}

			if (fromRange == toRange) return;

			// TODO: enable two ranges intersecting
			//if (fromRange.IntersectWith(toRange))
			//{
			//	throw new RangeIntersectionException(toRange);
			//}

			RangePosition intersectedRange = this.CheckIntersectedMergingRange(fromRange);

			if (intersectedRange != RangePosition.Empty)
			{
				throw new RangeIntersectionException(fromRange);
			}

			intersectedRange = this.CheckIntersectedMergingRange(toRange);

			if (intersectedRange != RangePosition.Empty)
			{
				throw new RangeIntersectionException(toRange);
			}

			if (CheckRangeReadonly(fromRange))
			{
				throw new RangeContainsReadonlyCellsException(fromRange);
			}

			if (CheckRangeReadonly(toRange))
			{
				throw new RangeContainsReadonlyCellsException(fromRange);
			}

			var bcmrearg = new BeforeCopyOrMoveRangeEventArgs(fromRange, toRange);

			if (moveRange)
			{
				this.BeforeRangeMove?.Invoke(this, bcmrearg);
			}
			else
			{
				this.BeforeRangeCopy?.Invoke(this, bcmrearg);
			}

			if (bcmrearg.IsCancelled) return;

			var pgrid = GetPartialGrid(fromRange);

			// move range
			if (moveRange)
			{
				UnmergeRange(fromRange);
				ClearRangeContent(fromRange, CellElementFlag.All);
			}

			SetPartialGrid(toRange, pgrid);

			if (moveRange)
			{
				AfterRangeMove?.Invoke(this, new CopyOrMoveRangeEventArgs(fromRange, toRange));
			}
			else
			{
				AfterRangeCopy?.Invoke(this, new CopyOrMoveRangeEventArgs(fromRange, toRange));
			}
		}

		/// <summary>
		/// Event raised before move a range.
		/// </summary>
		public event EventHandler<BeforeCopyOrMoveRangeEventArgs> BeforeRangeMove;

		/// <summary>
		/// Event raised before copy a range.
		/// </summary>
		public event EventHandler<BeforeCopyOrMoveRangeEventArgs> BeforeRangeCopy;

		/// <summary>
		/// Event raised after move a range.
		/// </summary>
		public event EventHandler<CopyOrMoveRangeEventArgs> AfterRangeMove;

		/// <summary>
		/// Event rasied after copy a range.
		/// </summary>
		public event EventHandler<CopyOrMoveRangeEventArgs> AfterRangeCopy;

		#endregion // Drag and drop Range

		/// <summary>
		/// Clear content inside specified range.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate the range.</param>
		/// <param name="flags">Elements in cell specified by this flag to be removed.</param>
		public void ClearRangeContent(string addressOrName, CellElementFlag flags)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				ClearRangeContent(new RangePosition(addressOrName), flags);
			}
			else if (this.TryGetNamedRange(addressOrName, out namedRange))
			{
				ClearRangeContent(namedRange.Position, flags);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Clear content inside specified range.
		/// </summary>
		/// <param name="range">The range to be clear.</param>
		/// <param name="flags">Elements in cell specified by this flag to be removed.</param>
		/// <param name="checkReadonly">True to ignore read-only cells; False to delete content from read-only cells.</param>
		public void ClearRangeContent(RangePosition range, CellElementFlag flags, bool checkReadonly = true)
		{
			var fixedRange = FixRange(range);

			bool deleteData = (flags & CellElementFlag.Data) == CellElementFlag.Data;
			bool deleteFormula = (flags & CellElementFlag.Formula) == CellElementFlag.Formula;
			bool deleteDataFormat = (flags & CellElementFlag.DataFormat) == CellElementFlag.DataFormat;
			bool deleteBody = (flags & CellElementFlag.Body) == CellElementFlag.Body;
			bool deleteStyle = (flags & CellElementFlag.Style) == CellElementFlag.Style;

			int maxcol = range.Col;

#if FORMULA
			List<Cell> formulaDirtyCells = new List<Cell>();
#endif // FORMULA

			for (int r = fixedRange.Row; r <= fixedRange.EndRow; r++)
			{
				for (int c = fixedRange.Col; c <= fixedRange.EndCol; c++)
				{
					var cell = this.cells[r, c];

					if (cell != null)
					{
						bool dirty = false;

						if ((!checkReadonly || !cell.IsReadOnly))
						{
							if (deleteData)
							{
								cell.InnerData = null;
								cell.InnerDisplay = string.Empty;

								if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
								{
									cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
								}

								dirty = true;
							}

							if (deleteFormula)
							{
								cell.InnerFormula = null;
#if FORMULA
								ClearCellReferenceList(cell);
#endif // FORMULA
							}

							if (deleteDataFormat)
							{
								cell.DataFormat = DataFormat.CellDataFormatFlag.General;
								cell.DataFormatArgs = null;

								dirty = true;
							}

							if (deleteBody)
							{
								cell.body = null;

								dirty = true;
							}

							if (dirty)
							{
								if (maxcol < c) maxcol = c;

								//cell.RenderTextBounds = RGRectF.Empty;
								//cell.RenderColor = null;
								//cell.RenderScaleFactor = this.scaleFactor;

								// TODO: auto adjust row height

#if FORMULA
								foreach (var referecedRange in this.formulaRanges)
								{
									if (referecedRange.Value.Any(rr => rr.Contains(cell.InternalPos))
										&& !formulaDirtyCells.Contains(referecedRange.Key))
									{
										formulaDirtyCells.Add(referecedRange.Key);
									}
								}
#endif // FORMULA

								this.RaiseCellDataChangedEvent(cell);
							}
						}
					}
				}
			}

			if (deleteStyle)
			{
				this.RemoveRangeStyles(fixedRange, PlainStyleFlag.All);
			}

#if FORMULA
			foreach (var dirtyCell in formulaDirtyCells)
			{
				RecalcCell(dirtyCell);
			}
#endif // FORMULA

			// only update the changed columns (up to maxcol)
			for (int i = fixedRange.Col; i <= maxcol; i++)
			{
				var header = this.cols[i];
				if (header.Body != null) header.Body.OnDataChange(fixedRange.Row, fixedRange.EndRow);
			}

			//var fromRange = FixRange(range);

			//if (flags == CellElementFlag.All)
			//{
			//	for (int r = fromRange.Row; r <= fromRange.EndRow; r++)
			//	{
			//		for (int c = fromRange.Col; c <= fromRange.EndCol; c++)
			//		{
			//			this.cells[r, c] = null;
			//		}
			//	}

			//	InvalidateCanvas();
			//}
			//else
			//{
			//	if ((flags & CellElementFlag.Data) == CellElementFlag.Data)
			//	{
			//		DeleteRangeData(fromRange, true);
			//	}

			//	if ((flags & CellElementFlag.Body) == CellElementFlag.Body)
			//	{
			//		this.IterateCells(fromRange, (r, c, cell) =>
			//		{
			//			cell.body = null;
			//			return true;
			//		});

			//		InvalidateCanvas();
			//	}

			//	if ((flags & CellElementFlag.DataFormat) == CellElementFlag.DataFormat)
			//	{
			//		this.DeleteRangeDataFormat(fromRange);
			//	}

			//	if ((flags & CellElementFlag.Style) == CellElementFlag.Style)
			//	{
			//		RemoveRangeStyle(fromRange, PlainStyleFlag.All);
			//	}
			//}

			if ((flags & CellElementFlag.Border) == CellElementFlag.Border)
			{
				this.RemoveRangeBorders(fixedRange, BorderPositions.All);
			}

			RequestInvalidate();
		}

		#endregion Utility
	}
}
