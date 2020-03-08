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

using System.Collections.Generic;

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Set height of row action
	/// </summary>
	public class SetRowsHeightAction : WorksheetReusableAction
	{
		private ushort height;

		/// <summary>
		/// Height to be set
		/// </summary>
		public ushort Height { get { return height; } set { height = value; } }

		/// <summary>
		/// Create instance for SetRowsHeightAction
		/// </summary>
		/// <param name="row">Index of row start to set</param>
		/// <param name="count">Number of rows to be set</param>
		/// <param name="height">New height to set to specified rows</param>
		public SetRowsHeightAction(int row, int count, ushort height)
			: base(new RangePosition(row, 0, count, -1))
		{
			this.height = height;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			int row = Range.Row;
			int count = Range.Rows;

			backupRows.Clear();

			int r2 = row + count;
			for (int r = row; r < r2; r++)
			{
				RowHeader rowHead = Worksheet.RetrieveRowHeader(r);

				backupRows.Add(r, new RowHeadData
				{
					autoHeight = rowHead.IsAutoHeight,
					row = rowHead.Row,
					height = rowHead.InnerHeight,
				});

				// disable auto-height-adjusting if user has changed height of this row
				rowHead.IsAutoHeight = false;
			}

			Worksheet.SetRowsHeight(row, count, height);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetRowsHeight(Range.Row, Range.Rows, r => backupRows[r].height, true);

			foreach (int r in backupRows.Keys)
			{
				RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
				rowHead.IsAutoHeight = backupRows[r].autoHeight;
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Rows Height: " + height;
		}

		Dictionary<int, RowHeadData> backupRows = new Dictionary<int, RowHeadData>();

		internal struct RowHeadData
		{
			internal int row;
			internal ushort height;
			internal bool autoHeight;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRowsHeightAction(range.Row, range.Rows, height);
		}
	}
}
