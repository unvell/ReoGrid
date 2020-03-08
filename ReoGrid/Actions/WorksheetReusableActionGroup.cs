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
	/// Reusable action group is one type of RGActionGroup to support repeat 
	/// operation to a specified range. It is good practice to make all reusable 
	/// action groups to inherit from this class.
	/// </summary>
	public class WorksheetReusableActionGroup : WorksheetReusableAction
	{
		private List<WorksheetReusableAction> actions;

		/// <summary>
		/// All reusable actions stored in this list will be performed together.
		/// </summary>
		public List<WorksheetReusableAction> Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		public WorksheetReusableActionGroup(RangePosition range)
			: base(range)
		{
			this.actions = new List<WorksheetReusableAction>();
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		/// <param name="actions">Action list to be performed together</param>
		public WorksheetReusableActionGroup(RangePosition range, List<WorksheetReusableAction> actions)
			: base(range)
		{
			this.actions = actions;
		}

		private bool first = true;

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			if (first)
			{
				for (int i = 0; i < actions.Count; i++)
					actions[i].Worksheet = this.Worksheet;
				first = false;
			}

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
			{
				actions[i].Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Multi-Aciton[" + actions.Count + "]";
		}

		/// <summary>
		/// Create cloned reusable action group from this action group
		/// </summary>
		/// <param name="range">Specified new range to apply this action group</param>
		/// <returns>New reusable action group cloned from this action group</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			List<WorksheetReusableAction> clonedActions = new List<WorksheetReusableAction>();

			foreach (WorksheetReusableAction action in actions)
			{
				clonedActions.Add(action.Clone(range));
			}

			return new WorksheetReusableActionGroup(range, clonedActions);
		}
	}

}
