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
using System.Linq;
using unvell.ReoGrid.Actions;

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

using unvell.ReoGrid.Data;
using unvell.ReoGrid.Interaction;
using System.Collections.Generic;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Filter

		///// <summary>
		///// Create filter on specified column.
		///// </summary>
		///// <param name="column">Column code that locates a column to create filter.</param>
		///// <param name="titleRows">Indicates how many title rows exist at the top of worksheet,
		///// title rows will not be included in filter range.</param>
		///// <returns>Instance of column filter.</returns>
		//public AutoColumnFilter CreateColumnFilter(string column, int titleRows = 0,
		//	AutoColumnFilterUI columnFilterUI = AutoColumnFilterUI.DropdownButtonAndPane)
		//{
		//	return CreateColumnFilter(column, column, titleRows, columnFilterUI);
		//}

		/// <summary>
		/// Create column filter.
		/// </summary>
		/// <param name="startColumn">First column specified by an address to create filter.</param>
		/// <param name="endColumn">Last column specified by an address to the filter.</param>
		/// <param name="titleRows">Indicates that how many title rows exist at the top of spreadsheet,
		/// title rows will not be included in filter apply range.</param>
		/// <param name="columnFilterUI">Indicates whether allow to create graphics user interface (GUI), 
		/// by default the dropdown-button on the column and candidates dropdown-panel will be created.
		/// Set this argument as NoGUI to create filter without GUI.</param>
		/// <returns>Instance of column filter.</returns>
		public AutoColumnFilter CreateColumnFilter(string startColumn, string endColumn, int titleRows = 0,
			AutoColumnFilterUI columnFilterUI = AutoColumnFilterUI.DropdownButtonAndPanel)
		{
			int startIndex = RGUtility.GetNumberOfChar(startColumn);
			int endIndex = RGUtility.GetNumberOfChar(endColumn);

			return CreateColumnFilter(startIndex, endIndex, titleRows, columnFilterUI);
		}

		/// <summary>
		/// Create column filter.
		/// </summary>
		/// <param name="column">Column to create filter.</param>
		/// <param name="titleRows">indicates that how many title rows exist at the top of spreadsheet,
		/// title rows will not be included in filter apply range.</param>
		/// <param name="columnFilterUI">Indicates whether allow to create graphics user interface (GUI), 
		/// by default the dropdown-button on the column and candidates dropdown-panel will be created.
		/// Set this argument as NoGUI to create filter without GUI.</param>
		/// <returns>Instance of column filter.</returns>
		public AutoColumnFilter CreateColumnFilter(int column, int titleRows, AutoColumnFilterUI columnFilterUI)
		{
			return this.CreateColumnFilter(column, column, titleRows, columnFilterUI);
		}

		/// <summary>
		/// Create column filter.
		/// </summary>
		/// <param name="startColumn">first column specified by a zero-based number of column to create filter</param>
		/// <param name="endColumn">last column specified by a zero-based number of column to create filter</param>
		/// <param name="titleRows">indicates that how many title rows exist at the top of spreadsheet,
		/// title rows will not be included in filter apply range.</param>
		/// <param name="columnFilterUI">Indicates whether or not to show GUI for filter, 
		/// by default the drop-down button displayed on column header and a candidates list popuped up when dropdown-panel opened.
		/// Set this argument as NoGUI to create filter without GUI.</param>
		/// <returns>Instance of column filter.</returns>
		public AutoColumnFilter CreateColumnFilter(int startColumn, int endColumn, int titleRows = 0,
			AutoColumnFilterUI columnFilterUI = AutoColumnFilterUI.DropdownButtonAndPanel)
		{
			if (startColumn < 0 || startColumn >= this.ColumnCount)
			{
				throw new ArgumentOutOfRangeException("startColumn", "number of column start to filter out of valid spreadsheet range");
			}

			if (endColumn < startColumn)
			{
				throw new ArgumentOutOfRangeException("endColumn", "end column must be greater than start column");
			}

			if (endColumn >= this.ColumnCount)
			{
				throw new ArgumentOutOfRangeException("endColumn", "end column out of valid spreadsheet range");
			}

			return CreateColumnFilter(new RangePosition(titleRows, startColumn,
				this.MaxContentRow - titleRows + 1, endColumn - startColumn + 1), columnFilterUI);
		}

		/// <summary>
		/// Create automatic column filter and display on specified headers of worksheet.
		/// </summary>
		/// <param name="range">Range to filter data.</param>
		/// <param name="columnFilterUI">Indicates whether or not to show GUI for filter, 
		/// by default the drop-down button displayed on column header and a candidates list popuped up when dropdown-panel opened.
		/// Set this argument as NoGUI to create filter without GUI.</param>
		/// <returns>Instance of column filter.</returns>
		public AutoColumnFilter CreateColumnFilter(RangePosition range,
			AutoColumnFilterUI columnFilterUI = AutoColumnFilterUI.DropdownButtonAndPanel)
		{
			var filter = new AutoColumnFilter(this, this.FixRange(range));

			filter.Attach(this, columnFilterUI);

			return filter;
		}

		/// <summary>
		/// Do a filter on specified rows. Determines whether or not to show or hide a row.
		/// </summary>
		/// <param name="startRow">Number of row start to check.</param>
		/// <param name="rows">Number of rows to be checked.</param>
		/// <param name="filter">A callback filter function to check specified row from worksheet.</param>
		public void DoFilter(RangePosition range, Func<int, bool> filter)
		{
			try
			{
				this.controlAdapter.ChangeCursor(CursorStyle.Busy);

				this.SetRowsHeight(range.Row, range.Rows, r =>
				{
					bool showRow = filter(r);

					if (showRow)
					{
						var rowhead = this.RetrieveRowHeader(r);

						// don't hide row, show the row
						// if row is hidden, return lastHeight to show the row
						return rowhead.InnerHeight == 0 ? rowhead.LastHeight : rowhead.InnerHeight;
					}
					else
					{
						return 0;
					}
				}, true);
			}
			finally
			{
				if (this.controlAdapter != null)
				{
					this.ControlAdapter.ChangeCursor(Interaction.CursorStyle.PlatformDefault);
				}
			}

			this.RowsFiltered?.Invoke(this, null);
		}

		/// <summary>
		/// Event raised when rows filtered on this worksheet.
		/// </summary>
		public event EventHandler RowsFiltered;

		#endregion // Filter

		#region Sort

		/// <summary>
		/// Sort data on specified column.
		/// </summary>
		/// <param name="columnAddress">Base column specified by an address to sort data.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer">Custom cell data comparer, compares two cells and returns an integer. 
		/// Set this value to null to use default built-in comparer.</param>
		/// <returns>Data changed range</returns>
		public RangePosition SortColumn(string columnAddress, SortOrder order = SortOrder.Ascending,
			CellElementFlag moveElementFlag = CellElementFlag.Data,
			Func<object, object, int> cellDataComparer = null)
		{
			return SortColumn(RGUtility.GetNumberOfChar(columnAddress), order, moveElementFlag, cellDataComparer);
		}

		/// <summary>
		/// Sort data on specified column.
		/// </summary>
		/// <param name="columnIndex">Zero-based number of column to sort data.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer">custom cell data comparer, compares two cells and returns an integer. 
		/// Set this value to null to use default built-in comparer.</param>
		/// <returns>Data changed range</returns>
		public RangePosition SortColumn(int columnIndex, SortOrder order = SortOrder.Ascending,
			CellElementFlag moveElementFlag = CellElementFlag.Data,
			Func<object, object, int> cellDataComparer = null)
		{
			return SortColumn(columnIndex, 0, MaxContentRow, 0, MaxContentCol, order, moveElementFlag, cellDataComparer);
		}

		/// <summary>
		/// Sort data on specified column.
		/// </summary>
		/// <param name="columnIndex">Zero-based number of column to sort data</param>
		/// <param name="titleRows">Indicates that how many title rows exist at the top of worksheet, 
		/// Title rows will not be included in sort apply range.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer">Custom cell data comparer, compares two cells and returns an integer.  
		/// Set this value to null to use default built-in comparer.</param>
		/// <returns>Data changed range.</returns>
		public RangePosition SortColumn(int columnIndex, int titleRows, SortOrder order = SortOrder.Ascending,
				CellElementFlag moveElementFlag = CellElementFlag.Data,
		Func<object, object, int> cellDataComparer = null)
		{
			return SortColumn(columnIndex, titleRows, MaxContentRow, 0, MaxContentCol, order, moveElementFlag, cellDataComparer);
		}

		/// <summary>
		/// Sort data on specified column.
		/// </summary>
		/// <param name="columnIndex">Zero-based number of column to sort data.</param>
		/// <param name="startRow">First number of row to allow sort data.</param>
		/// <param name="endRow">Last number of number of row to allow sort data.</param>
		/// <param name="startColumn">First number of column to allow sort data.</param>
		/// <param name="endColumn">Last number of column to allow sort data.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer">Custom cell data comparer, compares two cells and returns an integer. 
		/// Set this value to null to use default built-in comparer.</param>
		/// <returns>Data changed range.</returns>
		public RangePosition SortColumn(int columnIndex, int startRow, int endRow, int startColumn, int endColumn,
			SortOrder order = SortOrder.Ascending,
			CellElementFlag moveElementFlag = CellElementFlag.Data,
			Func<object, object, int> cellDataComparer = null)
		{
			return SortColumn(columnIndex, new RangePosition(startRow, startColumn, MaxContentRow - startRow + 1,
				endColumn - startColumn + 1), order, cellDataComparer);
		}

		/// <summary>
		/// Sort data inside specified range.
		/// </summary>
		/// <param name="columnIndex">Data will be sorted based on this column.</param>
		/// <param name="applyRange">Affect range.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer"></param>
		/// <returns></returns>
		public RangePosition SortColumn(int columnIndex, string applyRange, SortOrder order = SortOrder.Ascending,
			CellElementFlag moveElementFlag = CellElementFlag.Data,
			Func<object, object, int> cellDataComparer = null)
		{
			if (RangePosition.IsValidAddress(applyRange))
			{
				return this.SortColumn(columnIndex, new RangePosition(applyRange), order, cellDataComparer);
			}
			else if (this.TryGetNamedRangePosition(applyRange, out var range))
			{
				return this.SortColumn(columnIndex, range, order, cellDataComparer);
			}
			else
				throw new InvalidAddressException(applyRange);
		}

		/// <summary>
		/// Sort data on specified column.
		/// </summary>
		/// <param name="columnIndex">Zero-based number of column to sort data.</param>
		/// <param name="applyRange">Data only be changed in this range during sort.</param>
		/// <param name="order">Order of data sort.</param>
		/// <param name="cellDataComparer">Custom cell data comparer, compares two cells and returns an integer. 
		/// Set this value to null to use default built-in comparer.</param>
		/// <returns>Data changed range.</returns>
		public RangePosition SortColumn(int columnIndex, RangePosition applyRange,
			SortOrder order = SortOrder.Ascending,
			Func<object, object, int> cellDataComparer = null)
		{

			var range = FixRange(applyRange);

			RangePosition affectRange = RangePosition.Empty;

			if (cellDataComparer != null)
			{
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif // DEBUG

			// stop fire events
			this.SuspendDataChangedEvents();

			try
			{
				this.controlAdapter.ChangeCursor(CursorStyle.Busy);

				if (!this.CheckQuickSortRange(range.Row, range.EndRow, range.Col, range.EndCol))
				{
					throw new InvalidOperationException("Cannot change a part of range, all cells should be having same colspan on column.");
				}

				IComparer<object> comparer = cellDataComparer == null ? (IComparer<object>)new CellComparer(order) : new CellComparerAdapter(cellDataComparer, order);

				var data = this.GetSortedData(columnIndex, range.Row, range.EndRow, range.Col, range.EndCol, ref affectRange, comparer, order);

				DoAction(new SetSortedRangeDataAction(range, data));

#if DEBUG
				sw.Stop();

				if (sw.ElapsedMilliseconds > 10)
				{
					Debug.WriteLine("sort column by {0} on [{1}-{2}]: {3} ms", columnIndex, range.Col, range.EndCol, sw.ElapsedMilliseconds);
				}
#endif // DEBUG
			}
			finally
			{
				// resume to fire events
				this.ResumeDataChangedEvents();
			}

			this.RequestInvalidate();

			this.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);

			if (!affectRange.IsEmpty)
			{
				for (int c = affectRange.Col; c <= affectRange.EndCol; c++)
				{
					var header = this.cols[c];

					if (header.Body != null)
					{
						header.Body.OnDataChange(affectRange.Row, affectRange.EndRow);
					}
				}

				this.RaiseRangeDataChangedEvent(affectRange);

				this.RowsSorted?.Invoke(this, new Events.RangeEventArgs(affectRange));
			}

			return affectRange;

		}

		private bool CheckQuickSortRange(int row, int endRow, int col, int endCol)
		{
			for (int c = col; c <= endCol; c++)
			{
				var cell1 = this.cells[row, c];

				for (int r = row + 1; r <= endRow; r++)
				{
					var cell2 = this.cells[r, c];

					if (cell1 != null && cell2 != null
						&& ((cell1.IsValidCell && !cell2.IsValidCell)
						|| (!cell1.IsValidCell && cell2.IsValidCell)))
					{
						return false;
					}
				}
			}

			return true;
		}

		private object[,] GetSortedData(int columnIndex, int startRow, int endRow, int startColumn,
			int endColumn,
			ref RangePosition affectRange, IComparer<object> cellComparer, SortOrder sortOrder)
		{

			if (affectRange.IsEmpty)
			{
				affectRange.Col = startColumn;
				affectRange.EndCol = endColumn;
			}

			int affectedRangeStartRow = int.MaxValue;
			int affectedRangeEndRow = int.MinValue;

			var sortedData = new object[endRow - startRow + 1, endColumn - startColumn + 1];

			var rangeData = Enumerable.Range(startRow, endRow - startRow + 1)
									 .Select(x => new
									 {
										 Cells = Enumerable.Range(startColumn, endColumn - startColumn + 1)
											 .Select(y => cells[x, y])
											 .ToArray()
									 });

			var columnRangeIndex = columnIndex - startColumn;

			rangeData = sortOrder == SortOrder.Ascending
				? rangeData.OrderBy(x => x.Cells[columnRangeIndex]?.InnerData, cellComparer)
				: rangeData.OrderByDescending(x => x.Cells[columnRangeIndex]?.InnerData, cellComparer);

			var ordered = rangeData.ToArray();

			for (int newRowIndex = 0; newRowIndex < ordered.Length; newRowIndex++)
			{
				var orderedData = ordered[newRowIndex];

				for (int col = 0; col < endColumn - startColumn + 1; col++)
				{
					var newCellData = orderedData.Cells[col]?.InnerData;

					if (newCellData == null && cells[newRowIndex + startRow, col + startColumn]?.InnerData == null)
					{
						continue;
					}

					sortedData[newRowIndex, col] = newCellData;

					if (affectedRangeStartRow > newRowIndex + startRow) affectedRangeStartRow = newRowIndex + startRow;
					if (affectedRangeEndRow < newRowIndex + startRow) affectedRangeEndRow = newRowIndex + startRow;
				}
			}

			affectRange.Row = affectedRangeStartRow == int.MaxValue ? 0 : affectedRangeStartRow;
			affectRange.EndRow = affectedRangeEndRow == int.MinValue ? 0 : affectedRangeEndRow;

			bool canTrimData = !new RangePosition(startRow, startColumn, endRow - startRow + 1, endColumn - startColumn + 1).Equals(affectRange);

			return canTrimData
				? (object[,])ResizeArray(sortedData, new[] { affectRange.Rows, affectRange.Cols })
				: sortedData;
		}

		private static Array ResizeArray(Array arr, int[] newSizes)
		{
			if (newSizes.Length != arr.Rank)
				throw new ArgumentException("Array must have the same number of dimensions as there are elements in newSizes", nameof(newSizes));

			var newArray = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
			int length = arr.Length <= newArray.Length ? arr.Length : newArray.Length;
			Array.ConstrainedCopy(arr, 0, newArray, 0, length);
			return newArray;
		}

		/// <summary>
		/// Event raised when rows sorted on this worksheet.
		/// </summary>
		public event EventHandler<Events.RangeEventArgs> RowsSorted;
		#endregion // Sort

	}

	/// <summary>
	/// Sort order.
	/// </summary>
	public enum SortOrder
	{
		/// <summary>
		/// Ascending
		/// </summary>
		Ascending,

		/// <summary>
		/// Descending
		/// </summary>
		Descending,
	}

	internal class CellComparer : IComparer<object>
	{
		private readonly int sign;

		public CellComparer(SortOrder order)
		{
			// this needs to leave empty cells always at the bottom of list
			sign = order == SortOrder.Ascending ? 1 : -1;
		}

		public int Compare(object x, object y)
		{
			var data = x as IComparable;

			if (data is string str && string.IsNullOrEmpty(str))
			{
				data = null;
			}

			if (y is string str1 && string.IsNullOrEmpty(str1))
			{
				y = null;
			}

			if (data == null && y == null) return 0;

			if (data != null && y == null) return -sign;

			if (data == null) return sign;

			if (data.GetType() == y.GetType())
			{
				return data.CompareTo(y);
			}

			if (y is string)
			{
				return Convert.ToString(data).CompareTo(y);
			}

			if (data is string)
			{
				return data.CompareTo(Convert.ToString(y));
			}

			try
			{
				return ((double)Convert.ChangeType(data, typeof(double))).CompareTo(Convert.ChangeType(y, typeof(double)));
			}
			catch
			{
				return Convert.ToString(data).CompareTo(Convert.ToString(y));
			}
		}
	}

	internal class CellComparerAdapter : IComparer<object>
	{
		private readonly Func<object, object, int> comparerFunc;
		private readonly int sign;

		public CellComparerAdapter(Func<object, object, int> comparerFunc, SortOrder order)
		{
			this.comparerFunc = comparerFunc ?? throw new ArgumentNullException(nameof(comparerFunc));
			// this needs to leave empty cells always at the bottom of list
			sign = order == SortOrder.Ascending ? 1 : -1;
		}

		public int Compare(object x, object y)
		{
			if (x is string str && string.IsNullOrEmpty(str))
			{
				x = null;
			}

			if (y is string str1 && string.IsNullOrEmpty(str1))
			{
				y = null;
			}

			if (x == null && y == null) return 0;

			if (x != null && y == null) return -sign;

			if (x == null) return sign;

			return comparerFunc(x, y);
		}
	}
}
