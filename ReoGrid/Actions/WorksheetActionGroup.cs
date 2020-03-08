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
	/// The action group is one type of RGAction to support Do/Undo/Redo a series of actions.
	/// </summary>
	public class WorksheetActionGroup : BaseWorksheetAction
	{
		/// <summary>
		/// Actions stored in this list will be Do/Undo/Redo together
		/// </summary>
		public List<BaseWorksheetAction> Actions { get; set; }

		/// <summary>
		/// Create instance for RGActionGroup
		/// </summary>
		public WorksheetActionGroup()
		{
			this.Actions = new List<BaseWorksheetAction>();
		}

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			foreach (var action in Actions)
			{
				action.Worksheet = this.Worksheet;
				action.Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = Actions.Count - 1; i >= 0; i--)
			{
				var action = Actions[i];

				action.Worksheet = this.Worksheet;
				action.Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "ReoGrid Action Group";
		}
	}

}
