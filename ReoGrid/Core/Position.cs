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
using System.Text;
using System.Text.RegularExpressions;
using unvell.ReoGrid.Core;

namespace unvell.ReoGrid.Core
{
	/// <summary>
	/// Absolute address interface reference to a cell or range on spreadsheet
	/// </summary>
	internal interface ISheetAddress
	{
		/// <summary>
		/// Convert into address identifier.
		/// </summary>
		/// <returns>Address identifier in string.</returns>
		string ToAddress();

		/// <summary>
		/// Convert into relative address identifier.
		/// </summary>
		/// <returns>Address identifier in string.</returns>
		string ToRelativeAddress();

		/// <summary>
		/// Convert into absolute address identifier.
		/// </summary>
		/// <returns>Address identifier in string.</returns>
		string ToAbsoluteAddress();
	}

	/// <summary>
	/// Interface that represents a range of row on worksheet.
	/// </summary>
	internal interface IRowRange
	{
		/// <summary>
		/// Get or set the number of row.
		/// </summary>
		int Row { get; set; }

		/// <summary>
		/// Get or set the number of rows in this range.
		/// </summary>
		int Rows { get; set; }

		/// <summary>
		/// Get the number of last row in this range
		/// </summary>
		int EndRow { get; }
	}

	/// <summary>
	/// Interface to represents a range of column on worksheet.
	/// </summary>
	internal interface IColumnRange
	{
		/// <summary>
		/// Get or set the number of col.
		/// </summary>
		int Col { get; set; }

		/// <summary>
		/// Get or set the number of rows in this range.
		/// </summary>
		int Cols { get; set; }

		/// <summary>
		/// Get the number of last column in this range.
		/// </summary>
		int EndCol { get; }
	}

	/// <summary>
	/// Interface that represents a range location on worksheet. (Combine from IRowRange and IColumnRange)
	/// </summary>
	internal interface IRange : IRowRange, IColumnRange { }

	static class PositionAbsoluteBits
	{
		internal const byte StartRow = 1;
		internal const byte StartCol = 2;
		internal const byte EndRow = 4;
		internal const byte EndCol = 8;
	}
}

namespace unvell.ReoGrid
{
	#region Cell Position

	/// <summary>
	/// Represents a cell position on worksheet.
	/// </summary>
	[Serializable]
	public struct CellPosition : ISheetAddress
	{
		#region Attributes
		private int row;

		/// <summary>
		/// Zero-based number of row to locate the cell on worksheet.
		/// </summary>
		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		private int col;

		/// <summary>
		/// Zero-based number of column to locate the cell on worksheet.
		/// </summary>
		public int Col
		{
			get { return col; }
			set { col = value; }
		}

		private byte positionProperties;

