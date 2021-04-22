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

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class FunctionTest : ReoGridTestSet
	{
		[TestCase]
		void ROW_AND_COLUMN()
		{
			SetUp(20, 20);

			worksheet["A1"] = "=ROW()";
			AssertSame(worksheet["A1"], 1);

			worksheet["A2"] = "=ROW()+5";
			AssertEquals(worksheet.GetCellData<int>("A2"), 7);

			worksheet["D1"] = "=COLUMN()";
			AssertSame(worksheet["D1"], 4);

			worksheet["D2"] = "=COLUMN()+3";
			AssertEquals(worksheet.GetCellData<int>("D2"), 7);
		}

		[TestCase]
		void ADDRESS()
		{
			worksheet["F1"] = "=ADDRESS(2, 3)";
			AssertSame(worksheet["F1"], "$C$2");
		}

		[TestCase]
		void RangeCalc()
		{
			var range = new RangePosition("A2:C5");

			worksheet[range] = new object[,] {
				{1, 2, 3},
				{4, 5, 6},
				{7, 8, 9},
			};

			// sum range equals to data range
			worksheet["E2"] = "=SUM(" + range.ToAddress() + ")";
			AssertEquals(worksheet.GetCellData<int>("E2"), 45);

			// sum range is larger than data range
			worksheet["F2"] = "=SUM(A2:C7)";
			AssertEquals(worksheet.GetCellText("F2"), "45");

			// sum result should be same
			AssertEquals(worksheet.GetCellText("E2"), worksheet.GetCellText("F2"));
		}

		[TestCase]
		void MathFunctions()
		{
			SetUp(20, 20);

			// ABS
			worksheet["A1"] = new object[] { "=ABS(-10)", "=ABS(10)", "=ABS(-1.1)", "=ABS(1.1)" };
			AssertSame(worksheet["A1"], 10, "=ABS(-10)");
			AssertSame(worksheet["B1"], 10, "=ABS(10)");
			AssertSame(worksheet["C1"], 1.1, "=ABS(-1.1)");
			AssertSame(worksheet["D1"], 1.1, "=ABS(1.1)");

			// ROUND
			worksheet["A2"] = new object[] { "=ROUND(1.1)", "=ROUND(1.5)", "=ROUND(1.01)", "=ROUND(1.71)", "=ROUND(1.02, 1)", "=ROUND(1.08, 1)" };
			AssertSame(worksheet["A2"], 1);
			AssertSame(worksheet["B2"], 2);
			AssertSame(worksheet["C2"], 1);
			AssertSame(worksheet["D2"], 2);
			AssertSame(worksheet["E2"], 1);
			AssertSame(worksheet["F2"], 1.1);

			// CEILING
			worksheet["A3"] = new object[] { "=CEILING(1.1)", "=CEILING(1.5)", "=CEILING(1.01)", "=CEILING(3.71)", "=CEILING(1.02, 5)", "=CEILING(2.35, 0.2)" };
			AssertSame(worksheet["A3"], 2);
			AssertSame(worksheet["B3"], 2);
			AssertSame(worksheet["C3"], 2);
			AssertSame(worksheet["D3"], 4);
			AssertSame(worksheet["E3"], 5);
			AssertApproximatelySame(worksheet.GetCellText("F3"), 2.4, "A3");

			// FLOOR
			worksheet["A4"] = new object[] { "=FLOOR(1.1)", "=FLOOR(1.5)", "=FLOOR(1.01)", "=FLOOR(3.71)", "=FLOOR(3.72, 1)", "=FLOOR(3.72, 0.1)" };
			AssertSame(worksheet["A4"], 1);
			AssertSame(worksheet["B4"], 1);
			AssertSame(worksheet["C4"], 1);
			AssertSame(worksheet["D4"], 3);
			AssertSame(worksheet["E4"], 3);
			AssertApproximatelySame(worksheet.GetCellText("F4"), 3.7);

			// SIN
			worksheet["A5"] = new object[] { "=ROUND(SIN(0),5)", "=ROUND(SIN(3.14/2),5)", "=ROUND(SIN(3.14/4),5)", "=ROUND(SIN(3.14),5)" };
			AssertSame(worksheet["A5"], 0);
			AssertSame(worksheet["B5"], 1);
			AssertSame(worksheet["C5"], 0.70683);
			AssertSame(worksheet["D5"], 0.00159);

			// COS
			worksheet["A6"] = new object[] { "=ROUND(COS(0),5)", "=ROUND(COS(3.14/2),5)", "=ROUND(COS(3.14/4),5)", "=ROUND(COS(3.14),5)" };
			AssertSame(worksheet["A6"], 1);
			AssertSame(worksheet["B6"], 0.0008);
			AssertSame(worksheet["C6"], 0.70739);
			AssertSame(worksheet["D6"], -1);

		}
	}
}
