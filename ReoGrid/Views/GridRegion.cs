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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jingwood, unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#define VP_DEBUG

using System.Collections.Generic;
using System.Diagnostics;

#if WINFORM || ANDROID
using RGFloat = System.Single;
using RGIntDouble = System.Int32;

#elif WPF
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#elif iOS
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#endif

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{
	internal struct GridRegion
	{
		internal int startRow;
		internal int endRow;
		internal int startCol;
		internal int endCol;
		internal static readonly GridRegion Empty = new GridRegion()
		{
			startRow = 0,
			startCol = 0,
			endRow = 0,
			endCol = 0
		};
		public GridRegion(int startRow, int startCol, int endRow, int endCol)
		{
			this.startRow = startRow;
			this.startCol = startCol;
			this.endRow = endRow;
			this.endCol = endCol;
		}
		public bool Contains(CellPosition pos)
		{
			return Contains(pos.Row, pos.Col);
		}
		public bool Contains(int row, int col)
		{
			return this.startRow <= row && this.endRow >= row && this.startCol <= col && this.endCol >= col;
		}
		public bool Contains(RangePosition range)
		{
			return range.Row >= this.startRow && range.Col >= this.startCol
				&& range.EndRow <= this.endRow && range.EndCol <= this.endCol;
		}
		public bool Intersect(RangePosition range)
		{
			return (range.Row < this.startRow && range.EndRow > this.startRow)
				|| (range.Row < this.endRow && range.EndRow > this.endRow)
				|| (range.Col < this.startCol && range.EndCol > this.startCol)
				|| (range.Col < this.endCol && range.EndCol > this.endCol);
		}
		public bool IsOverlay(RangePosition range)
		{
			return Contains(range) || Intersect(range);
		}
		public override bool Equals(object obj)
		{
			if ((obj as GridRegion?) == null) return false;

			GridRegion gr2 = (GridRegion)obj;
			return startRow == gr2.startRow && startCol == gr2.startCol
				&& endRow == gr2.endRow && endCol == gr2.endCol;
		}
		public override int GetHashCode()
		{
			return startRow ^ startCol ^ endRow ^ endCol;
		}
		public static bool operator ==(GridRegion gr1, GridRegion gr2)
		{
			return gr1.Equals(gr2);
		}
		public static bool operator !=(GridRegion gr1, GridRegion gr2)
		{
			return !gr1.Equals(gr2);
		}
		public bool IsEmpty { get { return this.Equals(Empty); } }
		public int Rows { get { return endRow - startRow + 1; } set { endRow = startRow + value - 1; } }
		public int Cols { get { return endCol - startCol + 1; } set { endCol = startCol + value - 1; } }

		public override string ToString()
		{
			return string.Format("VisibleRegion[{0},{1}-{2},{3}]", startRow, startCol, endRow, endCol);
		}

		/// <summary>
		/// Convert into range struct
		/// </summary>
		/// <returns></returns>
		public RangePosition ToRange()
		{
			return new RangePosition(startRow, startCol, endRow - startRow + 1, endCol - startCol + 1);
		}
	}
}

