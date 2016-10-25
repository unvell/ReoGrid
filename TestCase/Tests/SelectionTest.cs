/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Source code in test-case project released under BSD license.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class SelectionTest : ReoGridTestSet
	{
		[TestCase]
		void SelectMergedCells()
		{
			// merge test dummy cells
			worksheet.MergeRange("B2:D5");
			worksheet.Ranges["E3:F4"].Merge();

			// select single cell cause to select entire merged cell
			worksheet.SelectionRange = new RangePosition("B2");
			AssertEquals(worksheet.SelectionRange, new RangePosition("B2:D5"));
			worksheet.SelectionRange = new RangePosition("A1");	// reset selection

			// select single cell cause to select entire merged cell
			worksheet.SelectionRange = new RangePosition("D5");
			AssertEquals(worksheet.SelectionRange, new RangePosition("B2:D5"));
			worksheet.SelectionRange = new RangePosition("A1");	// reset selection
			
			// select first merged cell
			worksheet.SelectionRange = new RangePosition("B2:D5");
			AssertEquals(worksheet.SelectionRange, new RangePosition("B2:D5"));
			worksheet.SelectionRange = new RangePosition("A1");	// reset selection

			// select two cells cause to select entire two merged cells
			worksheet.SelectionRange = new RangePosition("D3:E3");
			AssertEquals(worksheet.SelectionRange, new RangePosition("B2:F5"));
			worksheet.SelectionRange = new RangePosition("A1");	// reset selection

			// select two merged cells
			worksheet.SelectionRange = new RangePosition("B2:F5");
			AssertEquals(worksheet.SelectionRange, new RangePosition("B2:F5"));
			worksheet.SelectionRange = new RangePosition("A1");	// reset selection
		}

		[TestCase]
		void KeyboardMove()
		{
			SetUp(20, 20);

			worksheet.SelectionRange = new RangePosition("D5");

			worksheet.MoveSelectionUp();
			AssertEquals(worksheet.SelectionRange, new RangePosition("D4"));

			worksheet.MoveSelectionLeft();
			AssertEquals(worksheet.SelectionRange, new RangePosition("C4"));

			worksheet.MoveSelectionDown();
			AssertEquals(worksheet.SelectionRange, new RangePosition("C5"));

			worksheet.MoveSelectionRight();
			AssertEquals(worksheet.SelectionRange, new RangePosition("D5"));
		}

		[TestCase]
		void KeyboardMoveCrossMergedCell()
		{
			// init the selection range to center
			worksheet.SelectionRange = new RangePosition("D5");

			// merge a range that contains the selection will change it fill to select entire merged cell
			worksheet.MergeRange("C4:E6");
			AssertEquals(worksheet.SelectionRange, new RangePosition("C4:E6"));

			// move-up jumps the merged cell
			worksheet.MoveSelectionUp();
			AssertEquals(worksheet.SelectionRange, new RangePosition("D3"));

			// move-down force to select entire merged cell
			worksheet.MoveSelectionDown();
			AssertEquals(worksheet.SelectionRange, new RangePosition("C4:E6"));

			// move-down jumps the merged cell
			worksheet.MoveSelectionDown();
			AssertEquals(worksheet.SelectionRange, new RangePosition("D7"));

			// change select into a merged cell it will be forced to select entire merged cell
			worksheet.SelectionRange = new RangePosition("D5");
			AssertEquals(worksheet.SelectionRange, new RangePosition("C4:E6"));

			// move-left jumps the merged cell, cell at bottom of range since last focus cell is bottom
			worksheet.MoveSelectionLeft();
			AssertEquals(worksheet.SelectionRange, new RangePosition("B5"));

			// move-right force to select entire merged cell
			worksheet.MoveSelectionRight();
			AssertEquals(worksheet.SelectionRange, new RangePosition("C4:E6"));

			// move-right jumps the merged cell
			worksheet.MoveSelectionRight();
			AssertEquals(worksheet.SelectionRange, new RangePosition("F5"));
		}

		[TestCase]
		void MoveDownSkipHiddenCells()
		{
			SetUp(20, 20);

			worksheet.SelectionRange = new RangePosition(4, 0, 1, 1);
			worksheet.RowHeaders[5].IsVisible = false;

			// move down should skip row 5 (A6)
			worksheet.MoveSelectionDown();
			AssertEquals(worksheet.SelectionRange, new RangePosition(6, 0, 1, 1));

			worksheet.SelectionRange = new RangePosition(18, 0, 1, 1);
			// last row is hidden
			worksheet.RowHeaders[19].IsVisible = false;
			// move should be abort, row 18 still be selected
			worksheet.MoveSelectionDown();

			AssertEquals(worksheet.SelectionRange, new RangePosition(18, 0, 1, 1));

			worksheet.MoveSelectionUp();
			AssertEquals(worksheet.SelectionRange, new RangePosition(17, 0, 1, 1));
		}

		[TestCase]
		void FocusCellInMergedCell()
		{
			SetUp(20, 20);

			worksheet.MergeRange("B2:D4");

			worksheet.SelectionRange = new RangePosition("B2:D4");
			AssertEquals(worksheet.FocusPos, new CellPosition("B2"));

			worksheet.FocusPos = new CellPosition("A1");

			worksheet.SelectionRange = new RangePosition("B1:D4");
			AssertEquals(worksheet.FocusPos, new CellPosition("B1"));

			// attempt to select B3:D4 but B3 is a part of merged cell of B2:D4,
			// change the selection to B2:D4 and focus pos to B2
			worksheet.SelectionRange = new RangePosition("B3:D4");
			AssertEquals(worksheet.FocusPos, new CellPosition("B2"));

			// merge range A1:D4 force change the focus pos to A1
			worksheet.MergeRange("A1:D4");
			AssertEquals(worksheet.FocusPos, new CellPosition("A1"));
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=259
		/// </summary>
		[TestCase]
		void PageDownPageUp()
		{
			SetUp(20, 10);

			worksheet.MergeRange(15, 0, 5, 3);
			worksheet.MoveSelectionPageDown();
			worksheet.MoveSelectionPageDown();

			worksheet.MergeRange(0, 0, 5, 3);
			worksheet.MoveSelectionPageUp();
			worksheet.MoveSelectionPageUp();
		}
	}
}
