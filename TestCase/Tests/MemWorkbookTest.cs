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
	class MemoryWorkbookTest : TestSet
	{
		[TestCase]
		void FrozenWorkbookRGF()
		{
			// https://reogrid.net/forum/viewtopic.php?id=225
			var wb = ReoGridControl.CreateMemoryWorkbook();

			var sheet = wb.Worksheets[0];

			sheet["A1"] = "Hello";
			sheet.FreezeToCell("C3");
			sheet["D4"] = 12345;
			sheet.SaveRGF("RGF_MB_Frozen.rgf");

			sheet.Reset();

			sheet.LoadRGF("RGF_MB_Frozen.rgf");
			AssertEquals(sheet["A1"], "Hello");
			AssertSame(sheet["D4"], 12345);
			AssertEquals(sheet.FreezePos, new CellPosition("C3"), "sheet.FreezePos = C3");
		}

	}

}
