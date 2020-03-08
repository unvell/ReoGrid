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
	/// Action for removing worksheet
	/// </summary>
	public class RemoveWorksheetAction : WorkbookAction
	{
		/// <summary>
		/// Number of worksheet
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Worksheet instance
		/// </summary>
		public Worksheet Worksheet { get; private set; }

		/// <summary>
		/// Create this action to insert worksheet
		/// </summary>
		/// <param name="index">Number of worksheet</param>
		/// <param name="worksheet">Worksheet instance</param>
		public RemoveWorksheetAction(int index, Worksheet worksheet)
		{
			this.Index = index;
			this.Worksheet = worksheet;
		}

		/// <summary>
		/// Do this action to remove worksheet
		/// </summary>
		public override void Do()
		{
			this.Workbook.RemoveWorksheet(this.Index);
		}

		/// <summary>
		/// Undo this action to restore the removed worksheet
		/// </summary>
		public override void Undo()
		{
			this.Workbook.InsertWorksheet(this.Index, this.Worksheet);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Worksheet: " + this.Worksheet.Name;
		}
	}
}
