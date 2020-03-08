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
	/// Action to move specified range from a position to another position.
	/// </summary>
	public class MoveRangeAction : BaseWorksheetAction
	{
		/// <summary>
		/// Specifies the content to be moved: data, borders and styles.
		/// </summary>
		public PartialGridCopyFlag ContentFlags { get; set; }

		/// <summary>
		/// Range to be moved.
		/// </summary>
		public RangePosition FromRange { get; set; }

		/// <summary>
		/// Position that range will be moved to.
		/// </summary>
		public CellPosition ToPosition { get; set; }

		/// <summary>
		/// Construct this action to move specified range from a position to another position.
		/// </summary>
		/// <param name="fromRange">range to be moved.</param>
		/// <param name="toPosition">position to be moved to.</param>
		public MoveRangeAction(RangePosition fromRange, CellPosition toPosition)
		{
			this.ContentFlags = PartialGridCopyFlag.All;
			this.FromRange = fromRange;
			this.ToPosition = toPosition;
		}

		private PartialGrid backupGrid;

		/// <summary>
		/// Do this action.
		/// </summary>
		public override void Do()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			backupGrid = this.Worksheet.GetPartialGrid(targetRange);

			this.Worksheet.MoveRange(this.FromRange, targetRange);

			this.Worksheet.SelectionRange = targetRange;
		}

		/// <summary>
		/// Undo this action.
		/// </summary>
		public override void Undo()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			this.Worksheet.MoveRange(targetRange, this.FromRange);

			this.Worksheet.SetPartialGrid(targetRange, this.backupGrid);

			this.Worksheet.SelectionRange = this.FromRange;
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Move Range";
		}
	}
}
