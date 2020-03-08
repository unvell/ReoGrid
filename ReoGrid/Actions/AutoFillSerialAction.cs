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

using System;

#if FORMULA

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Action that performs serial range fill according to given source and target ranges.
	/// </summary>
	public class AutoFillSerialAction : BaseWorksheetAction
	{
		/// <summary>
		/// Source range to be rerferenced.
		/// </summary>
		public RangePosition SourceRange { get; set; }

		/// <summary>
		/// Target range to be filled.
		/// </summary>
		public RangePosition TargetRange { get; set; }

		public AutoFillSerialAction(RangePosition sourceRange, RangePosition targetRange)
		{
			this.SourceRange = sourceRange;
			this.TargetRange = targetRange;
		}

		/// <summary>
		/// Backuped cells content used to recover the target range.
		/// </summary>
		PartialGrid backupedGrid;

		public override void Do()
		{
			// Get a backup of target range that will be overwritten
			backupedGrid = Worksheet.GetPartialGrid(TargetRange, PartialGridCopyFlag.CellData, ExPartialGridCopyFlag.None);

			try
			{
				Worksheet.AutoFillSerial(SourceRange, TargetRange);
				Worksheet.SelectionRange = RangePosition.Union(SourceRange, TargetRange);
			}
			catch (Exception ex)
			{
				Worksheet.NotifyExceptionHappen(ex);
			}
		}

		public override void Undo()
		{
			try
			{
				// Restore the range filled by this action
				Worksheet.SetPartialGrid(TargetRange, backupedGrid);
				Worksheet.SelectionRange = SourceRange;
			}
			catch (Exception ex)
			{
				Worksheet.NotifyExceptionHappen(ex);
			}
		}

		public override string GetName()
		{
			return "Fill Serial Action";
		}
	}
}

#endif // FORMULA

