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
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.Outline;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class OutlineTest : ReoGridTestSet
	{
		[TestCase]
		void RowSingleOutline()
		{
			SetUp(20, 20);

			worksheet.AddOutline(RowOrColumn.Row, 4, 2);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 1);

			var outline = outlines[0][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);
		}

		[TestCase]
		void RowSeparatedOutline()
		{
			worksheet.AddOutline(RowOrColumn.Row, 8, 2);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 2);

			var outline = outlines[0][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);

			var outline2 = outlines[0][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);
			AssertEquals(outline2.End, 10);
		}

		[TestCase]
		void RowOverlappedOutline()
		{
			worksheet.AddOutline(RowOrColumn.Row, 4, 3);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 3);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 2);

			var outline3 = outlines[0][0];

			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 3);
			AssertEquals(outline3.End, 7);

			var outline = outlines[1][0];

			AssertEquals(outline.Start, 4);
			AssertEquals(outline.Count, 2);
			AssertEquals(outline.End, 6);

			var outline2 = outlines[1][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);
			AssertEquals(outline2.End, 10);
		}

		[TestCase]
		void CollapseOne()
		{
			var outline = worksheet.CollapseOutline(RowOrColumn.Row, 4, 2);

			AssertTrue(outline != null);
			AssertEquals(worksheet.GetRowHeight(4), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);
		}
	
		[TestCase]
		void ExpandOne()
		{
			var outline = worksheet.ExpandOutline(RowOrColumn.Row, 4, 2);

			AssertTrue(outline != null);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(5), Worksheet.InitDefaultRowHeight);
		}

		[TestCase]
		void RemoveOutline()
		{
			// remove first
			worksheet.RemoveOutline(RowOrColumn.Row, 4, 3);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 2);
			AssertEquals(outlines[1].Count, 0);

			var outline3 = outlines[0][0];

			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 2);

			var outline2 = outlines[0][1];

			AssertEquals(outline2.Start, 8);
			AssertEquals(outline2.Count, 2);

			// remove second
			worksheet.RemoveOutline(RowOrColumn.Row, 8, 2);

			AssertEquals(outlines.Count, 2);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 0);

			outline3 = outlines[0][0];
			AssertEquals(outline3.Start, 4);
			AssertEquals(outline3.Count, 2);

			// remove third
			worksheet.RemoveOutline(RowOrColumn.Row, 4, 2);

			AssertEquals(outlines.Count, 1);
			AssertEquals(outlines[0].Count, 0);
		}

		[TestCase]
		void InsertMiddle()
		{
			worksheet.AddOutline(RowOrColumn.Row, 4, 2); // 6
			worksheet.AddOutline(RowOrColumn.Row, 2, 8); // 10
			worksheet.AddOutline(RowOrColumn.Row, 3, 4); // 7

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 4);
			AssertEquals(outlines[0].Count, 1);
			AssertEquals(outlines[1].Count, 1);
			AssertEquals(outlines[2].Count, 1);

			AssertEquals(outlines[0][0].Start, 2);
			AssertEquals(outlines[0][0].Count, 8);
			AssertEquals(outlines[1][0].Start, 3);
			AssertEquals(outlines[1][0].Count, 4);
			AssertEquals(outlines[2][0].Start, 4);
			AssertEquals(outlines[2][0].Count, 2);
		}

		[TestCase]
		void RemoveMiddle()
		{
			worksheet.RemoveOutline(RowOrColumn.Row, 3, 4);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines.Count, 3);
			AssertEquals(outlines[0][0].Start, 2);
			AssertEquals(outlines[0][0].Count, 8);
			AssertEquals(outlines[1][0].Start, 4);
			AssertEquals(outlines[1][0].Count, 2);
		}

		[TestCase]
		void ExpandOneLevelOutline()
		{
			IReoGridOutline inner = worksheet.CollapseOutline(RowOrColumn.Row, 4, 2);
			AssertEquals(inner.Collapsed, true);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);

			IReoGridOutline outer = worksheet.CollapseOutline(RowOrColumn.Row, 2, 8);
			AssertEquals(outer.Collapsed, true);
			AssertEquals(worksheet.GetRowHeight(3), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(7), (ushort)0);

			outer.Expand();
			AssertEquals(outer.Collapsed, false);
			AssertEquals(worksheet.GetRowHeight(3), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(8), Worksheet.InitDefaultRowHeight);

			inner.Expand();
			AssertEquals(inner.Collapsed, false);
			AssertEquals(worksheet.GetRowHeight(5), Worksheet.InitDefaultRowHeight);
		}

		[TestCase]
		void CollapseAllInGroup()
		{
			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			IReoGridOutline inner = worksheet.GetOutline(RowOrColumn.Row, 4, 2);
			inner.Expand();
			AssertEquals(inner.Collapsed, false);
			AssertEquals(worksheet.GetRowHeight(5), Worksheet.InitDefaultRowHeight);

			AssertEquals(outlines[1].Count, 1);
			worksheet.AddOutline(RowOrColumn.Row, 11, 3);
			AssertEquals(outlines[1].Count, 2);

			outlines[1].CollapseAll();
			AssertEquals(outlines[1][0].Collapsed, true);
			AssertEquals(outlines[1][1].Collapsed, true);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(12), (ushort)0);
		}

		[TestCase]
		void ExpandBySetRowHeight()
		{
			worksheet.SetRowsHeight(2, 12, Worksheet.InitDefaultRowHeight);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines[1][0].Collapsed, false);
			AssertEquals(outlines[1][1].Collapsed, false);
			AssertEquals(worksheet.GetRowHeight(5), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(12), Worksheet.InitDefaultRowHeight);
		}

		[TestCase]
		void CollapseByHideRows()
		{
			worksheet.HideRows(2, 12);

			var outlines = worksheet.GetOutlines(RowOrColumn.Row);

			AssertEquals(outlines[1][0].Collapsed, true);
			AssertEquals(outlines[1][1].Collapsed, true);
			AssertEquals(worksheet.GetRowHeight(5), (ushort)0);
			AssertEquals(worksheet.GetRowHeight(12), (ushort)0);
		}

		[TestCase]
		void ExpandEntireGroup()
		{
			var outlines = worksheet.GetOutlines(RowOrColumn.Row);
			outlines[1].ExpandAll();

			AssertEquals(outlines[1][0].Collapsed, false);
			AssertEquals(outlines[1][1].Collapsed, false);
			AssertEquals(worksheet.GetRowHeight(5), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(12), Worksheet.InitDefaultRowHeight);
		}

		[TestCase]
		void UpdateOutlineByRemoveRows()
		{
			SetUp(50, 50);

			for (int i = 0; i < 4; i++)
			{
				worksheet.AddOutline(RowOrColumn.Column, 3, i + 3);
			}

			for (int i = 0; i < 4; i++)
			{
				worksheet.AddOutline(RowOrColumn.Row, 3, i + 3);
			}
		}

		[TestCase]
		void CollapseInnerRowOutlines()
		{
			SetUp(20, 20);

			var l1 = worksheet.RowOutlines.AddOutline(3, 6); // 
			var l2 = worksheet.RowOutlines.AddOutline(3, 5); // same start
			var l3 = worksheet.RowOutlines.AddOutline(4, 4); // same end
			var l4 = worksheet.RowOutlines.AddOutline(4, 3); // same start inside
			var l5 = worksheet.RowOutlines.AddOutline(5, 2); // same start inside

			l1.Collapse();
			AssertEquals(l1.Collapsed, true, "l2");
			AssertEquals(l2.Collapsed, false, "l2");
			AssertEquals(l3.Collapsed, false, "l3");
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");

			l2.Collapse();
			AssertEquals(l2.Collapsed, true, "l2");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");

			l4.Collapse();
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l5");
		}
		
		[TestCase]
		void ExpandInnerRowOutlines()
		{
			var l1 = worksheet.RowOutlines[3, 6]; // 
			var l2 = worksheet.RowOutlines[3, 5]; // same start
			var l3 = worksheet.RowOutlines[4, 4]; // same end
			var l4 = worksheet.RowOutlines[4, 3]; // same start inside
			var l5 = worksheet.RowOutlines[5, 2]; // same start inside

			l1.Expand();
			AssertEquals(l1.Collapsed, false, "l2");
			AssertEquals(l2.Collapsed, true, "l2");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l2.Expand();
			AssertEquals(l2.Collapsed, false, "l3");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l3.Expand();
			AssertEquals(l2.Collapsed, false, "l3");
			AssertEquals(l3.Collapsed, false, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l4.Expand();
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, true, "l4");
			
			l5.Expand();
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");
		}


		[TestCase]
		void CollapseInnerColumnOutlines()
		{
			SetUp(20, 20);

			var l1 = worksheet.ColumnOutlines.AddOutline(3, 6); // 
			var l2 = worksheet.ColumnOutlines.AddOutline(3, 5); // same start
			var l3 = worksheet.ColumnOutlines.AddOutline(4, 4); // same end
			var l4 = worksheet.ColumnOutlines.AddOutline(4, 3); // same start inside
			var l5 = worksheet.ColumnOutlines.AddOutline(5, 2); // same start inside

			l1.Collapse();
			AssertEquals(l1.Collapsed, true, "l2");
			AssertEquals(l2.Collapsed, false, "l2");
			AssertEquals(l3.Collapsed, false, "l3");
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");

			l2.Collapse();
			AssertEquals(l2.Collapsed, true, "l2");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");

			l4.Collapse();
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l5");
		}

		[TestCase]
		private void ExpandInnerColumnOutlines()
		{
			var l1 = worksheet.ColumnOutlines[3, 6]; // 
			var l2 = worksheet.ColumnOutlines[3, 5]; // same start
			var l3 = worksheet.ColumnOutlines[4, 4]; // same end
			var l4 = worksheet.ColumnOutlines[4, 3]; // same start inside
			var l5 = worksheet.ColumnOutlines[5, 2]; // same start inside

			l1.Expand();
			AssertEquals(l1.Collapsed, false, "l2");
			AssertEquals(l2.Collapsed, true, "l2");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l2.Expand();
			AssertEquals(l2.Collapsed, false, "l3");
			AssertEquals(l3.Collapsed, true, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l3.Expand();
			AssertEquals(l2.Collapsed, false, "l3");
			AssertEquals(l3.Collapsed, false, "l3");
			AssertEquals(l4.Collapsed, true, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l4.Expand();
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, true, "l4");

			l5.Expand();
			AssertEquals(l4.Collapsed, false, "l4");
			AssertEquals(l5.Collapsed, false, "l4");
		}

		[TestCase]
		void FreezeAndColumnOutline()
		{
			SetUp(20, 20);

			worksheet.GroupColumns(2, 10);
			worksheet.FreezeToCell(1, 1);

			SetUp(20, 20);

			worksheet.FreezeToCell(1, 1);
			worksheet.GroupColumns(2, 10);
		}
	}

}
