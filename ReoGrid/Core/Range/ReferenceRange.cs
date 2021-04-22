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

namespace unvell.ReoGrid
{
	/// <summary>
	/// Represents a range object refer to spreadsheet
	/// </summary>
	public class ReferenceRange : IRange
	{
		#region Value & Properties

		/// <summary>
		/// Get or set the worksheet which contains this range
		/// </summary>
		public Worksheet Worksheet { get; internal set; }

		private Cell startCell;
		private Cell endCell;
		//private ReoGridRange range = ReoGridRange.Empty;

		/// <summary>
		/// Get or set start position.
		/// </summary>
		public CellPosition StartPos
		{
			get { return this.startCell.Position; }
			set
			{
				this.startCell = this.Worksheet.CreateAndGetCell(this.Worksheet.FixPos(value));
			}
		}

		/// <summary>
		/// Get or set end position.
		/// </summary>
		public CellPosition EndPos
		{
			get { return this.endCell.Position; }
			set
			{
				this.endCell = this.Worksheet.CreateAndGetCell(this.Worksheet.FixPos(value));
			}
		}

		/// <summary>
		/// Zero-based number of row to locate the start position of this range.
		/// </summary>
		public int Row
		{
			get { return this.startCell.Row; }
			set
			{
				this.startCell = this.Worksheet.CreateAndGetCell(value, this.startCell.Column);
			}
		}

		/// <summary>
		/// Zero-based number of column to locate the start position of this range.
		/// </summary>
		public int Col
		{
			get { return this.startCell.Column; }
			set
			{
				this.startCell = this.Worksheet.CreateAndGetCell(this.startCell.Row, value);
			}
		}

		/// <summary>
		/// Get or set number of rows.
		/// </summary>
		public int Rows
		{
			get { return this.Position.Rows; }
			set
			{
				this.endCell = this.Worksheet.CreateAndGetCell(this.startCell.Row + value, this.endCell.Column);
			}
		}

		/// <summary>
		/// Get or set number of columns.
		/// </summary>
		public int Cols
		{
			get { return this.Position.Cols; }
			set
			{
				this.endCell = this.Worksheet.CreateAndGetCell(this.endCell.Row, this.startCell.Column + value);
			}
		}

		/// <summary>
		/// Get or set end number of row.
		/// </summary>
		public int EndRow
		{
			get { return this.endCell.Row; }
			set
			{
				this.endCell = this.Worksheet.CreateAndGetCell(new CellPosition(value, this.endCell.Column));
			}
		}

		/// <summary>
		/// Get or set end number of column.
		/// </summary>
		public int EndCol
		{
			get { return this.endCell.Column; }
			set
			{
				this.endCell = this.Worksheet.CreateAndGetCell(this.endCell.Row, value);
			}
		}

		/// <summary>
		/// Get or set the position of range on worksheet.
		/// </summary>
		public RangePosition Position
		{
			get { return new RangePosition(startCell.Position, endCell.Position); }
			set
			{
				var range = this.Worksheet.FixRange(value);

				this.startCell = this.Worksheet.CreateAndGetCell(range.StartPos);
				this.endCell = this.Worksheet.CreateAndGetCell(range.EndPos);
			}
		}

		#region Constructors
		internal ReferenceRange(Worksheet worksheet, Cell startCell, Cell endCell)
		{
			if (worksheet == null)
			{
				throw new ArgumentNullException("worksheet", "cannot create refereced range with null worksheet");
			}

			this.Worksheet = worksheet;
			this.startCell = startCell;
			this.endCell = endCell;
		}

		internal ReferenceRange(Worksheet worksheet, CellPosition startPos, CellPosition endPos)
			: this(worksheet, worksheet.CreateAndGetCell(startPos), worksheet.CreateAndGetCell(endPos))
		{
		}

		internal ReferenceRange(Worksheet worksheet, string address)
			: this(worksheet, new RangePosition(address))
		{
			// construct from address identifier
		}

		internal ReferenceRange(Worksheet worksheet, RangePosition range)
			: this(worksheet, worksheet.CreateAndGetCell(range.StartPos), worksheet.CreateAndGetCell(range.EndPos))
		{
			// construct from range position
		}

		internal ReferenceRange(Worksheet worksheet, CellPosition pos)
			: this(worksheet, pos, pos)
		{
			// construct from single cell position
		}
		#endregion // Constructors

		#endregion Value & Properties

		#region Utility
		/// <summary>
		/// Check whether or not the specified position is contained by this range.
		/// </summary>
		/// <param name="pos">Position to be checked.</param>
		/// <returns>True if specified position is contained by this range.</returns>
		public bool Contains(CellPosition pos)
		{
			var startPos = this.StartPos;
			var endPos = this.EndPos;

			return pos.Row >= startPos.Row && pos.Col >= startPos.Col
				&& pos.Row <= endPos.Row && pos.Col <= endPos.Col;
		}

