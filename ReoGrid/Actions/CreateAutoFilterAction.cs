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

using unvell.ReoGrid.Data;

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Action to create column filter
	/// </summary>
	public class CreateAutoFilterAction : BaseWorksheetAction
	{

		/// <summary>
		/// Get filter apply range.
		/// </summary>
		public RangePosition Range { get; private set; }

		private AutoColumnFilter autoColumnFilter;

		/// <summary>
		/// Get auto column filter instance created by this action. (Will be null before doing action)
		/// </summary>
		public AutoColumnFilter AutoColumnFilter { get { return this.autoColumnFilter; } }

		/// <summary>
		/// Create action to create column filter
		/// </summary>
		/// <param name="range">filter range</param>
		public CreateAutoFilterAction(RangePosition range)
		{
			this.Range = range;
		}

		///// <summary>
		///// Create action to create column filter
		///// </summary>
		///// <param name="startColumn">zero-based number of column begin to create filter</param>
		///// <param name="endColumn">zero-based number of column end to create filter</param>
		///// <param name="titleRows">number of rows as title rows will not be included in filter and sort range</param>
		//public CreateAutoFilterAction(int startColumn, int endColumn, int titleRows = 1)
		//{
		//	this.StartColumn = startColumn;
		//	this.EndColumn = endColumn;
		//	this.TitleRows = titleRows;
		//}

		/// <summary>
		/// Undo action to remove column filter that is created by this action
		/// </summary>
		public override void Undo()
		{
			if (autoColumnFilter != null)
			{
				autoColumnFilter.Detach();
			}
		}

		/// <summary>
		/// Do action to create column filter
		/// </summary>
		public override void Do()
		{
			if (this.autoColumnFilter == null)
			{
				this.autoColumnFilter = base.Worksheet.CreateColumnFilter(this.Range,
					AutoColumnFilterUI.DropdownButtonAndPanel);
			}
			else
			{
				this.autoColumnFilter.Attach(base.Worksheet);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>friendly name of this action</returns>
		public override string GetName()
		{
			return "Create Column Filter";
		}
	}
}
