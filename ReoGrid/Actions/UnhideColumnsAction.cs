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
	/// Unhide specified columns action
	/// </summary>
	public class UnhideColumnsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to show hidden columns
		/// </summary>
		/// <param name="col">Number of column start to unhide</param>
		/// <param name="count">Number of columns to be unhidden</param>
		public UnhideColumnsAction(int col, int count)
			: base(new RangePosition(0, col, -1, count)) { }

		/// <summary>
		/// Do action to show hidden columns
		/// </summary>
		public override void Do()
		{
			this.Worksheet.ShowColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Do action to hide specified visible columns
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.HideColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnhideColumnsAction(range.Col, range.Cols);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>friendly name of this action</returns>
		public override string GetName() { return "Unhide Columns"; }
	}
}