		/// <summary>
		/// Check whether or not a specified range is contained by this range.
		/// </summary>
		/// <param name="range">Range position to be checked.</param>
		/// <returns>True if the specified range is contained by this range; Otherwise return false.</returns>
		public bool Contains(ReferenceRange range)
		{
			return this.startCell.InternalRow <= range.startCell.InternalRow
				&& this.startCell.InternalCol <= range.startCell.InternalCol
				&& this.endCell.InternalRow >= range.endCell.InternalRow
				&& this.endCell.InternalCol >= range.endCell.InternalCol;
		}

		/// <summary>
		/// Check whether or not a specified range is contained by this range.
		/// </summary>
		/// <param name="range">Range position to be checked.</param>
		/// <returns>True if the specified range is contained by this range; Otherwise return false.</returns>
		public bool Contains(RangePosition range)
		{
			return this.startCell.InternalRow <= range.Row
					&& this.startCell.InternalCol <= range.Col
					&& this.endCell.InternalRow >= range.EndRow
					&& this.endCell.InternalCol >= range.EndCol;
		}

		/// <summary>
		/// Check whether or not that the specified range intersects with this range.
		/// </summary>
		/// <param name="range">The range to be checked.</param>
		/// <returns>True if specified range intersects with this range.</returns>
		public bool IntersectWith(RangePosition range)
		{
			return this.Position.IntersectWith(range);
		}

		/// <summary>
		/// Check whether or not that the specified range intersects with this range.
		/// </summary>
		/// <param name="range">The range to be checked.</param>
		/// <returns>True if specified range intersects with this range.</returns>
		public bool IntersectWith(ReferenceRange range)
		{
			return this.IntersectWith(range.Position);
		}

		/// <summary>
		/// Convert to ReoGridRange structure.
		/// </summary>
		/// <param name="refRange">The object to be converted.</param>
		/// <returns>ReoGridRange structure converted from reference range instance.</returns>
		public static implicit operator RangePosition(ReferenceRange refRange)
		{
			return refRange.Position;
		}

		/// <summary>
		/// Convert reference range into description string.
		/// </summary>
		/// <returns>String to describe this reference range.</returns>
		public override string ToString()
		{
			return this.Position.ToString();
		}

		/// <summary>
		/// Convert referenced range into address position string. 
		/// </summary>
		/// <returns>Address position string to describe this range on worksheet.</returns>
		public virtual string ToAddress()
		{
			return this.Position.ToAddress();
		}

		/// <summary>
		/// Convert referenced range into absolute address position string. 
		/// </summary>
		/// <returns>Absolute address position string to describe this range on worksheet.</returns>
		public virtual string ToAbsoluteAddress()
		{
			return this.Position.ToAbsoluteAddress();
		}
		#endregion // Utility

		#region Control API Routines

