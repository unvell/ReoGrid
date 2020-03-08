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
	/// Base class for all classes of single outline action
	/// </summary>
	public abstract class OutlineAction : BaseOutlineAction
	{
		internal int start;

		/// <summary>
		/// Number of line of start position to outilne
		/// </summary>
		public int Start { get { return this.start; } }

		internal int count;

		/// <summary>
		/// Number of lines does outline include
		/// </summary>
		public int Count { get { return this.count; } }
	
		/// <summary>
		/// Create base outline action instance
		/// </summary>
		/// <param name="rowOrColumn">Flag to specify row or column</param>
		/// <param name="start">Number of line to start add outline</param>
		/// <param name="count">Number of lines to be added into this outline</param>
		public OutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn)
		{
			this.start = start;
			this.count = count;
		}
	}
}

#endif // OUTLINE
