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
	/// Action to hide specified columns
	/// </summary>
	public class HideColumnsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to hide specified columns.
		/// </summary>
		/// <param name="col">zero-based number of column to start hide columns.</param>
		/// <param name="count">number of columns to be hidden.</param>
		public HideColumnsAction(int col, int count)
			: base(new RangePosition(0, col, -1, count)) { }

		/// <summary>
		/// Perform action to hide specified columns.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.HideColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Undo action to show hidden columns.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.ShowColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new HideColumnsAction(range.Col, range.Cols);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName() { return "Hide Columns"; }
	}
}