		/// <summary>
		/// Get or set data of this range.
		/// </summary>
		public object Data
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeData(this);
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeData(this, value);
			}
		}

		/// <summary>
		/// Select this range.
		/// </summary>
		public void Select()
		{
			CheckForOwnerAssociated();

			this.Worksheet.SelectRange(this.Position);
		}

		#region Style Wrapper
		private ReferenceRangeStyle referenceStyle = null;

		/// <summary>
		/// Get the style set from this range.
		/// </summary>
		public ReferenceRangeStyle Style
		{
			get
			{
				CheckForOwnerAssociated();

				if (this.referenceStyle == null)
				{
					this.referenceStyle = new ReferenceRangeStyle(this.Worksheet, this);
				}

				return this.referenceStyle;
			}
		}
		#endregion // Style Wrapper

		#region Border Wraps
		private RangeBorderProperty borderProperty = null;

		public RangeBorderProperty Border
		{
			get
			{
				if (borderProperty == null)
				{
					borderProperty = new RangeBorderProperty(this);
				}

				return borderProperty;
			}
		}

		/// <summary>
		/// Get or set left border styles for range.
		/// </summary>
		public RangeBorderStyle BorderLeft
		{
			get
			{
				CheckForOwnerAssociated();

				return Worksheet.GetRangeBorders(this.Position, BorderPositions.Left, true).Left;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.Left, value);
			}
		}

		/// <summary>
		/// Get or set top border styles for range.
		/// </summary>
		public RangeBorderStyle BorderTop
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.Top, true).Top;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.Top, value);
			}
		}

		/// <summary>
		/// Get or set right border styles for range.
		/// </summary>
		public RangeBorderStyle BorderRight
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.Right, true).Right;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.Right, value);
			}
		}

		/// <summary>
		/// Get or set bottom border styles for range.
		/// </summary>
		public RangeBorderStyle BorderBottom
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.Bottom, true).Bottom;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.Bottom, value);
			}
		}

		/// <summary>
		/// Get or set all inside borders style for range.
		/// </summary>
		public RangeBorderStyle BorderInsideAll
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideAll, true).InsideHorizontal; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideAll, value);
			}
		}

		/// <summary>
		/// Get or set all horizontal border styles for range.
		/// </summary>
		public RangeBorderStyle BorderInsideHorizontal
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideHorizontal, true).InsideHorizontal;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideHorizontal, value);
			}
		}

		/// <summary>
		/// Get or set all vertical border styles for range.
		/// </summary>
		public RangeBorderStyle BorderInsideVertical
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideVertical, true).InsideVertical;
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideVertical, value);
			}
		}

		/// <summary>
		/// Get or set all outside border styles for range.
		/// </summary>
		public RangeBorderStyle BorderOutside
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet == null ? RangeBorderStyle.Empty
					: this.Worksheet.GetRangeBorders(this.Position, BorderPositions.Outside, true).Left; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.Outside, value);
			}
		}

		/// <summary>
		/// Get or set all inside border styles for range.
		/// </summary>
		public RangeBorderStyle BorderAll
		{
			get
			{
				CheckForOwnerAssociated();

				return this.Worksheet.GetRangeBorders(this.Position, BorderPositions.All, true).Left; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.Worksheet.SetRangeBorders(this.Position, BorderPositions.All, value);
			}
		}

		#endregion // Border Wraps

		#region Merge & Group

		/// <summary>
		/// Merge this range into single cell
		/// </summary>
		public void Merge()
		{
			CheckForOwnerAssociated();

			this.Worksheet.MergeRange(this.Position);
		}

		/// <summary>
		/// Unmerge this range
		/// </summary>
		public void Unmerge()
		{
			CheckForOwnerAssociated();

			this.Worksheet.UnmergeRange(this.Position);
		}

		/// <summary>
		/// Determine whether or not this range contains only one merged cell
		/// </summary>
		public bool IsMergedCell
		{
			get
			{
				this.CheckForOwnerAssociated();

				var cell = Worksheet.GetCell(this.StartPos);

				return cell == null ? false : (cell.Rowspan == this.Rows && cell.Colspan == this.Cols);
			}
		}

#if OUTLINE
		/// <summary>
		/// Group all rows in this range
		/// </summary>
		public void GroupRows()
		{
			CheckForOwnerAssociated();

			this.Worksheet.GroupRows(this.Row, this.Rows);
		}

		/// <summary>
		/// Group all columns in this range
		/// </summary>
		public void GroupColumns()
		{
			CheckForOwnerAssociated();

			this.Worksheet.GroupColumns(this.Col, this.Cols);
		}

		/// <summary>
		/// Ungroup all rows in this range
		/// </summary>
		public void UngroupRows()
		{
			CheckForOwnerAssociated();

			this.Worksheet.UngroupRows(this.Row, this.Rows);
		}

		/// <summary>
		/// Ungroup all columns in this range
		/// </summary>
		public void UngroupColumns()
		{
			CheckForOwnerAssociated();

			this.Worksheet.UngroupColumns(this.Row, this.Rows);
		}
#endif // OUTLINE
		#endregion // Merge and Group

		#region Readonly

		/// <summary>
		/// Set or get readonly property to all cells inside this range
		/// </summary>
		public bool IsReadonly
		{
			get
			{
				bool allReadonly = true;
				foreach (var cell in this.Cells)
				{
					if (!cell.IsReadOnly)
					{
						allReadonly = false;
						break;
					}
				}
				return allReadonly;
			}
			set
			{
				foreach (var cell in this.Cells)
				{
					cell.IsReadOnly = value;
				}
			}
		}

		#endregion // Readonly

		private void CheckForOwnerAssociated()
		{
			if (this.Worksheet == null)
			{
				throw new ReferenceRangeNotAssociatedException(this);
			}
		}
		#endregion // Control API Routines

		#region Cells Collection
		private Worksheet.CellCollection cellsCollection;

		/// <summary>
		/// Get the collection of all cell instances in this range
		/// </summary>
		public Worksheet.CellCollection Cells
		{
			get
			{
				if (this.cellsCollection == null)
				{
					this.cellsCollection = new Worksheet.CellCollection(this.Worksheet, this);
				}

				return this.cellsCollection;
			}
		}

		#endregion // Cells Collection
	}
}
