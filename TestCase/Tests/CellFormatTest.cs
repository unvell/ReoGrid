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
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Formula;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class CellFormatTest : ReoGridTestSet
	{
		[TestCase]
		void DatetimeFormat()
		{
			SetUp();

			sheet["A1"] = new DateTime(2019, 5, 13);
			sheet.SetRangeDataFormat("A1", DataFormat.CellDataFormatFlag.DateTime, new DateTimeDataFormatter.DateTimeFormatArgs
			{
				CultureName = "en-US",
				Format = "yyyy/MM/dd",
			});
			AssertEquals(sheet.Cells["A1"].DisplayText, "2019/05/13", "A1");

			sheet["A2"] = new DateTime(2019, 5, 13);
			sheet.SetRangeDataFormat("A2", DataFormat.CellDataFormatFlag.DateTime, new DateTimeDataFormatter.DateTimeFormatArgs
			{
				CultureName = "en-US",
				Format = "yyyy/M/dd",
			});
			AssertEquals(sheet.Cells["A2"].DisplayText, "2019/5/13", "A2");

			sheet["A3"] = new DateTime(2019, 5, 13, 16, 32, 53);
			sheet.SetRangeDataFormat("A3", DataFormat.CellDataFormatFlag.DateTime, new DateTimeDataFormatter.DateTimeFormatArgs
			{
				CultureName = "en-US",
				Format = "mm:ss",
			});
			AssertEquals(sheet.Cells["A3"].DisplayText, "32:53", "A3");
		}

		[TestCase]
		void NumberFormat()
		{
			sheet["B1"] = 1234.5678;
			sheet.SetRangeDataFormat("B1", DataFormat.CellDataFormatFlag.Number, new NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 2,
				UseSeparator = true,
			});
			AssertEquals(sheet.Cells["B1"].DisplayText, "1,234.57", "B1");
		}

		[TestCase]
		void CurrencyFormat()
		{
			sheet["C1"] = 1234.5678;
			sheet.SetRangeDataFormat("C1", DataFormat.CellDataFormatFlag.Currency, new CurrencyDataFormatter.CurrencyFormatArgs
			{
				PrefixSymbol = "$",
				DecimalPlaces = 2,
				UseSeparator = true,
			});
			AssertEquals(sheet.Cells["C1"].DisplayText, "$1,234.57", "C1");
		}

		[TestCase]
		void PercentFormat()
		{
			sheet["D1"] = 0.25;
			sheet.SetRangeDataFormat("D1", DataFormat.CellDataFormatFlag.Percent, new NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 0,
				UseSeparator = false,
			});
			AssertEquals(sheet.Cells["D1"].DisplayText, "25%", "D1");

			sheet["D2"] = 0.2567;
			sheet.SetRangeDataFormat("D2", DataFormat.CellDataFormatFlag.Percent, new NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 0,
				UseSeparator = false,
			});
			AssertEquals(sheet.Cells["D2"].DisplayText, "26%", "D2");
		}
	}
}
