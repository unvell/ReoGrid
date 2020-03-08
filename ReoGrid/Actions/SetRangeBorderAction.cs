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
	/// Action to set borders to specified range
	/// </summary>
	public class SetRangeBorderAction : WorksheetReusableAction
	{
		private RangeBorderInfo[] borders;

		/// <summary>
		/// Borders to be set
		/// </summary>
		public RangeBorderInfo[] Borders
		{
			get { return borders; }
			set { borders = value; }
		}

		private PartialGrid backupData;

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		/// <param name="styles">Style of border</param>
		public SetRangeBorderAction(RangePosition range, BorderPositions pos, RangeBorderStyle styles)
			: this(range, new RangeBorderInfo[] { new RangeBorderInfo(pos, styles) }) { }

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="styles">Style of border</param>
		public SetRangeBorderAction(RangePosition range, RangeBorderInfo[] styles)
			: base(range)
		{
			this.borders = styles;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			for (int i = 0; i < borders.Length; i++)
			{
				Worksheet.SetRangeBorders(Range, borders[i].Pos, borders[i].Style);
			}
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
			return new SetRangeBorderAction(range, borders);
		}
	}
}
