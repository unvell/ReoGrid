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

#pragma warning disable 612, 618

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using unvell.ReoGrid;
using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class InsertColTestCases : ReoGridTestSet 
	{
		[TestCase]
		public void AppendCols()
		{
			SetUp();

			// resize to 10 columns
			int cols = worksheet.ColumnCount;
			// append 10 rows 
			worksheet.AppendColumns(10);
			AssertEquals(worksheet.ColumnCount, cols + 10);
		}

		[TestCase]
		public void Append10000Cols()
		{
			SetUp();

			// resize to 10 columns
			int cols = worksheet.ColumnCount;
			// append 10 rows 
			worksheet.AppendColumns(10000);
			AssertEquals(worksheet.ColumnCount, cols + 10000);
		}

		[TestCase]
		public void NormalInsert()
		{
			SetUp();

			// resize to 10 columns
			worksheet.SetCols(10);

			// insert at begin
			worksheet.InsertColumns(0, 1);
			AssertEquals(worksheet.ColumnCount, 11);

			// insert at center
			worksheet.InsertColumns(5, 1);
			AssertEquals(worksheet.ColumnCount, 12);

			// insert at last
			worksheet.InsertColumns(12, 1);
			AssertEquals(worksheet.ColumnCount, 13);
		}

		/// <summary>
		/// Insert Col in Merged Cell
		/// </summary>
		[TestCase]
		public void InsertColInMergedCell()
		{
			SetUp();

			worksheet.MergeRange(new RangePosition(2, 2, 3, 3));
			for (int i = 5; i > 1; i--)
			{
				worksheet.InsertColumns(i, 1);
				AssertTrue(worksheet._Debug_Validate_MergedCells());
				AssertTrue(worksheet._Debug_Validate_Unmerged_Range(RangePosition.EntireRange));
			}
		}

		/// <summary>
		/// Insert Col in Merged Cell (Multi-Cells)
		/// </summary>
		[TestCase]
		public void InsertColInMergedCell2()
		{
			SetUp();

			worksheet.MergeRange(new RangePosition(2, 3, 3, 3));
			worksheet.MergeRange(new RangePosition(5, 1, 3, 3));
			worksheet.MergeRange(new RangePosition(5, 4, 3, 3));
			worksheet.MergeRange(new RangePosition(8, 2, 3, 4));

			for (int i = 7; i > 4; i--)
			{
				worksheet.InsertColumns(i, 1);
				AssertTrue(worksheet._Debug_Validate_MergedCells());
				AssertTrue(worksheet._Debug_Validate_Unmerged_Range(RangePosition.EntireRange));
			}
		}

		[TestCase]
		public void BorderSpan()
		{
			SetUp();

			var range = new RangePosition(2, 2, 2, 5);
			worksheet.MergeRange(range);
			worksheet.SetRangeBorders(range, BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			worksheet.InsertColumns(4, 2);

			AssertTrue(worksheet._Debug_Validate_All());
		}

	}
}
