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

using System.Linq;

using unvell.Common;

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Make font size larger or smaller action.
	/// </summary>
	public class StepRangeFontSizeAction : WorksheetReusableAction
	{
		/// <summary>
		/// True if this action making font size larger.
		/// </summary>
		public bool Enlarge { get; set; }

		/// <summary>
		/// Create instance for this action with specified range and enlarge flag.
		/// </summary>
		/// <param name="range">Specified range to apply this action</param>
		/// <param name="enlarge">True to set text larger, false to set smaller</param>
		public StepRangeFontSizeAction(RangePosition range, bool enlarge)
			: base(range)
		{
			this.Enlarge = enlarge;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			Worksheet.StepRangeFont(Range, size =>
			{
				return Enlarge ?
						(size >= Toolkit.FontSizeList.Max()) ? size : Toolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= Toolkit.FontSizeList.Min()) ? size : Toolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.StepRangeFont(Range, size =>
			{
				return !Enlarge ?
						(size >= Toolkit.FontSizeList.Max()) ? size : Toolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= Toolkit.FontSizeList.Min()) ? size : Toolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return Enlarge ? "Make Text Bigger" : "Make Text Smaller";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new StepRangeFontSizeAction(range, Enlarge);
		}
	}
}
