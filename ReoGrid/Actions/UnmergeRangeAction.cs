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
	/// Unmerge range action.
	/// </summary>
	public class UnmergeRangeAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create instance for UnmergeRangeAction with specified range.
		/// </summary>
		/// <param name="range">The range to be unmerged</param>
		public UnmergeRangeAction(RangePosition range) : base(range) { }

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnmergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this action.
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Worksheet.UnmergeRange(Range);
		}

		/// <summary>
		/// Undo this action.
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Merge Range";
		}
	}
}
