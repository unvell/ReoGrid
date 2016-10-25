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
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Tests.TestCases
{
	[TestSet]
	class FreezeTest : ReoGridTestSet
	{
		[TestCase]
		void RemoveRowsBeforeFrozenRows_Top()
		{
			SetUp(10, 10);

			worksheet.FreezeToCell(5, 5, FreezeArea.Top);

			worksheet.DeleteRows(2, 2);

			AssertEquals(worksheet.FreezePos.Row, 3);
		}

		[TestCase]
		void RemoveRowsBeforeFrozenRows_Bottom()
		{
			SetUp(10, 10);

			worksheet.FreezeToCell(9, 5, FreezeArea.Bottom);

			worksheet.DeleteRows(2, 2);

			AssertEquals(worksheet.FreezePos.Row, 7);
		}

	}

}
