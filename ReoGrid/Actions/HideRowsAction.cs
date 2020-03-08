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
	/// Hide specified rows action
	/// </summary>
	public class HideRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to hide specified rows.
		/// </summary>
		/// <param name="row">Zero-based row index to start hiding.</param>
		/// <param name="count">Number of rows to be hidden.</param>
		public HideRowsAction(int row, int count)
			: base(new RangePosition(row, 0, count, -1)) { }

		/// <summary>
		/// Do action to hide specified rows.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.HideRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Undo action to show hidden rows.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.ShowRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new HideRowsAction(range.Row, range.Rows);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName() { return "Hide Rows"; }
	}
}
