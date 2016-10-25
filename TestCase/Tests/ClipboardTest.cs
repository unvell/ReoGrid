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
using System.Drawing;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class ClipboardTest : ReoGridTestSet
	{
		[TestCase]
		void FormattedCopy()
		{
			SetUp();

			var range = new RangePosition(1, 1, 3, 5);

			worksheet.SetRangeBorders(range, BorderPositions.Outside, RangeBorderStyle.BlackSolid);
			worksheet.SetRangeBorders(range, BorderPositions.InsideAll, RangeBorderStyle.BlackDotted);

			var data = new object[,] {
				{ "A1", "B1", "C1", "D1", "E1" },
				{ "A2", "B2", "C2", "D2", "E2" },
				{ "A3", "B3", "C3", "D3", "E3" },
			};

			worksheet[range] = data;

			worksheet.SelectionRange = range;
			worksheet.Copy();

			/////////////////////////////////////////////////////////////////////////////////

			// have a small rest since Clipboard may be still opened by Copy method
			// otherwise it may cause an exception 0x800401D0 (CLIPBRD_E_CANT_OPEN)
			System.Threading.Thread.Sleep(50);

			range = new RangePosition(10, 10, 3, 5);

			worksheet.SelectionRange = range;
			worksheet.Paste();

			AssertTrue(worksheet._Debug_Validate_All());

			var bi = worksheet.GetRangeBorders(range);
			AssertEquals(bi.NonUniformPos, BorderPositions.None);
			AssertEquals(bi.Left, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi.Right, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi.Top, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi.Bottom, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));

			AssertEquals(worksheet[range.StartPos], data[0, 0]);
			AssertEquals(worksheet[range.EndPos], data[2, 4]);

		}

		[TestCase]
		void UndoPaste()
		{
			var range = new RangePosition(10, 10, 3, 5);
		
			Grid.Undo();
			AssertTrue(worksheet._Debug_Validate_All());

			var bi = worksheet.GetRangeBorders(range);
			AssertEquals(bi.Left, RangeBorderStyle.Empty);
			AssertEquals(bi.Right, RangeBorderStyle.Empty);
			AssertEquals(bi.Top, RangeBorderStyle.Empty);
			AssertEquals(bi.Bottom, RangeBorderStyle.Empty);
			
			AssertEquals(worksheet[range.StartPos], null);
			AssertEquals(worksheet[range.EndPos], null);
		}

		[TestCase]
		void Cut()
		{
			var range = new RangePosition(1, 1, 3, 5);

			worksheet.SelectionRange = range;
			worksheet.Cut();
			AssertTrue(worksheet._Debug_Validate_All());

			var bi = worksheet.GetRangeBorders(range);
			AssertEquals(bi.Left, RangeBorderStyle.Empty);
			AssertEquals(bi.Right, RangeBorderStyle.Empty);
			AssertEquals(bi.Top, RangeBorderStyle.Empty);
			AssertEquals(bi.Bottom, RangeBorderStyle.Empty);

			AssertEquals(worksheet[range.StartPos], null);
			AssertEquals(worksheet[range.EndPos], null);

			/////////////////////////////////////////////////////////////////////////////////

			range = new RangePosition(10, 10, 3, 5);

			worksheet.SelectionRange = range;
			worksheet.Paste();
			AssertTrue(worksheet._Debug_Validate_All());

			var bi2 = worksheet.GetRangeBorders(range);
			AssertEquals(bi2.NonUniformPos, BorderPositions.None);
			AssertEquals(bi2.Left, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi2.Right, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi2.Top, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));
			AssertEquals(bi2.Bottom, new RangeBorderStyle(Color.Black, BorderLineStyle.Solid));

			AssertEquals(worksheet[range.StartPos], "A1");
			AssertEquals(worksheet[range.EndPos], "E3");

		}

		[TestCase]
		void UndoCut()
		{
			Grid.Undo();
			AssertTrue(worksheet._Debug_Validate_All());

			var range = new RangePosition(1, 1, 3, 5);
			
			var bi = worksheet.GetRangeBorders(range);
			AssertEquals(bi.Left, RangeBorderStyle.Empty);
			AssertEquals(bi.Right, RangeBorderStyle.Empty);
			AssertEquals(bi.Top, RangeBorderStyle.Empty);
			AssertEquals(bi.Bottom, RangeBorderStyle.Empty);

			AssertEquals(worksheet[range.StartPos], null);
			AssertEquals(worksheet[range.EndPos], null);

			range = new RangePosition(10, 10, 3, 5);
		
			bi = worksheet.GetRangeBorders(range);
			AssertEquals(bi.Left, RangeBorderStyle.Empty);
			AssertEquals(bi.Right, RangeBorderStyle.Empty);
			AssertEquals(bi.Top, RangeBorderStyle.Empty);
			AssertEquals(bi.Bottom, RangeBorderStyle.Empty);
			
			AssertEquals(worksheet[range.StartPos], null);
			AssertEquals(worksheet[range.EndPos], null);
		}

		[TestCase]
		void InputAfterCut()
		{
			SetUp(20, 20);

			var cell = worksheet.Cells["A1"];
			cell.Style.BackColor = Color.LightGreen;
			cell.Data = "AAA";

			worksheet.SelectionRange = new RangePosition("A1");
			worksheet.Cut();

			worksheet.SelectionRange = new RangePosition("A2");
			worksheet.Paste();

			worksheet["A1"] = 123;

			AssertSame(123, worksheet["A1"]);
			AssertEquals("123", worksheet.GetCellText("A1"));
			
		}
	}
}
