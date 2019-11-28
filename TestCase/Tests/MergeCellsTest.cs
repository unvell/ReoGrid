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

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class MergeCellsTest : ReoGridTestSet
	{
		[TestCase]
		void NormalMerge()
		{
			SetUp();
		
			worksheet.MergeRange(new RangePosition(2, 2, 3, 5));
			ValidateMergedCells();
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		void MergeByScript()
		{
			SetUp();

			Grid.RunScript("workbook.currentWorksheet.mergeRange(new Range(2,2,3,5));");
			ValidateMergedCells();
			AssertTrue(worksheet._Debug_Validate_All());
		}

		private void ValidateMergedCells()
		{
			AssertEquals(worksheet.IsMergedCell(2, 1), false);
			AssertEquals(worksheet.IsMergedCell(1, 2), false);
			AssertEquals(worksheet.IsMergedCell(2, 2), true);
			AssertEquals(worksheet.IsMergedCell(2, 3), false);
			AssertEquals(worksheet.IsMergedCell(3, 2), false);
			AssertEquals(worksheet.IsMergedCell(5, 7), false);

			AssertEquals(worksheet.IsValidCell(2, 1), true);
			AssertEquals(worksheet.IsValidCell(1, 2), true);
			AssertEquals(worksheet.IsValidCell(2, 2), true);
			AssertEquals(worksheet.IsValidCell(2, 3), false);
			AssertEquals(worksheet.IsValidCell(3, 2), false);
			AssertEquals(worksheet.IsValidCell(4, 6), false);
		}

		[TestCase]
		public void NormalUnmerge()
		{
			SetUp();

			worksheet.MergeRange(new RangePosition(2, 2, 3, 5));
			worksheet.UnmergeRange(new RangePosition(2, 2, 3, 5));
			ValidateUnmergedCells();
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void UnmergeByScript()
		{
			SetUp();

			Grid.RunScript("workbook.currentWorksheet.mergeRange(new Range(2,2,3,5));");
			Grid.RunScript("workbook.currentWorksheet.unmergeRange(new Range(2,2,3,5));");
			ValidateUnmergedCells();
			AssertTrue(worksheet._Debug_Validate_All());
		}

		private void ValidateUnmergedCells()
		{
			AssertEquals(worksheet.IsMergedCell(2, 1), false);
			AssertEquals(worksheet.IsMergedCell(1, 2), false);
			AssertEquals(worksheet.IsMergedCell(2, 2), false);
			AssertEquals(worksheet.IsMergedCell(2, 3), false);
			AssertEquals(worksheet.IsMergedCell(3, 2), false);
			AssertEquals(worksheet.IsMergedCell(5, 7), false);

			AssertEquals(worksheet.IsValidCell(2, 1), true);
			AssertEquals(worksheet.IsValidCell(1, 2), true);
			AssertEquals(worksheet.IsValidCell(2, 2), true);
			AssertEquals(worksheet.IsValidCell(2, 3), true);
			AssertEquals(worksheet.IsValidCell(3, 2), true);
			AssertEquals(worksheet.IsValidCell(4, 6), true);
		}

		[TestCase]
		public void TestIntersectedMerge()
		{
		}

		[TestCase]
		void RandomlyMerged()
		{
			SetUp(20, 20);

			worksheet.SetRowsHeight(0, 10, 10);
			worksheet.SetColumnsWidth(0, 10, 10);

			var rand = new Random();
			for (int i = 0; i < 20; )
			{
				int row = rand.Next(16);
				int col = rand.Next(16);
				int rows = 2 + rand.Next(2);
				int cols = 2 + rand.Next(2);

				var range = new RangePosition(row, col, rows, cols);
				if (worksheet.HasIntersectedMergingRange(range))
					continue;
				else 
					i++;

				worksheet.MergeRange(range);
				worksheet._Debug_Validate_All();
			}
		}

		/// <summary>
		/// Styles setting error in UnmergeRange method.
		/// Reported by Kevin 2015/7/11.
		/// </summary>
		[TestCase]
		void StyleTestAfterUnmerge()
		{
			SetUp(10, 10);

			var noneColor = new Graphics.SolidColor(0, 0, 0, 0);

			worksheet.Resize(10, 10);
			worksheet.ColumnHeaders[0].Style.BackColor = Graphics.SolidColor.Silver;

			AssertEquals(worksheet.Cells["A9"].Style.BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.Cells["B9"].Style.BackColor, noneColor);
			AssertEquals(worksheet.Cells["A10"].Style.BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.Cells["B10"].Style.BackColor, noneColor);
			AssertEquals(worksheet.Cells["G5"].Style.BackColor, noneColor);

			worksheet.MergeRange(8, 0, 2, 2);

			AssertEquals(worksheet.Cells["A9"].Style.BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.GetCellStyles("B9").BackColor, noneColor);
			AssertEquals(worksheet.GetCellStyles("A10").BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.GetCellStyles("B10").BackColor, noneColor);
			AssertEquals(worksheet.Cells["G5"].Style.BackColor, noneColor);
			
			worksheet.UnmergeRange(8, 0, 2, 2);

			AssertEquals(worksheet.Cells["B9"].Style.BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.Cells["C9"].Style.BackColor, noneColor);
			AssertEquals(worksheet.Cells["B10"].Style.BackColor, Graphics.SolidColor.Silver);
			AssertEquals(worksheet.Cells["C10"].Style.BackColor, noneColor);
			AssertEquals(worksheet.Cells["G5"].Style.BackColor, noneColor);
		}

		/// <summary>
		/// Issue #285 https://github.com/unvell/ReoGrid/issues/285
		/// </summary>
		[TestCase]
		void GetMergedCellOfRangeTest()
		{
			SetUp(20, 20);

			worksheet.MergeRange("C3:H10");

			AssertEquals(worksheet.GetMergedCellOfRange("C3").Address, "C3");
			AssertEquals(worksheet.GetMergedCellOfRange("C4").Address, "C3");
			AssertEquals(worksheet.GetMergedCellOfRange("D3").Address, "C3");
			AssertEquals(worksheet.GetMergedCellOfRange("G9").Address, "C3");
			AssertEquals(worksheet.GetMergedCellOfRange("H9").Address, "C3");
			AssertEquals(worksheet.GetMergedCellOfRange("G10").Address, "C3");

			AssertEquals(worksheet.GetMergedCellOfRange("A1").Address, "A1");
			AssertEquals(worksheet.GetMergedCellOfRange("J12").Address, "J12");

			AssertFalse(worksheet.Cells["A1"].IsMergedCell);
			AssertFalse(worksheet.Cells["A1"].InsideMergedRange);
			AssertFalse(worksheet.Cells["J12"].IsMergedCell);
			AssertFalse(worksheet.Cells["J12"].InsideMergedRange);

			AssertTrue(worksheet.Cells["C3"].IsMergedCell);
			AssertTrue(worksheet.Cells["C3"].InsideMergedRange);
			AssertFalse(worksheet.Cells["D4"].IsMergedCell);
			AssertTrue(worksheet.Cells["D4"].InsideMergedRange);
			AssertFalse(worksheet.Cells["H9"].IsMergedCell);
			AssertTrue(worksheet.Cells["H9"].InsideMergedRange);
			AssertFalse(worksheet.Cells["G10"].IsMergedCell);
			AssertTrue(worksheet.Cells["G10"].InsideMergedRange);

			AssertTrue(worksheet.IsMergedCell("C3"));
			AssertFalse(worksheet.IsMergedCell("C4"));
		}
	}
}
