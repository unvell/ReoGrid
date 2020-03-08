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
	/// Base class for all classes of outline actions
	/// </summary>
	public abstract class BaseOutlineAction: BaseWorksheetAction
	{
		/// <summary>
		/// Outline direction.
		/// </summary>
		internal protected RowOrColumn rowOrColumn;

		/// <summary>
		/// Specify row or column outline
		/// </summary>
		public RowOrColumn RowOrColumn { get { return this.rowOrColumn; } }

		/// <summary>
		/// Create base outline action instance
		/// </summary>
		/// <param name="rowOrColumn">Flag to specify row or column</param>
		public BaseOutlineAction(RowOrColumn rowOrColumn)
		{
			this.rowOrColumn = rowOrColumn;
		}

		/// <summary>
		/// Get description text of outline direction.
		/// </summary>
		/// <returns>Text of outline direction.</returns>
		internal protected string GetRowOrColumnDesc()
		{
			switch (this.rowOrColumn)
			{
				default:
				case ReoGrid.RowOrColumn.Both:
					return "Row and Column";
				case ReoGrid.RowOrColumn.Row:
					return "Row";
				case ReoGrid.RowOrColumn.Column:
					return "Column";
			}
		}
	}
}

#endif // OUTLINE
