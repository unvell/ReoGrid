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
using System.Windows.Forms;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class PartialGridTest : ReoGridTestSet
	{
		[TestCase]
		public void CopyData()
		{
			var objs = new object[,]{
				{1,2,3},
				{5,6,7},
				{"a","b","c"},
				{'a','b','c'},
			};

			worksheet[0, 0] = objs;

			PartialGrid subgrid = worksheet.GetPartialGrid(new RangePosition(0, 0, 4, 3));

			AssertEquals(subgrid.Cells[0, 0].Data, 1);
			AssertEquals(subgrid.Cells[0, 2].Data, 3);
			AssertEquals(subgrid.Cells[3, 0].Data, 'a');
			AssertEquals(subgrid.Cells[3, 2].Data, 'c');
		}

		[TestCase]
		public void DuplicateData()
		{
			SetUp();

			var objs = new object[,]{
				{1,2,3},
				{5,6,7},
				{"a","b","c"},
				{'a','b','c'},
			};

			worksheet[0, 0] = objs;

			var range = new RangePosition(0, 0, 4, 3);
			PartialGrid subgrid = worksheet.GetPartialGrid(range);

			worksheet[0, 5] = subgrid;

			for (int r = range.Row; r < range.EndRow; r++)
			{
				for (int c = range.Col; c <= range.EndCol; c++)
				{
					AssertEquals(worksheet.GetCellText(r, c), worksheet.GetCellText(r, c + 5));
				}
			}
		}

		[TestCase]
		public void DuplicateBorder()
		{
			SetUp();

			var range = new RangePosition(4, 4, 4, 4);
			worksheet.SetRangeBorders(range, BorderPositions.All, RangeBorderStyle.BlackSolid);

			var subgrid = worksheet.GetPartialGrid(range);

			worksheet[0, 4] = subgrid; // top
			worksheet[4, 0] = subgrid; // left
			worksheet[8, 4] = subgrid; // bottom
			worksheet[4, 8] = subgrid; // right
			AssertTrue(worksheet._Debug_Validate_All());

			// top
			RangeBorderInfoSet border = worksheet.GetRangeBorders(new RangePosition(0, 4, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, BorderPositions.None); // border at all positions are same

			// left
			border = worksheet.GetRangeBorders(new RangePosition(4, 0, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, BorderPositions.None); // border at all positions are same

			// bottom
			border = worksheet.GetRangeBorders(new RangePosition(8, 4, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, BorderPositions.None); // border at all positions are same

			// right
			border = worksheet.GetRangeBorders(new RangePosition(4, 8, 4, 4));
			AssertEquals(border.Top.Style, BorderLineStyle.Solid);
			AssertEquals(border.Left.Style, BorderLineStyle.Solid);
			AssertEquals(border.Right.Style, BorderLineStyle.Solid);
			AssertEquals(border.Bottom.Style, BorderLineStyle.Solid);
			AssertEquals(border.NonUniformPos, BorderPositions.None); // border at all positions are same

		}

		[TestCase]
		public void DuplicateMergedCell()
		{
			SetUp();

			var range = new RangePosition(4, 4, 4, 4);
			worksheet.MergeRange(range);

			var subgrid = worksheet.GetPartialGrid(range);

			worksheet[0, 4] = subgrid; // top
			AssertTrue(worksheet._Debug_Validate_All());

			worksheet[4, 0] = subgrid; // left
			AssertTrue(worksheet._Debug_Validate_All());

			worksheet[8, 4] = subgrid; // bottom
			AssertTrue(worksheet._Debug_Validate_All());

			worksheet[4, 8] = subgrid; // right
			AssertTrue(worksheet._Debug_Validate_All());

			AssertTrue(worksheet.IsMergedCell(0, 4));
			AssertEquals(worksheet.GetCell(0, 4).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(0, 4).GetRowspan(), (short)4);

			AssertTrue(worksheet.IsMergedCell(4, 0));
			AssertEquals(worksheet.GetCell(4, 0).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(4, 0).GetRowspan(), (short)4);

			AssertTrue(worksheet.IsMergedCell(8, 4));
			AssertEquals(worksheet.GetCell(8, 4).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(8, 4).GetRowspan(), (short)4);

			AssertTrue(worksheet.IsMergedCell(4, 8));
			AssertEquals(worksheet.GetCell(4, 8).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(4, 8).GetRowspan(), (short)4);
		}

		[TestCase]
		public void OverrideBorder()
		{
			SetUp();

			var range = new RangePosition(0, 0, 12, 12);
			worksheet.SetRangeBorders(range, BorderPositions.All, RangeBorderStyle.BlackSolid);

			var subgrid = new PartialGrid(4, 4);

			worksheet[0, 0] = subgrid; // left-top
			worksheet[8, 0] = subgrid; // left-bottom
			worksheet[0, 8] = subgrid; // right-top
			worksheet[8, 8] = subgrid; // right-bottom

			AssertTrue(worksheet._Debug_Validate_All());

			// left-top
			var borderInfo = worksheet.GetRangeBorders(new RangePosition(0, 0, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// left-bottom
			borderInfo = worksheet.GetRangeBorders(new RangePosition(8, 0, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// right-top
			borderInfo = worksheet.GetRangeBorders(new RangePosition(0, 8, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
			// right-bottom
			borderInfo = worksheet.GetRangeBorders(new RangePosition(8, 8, 4, 4));
			AssertEquals(borderInfo.Top.Color, Color.Empty);
		}

		[TestCase]
		public void CopyBorderWithoutRightSide()
		{
			SetUp(20, 20);

			worksheet.SetRangeBorders(new RangePosition(2, 4, 2, 4), BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			var pg = worksheet.GetPartialGrid(new RangePosition(2, 5, 2, 3));
			worksheet[2, 6] = pg;

			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void SetPartialMergedRangeHorizontal()
		{
			SetUp(20, 20);

			// before:
			// +------------+----+
			// |            |    |
			// +------------+----+
			//
			// after:
			// +-----------------+
			// |                 |
			// +-----------------+

			// test case double row and single row

			PartialGrid pg = null;

			// double right
			worksheet.MergeRange(2, 2, 2, 4);
			pg = worksheet.GetPartialGrid(2, 4, 2, 2);
			worksheet[2, 4] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(2, 2).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(2, 2).GetRowspan(), (short)2);

			// single right
			worksheet.MergeRange(6, 2, 1, 4);
			pg = worksheet.GetPartialGrid(6, 4, 1, 2);
			worksheet[6, 4] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(6, 2).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(6, 2).GetRowspan(), (short)1);

			// double left
			worksheet.MergeRange(8, 2, 2, 4);
			pg = worksheet.GetPartialGrid(8, 2, 2, 2);
			worksheet[8, 2] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(8, 2).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(8, 2).GetRowspan(), (short)2);

			// single left
			worksheet.MergeRange(12, 2, 1, 4);
			pg = worksheet.GetPartialGrid(12, 2, 1, 2);
			worksheet[12, 2] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(12, 2).GetColspan(), (short)4);
			AssertEquals(worksheet.GetCell(12, 2).GetRowspan(), (short)1);
		}

		[TestCase]
		public void SetPartialMergedRangeVertial()
		{
			SetUp(20, 20);

			PartialGrid pg = null;

			// double bottom
			worksheet.MergeRange(2, 2, 4, 2);
			pg = worksheet.GetPartialGrid(4, 2, 2, 2);
			worksheet[4, 2] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(2, 2).GetColspan(), (short)2);
			AssertEquals(worksheet.GetCell(2, 2).GetRowspan(), (short)4);

			// single bottom
			worksheet.MergeRange(2, 6, 4, 1);
			pg = worksheet.GetPartialGrid(4, 6, 2, 1);
			worksheet[4, 6] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(2, 6).GetColspan(), (short)1);
			AssertEquals(worksheet.GetCell(2, 6).GetRowspan(), (short)4);

			// double top
			worksheet.MergeRange(2, 8, 4, 2);
			pg = worksheet.GetPartialGrid(2, 8, 2, 2);
			worksheet[2, 8] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(2, 8).GetColspan(), (short)2);
			AssertEquals(worksheet.GetCell(2, 8).GetRowspan(), (short)4);

			// single top
			worksheet.MergeRange(2, 12, 4, 1);
			pg = worksheet.GetPartialGrid(2, 12, 2, 1);
			worksheet[2, 12] = pg;
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.GetCell(2, 12).GetColspan(), (short)1);
			AssertEquals(worksheet.GetCell(2, 12).GetRowspan(), (short)4);

		}

		[TestCase]
		void MoveMergedRange()
		{
			SetUp(10, 10);

			worksheet.Ranges[2, 2, 2, 2].Merge();
			worksheet.MoveRange(new RangePosition(2, 2, 2, 2), new RangePosition(5, 5, 2, 2));
			
			worksheet._Debug_Validate_MergedCells();
		}

		/// <summary>
		/// Moving a cell that cuases other cells losing styles,
		/// When the cell is unmerged from a range that has styles.
		/// </summary>
		[TestCase]
		void MoveUnmergedRange()
		{
			SetUp(10, 10);

			worksheet.Cells[2, 2].Style.BackColor = Graphics.SolidColor.Silver;

			worksheet.MergeRange(2, 2, 5, 5);

			worksheet.UnmergeRange(2, 2, 5, 5);

			AssertEquals(worksheet.Cells[2, 2].Style.BackColor, Color.Silver);
			AssertEquals(worksheet.GetCellStyles(8, 2).BackColor, new Graphics.SolidColor(0, 0, 0, 0));

			worksheet.MoveRange(new RangePosition(5, 5, 1, 1), new RangePosition(8, 2,1,1));

		}

		[TestCase]
		void CheckStylesAfterMoveRange()
		{
			SetUp(10, 10);

			var styles = worksheet.GetCellStyles("A1");

			worksheet["A1"] = "Hello";
			worksheet.MoveRange("A1", "B1");

			// check from cell
			AssertSame(worksheet.Cells["A1"].Style.TextColor, styles.TextColor);
			AssertSame(worksheet.Cells["A1"].Style.BackColor, styles.BackColor);
			AssertSame(worksheet.Cells["A1"].Style.FontName, styles.FontName);
			AssertSame(worksheet.Cells["A1"].Style.FontSize, styles.FontSize);
			AssertSame(worksheet.Cells["A1"].Style.Bold, styles.Bold);

			// check from cell
			AssertSame(worksheet.Cells["B1"].Style.TextColor, styles.TextColor);
			AssertSame(worksheet.Cells["B1"].Style.BackColor, styles.BackColor);
			AssertSame(worksheet.Cells["B1"].Style.FontName, styles.FontName);
			AssertSame(worksheet.Cells["B1"].Style.FontSize, styles.FontSize);
			AssertSame(worksheet.Cells["B1"].Style.Bold, styles.Bold);
		}

		//[TestCase]
		//void TryChangeReadonlyCells()
		//{
		//}

		[TestCase]
		void UndoDeletedMergedCell()
		{
			SetUp(10, 10);

			// 0 1 2 3 4 5 6
			// A B C D E F G
			worksheet.MergeRange("B3:D3");
			worksheet.MergeRange("D5:F5");
			worksheet.MergeRange("C7:E7");

			// remove D column
			Grid.DoAction(new Actions.RemoveColumnsAction(3, 1));

			Cell cell;

			cell = worksheet.Cells["B3"];
			AssertSame(cell.GetColspan(), 2);

			cell = worksheet.Cells["D5"];
			AssertSame(cell.GetColspan(), 2);

			cell = worksheet.Cells["C7"];
			AssertSame(cell.GetColspan(), 2);

			AssertTrue(worksheet._Debug_Validate_All());

			Grid.Undo();

			cell = worksheet.Cells["B3"];
			AssertSame(cell.GetColspan(), 3);

			cell = worksheet.Cells["D5"];
			AssertSame(cell.GetColspan(), 3);

			cell = worksheet.Cells["C7"];
			AssertSame(cell.GetColspan(), 3);

			AssertTrue(worksheet._Debug_Validate_All());
		}
	}

}
