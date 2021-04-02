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
using System.Linq;
using System.Text;

#if OUTLINE
using unvell.ReoGrid.Outline;
#endif // OUTLINE

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		/// <summary>
		/// Clone this worksheet, create a new instance.
		/// </summary>
		/// <returns>New instance cloned from current worksheet.</returns>
		public Worksheet Clone(string newName = null)
		{
			if (workbook == null)
			{
				throw new ReferenceObjectNotAssociatedException("worksheet must be added into workbook to do this");
			}

			if (string.IsNullOrEmpty(newName))
			{
				newName = this.workbook.GetAvailableWorksheetName();
			}

			Worksheet newSheet = new Worksheet(this.workbook, null, 0, 0)
			{
				name = newName,
				RootStyle = new WorksheetRangeStyle(this.RootStyle),
				//controlStyle = this.controlStyle,

				defaultColumnWidth = this.defaultColumnWidth,
				defaultRowHeight = this.defaultRowHeight,
				registeredNamedRanges = new Dictionary<string, NamedRange>(this.registeredNamedRanges),
#if OUTLINE
				outlines = this.outlines == null ? null : new Dictionary<RowOrColumn, OutlineCollection<ReoGridOutline>>(this.outlines),
#endif // OUTLINE
				highlightRanges = new List<HighlightRange>(this.highlightRanges),

#if FREEZE

#endif // FREEZE

#if PRINT
				pageBreakRows = this.pageBreakRows == null ? null : new List<int>(this.pageBreakRows),
				pageBreakCols = this.pageBreakCols == null ? null : new List<int>(this.pageBreakCols),
				userPageBreakCols = this.userPageBreakCols == null ? null : new List<int>(this.userPageBreakCols),
				userPageBreakRows = this.userPageBreakRows == null ? null : new List<int>(this.userPageBreakRows),
#endif // PRINT

				settings = this.settings,
			};

			newSheet.rows.Capacity = this.rows.Count;
			newSheet.cols.Capacity = this.cols.Count;

			// copy headers

			foreach (var rheader in this.rows)
			{
				newSheet.rows.Add(rheader.Clone(newSheet));
			}

			foreach (var cheader in this.cols)
			{
				newSheet.cols.Add(cheader.Clone(newSheet));
			}

			// copy cells
			var partialGrid = this.GetPartialGrid(RangePosition.EntireRange);
			newSheet.SetPartialGrid(RangePosition.EntireRange, partialGrid);

			//this.IterateCells(ReoGridRange.EntireRange, (row, col, cell) =>
			//{
			//	var toCell = newSheet.CreateAndGetCell(row, col);
			//	ReoGridCellUtility.CopyCell(toCell, cell);

			//	return true;
			//});

			// copy drawing objects  (TODO: cloen all objects)
			//newSheet.drawingCanvas.Children.AddRange(this.drawingCanvas.Children);

			//var nvc = newSheet.viewportController as Views.NormalViewportController;
			//if (nvc != null)
			//{
			//	nvc.Bounds = this.viewportController.Bounds;
			//	newSheet.UpdateViewportControllBounds();
			//}

			// copy freeze info
			var frozenPos = this.FreezePos;

			if (frozenPos.Row > 0 || frozenPos.Col > 0)
			{
				newSheet.FreezeToCell(frozenPos, this.FreezeArea);
			}

			newSheet.ScaleFactor = this.ScaleFactor;

			newSheet.UpdateViewportController();

			return newSheet;
		}
	}
}
