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

#if OUTLINE

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Add outline action
	/// </summary>
	public class AddOutlineAction : OutlineAction
	{
		/// <summary>
		/// Create action to add outline
		/// </summary>
		/// <param name="rowOrColumn">Row or column to be added</param>
		/// <param name="start">Number of line to start add outline</param>
		/// <param name="count">Number of lines to be added into this outline</param>
		public AddOutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn, start, count)
		{
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.AddOutline(this.rowOrColumn, start, count);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.RemoveOutline(this.rowOrColumn, start, count);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Add {0} Outline, Start at {1}, Count: {2}",
				base.GetRowOrColumnDesc(), this.start, this.count);
		}
	}
}

#endif // OUTLINE