		/// <summary>
		/// Determines the property (Absolute or Relative) for row position.
		/// </summary>
		public PositionProperty RowProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.StartRow) == PositionAbsoluteBits.StartRow ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.StartRow;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.StartRow);
				}
			}
		}

		/// <summary>
		/// Determines the property (Absolute or Relative) for column position.
		/// </summary>
		public PositionProperty ColumnProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.StartCol) == PositionAbsoluteBits.StartCol ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.StartCol;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.StartCol);
				}
			}
		}
		#endregion // Attributes

		#region Constructors
		/// <summary>
		/// Create position with specified number of row and number of column.
		/// </summary>
		/// <param name="row">Zero-based number of row.</param>
		/// <param name="col">Zero-based number of column.</param>
		public CellPosition(int row, int col)
		{
			this.row = row;
			this.col = col;
			this.positionProperties = 0;
		}

		internal CellPosition(int row, int col, byte positionProperties)
		{
			this.row = row;
			this.col = col;
			this.positionProperties = positionProperties;
		}

		/// <summary>
		/// Create instance with alphabet code of position. (e.g. new CellPosition("A10"))
		/// </summary>
		/// <param name="address">an address string to locate the cell in spreadsheet. (e.g. 'A10')</param>
		/// <exception cref="ArgumentException">if address is not in correct format.</exception>
		/// <example>var pos = new CellPosition("A10");</example>
		public CellPosition(string address)
		{
			Match m = RGUtility.CellReferenceRegex.Match(address);

			if (!m.Success)
			{
				throw new ArgumentException("invalid address for cell: " + address, "id");
			}

			this.row = 0;
			int.TryParse(m.Groups["row"].Value, out row);
			row--;
			this.col = RGUtility.GetNumberOfChar(m.Groups["col"].Value);

			this.positionProperties = 0;

			if (m.Groups["abs_row"].Success)
			{
				this.positionProperties |= PositionAbsoluteBits.StartRow;
			}

			if (m.Groups["abs_col"].Success)
			{
				this.positionProperties |= PositionAbsoluteBits.StartCol;
			}
		}
		#endregion // Constructors

		#region ToString, ToAddress, ToAbsoluteAddress, IsValidAddress
		/// <summary>
		/// Convert position to address string.
		/// </summary>
		/// <returns>String of this position.</returns>
		public override string ToString()
		{
			return this.ToAddress();
		}

		/// <summary>
		/// Convert position into relative address string. (format: A1)
		/// </summary>
		/// <returns>Related address in string.</returns>
		/// <seealso cref="ToAbsoluteAddress"/>
		public string ToAddress()
		{
			if (this.positionProperties == (PositionAbsoluteBits.StartRow | PositionAbsoluteBits.StartCol))
			{
				return "$" + RGUtility.GetAlphaChar(col) + "$" + (row + 1);
			}
			else if (this.positionProperties == PositionAbsoluteBits.StartCol)
			{
				return "$" + RGUtility.GetAlphaChar(col) + (row + 1);
			}
			else if (this.positionProperties == PositionAbsoluteBits.StartRow)
			{
				return RGUtility.GetAlphaChar(col) + "$" + (row + 1);
			}
			else
				return RGUtility.GetAlphaChar(col) + (row + 1);
		}

		public string ToRelativeAddress()
		{
			return RGUtility.GetAlphaChar(col) + (row + 1);
		}

		/// <summary>
		/// Convert position into absolute address string. (format: $A$1)
		/// </summary>
		/// <returns>Absolute address in string</returns>
		/// <seealso cref="ToAddress"/>
		public string ToAbsoluteAddress()
		{
			return string.Format("${0}${1}", RGUtility.GetAlphaChar(this.col), this.row + 1);
		}

		/// <summary>
		/// Check whether or not the specified string is valid address descriptor.
		/// </summary>
		/// <param name="address">address descriptor as string to be checked.</param>
		/// <returns>true if specified address is valid, otherwise returning false.</returns>
		public static bool IsValidAddress(string address)
		{
			return RGUtility.CellReferenceRegex.IsMatch(address);
		}
		#endregion // ToString, ToAddress, ToAbsoluteAddress, IsValidAddress

		#region IsEmpty, Equals, ==, !=
		internal static readonly CellPosition Empty = new CellPosition(-1, -1);

		internal bool IsEmpty
		{
			get { return row == Empty.row && col == Empty.col; }
		}

		/// <summary>
		/// Compare this position to another object
		/// </summary>
		/// <param name="obj">object to be compared</param>
		/// <returns>true if this position is same as the specified object, otherwise false</returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is CellPosition)) return false;
			CellPosition p1 = this;
			CellPosition p2 = (CellPosition)obj;
			return p1.row == p2.row && p1.col == p2.col
				&& p1.positionProperties == p2.positionProperties;
		}

		/// <summary>
		/// Compare this position to specified number of row and number of column
		/// </summary>
		/// <param name="row">number of row to be compared</param>
		/// <param name="col">number of column to be compared</param>
		/// <returns>true if position is same to specified number of row and number of column</returns>
		public bool Equals(int row, int col)
		{
			return this.row == row && this.col == col;
		}

		/// <summary>
		/// Compare position to specified address descriptor 
		/// </summary>
		/// <param name="address">address descriptor to be compared</param>
		/// <returns>true if position is same as the specified address descriptor, otherwise false if not</returns>
		public bool Equals(string address)
		{
			return !string.IsNullOrEmpty(address) && IsValidAddress(address) && Equals(new CellPosition(address));
		}

		/// <summary>
		/// Compare this position to another position, return true if two positions are same.
		/// </summary>
		/// <param name="r1">this position to be compared</param>
		/// <param name="r2">another position to be compared</param>
		/// <returns>true if two positions are same, otherwise false if not</returns>
		public static bool operator ==(CellPosition r1, CellPosition r2)
		{
			return r1.Equals(r2);
		}

		/// <summary>
		/// Compare this position to another position, return true if two positions are different.
		/// </summary>
		/// <param name="r1">this position to be compared</param>
		/// <param name="r2">another position to be compared</param>
		/// <returns>true if two positions are different, otherwise false if not</returns>
		public static bool operator !=(CellPosition r1, CellPosition r2)
		{
			return !r1.Equals(r2);
		}

		/// <summary>
		/// Compare two position.
		/// </summary>
		/// <param name="pos1">first position to be compared.</param>
		/// <param name="pos2">second position to be compared.</param>
		/// <returns>true if two positions are same, otherwise false if two positions are different.</returns>
		public static bool Equals(CellPosition pos1, CellPosition pos2)
		{
			return Equals(pos1, pos2.row, pos2.col);
		}

		/// <summary>
		/// Compare a position to specified number of row and number of column.
		/// </summary>
		/// <param name="pos">position to be compared.</param>
		/// <param name="row">number of row of another position to be compared.</param>
		/// <param name="col">number of column of another position to be compared.</param>
		/// <returns>true if the position is same as specified number of row and number of column.</returns>
		public static bool Equals(CellPosition pos, int row, int col)
		{
			return pos.row == row && pos.col == col;
		}

		/// <summary>
		/// Get hash code of this position.
		/// </summary>
		/// <returns>generated hash code</returns>
		public override int GetHashCode()
		{
			return row ^ col ^ positionProperties;
		}
		#endregion // IsEmpty, Equals, ==, !=

		#region Offset, +, -
		/// <summary>
		/// Offset by specified another position.
		/// </summary>
		/// <param name="pos">Position indicates that how many rows and columns to offset.</param>
		/// <returns>Position after offset.</returns>
		public CellPosition Offset(CellPosition pos)
		{
			return Offset(pos.row, pos.col);
		}

		/// <summary>
		/// Offset by specified rows and columns.
		/// </summary>
		/// <param name="rows">Number of rows to offset.</param>
		/// <param name="cols">Number of columns to offset.</param>
		/// <returns>Position after offset.</returns>
		public CellPosition Offset(int rows, int cols)
		{
			this.row += rows;
			this.col += cols;
			return this;
		}

		/// <summary>
		/// Offset pos1 by number of rows and columns that is specified in pos2. (Same as method Offset)
		/// </summary>
		/// <param name="pos1">Position to be offset.</param>
		/// <param name="pos2">Position indicates that how many rows and columns to offset.</param>
		/// <returns>Position after offset.</returns>
		public static CellPosition operator +(CellPosition pos1, CellPosition pos2)
		{
			return pos1.Offset(pos2.row, pos2.col);
		}

		/// <summary>
		/// Offset pos1 by number of rows and columns that is specified in pos2. (Same as method Offset)
		/// </summary>
		/// <param name="pos1">Position to be offset.</param>
		/// <param name="pos2">Position indicates that how many rows and columns to offset.</param>
		/// <returns>Position after offset.</returns>
		public static CellPosition operator -(CellPosition pos1, CellPosition pos2)
		{
			return pos1.Offset(-pos2.row, -pos2.col);
		}
		#endregion // Offset, +, -

		public static CellPosition Zero = new CellPosition(0, 0);
	}

	#endregion // Cell Position

	#region Range Position

	/// <summary>
	/// Define a range on spreadsheet
	/// </summary>
	[Serializable]
	public struct RangePosition : IRange, ISheetAddress
	{
		#region Attributes
		private int row;

		/// <summary>
		/// The start row of range
		/// </summary>
		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		private int col;

		/// <summary>
		/// The start column of range
		/// </summary>
		public int Col
		{
			get { return col; }
			set { col = value; }
		}

		private int rows;

		/// <summary>
		/// Rows of range. (minimum is 1)
		/// </summary>
		public int Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		private int cols;

		/// <summary>
		/// Columns of range. (minimum is 1)
		/// </summary>
		public int Cols
		{
			get { return cols; }
			set { cols = value; }
		}

		/// <summary>
		/// Then end row of range
		/// </summary>
		public int EndRow
		{
			get { return row + rows - 1; }
			set { rows = value - row + 1; }
		}

		/// <summary>
		/// The end column of range
		/// </summary>
		public int EndCol
		{
			get { return col + cols - 1; }
			set { cols = value - col + 1; }
		}

		/// <summary>
		/// The start location of range.
		/// </summary>
		public CellPosition StartPos
		{
			get
			{
				return new CellPosition(row, col,
					(byte)(this.positionProperties &
					(PositionAbsoluteBits.StartRow | PositionAbsoluteBits.StartCol)));
			}
			set
			{
				row = value.Row;
				col = value.Col;
				this.StartRowProperty = value.RowProperty;
				this.StartColumnProperty = value.ColumnProperty;
			}
		}

		/// <summary>
		/// The end location of range.
		/// </summary>
		public CellPosition EndPos
		{
			get
			{
				return new CellPosition(this.rows == EntireRange.rows ? 1048576 : EndRow, this.cols == EntireRange.cols ? 32768 : EndCol,
					(byte)((this.positionProperties & (PositionAbsoluteBits.EndRow | PositionAbsoluteBits.EndCol)) >> 2));
			}
			set
			{
				EndRow = value.Row;
				EndCol = value.Col;
				this.EndRowProperty = value.RowProperty;
				this.EndColumnProperty = value.ColumnProperty;
			}
		}

		#region Properties

		private byte positionProperties;

		/// <summary>
		/// Get or set the property of start row.
		/// </summary>
		public PositionProperty StartRowProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.StartRow) == PositionAbsoluteBits.StartRow ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.StartRow;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.StartRow);
				}
			}
		}

		/// <summary>
		/// Get or set the property of start column.
		/// </summary>
		public PositionProperty StartColumnProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.StartCol) == PositionAbsoluteBits.StartCol ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.StartCol;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.StartCol);
				}
			}
		}

		/// <summary>
		/// Get or set the property of end row.
		/// </summary>
		public PositionProperty EndRowProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.EndRow) == PositionAbsoluteBits.EndRow ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.EndRow;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.EndRow);
				}
			}
		}

		/// <summary>
		/// Get or set the property of end column.
		/// </summary>
		public PositionProperty EndColumnProperty
		{
			get
			{
				return (this.positionProperties & PositionAbsoluteBits.EndCol) == PositionAbsoluteBits.EndCol ?
					PositionProperty.Absolute : PositionProperty.Relative;
			}
			set
			{
				if (value == PositionProperty.Absolute)
				{
					this.positionProperties |= PositionAbsoluteBits.EndCol;
				}
				else
				{
					this.positionProperties = (byte)(this.positionProperties & ~PositionAbsoluteBits.EndCol);
				}
			}
		}

		#endregion // Properties

		#endregion // Attributes

		#region Constructors
		/// <summary>
		/// Create range position by specify the top left and bottom right cell position.
		/// </summary>
		/// <param name="startPos">Top left cell position to create range position.</param>
		/// <param name="endPos">Bottom right cell position to create range position.</param>
		/// <exception cref="ArgumentException">Throw when specified address is invalid.</exception>
		public RangePosition(CellPosition startPos, CellPosition endPos)
		{
			this.row = Math.Min(startPos.Row, endPos.Row);
			this.col = Math.Min(startPos.Col, endPos.Col);
			this.rows = Math.Max(startPos.Row, endPos.Row) - this.row + 1;
			this.cols = Math.Max(startPos.Col, endPos.Col) - this.col + 1;
			this.positionProperties = 0;
		}

		/// <summary>
		/// Create range position which contains single cell position.
		/// </summary>
		/// <param name="singlePos">Cell position used to create range position.</param>
		public RangePosition(CellPosition singlePos)
		{
			this.row = singlePos.Row;
			this.col = singlePos.Col;
			this.rows = 1;
			this.cols = 1;
			this.positionProperties = 0;
		}

		/// <summary>
		/// Create range position by specified start position and size.
		/// </summary>
		/// <param name="row">Zero-based number of row to start create range position.</param>
		/// <param name="col">Zero-based number of column to start create range position.</param>
		/// <param name="rows">Number of rows inside the range to be created. (At least one row required)</param>
		/// <param name="cols">Number of columns inside the range to be created. (At least one column required)</param>
		public RangePosition(int row, int col, int rows, int cols)
		{
			this.row = row;
			this.col = col;
			this.rows = rows;
			this.cols = cols;
			this.positionProperties = 0;
		}

		/// <summary>
		/// Create range position by specified two cell addresses. (Top left and bottom right) 
		/// </summary>
		/// <param name="startCell">Address at top left of range.</param>
		/// <param name="endCell">Address at bottom right of range.</param>
		public RangePosition(string startCell, string endCell)
			: this(new CellPosition(startCell), new CellPosition(endCell))
		{
		}

		/// <summary>
		/// Create range position from a range or cell address.
		/// </summary>
		/// <param name="address">Address to locate the range position.</param>
		/// <exception cref="ArgumentException">Throw when specified address is invalid.</exception>
		public RangePosition(string address)
		{
			this.positionProperties = 0;

			Match m = RGUtility.RangeReferenceRegex.Match(address);

			if (!m.Success)
			{
				m = RGUtility.SingleAbsoulteRangeRegex.Match(address);
			}

			if (!m.Success && CellPosition.IsValidAddress(address))
			{
				var pos = new CellPosition(address);

				this.row = pos.Row;
				this.col = pos.Col;
				this.rows = 1;
				this.cols = 1;

				this.StartRowProperty = pos.RowProperty;
				this.StartColumnProperty = pos.ColumnProperty;
				this.EndRowProperty = pos.RowProperty;
				this.EndColumnProperty = pos.ColumnProperty;
			}
			else
			{
				if (!m.Success
					//|| (m.Groups["to_col"].Length <= 0 || m.Groups["to_row"].Length <= 0
					//|| m.Groups["from_col"].Length <= 0 || m.Groups["from_row"].Length <= 0)
					)
				{
					throw new ArgumentException("range is invalid: " + address);
				}

				int fromCol = 0, fromRow = 0, toCol = 0, toRow = 0;
				bool fullRows = false, fullCols = false;

				if (m.Groups["from_row"].Success)
				{
					if (!int.TryParse(m.Groups["from_row"].Value, out fromRow))
					{
						throw new ArgumentException("range is invalid: " + address);
					}

					fromRow--;
				}
				else
				{
					fullRows = true;
				}

				if (!fullRows && m.Groups["to_row"].Success)
				{
					if (!int.TryParse(m.Groups["to_row"].Value, out toRow))
					{
						throw new ArgumentException("range is invalid: " + address);
					}
					toRow--;
				}

				if (m.Groups["from_col"].Success)
				{
					fromCol = RGUtility.GetNumberOfChar(m.Groups["from_col"].Value);
				}
				else
				{
					fullCols = true;
				}

				if (!fullCols && m.Groups["to_col"].Success)
				{
					toCol = RGUtility.GetNumberOfChar(m.Groups["to_col"].Value);
				}

				if (fullCols)
				{
					this.row = Math.Min(fromRow, toRow);
					this.rows = Math.Max(fromRow, toRow) - this.row + 1;
					this.col = 0;
					this.cols = -1;
				}
				else if (fullRows)
				{
					this.row = 0;
					this.rows = -1;
					this.col = Math.Min(fromCol, toCol);
					this.cols = Math.Max(fromCol, toCol) - this.col + 1;
				}
				else
				{
					this.row = Math.Min(fromRow, toRow);
					this.col = Math.Min(fromCol, toCol);
					this.rows = Math.Max(fromRow, toRow) - this.row + 1;
					this.cols = Math.Max(fromCol, toCol) - this.col + 1;
				}

				if (m.Groups["abs_from_row"].Success)
				{
					this.positionProperties |= PositionAbsoluteBits.StartRow;
				}

				if (m.Groups["abs_from_col"].Success)
				{
					this.positionProperties |= PositionAbsoluteBits.StartCol;
				}

				if (m.Groups["abs_to_row"].Success)
				{
					this.positionProperties |= PositionAbsoluteBits.EndRow;
				}

				if (m.Groups["abs_to_col"].Success)
				{
					this.positionProperties |= PositionAbsoluteBits.EndCol;
				}
			}

		}
		#endregion // Constructors

		#region Compare
		/// <summary>
		/// Check for whether or not specified object is same as this range.
		/// </summary>
		/// <param name="obj">target range to be checked.</param>
		/// <returns>true if two ranges are same, otherwise return false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is RangePosition)) return false;
			RangePosition r1 = this;
			RangePosition r2 = (RangePosition)obj;
			return r1.row == r2.row && r1.col == r2.col 
				&& r1.rows == r2.rows && r1.cols == r2.cols
				&& r1.positionProperties == r2.positionProperties;
		}

		/// <summary>
		/// Compare whether or not specified two ranges are same.
		/// </summary>
		/// <param name="r1">first range to be compared.</param>
		/// <param name="r2">second range to be compared.</param>
		/// <returns>true if two ranges are same, false if ranges are not same.</returns>
		public static bool operator ==(RangePosition r1, RangePosition r2)
		{
			return r1.Equals(r2);
		}

		/// <summary>
		/// Compare whether or not specified two ranges are same.
		/// </summary>
		/// <param name="r1">first range to be compared.</param>
		/// <param name="r2">second range to be compared.</param>
		/// <returns>true if two ranges are not same, false if ranges are same.</returns>
		public static bool operator !=(RangePosition r1, RangePosition r2)
		{
			return !r1.Equals(r2);
		}

		/// <summary>
		/// Calculate hash code for this range.
		/// </summary>
		/// <returns>hash code calculated for this range.</returns>
		public override int GetHashCode()
		{
			return row ^ col ^ rows ^ cols ^ positionProperties;
		}

		/// <summary>
		/// Check whether the position is contained by this range.
		/// </summary>
		/// <param name="pos">Position to be checked.</param>
		/// <returns>true if the position is contained by this range.</returns>
		public bool Contains(CellPosition pos)
		{
			return Contains(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether the position specified by row and col is contained by this range.
		/// </summary>
		/// <param name="row">row of position.</param>
		/// <param name="col">col of position.</param>
		/// <returns>true if position is contained by this range.</returns>
		public bool Contains(int row, int col)
		{
			return row >= this.row && col >= this.col && row <= EndRow && col <= EndCol;
		}

		/// <summary>
		/// Copmpare another range and check whether or not the range is contained by this range.
		/// </summary>
		/// <param name="range">Another range to be checked.</param>
		/// <returns>True if the specified range is contained by this range; otherwise false.</returns>
		public bool Contains(RangePosition range)
		{
			return row <= range.row && col <= range.col && this.EndRow >= range.EndRow && this.EndCol >= range.EndCol;
		}

		/// <summary>
		/// Check whether the spcified row is contained by this range.
		/// </summary>
		/// <param name="row">zero-based index of row to be checked.</param>
		/// <returns>true if specified row is contained by this range; otherwise false.</returns>
		public bool ContainsRow(int row)
		{
			return row >= this.row && row <= this.EndRow;
		}

		/// <summary>
		/// Check whether the specified column is contained by this range.
		/// </summary>
		/// <param name="col">zero-based index of column to be checked.</param>
		/// <returns>true if specified column is contained by this range; otherwise false.</returns>
		public bool ContainsColumn(int col)
		{
			return col >= this.col && col <= this.EndCol;
		}

		/// <summary>
		/// Check whether or not specified range is intersected with current range.
		/// </summary>
		/// <param name="range">another range to be checked.</param>
		/// <returns>true if specified range is intersected with current range; otherwise false.</returns>
		public bool IntersectWith(RangePosition range)
		{
			int row2 = this.row + this.rows - 1;
			int col2 = this.col + this.cols - 1;

			return !(this.col + this.cols - 1 < range.col
				|| range.col + range.cols - 1 < this.col
				|| this.row + this.rows - 1 < range.row
				|| range.row + range.rows - 1 < this.row);
		}
		#endregion // Compare

		#region Empty and Entire Check
		/// <summary>
		/// Empty range constant define.
		/// </summary>
		public static readonly RangePosition Empty = new RangePosition(0, 0, 0, 0);

		/// <summary>
		/// Entire range constant define.
		/// </summary>
		public static readonly RangePosition EntireRange = new RangePosition(0, 0, -1, -1);

		/// <summary>
		/// Return whether or not current range is empty. (Both Rows and Columns is zero)
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return rows == 0 || cols == 0;
			}
		}

		/// <summary>
		/// Check whether or not this range contains whole rows and columns.
		/// </summary>
		public bool IsEntire
		{
			get
			{
				return this == EntireRange;
			}
		}
		#endregion // Empty and Entire Check

		#region Stringify
		/// <summary>
		/// Convert range to address string.
		/// </summary>
		/// <returns>Address string of range.</returns>
		public override string ToString()
		{
			return this.ToAddress();
		}

		public string ToAddress()
		{
			if (this.rows <= -1 && this.cols <= -1)
			{
				#region full rows and cols
				StringBuilder sb = new StringBuilder();

				if (this.StartRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append("1:");

				if (this.EndRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append("1048576");

				return sb.ToString();
				#endregion // full rows and cols
			}
			else if (this.cols <= -1)
			{
				#region full cols
				StringBuilder sb = new StringBuilder();

				if (this.StartRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(this.row + 1);
				sb.Append(':');

				if (this.EndRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(this.EndRow + 1);

				return sb.ToString();
				#endregion // full cols
			}
			else if (this.rows <= -1)
			{
				#region full rows
				StringBuilder sb = new StringBuilder();

				if (this.StartColumnProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(RGUtility.GetAlphaChar(this.col));
				sb.Append(':');

				if (this.EndColumnProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(RGUtility.GetAlphaChar(this.EndCol));

				return sb.ToString();
				#endregion // full rows
			}
			else
			{
				#region normal address
				StringBuilder sb = new StringBuilder();

				// start
				if (this.StartColumnProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(RGUtility.GetAlphaChar(this.col));

				if (this.StartRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(this.row + 1);

				sb.Append(':');

				// end
				if (this.EndColumnProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(RGUtility.GetAlphaChar(this.EndCol));

				if (this.EndRowProperty == PositionProperty.Absolute)
				{
					sb.Append('$');
				}

				sb.Append(this.EndRow + 1);

				return sb.ToString();
				#endregion // normal address
			}
		}

		/// <summary>
		/// Convert range into address string A1:B1 style.
		/// </summary>
		/// <returns>Address string converted from range position.</returns>
		public string ToRelativeAddress()
		{
			if (this.rows <= -1 && this.cols <= -1)
			{
				return "1:1048576";
			}
			else if (this.cols <= -1)
			{
				return string.Format("{0}:{1}", this.row + 1, this.EndRow + 1);
			}
			else if (this.rows <= -1)
			{
				return string.Format("{0}:{1}", RGUtility.GetAlphaChar(this.col), RGUtility.GetAlphaChar(this.EndCol));
			}
			else
				return string.Format("{0}{1}:{2}{3}", RGUtility.GetAlphaChar(this.col), this.row + 1,
					RGUtility.GetAlphaChar(this.EndCol), this.EndRow + 1);
		}

		/// <summary>
		/// Convert into absolute address.
		/// </summary>
		/// <returns>absolute address identifier.</returns>
		public string ToAbsoluteAddress()
		{
			if (this.rows <= -1 && this.cols <= -1)
			{
				return "$1:$1048576";
			}
			else if (this.cols <= -1)
			{
				return string.Format("${0}:${1}", this.row + 1, this.EndRow + 1);
			}
			else if (this.rows <= -1)
			{
				return string.Format("${0}:${1}", RGUtility.GetAlphaChar(this.col), RGUtility.GetAlphaChar(this.EndCol));
			}
			else
				return string.Format("${0}${1}:${2}${3}", RGUtility.GetAlphaChar(this.col), this.row + 1,
					RGUtility.GetAlphaChar(this.EndCol), this.EndRow + 1);
		}

		/// <summary>
		/// Convert range into string in span style. (4R x 3C)
		/// </summary>
		/// <returns>converted string of span style.</returns>
		public string ToStringSpans()
		{
			return string.Format("{0}R x {1}C", rows, cols);
		}
		#endregion // Stringify

		#region Utility

		/// <summary>
		/// Offset range by specified number of rows and cols.
		/// </summary>
		/// <param name="rows">rows to be offseted.</param>
		/// <param name="cols">cols to be offseted.</param>
		public void Offset(int rows, int cols)
		{
			this.row += rows;
			this.col += cols;
		}

		/// <summary>
		/// Check whether the string is a valid address in current spreadsheet.
		/// </summary>
		/// <param name="address">address to be checked.</param>
		/// <returns>true if address is valid.</returns>
		public static bool IsValidAddress(string address)
		{
			return RGUtility.RangeReferenceRegex.IsMatch(address)
				|| RGUtility.CellReferenceRegex.IsMatch(address)
				|| RGUtility.SingleAbsoulteRangeRegex.IsMatch(address);
		}

		/// <summary>
		/// Identify whether this range only contains one single cell.
		/// </summary>
		public bool IsSingleCell
		{
			get { return rows == 1 && cols == 1; }
		}

		public static RangePosition FromCellPosition(CellPosition startPosition, CellPosition endPosition)
		{
			return FromCellPosition(startPosition.Row, startPosition.Col, endPosition.Row, endPosition.Col);
		}

		/// <summary>
		/// Create range position instance from specified coordinates. 
		/// This method finds and uses the minimum and maximum row and column automatically.
		/// </summary>
		/// <param name="startRow">Number of row.</param>
		/// <param name="startColumn">Number of column.</param>
		/// <param name="endRow">Number of end row.</param>
		/// <param name="endColumn">Number of end column.</param>
		/// <returns>Range created from two cell positions.</returns>
		public static RangePosition FromCellPosition(int startRow, int startColumn, int endRow, int endColumn)
		{
			RangePosition range;

			range.row = Math.Min(startRow, endRow);
			range.col = Math.Min(startColumn, endColumn);
			range.rows = Math.Max(startRow, endRow) - range.row + 1;
			range.cols = Math.Max(startColumn, endColumn) - range.col + 1;
			range.positionProperties = 0;

			return range;
		}

		/// <summary>
		/// Return a range position that is the minimum range contains two specified ranges.
		/// </summary>
		/// <param name="range1">The first range position.</param>
		/// <param name="range2">The secondary range position.</param>
		/// <returns>A range that contains the two specified ranges.</returns>
		public static RangePosition Union(RangePosition range1, RangePosition range2)
		{
			if (range1.IsEmpty && range2.IsEmpty)
				return RangePosition.Empty;
			else if (range1.IsEmpty)
				return range2;
			else if (range2.IsEmpty)
				return range1;

			int row = Math.Min(range1.row, range2.row);
			int col = Math.Min(range1.col, range2.col);
			int endrow = Math.Max(range1.EndRow, range2.EndRow);
			int endcol = Math.Max(range1.EndCol, range2.EndCol);

			return new RangePosition(row, col, endrow - row + 1, endcol - col + 1);
		}

		/// <summary>
		/// Set number of rows.
		/// </summary>
		/// <param name="rows">Number of rows.</param>
		public void SetRows(int rows)
		{
			this.rows = rows;
		}

		/// <summary>
		/// Set number of columns.
		/// </summary>
		/// <param name="cols">Number of columns.</param>
		public void SetCols(int cols)
		{
			this.cols = cols;
		}

		#endregion // Utility
	}

	/// <summary>
	/// Represents the range or cell position properties.
	/// </summary>
	public enum PositionProperty
	{
		/// <summary>
		/// Relative address for cell or range position.
		/// </summary>
		Relative,

		/// <summary>
		/// Absolute address for cell or range position.
		/// </summary>
		Absolute,
	}

	#endregion // Range Position
}
