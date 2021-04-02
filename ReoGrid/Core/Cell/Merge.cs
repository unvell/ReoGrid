/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid
{
	partial class Cell
	{
		private CellPosition mergeStartPos = CellPosition.Empty;

		internal CellPosition MergeStartPos
		{
			get { return mergeStartPos; }
			set
			{
#if DEBUG
				Debug.Assert(value.Row >= -1 && value.Col >= -1);
#endif

				mergeStartPos = value;
			}
		}

		private CellPosition mergeEndPos = CellPosition.Empty;

		internal CellPosition MergeEndPos
		{
			get { return mergeEndPos; }
			set
			{
#if DEBUG
				if (value.Row > -1 && value.Col <= -1
					|| value.Row <= -1 && value.Col > -1)
					Debug.Assert(false);

				Debug.Assert(value.Row >= -1 && value.Col >= -1);
#endif
				mergeEndPos = value;
			}
		}

		internal bool IsStartMergedCell
		{
			get { return this.InternalPos.Equals(MergeStartPos); }
		}

		internal bool IsEndMergedCell
		{
			get { return this.InternalPos.Row == mergeEndPos.Row && this.InternalPos.Col == mergeEndPos.Col; }
		}

		/// <summary>
		/// Check whether this cell is merged cell
		/// </summary>
		public bool IsMergedCell
		{
			get { return IsStartMergedCell; }
		}

		/// <summary>
		/// Check whether or not this cell is an valid cell, only valid cells can be set data and styles.
		/// Cells merged by another cell will become invalid.
		/// </summary>
		public bool IsValidCell
		{
			get { return rowspan >= 1 && colspan >= 1; }
		}

		/// <summary>
		/// Check whether or not this cell is inside a merged range
		/// </summary>
		public bool InsideMergedRange
		{
			get { return IsStartMergedCell || (rowspan == 0 && colspan == 0); }
		}
	}
}
