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
 * (c) 2012-2023 Jingwood, unvell inc. <jingwood at unvell.com>
 * 
 ****************************************************************************/

using System.Diagnostics;

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Create action to set data into specified range of spreadsheet.
	/// </summary>
	public class SetSortedRangeDataAction : BaseWorksheetAction
	{
		private readonly RangePosition range;
		private readonly object[,] data;
		private object[,] backupData;
		private bool isRegularExecution;

		/// <summary>
		/// Create action to set data into specified range of spreadsheet.
		/// </summary>
		/// <param name="range">range to set specified data.</param>
		/// <param name="data">data to be set.</param>
		public SetSortedRangeDataAction(RangePosition range, object[,] data)
		{
			this.range = range;
			this.data = data;
			this.isRegularExecution = true;
		}



		/// <summary>
		/// Do action to set data into specified range of spreadsheet
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetRangeData(range);
			Debug.Assert(backupData != null);
			Worksheet.SetRangeData(range, data, true);

			if (isRegularExecution)
			{
				isRegularExecution = false;
				return;
			}

			Worksheet.SelectRange(range);
		}

		/// <summary>
		/// Undo action to remove data which has been set into specified range of spreadsheet
		/// </summary>
		public override void Undo()
		{
			Debug.Assert(backupData != null);
			base.Worksheet.SetRangeData(range, backupData);
			isRegularExecution = false;
			Worksheet.SelectRange(range);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Set Sorted Cells Data";
		}
	}
}
