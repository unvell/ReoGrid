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

using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Tests
{

	[TestSet]
	class ExcelOutTest : ReoGridTestSet
	{
		static string GetExcelFileName(string name) => $"..\\..\\..\\xlsx\\{name}.xlsx";

		[TestCase]
		void OutputNumberFormat()
		{
			SetUp();

			worksheet["A1"] = new double[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };

			worksheet.SetRangeDataFormat("A1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 0 });
			worksheet.SetRangeDataFormat("B1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 1 });
			worksheet.SetRangeDataFormat("C1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 3 });

			worksheet.SetRangeDataFormat("D1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 0 });
			worksheet.SetRangeDataFormat("E1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 1 });
			worksheet.SetRangeDataFormat("F1", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 3 });

			worksheet.SetRangeDataFormat("G1", CellDataFormatFlag.Number, null);

			worksheet["A2"] = new double[] { -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890, -12345.67890 };

			worksheet.SetRangeDataFormat("A2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 0 });
			worksheet.SetRangeDataFormat("B2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 1 });
			worksheet.SetRangeDataFormat("C2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = false, DecimalPlaces = 3 });

			worksheet.SetRangeDataFormat("D2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 0 });
			worksheet.SetRangeDataFormat("E2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 1 });
			worksheet.SetRangeDataFormat("F2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 3 });

			worksheet.SetRangeDataFormat("G2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 0, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Red });
			worksheet.SetRangeDataFormat("H2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 1, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Brackets });
			worksheet.SetRangeDataFormat("I2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 3, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets });

			worksheet.SetRangeDataFormat("J2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 0, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedMinus });
			worksheet.SetRangeDataFormat("K2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 1, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.BracketsMinus });
			worksheet.SetRangeDataFormat("L2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 3, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBracketsMinus });

			worksheet.SetRangeDataFormat("M2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 0, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku });
			worksheet.SetRangeDataFormat("N2", CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs { UseSeparator = true, DecimalPlaces = 3, NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku });

			Grid.Save(GetExcelFileName("OUT01"));
		}

		[TestCase]
		void OutputDateTimeFormat()
		{
			worksheet["A3"] = new object[] { new DateTime(2001, 1, 2, 3, 4, 5), new DateTime(2002, 6, 7, 8, 9, 11),
				new DateTime(2003, 5, 6, 7, 8, 9), new DateTime(1999, 10, 25, 1, 2, 3)};

			worksheet.SetRangeDataFormat("A3", CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs { Format = "yyyy-MM-dd" });
			worksheet.SetRangeDataFormat("B3", CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs { Format = "MM-dd-yyyy" });
			worksheet.SetRangeDataFormat("C3", CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs { Format = "yyyy/MM/dd" });

			worksheet.SetRangeDataFormat("C3", CellDataFormatFlag.DateTime, null);

			Grid.Save(GetExcelFileName("OUT02"));
		}

		[TestCase]
		void OutputCurrencyFormat()
		{
			worksheet["A4"] = new double[] { 0, 1234, 123456789, 12345.6789, 0, 1234, 123456789, 12345.6789, -1234, -123456789, -12345.6789, -0.123456789, 1234.5678 };

			worksheet.SetRangeDataFormat("A4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = false, DecimalPlaces = 0, PrefixSymbol = "$" });
			worksheet.SetRangeDataFormat("B4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = false, DecimalPlaces = 0, PrefixSymbol = "$" });
			worksheet.SetRangeDataFormat("C4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = false, DecimalPlaces = 3, PrefixSymbol = "$" });
			worksheet.SetRangeDataFormat("D4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = false, DecimalPlaces = 3, PrefixSymbol = "$" });

			worksheet.SetRangeDataFormat("E4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 0, PostfixSymbol = " USD" });
			worksheet.SetRangeDataFormat("F4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 0, PostfixSymbol = " USD" });
			worksheet.SetRangeDataFormat("G4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 3, PostfixSymbol = " USD" });
			worksheet.SetRangeDataFormat("H4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 3, PostfixSymbol = " USD" });

			worksheet.SetRangeDataFormat("I4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 0, PrefixSymbol = "$ ", PostfixSymbol = " USD", NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Red });
			worksheet.SetRangeDataFormat("J4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 0, PrefixSymbol = "$ ", PostfixSymbol = " USD", NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Red });
			worksheet.SetRangeDataFormat("K4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 3, PrefixSymbol = "$ ", PostfixSymbol = " USD", NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets });
			worksheet.SetRangeDataFormat("L4", CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs { UseSeparator = true, DecimalPlaces = 3, PrefixSymbol = "$ ", PostfixSymbol = " USD", NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets });

			worksheet.SetRangeDataFormat("M4", CellDataFormatFlag.Currency, null);

			Grid.Save(GetExcelFileName("OUT03"));
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=268
		/// </summary>
		[TestCase]
		void LastRowAndColumnError()
		{
			SetUp(10, 10);

			var filename = GetExcelFileName("LastRowAndColumnError");

			worksheet[0, 9] = "A";
			worksheet.SetColumnsWidth(9, 1, 100);
			Grid.Save(filename);

			worksheet[9, 0] = "A";
			worksheet.SetRowsHeight(9, 1, 100);
			Grid.Save(filename);

			Grid.Reset();

			Grid.Load(filename);
			worksheet = Grid.CurrentWorksheet;
			AssertEquals(worksheet[0, 9], "A");
			AssertEquals(worksheet[9, 0], "A");

			// TODO: size incorrect
			//AssertSame(worksheet.ColumnHeaders[9].Width, 100);
			//AssertSame(worksheet.RowHeaders[9].Height, 100);
		}

		[TestCase]
		void BorderStyleReadBack()
		{
			SetUp(10, 10);

			var filename = GetExcelFileName("BorderStyleReadBack");

			worksheet = Grid.Worksheets[0];
			worksheet.SetRangeBorders("A1:J10", BorderPositions.Right | BorderPositions.Bottom, RangeBorderStyle.BlackDotted);
			AssertSame(worksheet.UsedRange, new RangePosition("A1:J10"));
			Grid.Save(filename);

			worksheet.Reset();

			Grid.Load(filename);
			worksheet = Grid.Worksheets[0];
			AssertSame(worksheet.UsedRange, new RangePosition("A1:J10"));
			var borders = worksheet.GetRangeBorders("J10");
			AssertSame(borders.Right, RangeBorderStyle.BlackDotted);
			AssertSame(borders.Bottom, RangeBorderStyle.BlackDotted);
		}
	}
}
