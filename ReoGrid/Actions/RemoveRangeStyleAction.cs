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
	/// Remove style from specified range action
	/// </summary>
	public class RemoveRangeStyleAction : WorksheetReusableAction
	{
		private PlainStyleFlag flag;

		/// <summary>
		/// Style flag indicates what type of style to be handled.
		/// </summary>
		public PlainStyleFlag Flag { get { return flag; } set { flag = value; } }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for action to remove style from specified range.
		/// </summary>
		/// <param name="range">Styles from this specified range to be removed</param>
		/// <param name="flag">Style flag indicates what type of style should be removed</param>
		public RemoveRangeStyleAction(RangePosition range, PlainStyleFlag flag)
			: base(range)
		{
			this.flag = flag;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range);
			Worksheet.RemoveRangeStyles(Range, flag);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Delete Style";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRangeStyleAction(Range, flag);
		}
	}
}
