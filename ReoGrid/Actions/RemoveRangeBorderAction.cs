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
	/// Action of Removing borders from specified range
	/// </summary>
	public class RemoveRangeBorderAction : WorksheetReusableAction
	{
		/// <summary>
		/// Get or set the position of borders to be removed
		/// </summary>
		public BorderPositions BorderPos { get; set; }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for SetRangeBorderAction with specified range and border styles.
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		public RemoveRangeBorderAction(RangePosition range, BorderPositions pos)
			: base(range)
		{
			this.BorderPos = pos;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			Worksheet.RemoveRangeBorders(Range, BorderPos);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Range Border";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRangeBorderAction(range, BorderPos);
		}
	}
}
