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

namespace unvell.ReoGrid.Tests.Tests
{
	[TestSet]
	class RGFTest : ReoGridTestSet
	{
		static string GetRGFFileName(string name) => $"..\\..\\..\\rgf\\{name}.rgf";

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=254
		/// </summary>
		[TestCase]
		void LastBordersInputOutput()
		{
			var mb = ReoGridControl.CreateMemoryWorkbook();
			var sheet = mb.Worksheets[0];

			sheet.Resize(10, 10);
			sheet.SetRangeBorders("A1:J10", BorderPositions.Right | BorderPositions.Bottom, RangeBorderStyle.BlackDotted);
			AssertSame(sheet.UsedRange, new RangePosition("A1:J10"));
			sheet.SaveRGF(GetRGFFileName("FrozenCellsInputOutput"));

			sheet.Reset();

			sheet.LoadRGF(GetRGFFileName("FrozenCellsInputOutput"));
			AssertSame(sheet.UsedRange, new RangePosition("A1:J10"));
			var borders = sheet.GetRangeBorders("J10");
			AssertSame(borders.Right, RangeBorderStyle.BlackDotted);
			AssertSame(borders.Bottom, RangeBorderStyle.BlackDotted);
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=277
		/// https://reogrid.net/forum/viewtopic.php?id=284
		/// </summary>
		[TestCase]
		void DeleteRowsAndSaveAs()
		{
			worksheet.LoadRGF(GetRGFFileName("4FactorsGrid"));
			worksheet.DeleteRows(5, 2);
			worksheet.SaveRGF(GetRGFFileName("4FactorsGrid_Output"));
			worksheet.LoadRGF(GetRGFFileName("4FactorsGrid_Output"));
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?pid=1112
		/// </summary>
		[TestCase]
		void SaveLastRowVBorder()
		{
			worksheet.Resize(5, 5);
			
			worksheet.SetRangeBorders(4, 0, 5, 5, BorderPositions.All, RangeBorderStyle.BlackSolid);

			worksheet.SaveRGF(GetRGFFileName("LastRowVBorderTest_Output"));
			worksheet.LoadRGF(GetRGFFileName("LastRowVBorderTest_Output"));

			var bstyle = worksheet.GetRangeBorders(4, 0, 5, 5, BorderPositions.All);
			AssertEquals(bstyle.InsideVertical.Color, Graphics.SolidColor.Black);

			AssertEquals(worksheet.Cells[4, 0].Border.Left.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[4, 0].Border.Right.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[4, 4].Border.Left.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[4, 4].Border.Right.Color, Graphics.SolidColor.Black);
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?pid=1112
		/// </summary>
		[TestCase]
		void SaveLastColumnHBorder()
		{
			worksheet.Resize(5, 5);

			worksheet.SetRangeBorders(0, 4, 5, 5, BorderPositions.All, RangeBorderStyle.BlackSolid);

			worksheet.SaveRGF(GetRGFFileName("LastColumnHBorderTest_Output"));
			worksheet.LoadRGF(GetRGFFileName("LastColumnHBorderTest_Output"));

			var bstyle = worksheet.GetRangeBorders(0, 4, 5, 5, BorderPositions.All);
			AssertEquals(bstyle.InsideHorizontal.Color, Graphics.SolidColor.Black);

			AssertEquals(worksheet.Cells[0, 4].Border.Top.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[0, 4].Border.Bottom.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[4, 4].Border.Top.Color, Graphics.SolidColor.Black);
			AssertEquals(worksheet.Cells[4, 4].Border.Bottom.Color, Graphics.SolidColor.Black);
		}

	}
}
