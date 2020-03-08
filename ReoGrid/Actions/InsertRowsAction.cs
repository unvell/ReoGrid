/*****************************************************************************
 * 
 * ReoGrid - Opensource .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Thank you to all contributors!
 * 
 * (c) 2012-2020 Jingwood, unvell.com <jingwood at unvell.com>
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Insert rows action
	/// </summary>
	public class InsertRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Index of row to insert empty rows. Set to Control.RowCount to 
		/// append columns at end of rows.
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Number of empty rows to be inserted
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Create instance for InsertRowsAction
		/// </summary>
		/// <param name="row">Index of row to insert</param>
		/// <param name="count">Number of rows to be inserted</param>
		public InsertRowsAction(int row, int count)
			: base(RangePosition.Empty)
		{
			this.Row = row;
			this.Count = count;
		}

		int insertedRow = -1;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			insertedRow = Row;
			Worksheet.InsertRows(Row, Count);
			Range = new RangePosition(Row, 0, Count, Worksheet.ColumnCount);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.DeleteRows(insertedRow, Count);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Rows";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new InsertRowsAction(range.Row, range.Rows);
		}
	}
}
