﻿/*****************************************************************************
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
 * (c) 2012-2025 Jingwood, UNVELL Inc. <jingwood at unvell.com>
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Merge range action
	/// </summary>
	public class MergeRangeAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create instance for MergeRangeAction with specified range
		/// </summary>
		/// <param name="range">The range to be merged</param>
		public MergeRangeAction(RangePosition range) : base(range) { }

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new MergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this operation.
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Worksheet.MergeRange(Range);
		}

		/// <summary>
		/// Undo this operation.
		/// </summary>
		public override void Undo()
		{
			Worksheet.UnmergeRange(base.Range);
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
