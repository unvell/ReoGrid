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
 * (c) 2012-2023 Jingwood, unvell inc. <jingwood at unvell.com>
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Action for inserting worksheet
	/// </summary>
	public class InsertWorksheetAction : WorkbookAction
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
		public InsertWorksheetAction(int index, Worksheet worksheet)
		{
			this.Index = index;
			this.Worksheet = worksheet;
		}

		/// <summary>
		/// Do this action to insert worksheet
		/// </summary>
		public override void Do()
		{
			this.Workbook.InsertWorksheet(this.Index, this.Worksheet);
		}

		/// <summary>
		/// Undo this action to remove the inserted worksheet
		/// </summary>
		public override void Undo()
		{
			this.Workbook.RemoveWorksheet(this.Index);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Worksheet: " + this.Worksheet.Name;
		}
	}

}
