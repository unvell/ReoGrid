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
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class ExcelInTest : ReoGridTestSet
	{
		static string GetExcelFileName(string name) => $"..\\..\\..\\xlsx\\{name}.xlsx";

		[TestCase]
		void A01_BasicCellValue()
		{
			SetUp();

			this.Grid.Load(GetExcelFileName("A01"), IO.FileFormat.Excel2007);

			AssertEquals(this.Grid.Worksheets.Count, 1);
			AssertEquals(this.Grid.Worksheets[0].Name, "A01");

			var sheet = this.Grid.Worksheets[0];
			AssertEquals(sheet["A1"], "abcd");
			AssertEquals(sheet["A2"], "abcd");
			AssertSame(sheet["B1"], 12);
			AssertEquals(sheet["B2"], "<abc>");
			AssertEquals(sheet["C1"], "漢字");
			AssertEquals(sheet["C2"], "Ω№");
		}

		#region A02
		[TestCase]
		void A02_BasicFormula()
		{
			SetUp();

			this.Grid.Load(GetExcelFileName("A02"), IO.FileFormat.Excel2007);

			AssertEquals(this.Grid.Worksheets.Count, 2);
			AssertEquals(this.Grid.Worksheets[0].Name, "A02");

			var sheet = this.Grid.Worksheets[0];
			AssertSame(sheet["A1"], 1);
			AssertSame(sheet["B1"], 2);
			AssertSame(sheet["C1"], 3);
			AssertSame(sheet["A2"], 6);
			AssertSame(sheet["B2"], 6);
			AssertSame(sheet["C2"], 3);

			AssertEquals(sheet.GetCellFormula("A1"), null);
			AssertEquals(sheet.GetCellFormula("B1"), null);
			AssertEquals(sheet.GetCellFormula("C1"), "A1+B1");
			AssertEquals(sheet.GetCellFormula("A2"), "SUM(A1:C1)");
			AssertEquals(sheet.GetCellFormula("B2"), "A2");
			AssertEquals(sheet.GetCellFormula("C2"), "B2/2");

			#region cross worksheet reference
			// cross worksheet reference
			AssertSame(this.Grid.Worksheets["Sheet2"]["A1"], 10);
			AssertEquals(this.Grid.Worksheets[0].GetCellFormula("A10"), "Sheet2!A1");
			AssertSame(this.Grid.Worksheets[0]["A10"], 10);

			this.Grid.Worksheets["Sheet2"]["A1"] = 20;
			AssertSame(this.Grid.Worksheets[0]["A10"], 20);
			#endregion // cross worksheet reference
		}
		#endregion // A02

		#region A03
		void AssertCellText(string addr, string text)
		{
			AssertEquals(worksheet.GetCellText(addr), text, addr);
		}

		[TestCase]
		void A03_Styles()
		{
			SetUp();

			this.Grid.Load(GetExcelFileName("A03"), IO.FileFormat.Excel2007);

			AssertEquals(this.Grid.Worksheets.Count, 1);

			var sheet = this.Grid.Worksheets["A03"];
			AssertTrue(sheet != null);

			AssertEquals(sheet.Cells["A1"].Style.Bold, true, "bold");
			AssertSame(sheet.Cells["B1"].Style.TextColor, System.Drawing.Color.Red);
			AssertEquals(sheet.Cells["C1"].Style.Strikethrough, true, "strikethrough");
			AssertSame(sheet.Cells["A2"].Style.BackColor, System.Drawing.Color.Red);
			AssertEquals(sheet.Cells["B2"].Style.Italic, true, "italic");
			AssertEquals(sheet.Cells["C2"].Style.Underline, true, "underline");
			AssertSame(sheet.Cells["A3"].Style.FontSize, 18);
			AssertSame(sheet.Cells["B3"].Style.FontSize, 11);
			AssertSame(sheet.Cells["C3"].Style.FontSize, 8);

			AssertSame(sheet.Cells["A4"].Style.HAlign, ReoGridHorAlign.Left, "LT");
			AssertSame(sheet.Cells["A4"].Style.VAlign, ReoGridVerAlign.Top, "LT");
			AssertSame(sheet.Cells["B4"].Style.HAlign, ReoGridHorAlign.Center, "CT");
			AssertSame(sheet.Cells["B4"].Style.VAlign, ReoGridVerAlign.Top, "CT");
			AssertSame(sheet.Cells["C4"].Style.HAlign, ReoGridHorAlign.Right, "RT");
			AssertSame(sheet.Cells["C4"].Style.VAlign, ReoGridVerAlign.Top, "RT");
			AssertSame(sheet.Cells["A5"].Style.HAlign, ReoGridHorAlign.Left, "LM");
			AssertSame(sheet.Cells["A5"].Style.VAlign, ReoGridVerAlign.Middle, "LM");
			AssertSame(sheet.Cells["B5"].Style.HAlign, ReoGridHorAlign.Center, "CM");
			AssertSame(sheet.Cells["B5"].Style.VAlign, ReoGridVerAlign.Middle, "CM");
			AssertSame(sheet.Cells["C5"].Style.HAlign, ReoGridHorAlign.Right, "RM");
			AssertSame(sheet.Cells["C5"].Style.VAlign, ReoGridVerAlign.Middle, "RM");
			AssertSame(sheet.Cells["A6"].Style.HAlign, ReoGridHorAlign.Left, "LB");
			AssertSame(sheet.Cells["A6"].Style.VAlign, ReoGridVerAlign.Bottom, "LB");
			AssertSame(sheet.Cells["B6"].Style.HAlign, ReoGridHorAlign.Center, "CB");
			AssertSame(sheet.Cells["B6"].Style.VAlign, ReoGridVerAlign.Bottom, "CB");
			AssertSame(sheet.Cells["C6"].Style.HAlign, ReoGridHorAlign.Right, "RB");
			AssertSame(sheet.Cells["C6"].Style.VAlign, ReoGridVerAlign.Bottom, "RB");

			var oldworksheet = worksheet;
			worksheet = sheet;

			AssertCellText("A7", "1");
			AssertCellText("B7", "12");
			AssertCellText("C7", "123");
			AssertCellText("D7", "1234");
			AssertCellText("E7", "12345");
			AssertCellText("F7", "123456");
			AssertCellText("G7", "1234567");
			AssertCellText("H7", "12345678");
			AssertCellText("I7", "123456789");

			AssertCellText("A8", "1");
			AssertCellText("B8", "12");
			AssertCellText("C8", "123");
			AssertCellText("D8", "1,234");
			AssertCellText("E8", "12,345");
			AssertCellText("F8", "123,456");
			AssertCellText("G8", "1,234,567");
			AssertCellText("H8", "12,345,678");
			AssertCellText("I8", "123,456,789");

			AssertCellText("A9", "0.1");
			AssertCellText("B9", "0.12");
			AssertCellText("C9", "0.123");
			AssertCellText("D9", "0.1234");
			AssertCellText("E9", "0.12345");
			AssertCellText("F9", "0.123456");
			AssertCellText("G9", "0.1234567");
			AssertCellText("H9", "0.12345678");
			AssertCellText("I9", "0.123456789");

			AssertCellText("A10", "0%");
			AssertCellText("B10", "0.1%");
			AssertCellText("C10", "0.12%");
			AssertCellText("D10", "0.123%");
			AssertCellText("E10", "0.1234%");
			AssertCellText("F10", "1%");
			AssertCellText("G10", "12%");
			AssertCellText("H10", "123%");
			AssertCellText("I10", "1234%");

			AssertCellText("A13", "10.10");
			AssertCellText("B13", "%5%");

			// currency line 1
			AssertCellText("A15", "$1");
			AssertCellText("B15", "2.00 ₽");
			AssertCellText("C15", "¥3.00");
			AssertCellText("D15", "¥4.00");
			AssertCellText("E15", "€ 5.00");
			AssertCellText("F15", "JPY 6.00");
			AssertCellText("G15", "7.00 €");
			AssertCellText("H15", "₱8.00");
			AssertCellText("I15", "USD 9.00");

			// currency line 2
			AssertCellText("A16", "$1,234");
			AssertCellText("B16", "1,234.00 ₽");
			AssertCellText("C16", "¥1,234.00");
			AssertCellText("D16", "¥1,234.00");
			AssertCellText("E16", "€ 1,234.00");
			AssertCellText("F16", "JPY 1,234.00");
			AssertCellText("G16", "1,234.00 €");
			AssertCellText("H16", "₱1,234.00");
			AssertCellText("I16", "USD 1,234.00");

			// currency line 3
			AssertCellText("A17", "-$1");
			AssertCellText("B17", "-1.23 ₽");
			AssertCellText("C17", "-¥1.23");
			AssertCellText("D17", "-¥1.23");
			AssertCellText("E17", "-€ 1.23");
			AssertCellText("F17", "-JPY 1.23");
			AssertCellText("G17", "-1.23 €");
			AssertCellText("H17", "-₱1.23");
			AssertCellText("I17", "-USD 1.23");

			// currency line 4
			AssertCellText("A18", "-$1.2340");
			AssertCellText("B18", "-1.2340 ₽");
			AssertCellText("C18", "-¥1.2340");
			AssertCellText("D18", "-¥1.2340");
			AssertCellText("E18", "-€ 1.2340");
			AssertCellText("F18", "-JPY 1.2340");
			AssertCellText("G18", "-1.2340 €");
			AssertCellText("H18", "-₱1.2340");
			AssertCellText("I18", "-USD 1.2340");

			// negative number formats
			Cell cell;

			AssertEquals(sheet.GetCellText("A21"), "-10");

			cell = sheet.Cells["B21"];
			AssertEquals(cell.DataFormat, CellDataFormatFlag.Number);
			AssertEquals(((NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs).NegativeStyle, 
				NumberDataFormatter.NumberNegativeStyle.Red);

			cell = sheet.Cells["C21"];
			AssertEquals(cell.DataFormat, CellDataFormatFlag.Number);
			AssertEquals(((NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs).NegativeStyle,
				NumberDataFormatter.NumberNegativeStyle.Brackets);

			cell = sheet.Cells["D21"];
			AssertEquals(cell.DataFormat, CellDataFormatFlag.Number);
			AssertEquals(((NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs).NegativeStyle,
				NumberDataFormatter.NumberNegativeStyle.RedBrackets);

			cell = sheet.Cells["E21"];
			AssertEquals(cell.DataFormat, CellDataFormatFlag.Number);
			AssertEquals(((NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs).NegativeStyle,
				NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku);

			worksheet = oldworksheet;
		}
		#endregion // A03

		#region A04
		[TestCase]
		void A04_Borders()
		{
			SetUp();

			this.Grid.Load(GetExcelFileName("A04"), IO.FileFormat.Excel2007);

			AssertEquals(this.Grid.Worksheets.Count, 1);

			var sheet = this.Grid.Worksheets["A04"];
			AssertTrue(sheet != null);

			var bs = sheet.GetRangeBorders("B2:D4");

			AssertEquals(bs.Top, RangeBorderStyle.BlackSolid);
			AssertEquals(bs.Bottom, RangeBorderStyle.BlackSolid);
			AssertEquals(bs.Left, RangeBorderStyle.BlackSolid);
			AssertEquals(bs.Right, RangeBorderStyle.BlackSolid);
			AssertEquals(bs.InsideHorizontal, RangeBorderStyle.BlackDotted);
			AssertEquals(bs.InsideVertical, RangeBorderStyle.BlackDotted);

			bs = sheet.GetRangeBorders("F2:H4");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDotDot }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDotDot }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDotDot }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDotDot }, "Bottom");

			AssertEquals(bs.InsideHorizontal, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDot }, "Hor");
			AssertEquals(bs.InsideVertical, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DashDot }, "Ver");

			bs = sheet.GetRangeBorders("J2:L4");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed }, "Bottom");

			AssertEquals(bs.InsideHorizontal, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed2 }, "Hor");
			AssertEquals(bs.InsideVertical, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Dashed2 }, "Ver");

			bs = sheet.GetRangeBorders("B6:D8");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDotDot }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDotDot }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDotDot }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDotDot }, "Bottom");

			AssertEquals(bs.InsideHorizontal, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDotDot }, "Hor");
			AssertEquals(bs.InsideVertical, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDotDot }, "Ver");

			bs = sheet.GetRangeBorders("F6:H8");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDot }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDot }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDot }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashDot }, "Bottom");

			AssertEquals(bs.InsideHorizontal, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashed }, "Hor");
			AssertEquals(bs.InsideVertical, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldDashed }, "Ver");

			bs = sheet.GetRangeBorders("J6:L8");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolid }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolid }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolid }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolid }, "Bottom");

			AssertEquals(bs.InsideHorizontal, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolidStrong }, "Hor");
			AssertEquals(bs.InsideVertical, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.BoldSolidStrong }, "Ver");

			bs = sheet.GetRangeBorders("B10:D12");

			AssertEquals(bs.Top, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DoubleLine }, "Top");
			AssertEquals(bs.Left, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DoubleLine }, "Left");
			AssertEquals(bs.Right, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DoubleLine }, "Right");
			AssertEquals(bs.Bottom, new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.DoubleLine }, "Bottom");

			AssertEquals(bs.InsideHorizontal, RangeBorderStyle.Empty, "Hor");
			AssertEquals(bs.InsideVertical, RangeBorderStyle.Empty, "Ver");
		}
		#endregion // A04
	}
}