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
using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Tests.TestCases
{
	[TestSet]
	class WorkbookTest : ReoGridTestSet
	{
		[TestCase]
		void TestSheetManagement()
		{
			var wbc = this.Grid;

			wbc.Reset();
			int sheetCount = 1;

			wbc.WorksheetInserted += (s, e) => sheetCount++;
			wbc.WorksheetRemoved += (s, e) => sheetCount--;

			var s1 = wbc.CreateWorksheet();
			wbc.AddWorksheet(s1);

			AssertSame(sheetCount, 2);
			AssertSame(wbc.Worksheets.Count, 2);

			var s2 = wbc.CreateWorksheet();
			wbc.InsertWorksheet(0, s2);
			AssertSame(sheetCount, 3);
			AssertSame(wbc.Worksheets.Count, 3);

			wbc.RemoveWorksheet(1);
			AssertSame(sheetCount, 2);
			AssertSame(wbc.Worksheets.Count, 2);

			wbc.CopyWorksheet(0, 1);
			AssertSame(sheetCount, 3);
			AssertSame(wbc.Worksheets.Count, 3);
			
		}

		[TestCase]
		void DeleteActionForWorksheet()
		{
			this.Grid.Reset();
			
			this.Grid.DoAction(new SetCellDataAction("A1", "A"));
			this.Grid.DoAction(new SetCellDataAction("B2", "B"));
			this.Grid.DoAction(new SetCellDataAction("C3", "C"));
			this.Grid.DoAction(new SetCellDataAction("D4", "D"));

			var sheet2 = this.Grid.CreateWorksheet();
			this.Grid.AddWorksheet(sheet2);
			this.Grid.CurrentWorksheet = sheet2;

			this.Grid.DoAction(new SetCellDataAction("A1", "1"));
			this.Grid.DoAction(new SetCellDataAction("B2", "2"));
			this.Grid.DoAction(new SetCellDataAction("C3", "3"));
			this.Grid.DoAction(new SetCellDataAction("D4", "4"));

			this.Grid.Undo();
			this.Grid.Undo();
			this.Grid.Undo();
			this.Grid.Undo();
			this.Grid.Undo();

			this.Grid.RemoveWorksheet(sheet2);

			while (this.Grid.CanRedo())
			{
				this.Grid.Redo();
			}

			AssertEquals(this.Grid.Worksheets.Count, 1);
			AssertEquals(this.Grid.CurrentWorksheet["A1"], "A");
			AssertEquals(this.Grid.CurrentWorksheet["B2"], "B");
			AssertEquals(this.Grid.CurrentWorksheet["C3"], "C");
			AssertEquals(this.Grid.CurrentWorksheet["D4"], "D");
		}

		[TestCase]
		void ReferenceAfterCopy()
		{
			using (var wb = ReoGridControl.CreateMemoryWorkbook())
			{
				AssertEquals(wb.Worksheets.Count, 1);

				wb.Worksheets[0]["A1"] = 10;
				wb.Worksheets[0]["B1"] = "=A1";
				wb.CopyWorksheet(0, 1);

				AssertSame(wb.Worksheets.Count, 2);
				AssertSame(wb.Worksheets[0].Name, "Sheet1");
				AssertSame(wb.Worksheets[1].Name, "Sheet2");

				AssertSame(wb.Worksheets[0]["A1"], 10);
				AssertSame(wb.Worksheets[1]["A1"], 10);

				AssertSame(wb.Worksheets[0]["B1"], 10);
				AssertSame(wb.Worksheets[1]["B1"], 10);

				wb.Worksheets[0]["A1"] = 20;
				AssertSame(wb.Worksheets[0]["B1"], 20);
				AssertSame(wb.Worksheets[1]["B1"], 10);

				wb.Worksheets[1]["A1"] = 30;
				AssertSame(wb.Worksheets[0]["B1"], 20);
				AssertSame(wb.Worksheets[1]["B1"], 30);
			}
		}

		[TestCase]
		void ReferenceAfterCopy2()
		{
			using (var wb = ReoGridControl.CreateMemoryWorkbook())
			{
				AssertEquals(wb.Worksheets.Count, 1);

				wb.Worksheets[0]["A1"] = 10;
				wb.Worksheets[0]["B1"] = "=Sheet1!A1";
				wb.CopyWorksheet(0, 1);

				AssertSame(wb.Worksheets.Count, 2);
				AssertSame(wb.Worksheets[0].Name, "Sheet1");
				AssertSame(wb.Worksheets[1].Name, "Sheet2");

				AssertSame(wb.Worksheets[0]["A1"], 10);
				AssertSame(wb.Worksheets[1]["A1"], 10);

				AssertSame(wb.Worksheets[0]["B1"], 10);
				AssertSame(wb.Worksheets[1]["B1"], 10);

				wb.Worksheets[0]["A1"] = 20;
				AssertSame(wb.Worksheets[0]["B1"], 20);
				AssertSame(wb.Worksheets[1]["B1"], 20);

				wb.Worksheets[1]["A1"] = 30;
				AssertSame(wb.Worksheets[0]["B1"], 20);
				AssertSame(wb.Worksheets[1]["B1"], 20);
			}
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=222
		/// </summary>
		[TestCase]
		void CopyFrozenWorksheet()
		{
			Grid.CurrentWorksheet.FreezeToCell("A8");
			Grid.CurrentWorksheet.ScaleFactor = 1.1f;

			var newSheet = Grid.CopyWorksheet(0, 1);

			Grid.CurrentWorksheet = newSheet;

			AssertEquals(newSheet.FreezePos, new CellPosition("A8"), "Frozen cell should be A8");
			AssertSame(Math.Round(newSheet.ScaleFactor * 10), 11f);

			Grid.CurrentWorksheet = Grid.Worksheets[0];
		}

		[TestCase]
		void WorksheetDispose()
		{
			var sheet = Grid.CreateWorksheet();
			sheet.Dispose();
		}
	}

}
