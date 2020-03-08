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
	/// Action to unhide specified rows.
	/// </summary>
	public class UnhideRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to show specified rows.
		/// </summary>
		/// <param name="row">number of row to start unhidden.</param>
		/// <param name="count">number of rows to be unhidden.</param>
		public UnhideRowsAction(int row, int count)
			: base(new RangePosition(row, 0, count, -1)) { }

		/// <summary>
		/// Do action to show specified hidden rows.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.ShowRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Undo action to hide visible rows.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.HideRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnhideRowsAction(range.Row, range.Rows);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>firendly name of this action</returns>
		public override string GetName() { return "Unhide Rows"; }
	}
}
