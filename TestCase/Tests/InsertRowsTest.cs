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

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class InsertRowTestCases : ReoGridTestSet 
	{
		[TestCase]
		public void AppendRows()
		{
			SetUp();

			// resize to 10 columns
			int rows = worksheet.RowCount;
			// append 10 rows 
			worksheet.AppendRows(10);
			AssertEquals(worksheet.RowCount, rows + 10);
		}

		[TestCase]
		public void Append10000Rows()
		{
			SetUp();

			// resize to 10 columns
			int rows = worksheet.RowCount;
			// append 10 rows 
			worksheet.AppendRows(10000);
			AssertEquals(worksheet.RowCount, rows + 10000);
		}

		[TestCase]
		public void NormalInsert()
		{
			SetUp();

			// resize to 10 columns
			worksheet.SetRows(10);

			// insert at begin
			worksheet.InsertRows(0, 1);
			AssertEquals(worksheet.RowCount, 11);

			// insert at center
			worksheet.InsertRows(5, 1);
			AssertEquals(worksheet.RowCount, 12);

			// insert at last
			worksheet.InsertRows(12, 1);
			AssertEquals(worksheet.RowCount, 13);
		
			// insert at last
			worksheet.InsertRows(1, 1);
			AssertEquals(worksheet.RowCount, 14);
		}

		[TestCase]
		public void NormalInserts()
		{
			SetUp();

			// resize to 10 columns
			worksheet.SetRows(10);

			// insert at begin
			worksheet.InsertRows(0, 3);
			AssertEquals(worksheet.RowCount, 13);

			// insert at center
			worksheet.InsertRows(5, 3);
			AssertEquals(worksheet.RowCount, 16);

			// insert at last
			worksheet.InsertRows(16, 3);
			AssertEquals(worksheet.RowCount, 19);
		
			// insert before modified rows
			worksheet.InsertRows(1, 3);
			AssertEquals(worksheet.RowCount, 22);
		}

		/// <summary>
		///  O
		/// OO
		/// O
		/// </summary>
		[TestCase]
		public void InsertRow1()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(4, 6, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(2, 7, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());

			worksheet.InsertRows(3, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O
		/// OO
		///  O
		/// </summary>
		[TestCase]
		public void InsertRow2()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 6, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(4, 7, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());

			worksheet.InsertRows(3, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O O
		/// OOO
		///  O
		/// </summary>
		[TestCase]
		public void InsertRow3()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 5, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(4, 6, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(2, 7, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		
			worksheet.InsertRows(3, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		///  O
		/// OOO
		/// O O
		/// </summary>
		[TestCase]
		public void InsertRow4()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(4, 5, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(2, 6, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(4, 7, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());

			worksheet.InsertRows(3, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// O O
		/// OOO
		/// </summary>
		[TestCase]
		public void InsertRow5()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 5, 5, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(4, 6, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(2, 7, 5, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());

			worksheet.InsertRows(3, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// OOO
		///  O
		/// </summary>
		[TestCase]
		public void InsertRow11()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 3), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.SetRangeBorders(new RangePosition(5, 3, 3, 1), BorderPositions.All, new RangeBorderStyle
			{
				Color = Color.Black,
				Style = BorderLineStyle.Solid
			});

			worksheet.InsertRows(5, 1);
			AssertTrue(worksheet._Debug_Validate_BorderSpan());
		}

		/// <summary>
		/// Insert Row in Merged Cell
		/// </summary>
		[TestCase]
		public void InsertRowInMergedCell()
		{
			SetUp();
			worksheet.MergeRange(new RangePosition(2, 2, 3, 3));
			for (int i = 5; i > 1; i--)
			{
				worksheet.InsertRows(i, 1);
				AssertTrue(worksheet._Debug_Validate_MergedCells());
			}
		}

		/// <summary>
		/// Insert Row in Merged Cell (Multi-Cells)
		/// </summary>
		[TestCase]
		public void InsertRowInMergedCell2()
		{
			SetUp();

			worksheet.MergeRange(new RangePosition(2, 2, 4, 3));
			worksheet.MergeRange(new RangePosition(1, 5, 3, 3));
			worksheet.MergeRange(new RangePosition(4, 5, 3, 3));
			worksheet.MergeRange(new RangePosition(2, 8, 3, 3));

			for (int i = 0; i > 0; i--)
			{
				worksheet.InsertRows(i, 1);
				AssertTrue(worksheet._Debug_Validate_MergedCells());
			}
		}

	
	}
}
