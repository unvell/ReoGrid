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

using unvell.Common;

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Base action for all actions that are used for worksheet operations.
	/// </summary>
	public abstract class BaseWorksheetAction : IUndoableAction
	{
		/// <summary>
		/// Instance for the grid control will be setted before action performed.
		/// </summary>
		public Worksheet Worksheet { get; internal set; }

		/// <summary>
		/// Do this action.
		/// </summary>
		public abstract void Do();

		/// <summary>
		/// Undo this action.
		/// </summary>
		public abstract void Undo();

		/// <summary>
		/// Redo this action.
		/// </summary>
		public virtual void Redo()
		{
			this.Do();
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>Get friendly name of this action.</returns>
		public abstract string GetName();
	}
}
