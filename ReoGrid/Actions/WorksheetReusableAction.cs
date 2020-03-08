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
	/// Reusable action is one type of RGAction to support repeat operation
	/// to a specified range. It is good practice to make all actions with 
	/// a range target to inherit from this class.
	/// </summary>
	public abstract class WorksheetReusableAction : BaseWorksheetAction
	{
		/// <summary>
		/// Range to be appiled this action
		/// </summary>
		public RangePosition Range { get; set; }

		protected WorksheetReusableAction() { }

		/// <summary>
		/// Constructor of RGReusableAction 
		/// </summary>
		/// <param name="range">Range to be applied this action</param>
		public WorksheetReusableAction(RangePosition range)
		{
			this.Range = range;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public abstract WorksheetReusableAction Clone(RangePosition range);
	}

}
