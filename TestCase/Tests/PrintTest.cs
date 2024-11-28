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
	[TestSet(DebugEnabled=true)]
	class PrintTest : ReoGridTestSet
	{
		[TestCase]
		void NormalAutoPaging()
		{
			SetUp(40, 20);

			worksheet[10, 10] = "A";

			worksheet.AutoSplitPage();

			worksheet._Debug_Validate_PrintPageBreaks();
		}

		[TestCase]
		void SetPrintableRange()
		{
			SetUp(40, 20);

			worksheet[5, 5] = "A";

			worksheet.PrintableRange = new RangePosition(1, 1, 9, 9);

			worksheet.AutoSplitPage();

			worksheet._Debug_Validate_PrintPageBreaks();
		}

		[TestCase]
		void InsertOutsidePrintableRange()
		{
			SetUp(40, 20);

			worksheet[5, 5] = "A";

			// auto split page will initialize the printable range
			worksheet.AutoSplitPage();
			worksheet._Debug_Validate_PrintPageBreaks();

			AssertEquals(worksheet.PrintableRange.EndCol, 5);

			// changing the last column-break-index will make printable range changed also
			worksheet.ChangeColumnPageBreak(6, 7);
			
			// verify the printable range (column of print range = last page break - 1)
			AssertEquals(worksheet.PrintableRange.EndCol, 6);

			// insert after printable range will cause insert a new page
			worksheet.InsertColumnPageBreak(15);

			// pages count should be 2
			AssertEquals(worksheet.PrintPageCounts, 2);
		}

		[TestCase]
		void BoundaryTest_FullGridPrint()
		{
			SetUp(10, 6);

			worksheet["F10"] = "A";

			worksheet.PrintableRange = new RangePosition(0, 0, worksheet.RowCount, worksheet.ColumnCount);
			worksheet.AutoSplitPage();

			AssertEquals(worksheet.ColumnPageBreaks[0], 0);
			AssertEquals(worksheet.ColumnPageBreaks[1], 6);
			AssertEquals(worksheet.RowPageBreaks[0], 0);
			AssertEquals(worksheet.RowPageBreaks[1], 10);
		}

		[TestCase]
		void AddPageBreakIndex()
		{
			SetUp(10, 6);

			worksheet["F10"] = "A";
			worksheet.ColumnPageBreaks.Add(3);
			worksheet.AutoSplitPage();
	
			AssertEquals(worksheet.ColumnPageBreaks[0], 0);
			AssertEquals(worksheet.ColumnPageBreaks[1], 3);
			AssertEquals(worksheet.ColumnPageBreaks[2], 6);
			AssertEquals(worksheet.RowPageBreaks[0], 0);
			AssertEquals(worksheet.RowPageBreaks[1], 10);

			worksheet.RowPageBreaks.Add(5);

			AssertEquals(worksheet.ColumnPageBreaks[0], 0);
			AssertEquals(worksheet.ColumnPageBreaks[1], 3);
			AssertEquals(worksheet.ColumnPageBreaks[2], 6);
			AssertEquals(worksheet.RowPageBreaks[0], 0);
			AssertEquals(worksheet.RowPageBreaks[1], 5);
			AssertEquals(worksheet.RowPageBreaks[2], 10);
		}

		[TestCase]
		void ChangeColumnPageBreakIndex()
		{
			worksheet.Resize(10, 6);

			worksheet["F10"] = "A";

			worksheet.PrintableRange = new RangePosition(0, 0, worksheet.RowCount, worksheet.ColumnCount);
			worksheet.ColumnPageBreaks.Add(3);
			worksheet.AutoSplitPage();

			worksheet.ChangeColumnPageBreak(3, 4);

			AssertEquals(worksheet.ColumnPageBreaks[0], 0);
			AssertEquals(worksheet.ColumnPageBreaks[1], 4);
			AssertEquals(worksheet.ColumnPageBreaks[2], 6);
		}

		[TestCase]
		void RemovePageBreak()
		{
			worksheet.RemoveColumnPageBreak(4);

			AssertEquals(worksheet.ColumnPageBreaks[0], 0);
			AssertEquals(worksheet.ColumnPageBreaks[1], 6);
		}

		[TestCase]
		void ChangeRowPageBreakIndex()
		{
			worksheet.Resize(10, 6);

			worksheet["F10"] = "A";

			worksheet.PrintableRange = new RangePosition(0, 0, worksheet.RowCount, worksheet.ColumnCount);
			worksheet.RowPageBreaks.Add(5);
			worksheet.AutoSplitPage();

			worksheet.ChangeRowPageBreak(5, 6);

			AssertEquals(worksheet.RowPageBreaks[0], 0);
			AssertEquals(worksheet.RowPageBreaks[1], 6);
			AssertEquals(worksheet.RowPageBreaks[2], 10);
		}

		[TestCase]
		void RemoveRowPageBreak()
		{
			worksheet.RemoveRowPageBreak(6);

			AssertEquals(worksheet.RowPageBreaks[0], 0);
			AssertEquals(worksheet.RowPageBreaks[1], 10);
		}

		[TestCase]
		void DonotChangeScalingWhenPrintableRangeChanged()
		{
			SetUp(100, 6);

			// set dummy data
			worksheet["F10"] = "A";

			// do page split
			worksheet.AutoSplitPage();

			// change to 50 rows 
			worksheet.ChangeRowPageBreak(10, 90);
			// should be 2 pages
			AssertEquals(worksheet.PrintPageCounts, 3);

			// change top printable range to 40 rows
			worksheet.ChangeRowPageBreak(0, 40);
			// should be only one page
			AssertEquals(worksheet.PrintPageCounts, 2);
		}

		[TestCase]
		void PrintTest1()
		{
			SetUp(100, 10);

			worksheet["F50"] = "A";

			worksheet.PrintSettings.PrinterName = "Generic IBM Graphics 9pin";

			var ps = worksheet.CreatePrintSession();
			ps.Print();
		}

		[TestCase]
		void PrintDocument()
		{
			worksheet.Reset();

			worksheet.Load("..\\..\\..\\RGF\\printable_report.rgf");

			worksheet.CreatePrintSession().Print();
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=248
		/// </summary>
		[TestCase]
		void PrintWithGridLines()
		{
			SetUp(5, 5);

			worksheet[RangePosition.EntireRange] = new object[,] {
				{ 1, 2, 3, 4, 5 },
				{ 1, 2, 3, 4, 5 },
				{ 1, 2, 3, 4, 5 },
				{ 1, 2, 3, 4, 5 },
				{ 1, 2, 3, 4, 5 },
			};

			worksheet.PrintSettings.ShowGridLines = true;

			worksheet.CreatePrintSession().Print();
		}
	}
}
